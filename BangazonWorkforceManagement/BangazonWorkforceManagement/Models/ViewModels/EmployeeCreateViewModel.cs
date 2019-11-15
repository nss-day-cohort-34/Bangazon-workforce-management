using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforceManagement.Models.ViewModels
{
    public class EmployeeCreateViewModel
    {
        public Employee Employee { get; set; }
        public List<Department> Departments { get; set; } = new List<Department>();

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
    }
}