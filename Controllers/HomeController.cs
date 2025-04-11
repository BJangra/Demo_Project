using System.Diagnostics;
using System.Linq;
using FirstProject_ECommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FirstProject_ECommerce.Controllers
{
     public class HomeController : Controller
    {
        private readonly ApplicationDbContext context;
        public HomeController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            var product = context.Products.ToList();
            return View(product);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(Product product , IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if(imageFile !=null && imageFile.Length>0)
                {
                    var fileName = Path.GetFileName(imageFile.FileName);
                    var filePath = Path.Combine("wwwroot/images", fileName);
                    using (var stream = new FileStream(filePath,FileMode.Create))
                    {
                        imageFile.CopyTo(stream);
                    }
                    product.ImageUrl = "/images/" + fileName;
                }
                context.Products.Add(product);
                context.SaveChanges();
               return RedirectToAction(("Index"));
            }
            return View(product);
        }
           [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var product = context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id ,Product product, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                 if(id != product.Id)
                 {
                   return NotFound();
                 }
                if (imageFile != null && imageFile.Length > 0)
                {
                    var fileName = Path.GetFileName(imageFile.FileName);
                    var filePath = Path.Combine("wwwroot/images", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        imageFile.CopyTo(stream);
                    }
                    product.ImageUrl = "/images/" + fileName;
                }
                context.Products.Update(product);
                context.SaveChanges();
              return  RedirectToAction("Index","Home");
            }

            return View(product);
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var product = context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            return View("Delete",product);
        }

        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirm(int id)
        {
            var product = context.Products.Find(id);

            if (product != null)
            {
                context.Products.Remove(product);
                context.SaveChanges();
            }
            return RedirectToAction("Index");

        }



        public IActionResult Details(int? id)
        {
            var product = context.Products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
             
            }
            return View(product);

        }

        public async Task<IActionResult> Search( string Query)
        {
            if(string.IsNullOrEmpty(Query))
            {
                return View("Index", await context.Products.ToListAsync());
            }
            var searchProduct = await context.Products.Where
                (p => p.Name.Contains(Query) || p.Description.Contains(Query)).ToListAsync();
            return View("Index", searchProduct);
        }

        public IActionResult Privacy()
        {
            return View();
        }
       

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
