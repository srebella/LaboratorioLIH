using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace laberegisterLIH.Models
{
    public class Cliente : ApplicationUser
    {
        
        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Telephone { get; set; }

        public string Address { get; set; }
        
        public string PersonalId { get; set; }
        public Sucursal RegistrationPlace { get; set; }
        
        public ICollection<Examen> ExamsTaken { get; set; }
    }
}
