﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using IvaETicaret.Data;
using IvaETicaret.Email;
using IvaETicaret.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Encodings.Web;

namespace IvaETicaret.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;
        public List<SelectListItem> Stores;

        public bool Sec { get; set; }
        public RegisterModel(
        UserManager<IdentityUser> userManager,
        IUserStore<IdentityUser> userStore,
        SignInManager<IdentityUser> signInManager,
        ILogger<RegisterModel> logger,
        IEmailSender emailSender,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext db
     

        )
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _db = db;
    
        }


        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
            [Required]
            public string Name { get; set; }
            [Required]
            public string Surname { get; set; }
            public string? Adress { get; set; }
            public string? City { get; set; }
            public string? Country { get; set; }
            public string? PostaKodu { get; set; }
            public string? TelNo { get; set; }
            public string Role { get; set; }
            public Guid? StoreId { get; set; }
            public IEnumerable<SelectListItem> RoleList { get; set; }
            public bool secinmk { get; set; }
        }


        public async Task OnGetAsync(string title,bool secim, string? message, string returnUrl = null)
        {
           
            if (!string.IsNullOrEmpty(message))
            {
                ModelState.AddModelError(string.Empty, message);
            }
            ViewData["Title"] = title;
            Sec = secim;
            
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            Input = new InputModel()
            {
                RoleList = _roleManager.Roles.Where(i => i.Name != Diger.Role_Birey).Select(x => x.Name).Select(u => new SelectListItem
                {
                    Text = u,
                    Value = u
                })
            };
           
            var data = await _db.Stores.ToListAsync();
            if (Sec == false)
            {
                Stores = new List<SelectListItem>();
                foreach (var item in data)
                {
                    Stores.Add(new SelectListItem { Text = item.CompanyName, Value = item.Id.ToString() });
                }
            }

            //  Branches = new SelectList(await _db.Branches.ToListAsync(), nameof(Branch.Id).ToString(), nameof(Branch.CompanyName));
        }

        public async Task<IActionResult> OnPostAsync(string title,string returnUrl = null)
        {
        
            if (Input.StoreId != null && !User.IsInRole(Diger.Role_Admin))
            {
                var list = _db.Stores.Where(c => c.Id == Input.StoreId && c.IsActive==true).ToList();
                if (list.Count > 0)
                {
                    Sec = true;
                }
                else
                {
                    return RedirectToPage("Register", new { secim = true, message = "Mağaza Keyiniz Yanlış Lütfen Mailinizdeki Keyi Giriniz." });
                }
             

            }
            if (title == "Mağaza Kullanıcı Kayıt" && Input.StoreId==null && !User.IsInRole(Diger.Role_Admin))
            {
                return RedirectToPage("Register", new { secim = true, message = "Mağaza Keyi Giriniz",title= "Mağaza Kullanıcı Kayıt" });
            }


            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    PostaKodu = Input.PostaKodu,
                    StoreId = Input.StoreId,
                    Name = Input.Name,
                    Surname = Input.Surname,
                    Role = Input.Role,
                   

                };
                //if (!user.BranchId.HasValue)
                //{
                //    user.BranchId = 1;
                //}


                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    if (!await _roleManager.RoleExistsAsync(Diger.Role_Admin))
                    {

                        await _roleManager.CreateAsync(new IdentityRole(Diger.Role_Admin));
                    }
                    if (!await _roleManager.RoleExistsAsync(Diger.Role_User))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(Diger.Role_User));
                    }
                    if (!await _roleManager.RoleExistsAsync(Diger.Role_Birey))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(Diger.Role_Birey));
                    }
                    if (!await _roleManager.RoleExistsAsync(Diger.Role_Bayi))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(Diger.Role_Bayi));
                    }
                    if (user.Role == null && Input.StoreId == null && Sec == false)
                    {
                        await _userManager.AddToRoleAsync(user, Diger.Role_User);
                    }
                    else
                    {
                        if (Sec == true && Input.StoreId != null)
                        {
                          
                                await _userManager.AddToRoleAsync(user, Diger.Role_Bayi);
                           


                        }

                        else
                        {
                            await _userManager.AddToRoleAsync(user, user.Role);
                        }

                    }

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    SenderEmail.Gonder("e-postanızı onaylayın", $"Lütfen hesabınızı onaylayın <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Buraya Tıkla</a>.", user.Email
                       );


                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        if (user.Role == null)
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "User", new { Area = "Admin" });
                        }
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}
