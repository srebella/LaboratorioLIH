using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Claims;
using System.Threading.Tasks;
using laberegisterLIH.Data;
using laberegisterLIH.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QRCoder;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace laberegisterLIH.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ApiController : ControllerBase
    {
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
            //SendEmail();
            //GenerateQR();
            var results = _repository.GetAllExams();
          
            return results;
        }


        [HttpPost]
        [Route("SetAppointment")]
        public async Task<int> SetAsync([FromBody] CalendarModel data)
        {
            ClaimsPrincipal currentUser = this.User;
            var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            return await _repository.AddNewScheduleClientesAsync(currentUserId, data.ExamId, data.Date, data.Time, data.SucursalId);
        }
        
        [HttpPost]
        [Route("SetFeedback")]
        public bool SetFBAsync([FromBody] FeedbackModel data)
        {
            return _repository.SaveFeedback(data.Feedback, data.ApptId);
        }
        
        [HttpPost]
        [Route("UpdateAppointment")]
        public async Task<int> UpdateAsync([FromBody] CalendarModel data)
        {
            ClaimsPrincipal currentUser = this.User;
            if (ClaimTypes.NameIdentifier != null) {
                var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
                return await _repository.UpdateScheduleClientesAsync(currentUserId, data.Id, data.ExamId, data.Date, data.Time, data.SucursalId);
            }           
        }
        
        [HttpGet]
        [Route("GetAppointmentByUserId")]
        public IEnumerable<Appointment> GetAsync()
        {
            ClaimsPrincipal currentUser = this.User != null? this.User:null;
            if (currentUser != null && ClaimTypes.NameIdentifier != null) {
                var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
                return  _repository.GetAppointmentsByUser(currentUserId);
            }
            return null;
        }

        [HttpGet]
        [Route("CountAppointmentByUserId")]
        public int CountAsync()
        {
            ClaimsPrincipal currentUser = this.User;
            var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            return  _repository.CountAppointmentsByUser(currentUserId);
        }

        [HttpGet]
        [Route("GetUserById")]
        public async Task<ApplicationUser> GetUserAsync()
        {
            ClaimsPrincipal currentUser = this.User;
            if (ClaimTypes.NameIdentifier != null) {                
                var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier) != null ? currentUser.FindFirst(ClaimTypes.NameIdentifier).Value : null;                
                return currentUserId != null ? await _repository.GetUserDataAsync(currentUserId) : null;
            }
        }

        [HttpGet]
        [Route("GetAppointments")]
        public IEnumerable<Appointment> GetApptAsync()
        {
            //ClaimsPrincipal currentUser = this.User;
            //var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            //check if user is admin or not
            return  _repository.GetAllAppointments();
        }

        [HttpGet]
        [Route("GetAppointmentById")]
        public Appointment GetAppointmentById(string id)
        {
            return  _repository.GetAppointmentById(id);
        }

        [HttpGet]
        [Route("DeleteApptById")]
        public bool DeleteApptById(string id)
        {
            var results = _repository.DeleteAppointmentsById(id);
            return results;
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
    public string Id { get; set; }
    public string UserId { get; set; }
    public string ExamId { get; set; }
    public string Date { get; set; }
    public string Time { get; set; }
    public string SucursalId { get; set; }
}
public class FeedbackModel
{
    public string Id { get; set; }
    public string Feedback { get; set; }
    public string ApptId { get; set; }
}