using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using laberegisterLIH.Data;
using laberegisterLIH.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace laberegisterLIH.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ApiController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        private readonly ILogger<ApiController> _logger;
        private readonly ILabRepository _repository;
        public ApiController(ILogger<ApiController> logger, ILabRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [HttpGet]
        [Route("examenes")]
         public IEnumerable<Examen> Get()
        {
                 // Prepare Email Message  

        try
      {
        MailMessage mailMsg = new MailMessage();

        // To
        mailMsg.To.Add(new MailAddress("ing.bevi@gmail.com", "bevi"));

        // From
        mailMsg.From = new MailAddress("santi.rebella87@gmail.com", "santi");

        // Subject and multipart/alternative Body
        mailMsg.Subject = "¿Cómo fue tu experiencia en LIH Laboratorio de Investigación Hormonal?";
        //string text = "<p>ERES MUY IMPORTANTE PARA NOSOTROS</p><p>Por favor, permítenos conocer tu opinión sobre nuestro servicio, esto nos ayudará a seguir mejorando para ofrecerte una mejor experiencia.</p><p>Muchas gracias por tus respuestas</p><p>¡Buscamos en tu interior, la clave de tu bienestar!</p><p>Diligenciar encuesta</p><p>Correo enviado el Miércoles 03 de Febrero de 2021 2:41 pm</p></p>Laboratorio de Investigación Hormonal</p>";
        string html = @"<p></p>
                  ERES MUY IMPORTANTE PARA NOSOTROS
                  </p>
                  <p>
                  Por favor, permítenos conocer tu opinión sobre nuestro servicio, esto nos ayudará a seguir mejorando para ofrecerte una mejor experiencia.
                  </p><p>
                  Muchas gracias por tus respuestas
                  </p><p>


                  ¡Buscamos en tu interior, la clave de tu bienestar!
                  </p><p>
                  <a href='http://qworkslablih.azurewebsites.net/feedback?id=12'>Diligenciar encuesta</a>
                  </p><p>

                  Correo enviado el Miércoles 03 de Febrero de 2021 2:41 pm
                  </p></p>
                  Laboratorio de Investigación Hormonal
                  </p>";
// mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
        mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

        // Init SmtpClient and send
        SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", Convert.ToInt32(587));
        System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("santi.rebella87@gmail.com", "r6007021");
        smtpClient.Credentials = credentials;
        smtpClient.EnableSsl = true;
        smtpClient.Send(mailMsg);
        Console.WriteLine("Email sent!");

         var accountSid = "ACc5147d20e801376be9c9820c71e3e093"; 
        var authToken = "85259c16213f9da06ab9d5aa173fc3e5"; 
        TwilioClient.Init(accountSid, authToken); 
 
        var messageOptions = new CreateMessageOptions( 
            new PhoneNumber("whatsapp:+59899852623")); 
        messageOptions.From = new PhoneNumber("whatsapp:+14155238886");    
        messageOptions.Body = "http://qworkslablih.azurewebsites.net/feedback?id=12";   
 
        var message = MessageResource.Create(messageOptions); 
        Console.WriteLine(message.Body); 

        
      }
        catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }

            var rng = new Random();
            var results = _repository.GetAllExams();
            return results;
        }



        [HttpPost]
        [Route("examanes/set")]
        public bool Set([FromBody] CalendarModel data)
        {
            return _repository.AddNewScheduleClientes(data.UserId, data.ExamId, data.Date, data.Time, data.SucursalId);
        }
                
        [HttpGet]
        [Route("GetSucursales")]
         public IEnumerable<Sucursal> GetSucursales()
         {
            var results = _repository.GetAllSucursales();
            return results;
        }
    }
}
        public class CalendarModel
        {
            public string UserId { get; set; }
            public string ExamId { get; set; }
            public string Date { get; set; }
            public string Time { get; set; }
            public string SucursalId { get; set; }
}