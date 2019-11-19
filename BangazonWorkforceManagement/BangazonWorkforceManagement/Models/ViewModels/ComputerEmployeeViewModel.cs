using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforceManagement.Models.ViewModels
{
    public class ComputerEmployeeViewModel
    {
        
        public Computer Computer { get; set; }

        public List<Employee> Employees = new List<Employee>();
    }
}
