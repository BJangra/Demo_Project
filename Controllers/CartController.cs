using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using FirstProject_ECommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;



namespace FirstProject_ECommerce.Controllers
{
   // [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<IdentityUser> userManager;

        public CartController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

      
        public async Task<IActionResult> AddToCart(int productId)
        {
            Console.WriteLine($"Product id is received{productId}");
            var user = await userManager.GetUserAsync(User);
            if(user ==null)
            {
                return Redirect("/Identity/Account/Login");
            }
            var product = await context.Products.FindAsync(productId);

            if(product == null)
            {
                return NotFound();
            }
            var cartItem = await context.CartItems.FirstOrDefaultAsync
                (c => c.ProductId == productId && c.UserId == user.Id);
            
            if(cartItem == null)
            {
                cartItem = new CartItem
                {
                    ProductId =productId,
                    UserId = user.Id, 
                    Quantity =1
                };
                context.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity++;
            }
            ViewBag.item = cartItem.Quantity;

            await context.SaveChangesAsync();
           return RedirectToAction("Index");
          
        }

        public async Task<IActionResult> Index()
        {
            var user = await userManager.GetUserAsync(User);
            if(user == null)
            {
                return Redirect("/Identity/Account/Login");
            }
            var cartItem = await context.CartItems.Include(c => c.Product)
                .Where(c => c.UserId == user.Id).ToListAsync();

            return View(cartItem);

        }
        [HttpPost]
        public async Task<IActionResult> UpdateCart(int id , int quantity)
        {
            var cartItem = await context.CartItems.FindAsync(id);
            if(cartItem != null && quantity>0)
            {
                cartItem.Quantity = quantity;
                await context.SaveChangesAsync();
            }
                return RedirectToAction("Index");

        }

        public async Task<IActionResult> RemoveFromcart( int id)
        {
            var user = await userManager.GetUserAsync(User);
            if(user == null)
            {
                return Redirect("/Identity/Account/Login");
            }
            var cartItem = await context.CartItems.FirstOrDefaultAsync(c => c.Id == id && c.UserId == user.Id);
            if(cartItem != null)
            {
                context.CartItems.Remove(cartItem);
                await context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
