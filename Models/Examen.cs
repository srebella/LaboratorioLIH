using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace laberegisterLIH.Models
{
    public class Examen
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Requirements { get; set; }
        public string Price { get; set; }
        public string Protocols { get; set; }
        public string Tags { get; set; }
        public DateTime CreatedOn { get; set; }
        public ICollection<Sucursal> IsTaken { get; set; }
    }
}
