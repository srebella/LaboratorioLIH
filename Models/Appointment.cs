using System;

namespace laberegisterLIH.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public ApplicationUser User { get; set; }
        public Sucursal Sucursal { get; set; }
        public Examen Examen { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}