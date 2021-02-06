using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using laberegisterLIH.Models;

namespace laberegisterLIH.Data
{
    
    public class LabRepository : ILabRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LabRepository> _logger;

        public LabRepository(ApplicationDbContext context, ILogger<LabRepository> logger)
        {
            _context = context;
            _logger = logger;
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
        
        public bool AddNewScheduleClientes(string userId, string examId, string date, string time, string sucursalId)
        {
            try
            {
                //get appuser
                var user = _context.Clientes.Where(u => u.Id == userId).FirstOrDefault();
                //update values
                user.RegistrationPlace.Add(_context.Sucursales.Where(u => u.Id == Int32.Parse(sucursalId)).FirstOrDefault());
                user.ExamsTaken.Add(_context.Examenes.Where(u => u.Id == Int32.Parse(examId)).FirstOrDefault());
                //save 
                _context.SaveChanges();
                return false;
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Failed to add new schedule {ex}");
                return false;
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
    }
}