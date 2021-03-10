using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using laberegisterLIH.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace laberegisterLIH.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _config;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _config = config;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "El nombre es requerido")]
            [Display(Name = "Nombre")]
            [StringLength(100, ErrorMessage = "El nombre es requerido", MinimumLength = 1)]
            public string Name { get; set; }
           [Required(ErrorMessage = "El apellido es requerido")]
            [Display(Name = "Apellido")]
            [StringLength(100, ErrorMessage = "El nombre es requerido", MinimumLength = 1)]
            public string LastName { get; set; }  

            [Required(ErrorMessage = "El email es requerido")]
            [EmailAddress]
            [Display(Name = "Email")]
            [StringLength(100, ErrorMessage = "La contraseña debe tener al menos {0} caracteres", MinimumLength = 6)]
            public string Email { get; set; }
            [Required(ErrorMessage = "La contraseña es requerida")]
            [StringLength(100, ErrorMessage = "La contraseña debe tener al menos {0} caracteres", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Contraseña")]
            public string Password { get; set; }

            [Required(ErrorMessage = "La confirmación de contraseña es requerida")]
            [DataType(DataType.Password)]
            [Display(Name = "Confirmar contraseña")]
            [Compare("Password", ErrorMessage = "La contraseña y la confirmación deben coincidir")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email, Name = Input.Name, Surname = Input.LastName };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    // await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //     $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                    SendEmail(Input.Email);
                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _userManager.ConfirmEmailAsync(user, code);
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
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

        private void SendEmail(string EmailTo){
            try
            {
                var mailMsg = new GmailEmailSender(new GmailEmailSettings(_config.GetValue<string>(
                        "AppIdentitySettings:Username"), _config.GetValue<string>(
                        "AppIdentitySettings:Password")));

                var message = EmailMessageBuilder
                                    .Init()
                                    .AddSubject("Gracias por registrar tu turno con LIH Laboratorio de Investigación Hormonal")
                                    .AddFrom("qworks2021@gmail.com")
                                    .AddBody(@"<p></p>
                                Gracias por registrarse con nosotros
                                </p>
                                <p>Por favor inicie sesión ingresando <a href='https://qworkslablih.azurewebsites.net/Identity/Account/Login'>aqui</a>
                                </p>
                                <p>
                                Correo enviado el " + DateTime.UtcNow + @"
                                </p>
                                <p>
                                Laboratorio de Investigación Hormonal
                                </p>")
                                //     .AddBody(@"<p></p>
                                // ERES MUY IMPORTANTE PARA NOSOTROS
                                // </p>
                                // <p>
                                // Por favor, permítenos conocer tu opinión sobre nuestro servicio, esto nos ayudará a seguir mejorando para ofrecerte una mejor experiencia.
                                // </p><p>
                                // Muchas gracias por tus respuestas
                                // </p><p>


                                // ¡Buscamos en tu interior, la clave de tu bienestar!
                                // </p><p>
                                // <a href='http://qworkslablih.azurewebsites.net/feedback?id=12'>Diligenciar encuesta</a>
                                // </p><p>

                                // Correo enviado el " + DateTime.UtcNow + @"
                                // </p></p>
                                // Laboratorio de Investigación Hormonal
                                // </p>")
                                    .AddTo(EmailTo)
                                    .Build();

                // Send Email Message
                var response = mailMsg.SendAsync(message).Result;

                Console.WriteLine(response);


                //Twilio
                // var accountSid = "ACc5147d20e801376be9c9820c71e3e093"; 
                // var authToken = "85259c16213f9da06ab9d5aa173fc3e5"; 
                // TwilioClient.Init(accountSid, authToken); 

                // var messageOptions = new CreateMessageOptions( 
                //     new PhoneNumber("whatsapp:+59899852623")); 
                // messageOptions.From = new PhoneNumber("whatsapp:+14155238886");    
                // messageOptions.Body = "http://qworkslablih.azurewebsites.net/feedback?id=12";   

                // var messages = MessageResource.Create(messageOptions); 
                // Console.WriteLine(messages.Body); 


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
