using IvaETicaret.Data;
using IvaETicaret.Email;
using IvaETicaret.Models;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace IvaETicaret.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(ApplicationDbContext db, IEmailSender emailSender, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _emailSender = emailSender;
            _userManager = userManager;
        }
        //Sipariş Başlığı Sayfası
        public IActionResult Sumary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVM = new ShoppingCartVM()
            {
                OrderHeader = new OrderHeader(),
                ListCart = _db.ShoppingKarts.Where(i => i.ApplicationUserId == claim.Value).Include(i => i.Product).ThenInclude(c=>c.Category)

            };

            ShoppingCartVM.OrderHeader.OrderTotal = 0;
            var user = _db.ApplicationUsers.FirstOrDefault(c => c.Id == claim.Value);
            ShoppingCartVM.OrderHeader.ApplicationUser = user;
            ShoppingCartVM.OrderHeader.StoreId = _db.ShoppingKarts.Where(i => i.ApplicationUserId == claim.Value).Include(i => i.Product).ThenInclude(c => c.Category).FirstOrDefault().Product.Category.StoreId;
            ShoppingCartVM.OrderHeader.Name = user.Name;
            ShoppingCartVM.OrderHeader.SurName = user.Surname;
            foreach (var item in ShoppingCartVM.ListCart)
            {
                item.Price = item.Product.Price;
                ShoppingCartVM.OrderHeader.OrderTotal += (item.Count * item.Product.Price);

            }
            List<SelectListItem> Adress = _db.Adress.Where(c => c.ApplicationUserId == claim.Value).Select(c => new SelectListItem
            {
                Text = c.AdressTitle,
                Value = c.Id.ToString()
            }).ToList();
            List<SelectListItem> odemeTur = _db.OdemeTurleri.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            }).ToList();
            ViewBag.addressId = Adress;
            ViewBag.odemeTur = odemeTur;
            return View(ShoppingCartVM);
        }
        //Siparişi Kaydetme butonu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Sumary(ShoppingCartVM model)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var us = _db.ApplicationUsers.Where(c => c.Id == claim.Value).FirstOrDefault();
            var adress = _db.Adress.Where(c => c.Id == model.OrderHeader.AdressId).FirstOrDefault();
            model.OrderHeader.ApplicationUser = us;
            model.OrderHeader.Adress = adress;
            ShoppingCartVM.ListCart = _db.ShoppingKarts.Where(c => c.ApplicationUserId == claim.Value).Include(c => c.Product);
            ShoppingCartVM.OrderHeader.OrderStatus = Diger.Durum_Beklemede;
            ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;
            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            var payment = PaymentProcess(model);
            if (payment.Status == "success")
            {
                _db.OrderHeaders.Add(ShoppingCartVM.OrderHeader);
                _db.SaveChanges();
                foreach (var item in ShoppingCartVM.ListCart)
                {
                    item.Price = item.Product.Price;
                    OrderDetail orderDetail = new OrderDetail
                    {
                        ProductId = item.ProductId,
                        OrderId = ShoppingCartVM.OrderHeader.Id,
                        Price = item.Price,
                        Count = item.Count,

                    };
                    ShoppingCartVM.OrderHeader.OrderTotal += item.Count * item.Product.Price;
                    model.OrderHeader.OrderTotal += item.Count * item.Product.Price;
                    _db.OrderDetails.Add(orderDetail);

                }
                _db.ShoppingKarts.RemoveRange(ShoppingCartVM.ListCart);
                _db.SaveChanges();
                HttpContext.Session.SetInt32(Diger.ssShopingCart, 0);
                return RedirectToAction("SiparisTamam");
            }
            else if (payment.Status == "failure")
            {
                return RedirectToAction("KartHata");
            }
            return View();
       
        }
        //sanal pos
        private Payment PaymentProcess(ShoppingCartVM model)
        {
            Options options = new Options();
            options.ApiKey = "sandbox-9E4zw5Qm3seFVg8LWLwaGSnrPQC86ZBS";
            options.SecretKey = "sandbox-fwJkD2yR0RjUSXi7XiThZtmMHFSx5L7k";
            options.BaseUrl = "https://sandbox-api.iyzipay.com";

            CreatePaymentRequest request = new CreatePaymentRequest();
            request.Locale = Locale.TR.ToString();
            request.ConversationId =new Random().Next(1111,9999).ToString();
            request.Price = model.OrderHeader.OrderTotal.ToString();
            request.PaidPrice = model.OrderHeader.OrderTotal.ToString(); ;
            request.Currency = Currency.TRY.ToString();
            request.Installment = 1;
            request.BasketId = "B67832";
            request.PaymentChannel = PaymentChannel.WEB.ToString();
            request.PaymentGroup = PaymentGroup.PRODUCT.ToString();

            //PaymentCard paymentCard = new PaymentCard();
            //paymentCard.CardHolderName = "John Doe";
            //paymentCard.CardNumber = "5528790000000008";
            //paymentCard.ExpireMonth = "12";
            //paymentCard.ExpireYear = "2030";
            //paymentCard.Cvc = "123";
            //paymentCard.RegisterCard = 0;
            //request.PaymentCard = paymentCard;

            PaymentCard paymentCard = new PaymentCard();
            paymentCard.CardHolderName = model.OrderHeader.CartName;
            paymentCard.CardNumber = model.OrderHeader.CartNumber;
            paymentCard.ExpireMonth = model.OrderHeader.ExpirationMonth;
            paymentCard.ExpireYear = model.OrderHeader.ExpirationYear;
            paymentCard.Cvc = model.OrderHeader.Cvc;
            paymentCard.RegisterCard = 0;
            request.PaymentCard = paymentCard;

            Buyer buyer = new Buyer();
            buyer.Id = model.OrderHeader.Id.ToString();
            buyer.Name = model.OrderHeader.ApplicationUser.Name;
            buyer.Surname = model.OrderHeader.ApplicationUser.Surname;
            buyer.GsmNumber = model.OrderHeader.ApplicationUser.PhoneNumber; 
            buyer.Email = "email@email.com";
            buyer.IdentityNumber = "74300864791";
            buyer.LastLoginDate = "2015-10-05 12:43:35";
            buyer.RegistrationDate = "2013-04-21 15:12:09";
            buyer.RegistrationAddress = model.OrderHeader.Adress.TamAdres;
            buyer.Ip = "85.34.78.112";
            buyer.City = model.OrderHeader.Adress.Il;
            buyer.Country = "Turkey";
            buyer.ZipCode = model.OrderHeader.ApplicationUser.PostaKodu;
            request.Buyer = buyer;

            Address shippingAddress = new Address();
            shippingAddress.ContactName = "Jane Doe";
            shippingAddress.City = "Istanbul";
            shippingAddress.Country = "Turkey";
            shippingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            shippingAddress.ZipCode = "34742";
            request.ShippingAddress = shippingAddress;

            Address billingAddress = new Address();
            billingAddress.ContactName = "Jane Doe";
            billingAddress.City = "Istanbul";
            billingAddress.Country = "Turkey";
            billingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            billingAddress.ZipCode = "34742";
            request.BillingAddress = billingAddress;

            List<BasketItem> basketItems = new List<BasketItem>();
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            foreach (var item in _db.ShoppingKarts.Where(c=>c.ApplicationUserId==claim.Value).Include(c=>c.Product))
            {
                basketItems.Add(new BasketItem()
                {
                    Id = item.Id.ToString(),
                    Name = item.Product.Title,
                    Category1 = item.Product.CategoryId.ToString(),
                    ItemType=BasketItemType.PHYSICAL.ToString(),
                    Price=(item.Price * item.Count).ToString()
                });
            }
            request.BasketItems = basketItems;

          return Payment.Create(request, options);
        }

        public IActionResult SiparisTamam()
        {
            return View();
        } 
        public IActionResult KartHata()
        {
            return View();
        }
        //Sepetteki Ürünler Sayfası
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVM = new ShoppingCartVM()
            {
                OrderHeader = new OrderHeader(),
                ListCart = _db.ShoppingKarts.Where(i => i.ApplicationUserId == claim.Value).Include(i => i.Product)

            };

            ShoppingCartVM.OrderHeader.OrderTotal = 0;
            ShoppingCartVM.OrderHeader.ApplicationUser = _db.ApplicationUsers.FirstOrDefault(c => c.Id == claim.Value);
            foreach (var item in ShoppingCartVM.ListCart)
            {
                ShoppingCartVM.OrderHeader.OrderTotal += (item.Count * item.Product.Price);
            }
            return View(ShoppingCartVM);
        }
        //Email doğrulanmamışsa Doğrulama göndermek için çalışan Kod
        [HttpPost]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPOST()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = _db.ApplicationUsers.FirstOrDefault(c => c.Id == claim.Value);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Doğrulama Emaili Boş");
            }
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = user.Id, code = code },
                protocol: Request.Scheme);

            SenderEmail.Gonder("E-Postanızı Onaylayın", $"Hesabınızı Onaylamak İçin Tıklayın <a class='btn btn-success' href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Buraya Tıkla</a>.", user.Email
               );
            ModelState.AddModelError(string.Empty, "Email Doğrulama Kodu Gönder.");
            return RedirectToAction("Success");
        }
        //Doğrulama Mailinin gittiğine Dair sayfa
        public IActionResult Success()
        {
            return View();
        }
        //Spetteki ürünün sayısını Arttırma
        public IActionResult Add(int cardId)
        {
            var cart = _db.ShoppingKarts.FirstOrDefault(i => i.Id == cardId);
            cart.Count += 1;
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        //Spetteki ürünün sayısını Azaltma
        public IActionResult Decrease(int cardId)
        {
            var cart = _db.ShoppingKarts.FirstOrDefault(i => i.Id == cardId);
            if (cart.Count == 1)
            {
                var count = _db.ShoppingKarts.Where(c => c.ApplicationUserId == cart.ApplicationUserId).ToList().Count();
                _db.ShoppingKarts.Remove(cart);
                _db.SaveChanges();
                HttpContext.Session.SetInt32(Diger.ssShopingCart, count - 1);
            }
            else
            {
                cart.Count -= 1;
                _db.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
        //Sepetteki Ürünü silme
        public IActionResult Remove(int cardId)
        {
            var cart = _db.ShoppingKarts.FirstOrDefault(i => i.Id == cardId);

            var count = _db.ShoppingKarts.Where(c => c.ApplicationUserId == cart.ApplicationUserId).ToList().Count();
            _db.ShoppingKarts.Remove(cart);
            _db.SaveChanges();
            HttpContext.Session.SetInt32(Diger.ssShopingCart, count - 1);


            return RedirectToAction(nameof(Index));
        }

    }
}
