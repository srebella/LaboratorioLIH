using System;
using System.Collections.Generic;
using System.Linq;
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
