using FirstProject_ECommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FirstProject_ECommerce.Controllers
{
    public class OrderController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly ApplicationDbContext context;
        public OrderController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.context = context;
        }
        public async Task<IActionResult> Checkout()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Redirect("/Identity/Account/Login");
            }
            var cartitems = await context.CartItems.Where(p => p.UserId == user.Id).
                Include(c => c.Product).ToListAsync();
            if (!cartitems.Any())
            {
                TempData["error"] = "Cart is Empty";
                return RedirectToAction("Index", "Cart");
            }
           
            return View(cartitems);
        }
        public async Task<IActionResult> PlaceOrder()
        {
            var user = await userManager.GetUserAsync(User);
            if(user == null)
            {
                return Redirect("/Identity/Account/Login");
            }
            var cartitems = await context.CartItems.Where(p => p.UserId == user.Id).
               Include(c => c.Product).ToListAsync();

            if (!cartitems.Any())
            {
                TempData["error"] = "Cart is Empty";
                return RedirectToAction("Index", "Cart");
            }
            decimal totalAmount = cartitems.Sum(item => item.Product.Price * item.Quantity);

            var order = new Order()
            {
                UserId = user.Id,
                OrderDate = DateTime.Now,
                TotalAmount = totalAmount,
                OrderItems = cartitems.Select(c => new OrderItem
                {
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    Price= c.Product.Price
                }).ToList()
            };
            await context.Orders.AddAsync(order);
            await context.SaveChangesAsync();

            context.CartItems.RemoveRange(cartitems);

            await context.SaveChangesAsync();
            return RedirectToAction("OrderConfirmation", new { id = order.Id });
        }

        public async Task<IActionResult> OrderConfirmation(int id)
        {
            var order = await context.Orders.
                Include(c => c.OrderItems).ThenInclude(p => p.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }
        [Authorize]
        public async Task<IActionResult> OrderHistory()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Redirect("/Identity/Account/Login");
            }
            var orders = await context.Orders.Where(o => o.UserId == user.Id).
                Include(c => c.OrderItems).ThenInclude(p => p.Product)
                .OrderByDescending(o => o.OrderDate).ToListAsync();
            return View(orders);
        }

        public async Task<IActionResult> OrderDetails(int id )
        {
            var user = await userManager.GetUserAsync(User);
            if(user == null)
            {
                return Redirect("/Identity/Account/Login");
            }
            var order = await context.Orders.Where(p => p.UserId == user.Id && p.Id == id).
                Include(o => o.OrderItems).ThenInclude(q => q.Product).SingleOrDefaultAsync();
            if(order == null)
            {
                return NotFound();
            }
            return View(order);
        }
    
    }
}
