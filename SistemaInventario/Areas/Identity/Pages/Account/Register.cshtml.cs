﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using SistemaInventario.Modelos;
using SistemaInventario.Utilidades;

namespace SistemaInventario.Areas.Identity.Pages.Account
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

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
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
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength =4 )]
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
            public string NumeroTelefono { get; set; }
            [Required]
            public string Nombres { get; set; }
            [Required]
            public string Apellidos { get; set; }
            [Required]
            public string Direccion { get; set; }
            [Required]
            public string Ciudad { get; set; }
            [Required]
            public string Pais { get; set; }
            public string Role { get; set; }

            public IEnumerable<SelectListItem> ListaRoles { get; set; }

        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;

           
            

                Input = new InputModel()
                {
                    ListaRoles = _roleManager.Roles.Where(r => r.Name != DefinicionesEstaticas.RoleCliente).Select(n => n.Name).Select(l => new SelectListItem
                    {
                        Text = l,
                        Value = l
                    })
                };

            

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();


            bool isAdmin = User.IsInRole(DefinicionesEstaticas.RoleAdmin);


            if (ModelState.IsValid)
            {
                // var user = CreateUser();

                var user = new Usuario
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    PhoneNumber = Input.NumeroTelefono,
                    Nombres = Input.Nombres,
                    Apellidos = Input.Apellidos,
                    Dirección = Input.Direccion,
                    Ciudad = Input.Ciudad,
                    Pais = Input.Pais,
                    Role = Input.Role,



                };

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    if (!await _roleManager.RoleExistsAsync(DefinicionesEstaticas.RoleAdmin))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(DefinicionesEstaticas.RoleAdmin));
                    }

                    if (!await _roleManager.RoleExistsAsync(DefinicionesEstaticas.RoleCliente))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(DefinicionesEstaticas.RoleCliente));
                    }

                    if (!await _roleManager.RoleExistsAsync(DefinicionesEstaticas.RoleInventario))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(DefinicionesEstaticas.RoleInventario));
                    }

                    if (user.Role == null)//el valor lo recibe desde el page
                    {
                        await _userManager.AddToRoleAsync(user, DefinicionesEstaticas.RoleCliente);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, user.Role);
                    }


                    var userId = await _userManager.GetUserIdAsync(user);


                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirmar tu Email",
                        $"Confirma tu cuenta <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Haz click en el enlace</a>.");



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
                            return RedirectToAction("Index", "Usuario", new {Area = "Admin"});
                        }

                    }
                }

                Input = new InputModel()
                {
                    ListaRoles = _roleManager.Roles.Where(r => r.Name != DefinicionesEstaticas.RoleCliente).Select(n => n.Name).Select(l => new SelectListItem
                    {
                        Text = l,
                        Value = l
                    })
                };

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            if (isAdmin)
            {
                return Page();
            }
            else
            {
                // Si el usuario no es administrador, redirigir a la página de inicio u otra página apropiada
                return RedirectToPage("/Index");
            }
            // If we got this far, something failed, redisplay form
           
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
