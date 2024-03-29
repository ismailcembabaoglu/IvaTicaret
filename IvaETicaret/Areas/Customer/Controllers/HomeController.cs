﻿using IvaETicaret.Data;
using IvaETicaret.Email;
using IvaETicaret.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Media;
using System.Security.Claims;
using X.PagedList;

namespace IvaETicaret.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        public int idd;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _db = db;
            _logger = logger;
        }
        public IActionResult Search(string q, int p = 1)
        {
            if (!String.IsNullOrEmpty(q))
            {
                var ara = _db.Products.Where(c => c.Title.Contains(q) || c.Description.Contains(q)).Include(c => c.Category).ThenInclude(c => c.Store);
                //if (ara.ToList().Count() > 0)
                //{
                //    var bag = _db.Categories.FirstOrDefault(c => c.Id == ara.FirstOrDefault().CategoryId);
                //    var department = _db.Categories.FirstOrDefault(c => c.Id == bag.Id);
                //    ViewBag.Id = department.Id;
                //}
                //else
                //{
                //    return RedirectToAction("Index");
                //}
                return View(ara.ToPagedList(p, 40));

            }
            return View();
        }
        public IActionResult Index()
        {
            var department = _db.Departments.ToList();
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            //Console.Beep(38, 2000);
            if (claim != null)
            {
                var count = _db.ShoppingKarts.Where(c => c.ApplicationUserId == claim.Value).ToList().Count();
                HttpContext.Session.SetInt32(Diger.ssShopingCart, count);

            }
            if (User.IsInRole(Diger.Role_Bayi))
            {
                return LocalRedirect("/Admin/Order/Beklenen");
            }
            return View(department);
        }
        public IActionResult Location(int id)
        {
            StoreAdressVM storeAdressVM = new StoreAdressVM();
            storeAdressVM.DepartmentId = id;
            ViewData["CityId"] = _db.Cities.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.Name });
            return View(storeAdressVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Location(StoreAdressVM storeadress)
        {
            //        var store = _db.Stores.
            //Include(c => c.storeAdresses.
            //Where(c => c.CityId == storeadress.CityId && c.CountyId == storeadress.CountyId && c.DistrictId == storeadress.DistrictId))
            //.Where(c => c.DepartmentId == storeadress.DepartmentId).ToList();

            if (storeadress.CountyId > 0 && storeadress.DistrictId > 0)
            {

                return RedirectToAction("StoreList", storeadress);
            }
            else
            {
                return RedirectToAction("Location", storeadress.DepartmentId);
            }

        }
        public JsonResult ilcegetir(int p)
        {
            
            var ilceler = _db.Counties.Where(c => c.CityId == p).Select(c => new
            {
                Text = c.Name,
                Value = c.Id
            }).ToList();
            ilceler.Insert(0, new { Text = "Seçiniz", Value = 0 });
            return Json(ilceler);
        }
        public JsonResult mahallegetir(int p)
        {
            var mahalleler = _db.Districts.Where(c => c.CountyId == p).Select(c => new
            {
                Text = c.Name,
                Value = c.Id
            }).ToList();
            //mahalleler.Insert(0, new { Text = "Seçiniz", Value = 0 });
            return Json(mahalleler);
        }
        public IActionResult StoreList(StoreAdressVM storeadressvm, int p = 1)
        {
            var storeAdress = _db.StoreAdresses
                .Include(c => c.Store)
                .ThenInclude(c => c.Department).Include(c => c.District)
                .Where(c => c.CityId == storeadressvm.CityId && c.CountyId == storeadressvm.CountyId && c.DistrictId == storeadressvm.DistrictId && c.Store.Department.Id == storeadressvm.DepartmentId).ToList();
            //var store = _db.Stores.Include(c=>c.Department)
            //    .Where(c => c.DepartmentId == storeadressvm.DepartmentId).Include(c => c.storeAdresses.
            //    Where(c => c.CityId == storeadressvm.CityId && c.CountyId == storeadressvm.CountyId && c.DistrictId == storeadressvm.DistrictId)).ToList();
            return View(storeAdress);
        }
        public IActionResult Category(Guid id, int p = 1)
        {
            const int pageSize = 1;
            var cate = _db.Products.Include(c => c.Category).Where(c => c.Category.StoreId == id).ToList();
            PagedList<Product> product = new PagedList<Product>(null, p, 40);
            if (cate.Count() > 0)
            {

                product = new PagedList<Product>(cate, p, 40);
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                if (claim != null)
                {
                    var count = _db.ShoppingKarts.Where(c => c.ApplicationUserId == claim.Value).ToList().Count();
                    HttpContext.Session.SetInt32(Diger.ssShopingCart, count);
                }
                ViewBag.id = id;

                return View(product.ToPagedList(p, 40));

            }

            return RedirectToAction("Index");

        }
        public IActionResult Details(int id)
        {
            var product = _db.Products.FirstOrDefault(c => c.Id == id);
            ShoppingKart cart = new ShoppingKart()
            {
                Product = product,
                ProductId = product.Id,
                Price = product.Price,

            };
            return View(cart);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingKart scart)
        {
            scart.Id = 0;
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (ModelState.IsValid)
            {

                scart.ApplicationUserId = claim.Value;
                ShoppingKart cart = _db.ShoppingKarts.FirstOrDefault(c => c.ApplicationUserId == scart.ApplicationUserId && c.ProductId == scart.ProductId);
                if (cart == null)
                {
                    _db.ShoppingKarts.Add(scart);
                }
                else
                {
                    cart.Count += scart.Count;

                }
                _db.SaveChanges();
                var count = _db.ShoppingKarts.Where(i => i.ApplicationUserId == scart.ApplicationUserId).ToList().Count();
                HttpContext.Session.SetInt32(Diger.ssShopingCart, count);
                return RedirectToAction("Index");




            }
            else
            {
                var product = _db.Products.FirstOrDefault(c => c.Id == scart.Id);
                ShoppingKart cart = new ShoppingKart()
                {
                    Product = product,
                    ProductId = product.Id,
                    Price = product.Price,
                };
            }

            return View(scart);
        }
        public IActionResult CategoryDetails(int? Id, Guid StoreId, int p = 1)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var count = _db.ShoppingKarts.Where(c => c.ApplicationUserId == claim.Value).ToList().Count();
                HttpContext.Session.SetInt32(Diger.ssShopingCart, count);

            }
            if (Id != null)
            {
                var product = _db.Products.Where(i => i.CategoryId == Id && i.Active);

                ViewBag.KategoriId = Id;
                ViewBag.storeId = StoreId;
                return View(product.ToPagedList(p, 40));
            }
            else
            {
                PagedList<Product> product = new PagedList<Product>(null, p, 40);
                var prod = _db.Products.Include(c => c.Category).Where(c => c.Category.StoreId == StoreId).ToList();

                product = new PagedList<Product>(prod, p, 40);
                ViewBag.KategoriId = Id;
                ViewBag.storeId = StoreId;
                return View(product.ToPagedList(p, 40));
            }

        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult DistanceSellingContract()
        {
            return View();
        }
        public IActionResult CancellationRefund()
        {
            return View();
        }
        public IActionResult Hakkimizda()
        {
            return View();
        }
        public IActionResult iletisim()
        {
            EmailForm emailForm = new EmailForm();

            return View(emailForm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Iletisim([Bind("FirmaAdi,Aciklama")] EmailForm emailForm)
        {
            SenderEmail.Gonder("İstek Bildirimi"
                , $"<div class='row'>Firma Adı : '{emailForm.FirmaAdi}'</div> <div class'row'>İstek : '{emailForm.Aciklama}'</div>"
                , "yalcinvakif.7903@gmail.com");

            return RedirectToAction(nameof(Index));


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Degistir()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = _db.ApplicationUsers.Where(c => c.Id == claim.Value).FirstOrDefault().StoreId;
            var store = _db.Stores.Where(c => c.Id == user).FirstOrDefault();
            if (store.StoreIsActive)
            {
                store.StoreIsActive = false;
            }
            else
            {
                store.StoreIsActive = true;
            }
            _db.Update(store);
            await _db.SaveChangesAsync();
          return RedirectToAction("Index");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}