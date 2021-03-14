using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using laberegisterLIH.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;

namespace laberegisterLIH.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _config;

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IConfiguration config)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _config = config;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "El email es requerido")]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);

                // await _emailSender.SendEmailAsync(
                //     Input.Email,
                //     "Reset Password",
                //     $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                SendEmail(Input.Email, HtmlEncoder.Default.Encode(callbackUrl));
                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }

        private void SendEmail(string EmailTo, string url){
            try
            {
                var mailMsg = new GmailEmailSender(new GmailEmailSettings(_config.GetValue<string>(
                        "AppIdentitySettings:Username"), _config.GetValue<string>(
                        "AppIdentitySettings:Password")));

                var message = EmailMessageBuilder
                                    .Init()
                                    .AddSubject("Cambio de Contraseña LIH Laboratorio de Investigación Hormonal")
                                    .AddFrom("qworks2021@gmail.com")
                                    .AddBody(@"<p></p>
                                Cambio de Contraseña LIH Laboratorio de Investigación Hormonal
                                </p>
                                <p>Por favor <a href='"+ url + @"'>haga clic aqui</a> para re-establecer su contraseña
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
