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

namespace laberegisterLIH.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ExamenesController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        private readonly ILogger<ExamenesController> _logger;
        private readonly ILabRepository _repository;
        public ExamenesController(ILogger<ExamenesController> logger, ILabRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [HttpGet]
         public IEnumerable<Examen> Get()
        {
                 // Prepare Email Message  
            var message = EmailMessageBuilder  
                            .Init()  
                            .AddSubject("subject")  
                            .AddFrom("mail@mailinator.com")  
                            .AddBody("body")  
                            .AddTo("test987@mailinator.com")  
                            .Build();  
   
            // Send Email Message  
            AzureEmailSender sender =  
new AzureEmailSender(new AzureEmailSettings("rAMVD750lDTlFP6"));  
            var response = sender.SendAsync(message).Result;  
            Console.WriteLine(response.StatusCode);
// try
//       {
//         MailMessage mailMsg = new MailMessage();

//         // To
//         mailMsg.To.Add(new MailAddress("test987@mailinator.com", "To Name"));

//         // From
//         mailMsg.From = new MailAddress("mail@mailinator.com", "From Name");

//         // Subject and multipart/alternative Body
//         mailMsg.Subject = "subject";
//         string text = "text body";
//         string html = @"<p>html body</p>";
//         mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
//         mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

//         // Init SmtpClient and send
//         SmtpClient smtpClient = new SmtpClient("smtp.sendgrid.net", Convert.ToInt32(587));
//         System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("azure_8f53af5aabe27a91e4ca619652f1fa9f@azure.com", "rAMVD750lDTlFP6");
//         smtpClient.Credentials = credentials;
//         smtpClient.EnableSsl = true;
//         smtpClient.Send(mailMsg);
//       }
//         catch (Exception ex)
//       {
//         Console.WriteLine(ex.Message);
//       }

            var rng = new Random();
            var results = _repository.GetAllExams();
            return results;
        }

        [HttpPost]
         public bool Set(string userId, string examId, string date, string time, string sucursalId)
        {
            var rng = new Random();
            return _repository.AddNewScheduleClientes(userId, examId, date, time, sucursalId);
        }
    }
}
