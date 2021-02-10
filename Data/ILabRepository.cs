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
        bool AddNewScheduleClientes(string userId, string examId, string date, string time, string sucursalId);
        bool SaveAll();
    }
}
