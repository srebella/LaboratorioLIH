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
        Appointment GetAppointmentById(string id);
        bool DeleteAppointmentsById(string id);
        bool SaveFeedback(string feedback, string apptid);
        System.Threading.Tasks.Task<int> AddNewScheduleClientesAsync(string userId, string examId, string date, string time, string sucursalId);
        System.Threading.Tasks.Task<int> UpdateScheduleClientesAsync(string userId, string apptId, string examId, string date, string time, string sucursalId);
        bool SaveAll();
    }
}
