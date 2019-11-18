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

        public Computer Computer { get; set;}

        public int SelectedComputerId { get; set; }

        public List<Department> Departments { get; set; } = new List<Department>();

        public List<Computer> Computers { get; set; } = new List<Computer>();

        public List<SelectListItem> DepartmentOptions
        {
            get
            {
                if (Departments == null) return null;

                return Departments
                    .Select(d => new SelectListItem(d.Name, d.Id.ToString()))
                    .ToList();
            }
        }

        public List<SelectListItem> ComputerOptions
        {
            get
            {
                if (Computers == null) return null;

                return Computers
                    .Select(c => new SelectListItem(c.ComputerInfo, c.Id.ToString()))
                    .ToList();
            }
        }
    }
}