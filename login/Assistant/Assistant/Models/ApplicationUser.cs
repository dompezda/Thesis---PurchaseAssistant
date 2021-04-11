using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assistant.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Home { get; set; }
        public string Gender { get; set; }
        public string Salary { get; set; }
    }
}
