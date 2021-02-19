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
    [Authorize]
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