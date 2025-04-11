using FirstProject_ECommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;

namespace FirstProject_ECommerce.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly ApplicationDbContext context;

        public AdminController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.context = context;
        }

        [Authorize(Roles = "Admin")]
       
        public IActionResult Dashboard()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult ManageOrders()
        {
            var orders = context.Orders.Include(o => o.OrderItems).ThenInclude(p => p.Product).ToList();
            return View(orders);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult ManageProducts()
        {
            var product =  context.Products.ToList();
            return View(product);
        }
        public async Task<IActionResult> CheckRole()
        {
            var user = await userManager.GetUserAsync(User);
            if(user!= null)
            {
                var roles = await userManager.GetRolesAsync(user);
                return Json(roles);
            }
            return Json(new { Message = "No User Logged in" });
        }
    }
}
