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
        bool SaveAll();
    }
}