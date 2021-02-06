using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace laberegisterLIH.Models
{
    public class Sucursal
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Telephone { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }    
        public string City { get; set; }
        public string OpeningTime { get; set; }
        public string ClosingTime { get; set; }
        public string WorkingDays { get; set; }
        public DateTime CreatedOn { get; set; }
        public ICollection<Empleado> Workers { get; set; }
        //public Date? Calendar { get; set; } // para guardar datos de la reserva
    }
}
