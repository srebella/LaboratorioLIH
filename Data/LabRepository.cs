using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using laberegisterLIH.Models;
using Microsoft.AspNetCore.Identity;
using System.IO;
using System.Drawing;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace laberegisterLIH.Data
{
    
    public class LabRepository : ILabRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LabRepository> _logger;
        private UserManager<ApplicationUser> _userManager;
        private IConfiguration _config;

        public LabRepository(ApplicationDbContext context, ILogger<LabRepository> logger, UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _config = config;
        }
        public IEnumerable<Examen> GetAllExams()
        {
            try
            {
                return _context.Examenes.OrderBy(e => e.Name).ToList();
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Failed to get all exams {ex}");
                return null;
            }
        }
        
        public IEnumerable<Sucursal> GetAllSucursales()
        {
            try
            {
                return _context.Sucursales.OrderBy(e => e.Name).ToList();
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Failed to get all exams {ex}");
                return null;
            }
        }
        
        public IEnumerable<Appointment> GetAllAppointments()
        {
            try
            {
                return _context.Appointments.Where(e => !e.IsDeleted).OrderBy(e => e.Date).ToList();
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Failed to get all appointment {ex}");
                return null;
            }
        }
        
        public IEnumerable<Appointment> GetAppointmentsByUser(string userId)
        {
            try
            {
                var a = _context.Appointments.Include(r=>r.User).Include(r=>r.Examen).Include(r=>r.Sucursal)
                            .Where(e => e.User.Id == userId && !e.IsDeleted).ToList();
                return a;
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Failed to get all appointment {ex}");
                return null;
            }
        }
        
        public bool DeleteAppointmentsById(string id)
        {
            try
            {
                var a = _context.Appointments.Where(e => e.Id == Int32.Parse(id)).FirstOrDefault();
                a.IsDeleted = true;
                _context.SaveChanges();
                return true;
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Failed to delete appointment id {id} {ex}");
                return false;
            }
        }
        
        public async System.Threading.Tasks.Task<int> AddNewScheduleClientesAsync(string userId, string examId, string date, string time, string sucursalId)
        {
            try
            {
                //get appuser
                var user = await _userManager.FindByIdAsync(userId);
                var exam = _context.Examenes.Where(u => u.Name == examId).FirstOrDefault();
                var sucursal = _context.Sucursales.Where(u => u.Name == sucursalId).FirstOrDefault();

                //update values
                var dtStr = date+ " " +time+":00";
                DateTime? dt = DateTime.ParseExact(dtStr, "yyyy-MM-dd HH:mm", null);
                var appt = new Appointment(){
                    User = user,
                    Examen = exam,
                    Date = (DateTime)dt,
                    Sucursal = sucursal,
                    CreatedOn = DateTime.UtcNow
                };

                //save 
                _context.Add(appt);
                if (_context.SaveChanges() > 0){
                    //Generate QR 
                    //var imageQR = GenerateQR();
                    //Attach image in email
                    SendEmail(user.UserName, dt.ToString(), time, sucursal.Name + " " + sucursal.Address, exam.Name);
                }
                return _context.SaveChanges();
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Failed to add new schedule {ex}");
                return 0;
            }            
        }
        public bool SaveAll()
        {
            try
            {
                return _context.SaveChanges() > 0;
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Failed to save data {ex}");
                return false;
            }            
        }

        
        static string BytesToString(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                using (StreamReader streamReader = new StreamReader(stream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        private void GenerateQR(){
              //  QRCodeGenerator _qrCode = new QRCodeGenerator();      
            // QRCodeData _qrCodeData = _qrCode.CreateQrCode("https://qworkslablih.azurewebsites.net/",QRCodeGenerator.ECCLevel.Q);      
            // QRCode qrCode = new QRCode(_qrCodeData);      
            // Bitmap qrCodeImage = qrCode.GetGraphic(20);

            // results.FirstOrDefault<Examen>().Name = BytesToString(BitmapToBytesCode(qrCodeImage, new MemoryStream()));

        }

        private void SendEmail(string EmailTo, string date, string hour, string sucursal, string examen){
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
                                Gracias por registrar su turno con nosotros
                                </p>
                                <p>
                                Sus datos del turno son:
                                </p>
                                <p>
                                Fecha y hora: " + date + @"
                                </p>
                                <p>
                                Examen: " + examen + @"
                                </p>
                                <p>
                                Sucursal: " + sucursal + @"
                                </p>

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

        private Byte[] BitmapToBytesCode(Bitmap qrCodeImage, MemoryStream stream)
        {
            qrCodeImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            return stream.ToArray();
        }
    }
}