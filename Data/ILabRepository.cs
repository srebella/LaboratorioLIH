using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using laberegisterLIH.Models;

namespace laberegisterLIH.Data
{
    
    public interface ILabRepository
    {
        public IEnumerable<Examen> GetAllExams();
        public IEnumerable<Sucursal> GetAllSucursales();
        public IEnumerable<Appointment> GetAllAppointments();
        IEnumerable<Appointment> GetAppointmentsByUser(string userId);
        bool DeleteAppointmentsById(string id);
        System.Threading.Tasks.Task<int> AddNewScheduleClientesAsync(string userId, string examId, string date, string time, string sucursalId);
        bool SaveAll();
    }
}
