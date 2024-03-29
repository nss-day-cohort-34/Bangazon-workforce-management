﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforceManagement.Models
{
    public class Department
    {

        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        [DisplayName("Department")]
        public string Name { get; set;}

        [Required]
        public int Budget { get; set; }

        public List<Employee> Employees { get; set; } = new List<Employee>();


    }
}
