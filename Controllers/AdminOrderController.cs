using FirstProject_ECommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FirstProject_ECommerce.Controllers
{
       [Authorize(Roles ="Admin")]
    public class AdminOrderController : Controller
    {
        private readonly ApplicationDbContext context;

        public AdminOrderController(ApplicationDbContext context)
        {
            this.context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var orders = await context.Orders.ToListAsync();
            return View(orders);
        }
        public async Task<IActionResult> Details(int id)
        {
            var order = await context.Orders.Include(o => o.OrderItems)
                .ThenInclude(p => p.Product).FirstOrDefaultAsync(m => m.Id == id);
            if(order == null)
            {
                return NotFound();
            }
            return View(order);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var order = await context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id,Order order)
        {
          
            var orderExist = await context.Orders.FindAsync(id);
            if (orderExist == null)
            {
                return NotFound();
            }

                orderExist.Status = order.Status;
                    await context.SaveChangesAsync();
                return RedirectToAction("Index","AdminOrder");
        }

       
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var order = context.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }
            return View("Delete", order);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(int id,Order order)
        { 
            var dlt = await context.Orders.FindAsync(id);

            if (dlt != null)
            {
                context.Orders.Remove(dlt);
              await  context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
