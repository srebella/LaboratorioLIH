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