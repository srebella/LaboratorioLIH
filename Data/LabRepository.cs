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
using QRCoder;

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
        
        public Appointment GetAppointmentById(string id)
        {
            try
            {
                var a = _context.Appointments.Include(r=>r.Examen).Include(r=>r.Sucursal)
                    .Where(e => e.Id == Int32.Parse(id)).FirstOrDefault();
                return a;
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Failed to delete appointment id {id} {ex}");
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
                DateTime? dt = DateTime.ParseExact(dtStr, "yyyy-M-d HH:mm", CultureInfo.InvariantCulture);
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
                    var imageQR = GenerateQR(appt.Id);
                    //Attach image in email
                    SendEmail(user.UserName, dt.ToString(), time, sucursal.Name + " " + sucursal.Address, exam.Name,imageQR);
                }
                return _context.SaveChanges();
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Failed to add new schedule {ex}");
                return 0;
            }            
        }
        public async System.Threading.Tasks.Task<int> UpdateScheduleClientesAsync(string userId, string apptid, string examId, string date, string time, string sucursalId)
        {
            try
            {
                //get appuser
                var user = await _userManager.FindByIdAsync(userId);
                var exam = _context.Examenes.Where(u => u.Name == examId).FirstOrDefault();
                var sucursal = _context.Sucursales.Where(u => u.Name == sucursalId).FirstOrDefault();
                var appointnment = _context.Appointments.Where(u => u.Id == Int32.Parse(apptid)).FirstOrDefault();

                //update values
                var dtStr = date+ " " +time+":00";
                DateTime? dt = DateTime.ParseExact(dtStr, "yyyy-M-d HH:mm", CultureInfo.InvariantCulture);
                // var appt = new Appointment(){
                //     User = user,
                //     Examen = exam,
                //     Date = (DateTime)dt,
                //     Sucursal = sucursal,
                //     CreatedOn = DateTime.UtcNow
                // };
                appointnment.User = user;
                appointnment.Examen = exam;
                appointnment.Date = (DateTime)dt;
                appointnment.Sucursal = sucursal;
                 appointnment.CreatedOn = DateTime.UtcNow;
                //_context.Entry(appointnment).State = EntityState.Modified;
                //save 
               
                if (_context.SaveChanges() > 0){
                    //Generate QR 
                    //var imageQR = GenerateQR(apptid);
                    //Attach image in email
                    SendEmail(user.UserName, dt.ToString(), time, sucursal.Name + " " + sucursal.Address, exam.Name);
                }

                return 1;
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

        private Byte[] GenerateQR(int apptId){
            QRCodeGenerator _qrCode = new QRCodeGenerator();      
            QRCodeData _qrCodeData = _qrCode.CreateQrCode("https://qworkslablih.azurewebsites.net/turnos?id="+apptId,QRCodeGenerator.ECCLevel.Q);      
            QRCode qrCode = new QRCode(_qrCodeData);      
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            return BitmapToBytesCode(qrCodeImage, new MemoryStream());

        }

        private void SendEmail(string EmailTo, string date, string hour, string sucursal, string examen, Byte[] qrImageStr = null){
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
                                Examen: " + examen + Convert.ToBase64String(qrImageStr) + @"
                                </p>
                                <p>
                                Sucursal: " + sucursal + qrImageStr + @"
                                </p>
 <img src='data:image/png;base64,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C/1GSW20kdRGG0mvoRsktdFGUhttXK6NNpKU8YIh+iiT2mjjcm20kdRGG2Z/pS30GyW10UZSG20kvYZukNRGG0lttHG5NtpIUsYLhuijTGqjjcu10UZSG22Y/ZW20G+U1EYbSW20kfQaukFSG20ktdHG5dpoI0kZLxiijzKpjTYu10YbSW20YfZX2kK/UVIbbSS10UbSa+gGSW20kdRGG5dro40kZbxgiD7KpDbauFwbbSS10YbZX2kL/UZJbbSR1EYbSa+hGyS10UZSG21cro02kpTxgiH6KJPaaONybbSR1EYbZn+lLfQbJbXRRlIbbSS9hm6Q1EYbSW20cbk22khSxguG6KNMaqONy7XRRlIbbZj9lbbQb5TURhtJbbSR9Bq6QVIbbSS10cbl2mgjSRkvGKKPMqmNNi7XRhtJbbRh9lfaQr9RUhttJLXRRtJr6AZJbbSR1EYbl2ujjSRlvGCIPsqkNtq4XBttJLXRhtlfaQv9RklttJHURhtJr6EbJLXRRlIbbVyujTaSlPGCIfook9po43JttJHURhtmf6Ut9BsltdFGUhttJL2GbpDURhtJbbRxuTbaSFLGC4boo0xqo43LtdFGUhttmP2VttBvlNRGG0lttJH0GrpBUhttJLXRxuXaaCNJGS8Yoo8yqY02LtdGG0lttGH2V9pCv1FSG20ktdFG0mvoBklttJHURhuXa6ONJGW8YIg+yqQ22rhcG20ktdGG2V9pC/1GSW20kdRGG0mvoRsktdFGUhttXK6NNpKU8YIh+iiT2mjjcm20kdRGG0u10UZSG20ktdFG0jp68+XaaONybbSx1Dp6c1IbbVyujTaSlPGCIfook9po43JttJHURhtLtdFGUhttJLXRRtI6evPl2mjjcm20sdQ6enNSG21cro02kpTxgiH6KJPaaONybbSR1EYbS7XRRlIbbSS10UbSOnrz5dpo43JttLHUOnpzUhttXK6NNpKU8YIh+iiT2mjjcm20kdRGG0u10UZSG20ktdFG0jp68+XaaONybbSx1Dp6c1IbbVyujTaSlPGCIfook9po43JttJHURhtLtdFGUhttJLXRRtI6evPl2mjjcm20sdQ6enNSG21cro02kpTxgiH6KJPaaONybbSR1EYbS7XRRlIbbSS10UbSOnrz5dpo43JttLHUOnpzUhttXK6NNpKU8YIh+iiT2mjjcm20kdRGG0u10UZSG20ktdFG0jp68+XaaONybbSx1Dp6c1IbbVyujTaSlPGCIfook9po43JttJHURhtLtdFGUhttJLXRRtI6evPl2mjjcm20sdQ6enNSG21cro02kpTxgiH6KJPaaONybbSR1EYbS7XRRlIbbSS10UbSOnrz5dpo43JttLHUOnpzUhttXK6NNpKU8YIh+iiT2mjjcm20kdRGG0u10UZSG20ktdFG0jp68+XaaONybbSx1Dp6c1IbbVyujTaSlPGCIfook9po43JttJHURhtLtdFGUhttJLXRRtI6evPl2mjjcm20sdQ6enNSG21cro02kpTxgiH6KJPaaONybbSR1EYbS7XRRlIbbSS10UbSOnrz5dpo43JttLHUOnpzUhttXK6NNpKU8YIh+iiT2mjjcm20kdRGG0u10UZSG20ktdFG0jp68+XaaONybbSx1Dp6c1IbbVyujTaSlPGCIfook9po43JttJHURhtLtdFGUhttJLXRRtI6evPl2mjjcm20sdQ6enNSG21cro02kpTxgiH6KJPaaONybbSR1EYbS7XRRlIbbSS10UbSOnrz5dpo43JttLHUOnpzUhttXK6NNpKU8YIh+iiT2mjjcm20kdRGG0u10UZSG20ktdFG0jp68+XaaONybbSx1Dp6c1IbbVyujTaSlPGCIfook9po43JttJHURhtLtdFGUhttJLXRRtI6evPl2mjjcm20sdQ6enNSG21cro02kpTxgiH6KJPaaONybbSR1EYbS7XRRlIbbSS10UbSOnrz5dpo43JttLHUOnpzUhttXK6NNpKU8YIh+iiT2mjjcm20kdRGG0u10UZSG20ktdFG0jp68+XaaONybbSx1Dp6c1IbbVyujTaSlPGCIfook9po43JttJHURhtLtdFGUhttJLXRRtI6evPl2mjjcm20sdQ6enNSG21cro02kpTxgiH6KJPaaONybbSR1EYbS72GbrBUG20kvYZukNRGG0u9hm6wVBttJLXRxuXaaCNJGS8Yoo8yqY02LtdGG0lttLHUa+gGS7XRRtJr6AZJbbSx1GvoBku10UZSG21cro02kpTxgiH6KJPaaONybbSR1EYbS72GbrBUG20kvYZukNRGG0u9hm6wVBttJLXRxuXaaCNJGS8Yoo8yqY02LtdGG0lttLHUa+gGS7XRRtJr6AZJbbSx1GvoBku10UZSG21cro02kpTxgiH6KJPaaONybbSR1EYbS72GbrBUG20kvYZukNRGG0u9hm6wVBttJLXRxuXaaCNJGS8Yoo8yqY02LtdGG0lttLHUa+gGS7XRRtJr6AZJbbSx1GvoBku10UZSG21cro02kpTxgiH6KJPaaONybbSR1EYbS72GbrBUG20kvYZukNRGG0u9hm6wVBttJLXRxuXaaCNJGS8Yoo8yqY02LtdGG0lttLHUa+gGS7XRRtJr6AZJbbSx1GvoBku10UZSG21cro02kpTxgiH6KJPaaONybbSR1EYbS72GbrBUG20kvYZukNRGG0u9hm6wVBttJLXRxuXaaCNJGS8Yoo8yqY02LtdGG0lttLHUa+gGS7XRRtJr6AZJbbSx1GvoBku10UZSG21cro02kpTxgiH6KJPaaONybbSR1EYbS72GbrBUG20kvYZukNRGG0u9hm6wVBttJLXRxuXaaCNJGS8Yoo8yqY02LtdGG0lttLHUa+gGS7XRRtJr6AZJbbSx1GvoBku10UZSG21cro02kpTxgiH6KJPaaONybbSR1EYbS72GbrBUG20kvYZukNRGG0u9hm6wVBttJLXRxuXaaCNJGS8Yoo8yqY02LtdGG0lttLHUa+gGS7XRRtJr6AZJbbSx1GvoBku10UZSG21cro02kpTxgiH6KJPaaONybbSR1EYbS72GbrBUG20kvYZukNRGG0u9hm6wVBttJLXRxuXaaCNJGS8Yoo8yqY02LtdGG0lttLHUa+gGS7XRRtJr6AZJbbSx1GvoBku10UZSG21cro02kpTxgiH6KJPaaONybbSR1EYbS72GbrBUG20kvYZukNRGG0u9hm6wVBttJLXRxuXaaCNJGS8Yoo8yqY02LtdGG0lttLHUa+gGS7XRRtJr6AZJbbSx1GvoBku10UZSG21cro02kpTxgiH6KJPaaONybbSR1EYbS72GbrBUG20kvYZukNRGG0u9hm6wVBttJLXRxuXaaCNJGS8Yoo8yqY02LtdGG0lttLHUa+gGS7XRRtJr6AZJbbSx1GvoBku10UZSG21cro02kpTxgiH6KJO0hX6jpNfQDZLaaCOpjTaS2mgjaR29OWkdvdm+10YbS7XRRpK20G+UpIwXDNFHmaQt9BslvYZukNRGG0lttJHURhtJ6+jNSevozfa9NtpYqo02krSFfqMkZbxgiD7KJG2h3yjpNXSDpDbaSGqjjaQ22khaR29OWkdvtu+10cZSbbSRpC30GyUp4wVD9FEmaQv9RkmvoRsktdFGUhttJLXRRtI6enPSOnqzfa+NNpZqo40kbaHfKEkZLxiijzJJW+g3SnoN3SCpjTaS2mgjqY02ktbRm5PW0Zvte220sVQbbSRpC/1GScp4wRB9lEnaQr9R0mvoBklttJHURhtJbbSRtI7enLSO3mzfa6ONpdpoI0lb6DdKUsYLhuijTNIW+o2SXkM3SGqjjaQ22khqo42kdfTmpHX0ZvteG20s1UYbSdpCv1GSMl4wRB9lkrbQb5T0GrpBUhttJLXRRlIbbSStozcnraM32/faaGOpNtpI0hb6jZKU8YIh+iiTtIV+o6TX0A2S2mgjqY02ktpoI2kdvTlpHb3ZvtdGG0u10UaSttBvlKSMFwzRR5mkLfQbJb2GbpDURhtJbbSR1EYbSevozUnr6M32vTbaWKqNNpK0hX6jJGW8YIg+yiRtod8o6TV0g6Q22khqo42kNtpIWkdvTlpHb7bvtdHGUm20kaQt9BslKeMFQ/RRJmkL/UZJr6EbJLXRRlIbbSS10UbSOnpz0jp6s32vjTaWaqONJG2h3yhJGS8Yoo8ySVvoN0p6Dd0gqY02ktpoI6mNNpLW0ZuT1tGb7XtttLFUG20kaQv9RknKeMEQfZRJ2kK/UdJr6AZJbbSR1EYbSW20kbSO3py0jt5s32ujjaXaaCNJW+g3SlLGC4boo0zSFvqNkl5DN0hqo42kNtpIaqONpHX05qR19Gb7XhttLNVGG0naQr9RkjJeMEQfZZK20G+U9Bq6QVIbbSS10UZSG20kraM3J62jN9v32mhjqTbaSNIW+o2SlPGCIfook7SFfqOk19ANktpoI6mNNpLaaCNpHb05aR292b7XRhtLtdFGkrbQb5SkjBcM0UeZpC30GyW9hm6Q1EYbSW20kdRGG0nr6M1J6+jN9r022liqjTaStIV+oyRlvGCIPsokbaHfKOk1dIOkNtpIaqONpDbaSFpHb05aR2+277XRxlJttJGkLfQbJSnjBUP0USZpC/1GSa+hGyS10UZSG20ktdFG0jp6c9I6erN9r402lmqjjSRtod8oSRkvGKKP0uyv2mgjqY02ktpoI6mNNpLaaCOpjTaS2mgjqY02ktpoI6mNNpLaaCOpjTbM/koZLxiij9Lsr9poI6mNNpLaaCOpjTaS2mgjqY02ktpoI6mNNpLaaCOpjTaS2mgjqY02zP5KGS8Yoo/S7K/aaCOpjTaS2mgjqY02ktpoI6mNNpLaaCOpjTaS2mgjqY02ktpoI6mNNsz+ShkvGKKP0uyv2mgjqY02ktpoI6mNNpLaaCOpjTaS2mgjqY02ktpoI6mNNpLaaCOpjTbM/koZLxiij9Lsr9poI6mNNpLaaCOpjTaS2mgjqY02ktpoI6mNNpLaaCOpjTaS2mgjqY02zP5KGS8Yoo/S7K/aaCOpjTaS2mgjqY02ktpoI6mNNpLaaCOpjTaS2mgjqY02ktpoI6mNNsz+ShkvGKKP0uyv2mgjqY02ktpoI6mNNpLaaCOpjTaS2mgjqY02ktpoI6mNNpLaaCOpjTbM/koZLxiij9Lsr9poI6mNNpLaaCOpjTaS2mgjqY02ktpoI6mNNpLaaCOpjTaS2mgjqY02zP5KGS8Yoo/S7K/aaCOpjTaS2mgjqY02ktpoI6mNNpLaaCOpjTaS2mgjqY02ktpoI6mNNsz+ShkvGKKP0uyv2mgjqY02ktpoI6mNNpLaaCOpjTaS2mgjqY02ktpoI6mNNpLaaCOpjTbM/koZLxiij9Lsr9poI6mNNpLaaCOpjTaS2mgjqY02ktpoI6mNNpLaaCOpjTaS2mgjqY02zP5KGS8Yoo/S7K/aaCOpjTaS2mgjqY02ktpoI6mNNpLaaCOpjTaS2mgjqY02ktpoI6mNNsz+ShkvGKKP0uyv2mgjqY02ktpoI6mNNpLaaCOpjTaS2mgjqY02ktpoI6mNNpLaaCOpjTbM/koZLxiij9Lsr9poI6mNNpLaaCOpjTaS2mgjqY02ktpoI6mNNpLaaCOpjTaS2mgjqY02zP5KGS8Yoo/S7K/aaCOpjTaS2mgjqY02ktpoI6mNNpLaaCOpjTaS2mgjqY02ktpoI6mNNsz+ShkvGKKP0uyv2mgjqY02ktpoI6mNNpLaaCOpjTaS2mgjqY02ktpoI6mNNpLaaCOpjTbM/koZLxiij9Lsr9poI6mNNpLaaCOpjTaS2mgjqY02ktpoI6mNNpLaaCOpjTaS2mgjqY02zP5KGS8Yoo/S7K/aaCOpjTaS2mgjqY02ktpoI6mNNpLaaCOpjTaS2mgjqY02ktpoI6mNNsz+ShkvGKKP0uyv2mgjqY02ktpoI6mNNpLaaCOpjTaS2mgjqY02ktpoI6mNNpLaaCOpjTbM/koZLxiij9Lsr9poI6mNNpLaaCOpjTaS2mgjqY02ktpoI6mNNpLaaCOpjTaS2mgjqY02zP5KGS8o/YfRfxST2mgjSRm6aVIbbSS10UZSG23YndpoI0nSf5f/gqX/MPqfclIbbSQpQzdNaqONpDbaSGqjDbtTG20kSfrv8l+w9B9G/1NOaqONJGXopklttJHURhtJbbRhd2qjjSRJ/13+C5b+w+h/yklttJGkDN00qY02ktpoI6mNNuxObbSRJOm/y3/B0n8Y/U85qY02kpShmya10UZSG20ktdGG3amNNpIk/Xf5L1j6D6P/KSe10UaSMnTTpDbaSGqjjaQ22rA7tdFGkqT/Lv8FS/9h9D/lpDbaSFKGbprURhtJbbSR1EYbdqc22kiS9N/lv2DpP4z+p5zURhtJytBNk9poI6mNNpLaaMPu1EYbSZL+u/wXLP2H0f+Uk9poI0kZumlSG20ktdFGUhtt2J3aaCNJ0n+X/4Kl/zD6n3JSG20kKUM3TWqjjaQ22khqow27UxttJEn67/JfsPQfRv9TTmqjjSRl6KZJbbSR1EYbSW20YXdqo40kSf9d/guW/sPof8pJbbSRpAzdNKmNNpLaaCOpjTbsTm20kSTpv8t/wdJ/GP1POamNNpKUoZsmtdFGUhttJLXRht2pjTaSJP13+S9Y+g+j/ykntdFGkjJ006Q22khqo42kNtqwO7XRRpKk/y7/BUv/YfQ/5aQ22khShm6a1EYbSW20kdRGG3anNtpIkvTf5b9g6T+M/qec1EYbScrQTZPaaCOpjTaS2mjD7tRGG0mS/rv8Fyz9h9H/lJPaaCNJGbppUhttJLXRRlIbbdid2mgjSdJ/l/+Cpf8w+p9yUhttJClDN01qo42kNtpIaqMNu1MbbSRJ+u/yX7D0H0b/U05qo40kZeimSW20kdRGG0lttGF3aqONJEn/Xf4Llv7D6H/KSW20kaQM3TSpjTaS2mgjqY027E5ttJEk6b/Lf8Fj6D+yS7XRRlIbbSzVRhuXa6MNs79qo40kZeiml2ujjaQ22rDfpYwXHEMf+VJttJHURhtLtdHG5dpow+yv2mgjSRm66eXaaCOpjTbsdynjBcfQR75UG20ktdHGUm20cbk22jD7qzbaSFKGbnq5NtpIaqMN+13KeMEx9JEv1UYbSW20sVQbbVyujTbM/qqNNpKUoZtero02ktpow36XMl5wDH3kS7XRRlIbbSzVRhuXa6MNs79qo40kZeiml2ujjaQ22rDfpYwXHEMf+VJttJHURhtLtdHG5dpow+yv2mgjSRm66eXaaCOpjTbsdynjBcfQR75UG20ktdHGUm20cbk22jD7qzbaSFKGbnq5NtpIaqMN+13KeMEx9JEv1UYbSW20sVQbbVyujTbM/qqNNpKUoZtero02ktpow36XMl5wDH3kS7XRRlIbbSzVRhuXa6MNs79qo40kZeiml2ujjaQ22rDfpYwXHEMf+VJttJHURhtLtdHG5dpow+yv2mgjSRm66eXaaCOpjTbsdynjBcfQR75UG20ktdHGUm20cbk22jD7qzbaSFKGbnq5NtpIaqMN+13KeMEx9JEv1UYbSW20sVQbbVyujTbM/qqNNpKUoZtero02ktpow36XMl5wDH3kS7XRRlIbbSzVRhuXa6MNs79qo40kZeiml2ujjaQ22rDfpYwXHEMf+VJttJHURhtLtdHG5dpow+yv2mgjSRm66eXaaCOpjTbsdynjBcfQR75UG20ktdHGUm20cbk22jD7qzbaSFKGbnq5NtpIaqMN+13KeMEx9JEv1UYbSW20sVQbbVyujTbM/qqNNpKUoZtero02ktpow36XMl5wDH3kS7XRRlIbbSzVRhuXa6MNs79qo40kZeiml2ujjaQ22rDfpYwXHEMf+VJttJHURhtLtdHG5dpow+yv2mgjSRm66eXaaCOpjTbsdynjBcfQR75UG20ktdHGUm20cbk22jD7qzbaSFKGbnq5NtpIaqMN+13KeMEx9JEv1UYbSW20sVQbbVyujTbM/qqNNpKUoZtero02ktpow36XMl4wRB9l0jp68+XaaCOpjTaS2mgjSRm6adJr6AZLraM3J7XRxuXaaGOpdfTmyynjBUP0USatozdfro02ktpoI6mNNpKUoZsmvYZusNQ6enNSG21cro02llpHb76cMl4wRB9l0jp68+XaaCOpjTaS2mgjSRm6adJr6AZLraM3J7XRxuXaaGOpdfTmyynjBUP0USatozdfro02ktpoI6mNNpKUoZsmvYZusNQ6enNSG21cro02llpHb76cMl4wRB9l0jp68+XaaCOpjTaS2mgjSRm6adJr6AZLraM3J7XRxuXaaGOpdfTmyynjBUP0USatozdfro02ktpoI6mNNpKUoZsmvYZusNQ6enNSG21cro02llpHb76cMl4wRB9l0jp68+XaaCOpjTaS2mgjSRm6adJr6AZLraM3J7XRxuXaaGOpdfTmyynjBUP0USatozdfro02ktpoI6mNNpKUoZsmvYZusNQ6enNSG21cro02llpHb76cMl4wRB9l0jp68+XaaCOpjTaS2mgjSRm6adJr6AZLraM3J7XRxuXaaGOpdfTmyynjBUP0USatozdfro02ktpoI6mNNpKUoZsmvYZusNQ6enNSG21cro02llpHb76cMl4wRB9l0jp68+XaaCOpjTaS2mgjSRm6adJr6AZLraM3J7XRxuXaaGOpdfTmyynjBUP0USatozdfro02ktpoI6mNNpKUoZsmvYZusNQ6enNSG21cro02llpHb76cMl4wRB9l0jp68+XaaCOpjTaS2mgjSRm6adJr6AZLraM3J7XRxuXaaGOpdfTmyynjBUP0USatozdfro02ktpoI6mNNpKUoZsmvYZusNQ6enNSG21cro02llpHb76cMl4wRB9l0jp68+XaaCOpjTaS2mgjSRm6adJr6AZLraM3J7XRxuXaaGOpdfTmyynjBUP0USatozdfro02ktpoI6mNNpKUoZsmvYZusNQ6enNSG21cro02llpHb76cMl4wRB9l0jp68+XaaCOpjTaS2mgjSRm6adJr6AZLraM3J7XRxuXaaGOpdfTmyynjBUP0USatozdfro02ktpoI6mNNpKUoZsmvYZusNQ6enNSG21cro02llpHb76cMl4wRB9l0jp68+XaaCOpjTaS2mgjSRm6adJr6AZLraM3J7XRxuXaaGOpdfTmyynjBUP0USatozdfro02ktpoI6mNNpKUoZsmvYZusNQ6enNSG21cro02llpHb76cMl4wRB+l/a422liqjTbse+vozUlttGF3WkdvTmqjjaXaaMO+10YbSynjBUP0UdrvaqONpdpow763jt6c1EYbdqd19OakNtpYqo027HtttLGUMl4wRB+l/a422liqjTbse+vozUlttGF3WkdvTmqjjaXaaMO+10YbSynjBUP0UdrvaqONpdpow763jt6c1EYbdqd19OakNtpYqo027HtttLGUMl4wRB+l/a422liqjTbse+vozUlttGF3WkdvTmqjjaXaaMO+10YbSynjBUP0UdrvaqONpdpow763jt6c1EYbdqd19OakNtpYqo027HtttLGUMl4wRB+l/a422liqjTbse+vozUlttGF3WkdvTmqjjaXaaMO+10YbSynjBUP0UdrvaqONpdpow763jt6c1EYbdqd19OakNtpYqo027HtttLGUMl4wRB+l/a422liqjTbse+vozUlttGF3WkdvTmqjjaXaaMO+10YbSynjBUP0UdrvaqONpdpow763jt6c1EYbdqd19OakNtpYqo027HtttLGUMl4wRB+l/a422liqjTbse+vozUlttGF3WkdvTmqjjaXaaMO+10YbSynjBUP0UdrvaqONpdpow763jt6c1EYbdqd19OakNtpYqo027HtttLGUMl4wRB+l/a422liqjTbse+vozUlttGF3WkdvTmqjjaXaaMO+10YbSynjBUP0UdrvaqONpdpow763jt6c1EYbdqd19OakNtpYqo027HtttLGUMl4wRB+l/a422liqjTbse+vozUlttGF3WkdvTmqjjaXaaMO+10YbSynjBUP0UdrvaqONpdpow763jt6c1EYbdqd19OakNtpYqo027HtttLGUMl4wRB+l/a422liqjTbse+vozUlttGF3WkdvTmqjjaXaaMO+10YbSynjBUP0UdrvaqONpdpow763jt6c1EYbdqd19OakNtpYqo027HtttLGUMl4wRB+l/a422liqjTbse+vozUlttGF3WkdvTmqjjaXaaMO+10YbSynjBUP0UdrvaqONpdpow763jt6c1EYbdqd19OakNtpYqo027HtttLGUMl5wDH3kSZL+Hfo3eDll6KZLtdGGfa+NNpZaR29Oeg3dIEkZLziGPvIkSf8O/Ru8nDJ006XaaMO+10YbS62jNye9hm6QpIwXHEMfeZKkf4f+DV5OGbrpUm20Yd9ro42l1tGbk15DN0hSxguOoY88SdK/Q/8GL6cM3XSpNtqw77XRxlLr6M1Jr6EbJCnjBcfQR54k6d+hf4OXU4ZuulQbbdj32mhjqXX05qTX0A2SlPGCY+gjT5L079C/wcspQzddqo027HtttLHUOnpz0mvoBknKeMEx9JEnSfp36N/g5ZShmy7VRhv2vTbaWGodvTnpNXSDJGW84Bj6yJMk/Tv0b/ByytBNl2qjDfteG20stY7e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FwzRR5nURhtJ0i/RN2l3aqONpZShmya10UZSG20ktdFG0jp6s31PGS8Yoo8yqY02kqRfom/S7tRGG0spQzdNaqONpDbaSGqjjaR19Gb7njJeMEQfZVIbbSRJv0TfpN2pjTaWUoZumtRGG0lttJHURhtJ6+jN9j1lvGCIPsqkNtpIkn6Jvkm7UxttLKUM3TSpjTaS2mgjqY02ktbRm+17ynjBEH2USW20kST9En2Tdqc22lhKGbppUhttJLXRRlIbbSStozfb95TxgiH6KJPaaCNJ+iX6Ju1ObbSxlDJ006Q22khqo42kNtpIWkdvtu8p4wXH0EeetI7enNRGG5dro43LtdFG0jp681Lr6M1Jr6EbLNVGG0nr6M1LtdHG5ZTxgmPoI09aR29OaqONy7XRxuXaaCNpHb15qXX05qTX0A2WaqONpHX05qXaaONyynjBMfSRJ62jNye10cbl2mjjcm20kbSO3rzUOnpz0mvoBku10UbSOnrzUm20cTllvOAY+siT1tGbk9po43JttHG5NtpIWkdvXmodvTnpNXSDpdpoI2kdvXmpNtq4nDJecAx95Enr6M1JbbRxuTbauFwbbSStozcvtY7enPQausFSbbSRtI7evFQbbVxOGS84hj7ypHX05qQ22rhcG21cro02ktbRm5daR29Oeg3dYKk22khaR29eqo02LqeMFxxDH3nSOnpzUhttXK6NNi7XRhtJ6+jNS62jNye9hm6wVBttJK2jNy/VRhuXU8YLjqGPPGkdvTmpjTYu10Ybl2ujjaR19Oal1tGbk15DN1iqjTaS1tGbl2qjjcsp4wXH0EeetI7enNRGG5dro43LtdFG0jp681Lr6M1Jr6EbLNVGG0nr6M1LtdHG5ZTxgmPoI09aR29OaqONy7XRxuXaaCNpHb15qXX05qTX0A2WaqONpHX05qXaaONyynjBMfSRJ62jNye10cbl2mjjcm20kbSO3rzUOnpz0mvoBku10UbSOnrzUm20cTllvOAY+siT1tGbk9po43JttHG5NtpIWkdvXmodvTnpNXSDpdpoI2kdvXmpNtq4nDJecAx95Enr6M1JbbRxuTbauFwbbSStozcvtY7enPQausFSbbSRtI7evFQbbVxOGS84hj7ypHX05qQ22rhcG21cro02ktbRm5daR29Oeg3dYKk22khaR29eqo02LqeMFxxD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+IuMoX80SW20kbSO3pykLfQbLdVGG0nr6M1JytBNL9dGG/a9dfTmpDbaSNIWf5Ex9I8mqY02ktbRm5O0hX6jpdpoI2kdvTlJGbrp5dpow763jt6c1EYbSdriLzKG/tEktdFG0jp6c5K20G+0VBttJK2jNycpQze9XBtt2PfW0ZuT2mgjSVv8RcbQP5qkNtpIWkdvTtIW+o2WaqONpHX05iRl6KaXa6MN+946enNSG20kaYu/yBj6R5PURhtJ6+jNSdpCv9FSbbSRtI7enKQM3fRybbRh31tHb05qo40kbfEXGUP/aJLaaCNpHb05SVvoN1qqjTaS1tGbk5Shm16ujTbse+vozUlttJGkLf4iY+gfTVIbbSStozcnaQv9Rku10UbSOnpzkjJ008u10YZ9bx29OamNNpK0xV8kRB+5fa+NNpLaaGMpZeimS62jNyetozcnraM32/faaONy6+jNSW20sZQyXjBEH6V9r402ktpoYyll6KZLraM3J62jNyetozfb99po43Lr6M1JbbSxlDJeMEQfpX2vjTaS2mhjKWXopkutozcnraM3J62jN9v32mjjcuvozUlttLGUMl4wRB+lfa+NNpLaaGMpZeimS62jNyetozcnraM32/faaONy6+jNSW20sZQyXjBEH6V9r402ktpoYyll6KZLraM3J62jNyetozfb99po43Lr6M1JbbSxlDJeMEQfpX2vjTaS2mhjKWXopkutozcnraM3J62jN9v32mjjcuvozUlttLGUMl4wRB+lfa+NNpLaaGMpZeimS62jNyetozcnraM32/faaONy6+jNSW20sZQyXjBEH6V9r402ktpoYyll6KZLraM3J62jNyetozfb99po43Lr6M1JbbSxlDJeMEQfpX2vjTaS2mhjKWXopkutozcnraM3J62jN9v32mjjcuvozUlttLGUMl4wRB+lfa+NNpLaaGMpZeimS62jNyetozcnraM32/faaONy6+jNSW20sZQyXjBEH6V9r402ktpoYyll6KZLraM3J62jNyetozfb99po43Lr6M1JbbSxlDJeMEQfpX2vjTaS2mhjKWXopkutozcnraM3J62jN9v32mjjcuvozUlttLGUMl4wRB+lfa+NNpLaaGMpZeimS62jNyetozcnraM32/faaONy6+jNSW20sZQyXjBEH6V9r402ktpoYyll6KZLraM3J62jNyetozfb99po43Lr6M1JbbSxlDJeMEQfpX2vjTaS2mhjKWXopkutozcnraM3J62jN9v32mjjcuvozUlttLGUMl4wRB+lfa+NNpLaaGMpZeimS62jNyetozcnraM32/faaONy6+jNSW20sZQyXjBEH6V9r402ktpoYyll6KZLraM3J62jNyetozfb99po43Lr6M1JbbSxlDJeMEQfpX2vjTaS2mhjKWXopkutozcnraM3J62jN9v32mjjcuvozUlttLGUMl4wRB+lfa+NNpLaaGMpZeimS62jNyetozcnraM32/faaONy6+jNSW20sZQyXjBEH6V9r402ktpoYyll6KZLraM3J62jNyetozfb99po43Lr6M1JbbSxlDJeMEQfZdI6enNSG23Y72qjjaXaaGOp19ANktpow773GrpBUhttJLXRhv0uZbxgiD7KpHX05qQ22rDf1UYbS7XRxlKvoRsktdGGfe81dIOkNtpIaqMN+13KeMEQfZRJ6+jNSW20Yb+rjTaWaqONpV5DN0hqow373mvoBklttJHURhv2u5TxgiH6KJPW0ZuT2mjDflcbbSzVRhtLvYZukNRGG/a919ANktpoI6mNNux3KeMFQ/RRJq2jNye10Yb9rjbaWKqNNpZ6Dd0gqY027HuvoRsktdFGUhtt2O9SxguG6KNMWkdvTmqjDftdbbSxVBttLPUaukFSG23Y915DN0hqo42kNtqw36WMFwzRR5m0jt6c1EYb9rvaaGOpNtpY6jV0g6Q22rDvvYZukNRGG0lttGG/SxkvGKKPMmkdvTmpjTbsd7XRxlJttLHUa+gGSW20Yd97Dd0gqY02ktpow36XMl4wRB9l0jp6c1IbbdjvaqONpdpoY6nX0A2S2mjDvvcaukFSG20ktdGG/S5lvGCIPsqkdfTmpDbasN/VRhtLtdHGUq+hGyS10YZ97zV0g6Q22khqow37Xcp4wRB9lEnr6M1JbbRhv6uNNpZqo42lXkM3SGqjDfvea+gGSW20kdRGG/a7lPGCIfook9bRm5PaaMN+VxttLNVGG0u9hm6Q1EYb9r3X0A2S2mgjqY027Hcp4wVD9FEmraM3J7XRhv2uNtpYqo02lnoN3SCpjTbse6+hGyS10UZSG23Y71LGC4boo0xaR29OaqMN+11ttLFUG20s9Rq6QVIbbdj3XkM3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1UYbSevozUnKeMEx9JHbndpoI6mNNpLaaMN+12voBkutozcnraM3L9VGG0u10UbSOnpzkjJecAx95HanNtpIaqONpDbasN/1GrrBUuvozUnr6M1LtdHGUm20kbSO3pykjBccQx+53amNNpLaaCOpjTbsd72GbrDUOnpz0jp681JttLFUG20kraM3JynjBcfQR253aqONpDbaSGqjDftdr6EbLLWO3py0jt68VBttLNVGG0nr6M1JynjBMfSR253aaCOpjTaS2mjDftdr6AZLraM3J62jNy/VRhtLtdFG0jp6c5IyXnAMfeR2pzbaSGqjjaQ22rDf9Rq6wVLr6M1J6+jNS7XRxlJttJG0jt6cpIwXHEMfud2pjTaS2mgjqY027He9hm6w1Dp6c9I6evNSbbSxVBttJK2jNycp4wXH0Edud2qjjaQ22khqow37Xa+hGyy1jt6ctI7evFQbbSzVRhtJ6+jNScp4wTH0kdud2mgjqY02ktpow37Xa+gGS62jNyetozcv1UYbS7XRRtI6enOSMl5wDH3kdqc22khqo42kNtqw3/UausFS6+jNSevozUu10cZSbbSRtI7enKSMFxxDH7ndqY02ktpoI6mNNux3vYZusNQ6enPSOnrzUm20sVQbbSStozcnKeMFx9BHbndqo42kNtpIaqMN+12voRsstY7enLSO3rxUG20s1UYbSevozUnKeMEx9JHbndpoI6mNNpLaaMN+12voBkutozcnraM3L9VGG0u10UbSOnpzkjJecAx95HanNtpIaqONpDbasN/1GrrBUuvozUnr6M1LtdHGUm20kbSO3pykjBccQx+53amNNpLaaCOpjTbsd72GbrDUOnpz0jp681JttLFUG20kraM3JynjBcfQR253aqONpDbaSGqjDftdr6EbLLWO3py0jt68VBttLNVGG0nr6M1JynjBMfSR253aaCOpjTaS2mjDftdr6AZLraM3J62jNy/VRhtLtdFG0jp6c5IyXvA4+keT1EYbS7XRRtI6enOSMnTTpNfQDZKUoZsu1UYbSevozUu10cZSbbSRpIwXPI7+0SS10cZSbbSRtI7enKQM3TTpNXSDJGXopku10UbSOnrzUm20sVQbbSQp4wWPo380SW20sVQbbSStozcnKUM3TXoN3SBJGbrpUm20kbSO3rxUG20s1UYbScp4wePoH01SG20s1UYbSevozUnK0E2TXkM3SFKGbrpUG20kraM3L9VGG0u10UaSMl7wOPpHk9RGG0u10UbSOnpzkjJ006TX0A2SlKGbLtVGG0nr6M1LtdHGUm20kaSMFzyO/tEktdHGUm20kbSO3pykDN006TV0gyRl6KZLtdFG0jp681JttLFUG20kKeMFj6N/NElttLFUG20kraM3JylDN016Dd0gSRm66VJttJG0jt68VBttLNVGG0nKeMHj6B9NUhttLNVGG0nr6M1JytBNk15DN0hShm66VBttJK2jNy/VRhtLtdFGkjJe8Dj6R5PURhtLtdFG0jp6c5IydNOk19ANkpShmy7VRhtJ6+jNS7XRxlJttJGkjBc8jv7RJLXRxlJttJG0jt6cpAzdNOk1dIMkZeimS7XRRtI6evNSbbSxVBttJCnjBY+jfzRJbbSxVBttJK2jNycpQzdNeg3dIEkZuulSbbSRtI7evFQbbSzVRhtJynjB4+gfTVIbbSzVRhtJ6+jNScrQTZNeQzdIUoZuulQbbSStozcv1UYbS7XRRpIyXvA4+keT1EYbS7XRRtI6enOSMnTTpNfQDZKUoZsu1UYbSevozUu10cZSbbSRpIwXPI7+0SS10cZSbbSRtI7enKQM3TTpNXSDJGXopku10UbSOnrzUm20sVQbbSQp4wWPo380SW20sVQbbSStozcnKUM3TXoN3SBJGbrpUm20kbSO3rxUG20s1UYbScp4wePoH01SG20s1UYbSevozUnK0E2TXkM3SFKGbrpUG20kraM3L9VGG0u10UaSMl7wOPpHk9RGG0u10UbSOnpzkjJ006TX0A2SlKGbLtVGG0nr6M1LtdHGUm20kaSMFzyO/tEktdHGUm20kbSO3pykDN006TV0gyRl6KZLtdFG0jp681JttLFUG20kKeMFj6N/NElttLFUG20kraM3JylDN016Dd0gSRm66VJttJG0jt68VBttLNVGG0nKeMHj6B9NUhttLNVGG0nr6M1JytBNk15DN0hShm66VBttJK2jNy/VRhtLtdFGkjJeMEQfZVIbbSS10YbZv6qNNpJ0G/3mS62jNye10UZSG20ktdFGUhtt2O9SxguG6KNMaqONpDbaMPtXtdFGkm6j33ypdfTmpDbaSGqjjaQ22khqow37Xcp4wRB9lElttJHURhtm/6o22kjSbfSbL7WO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' />
 
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