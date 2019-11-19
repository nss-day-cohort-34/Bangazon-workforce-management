using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforceManagement.Models.ViewModels
{
    public class EmployeeEditViewModel
    {
        public Employee Employee { get; set; }

        public int ComputerId { get; set; }

        public int SelectedComputerId { get; set; }

        public List<SelectListItem> Departments { get; set; }

        public List<SelectListItem> Computers { get; set; }

    }
}