using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace laberegisterLIH.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public Appointment Appointment { get; set; }        
    }
}
