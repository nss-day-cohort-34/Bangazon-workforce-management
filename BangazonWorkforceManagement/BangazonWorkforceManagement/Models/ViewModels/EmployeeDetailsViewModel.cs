using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforceManagement.Models.ViewModels
{
    public class EmployeeDetailsViewModel
    {
        public Employee Employee { get; set; }

        public Computer Computer { get; set; }

        public List<Department> Departments { get; set; } = new List<Department>();

        public List<Computer> Computers { get; set; } = new List<Computer>();

        [DisplayName("Training Programs")]
        public List<TrainingProgram> TrainingPrograms { get; set; } = new List<TrainingProgram>();

    }
}