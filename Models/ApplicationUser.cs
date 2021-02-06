﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace laberegisterLIH.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime CreatedOn { get; set; }
    }
}
