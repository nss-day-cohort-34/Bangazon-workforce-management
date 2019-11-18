using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforceManagement.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [Required]
        public int DepartmentId { get; set; }


        public Department Department { get; set; }

        [Required]
        [DisplayName("Position")]
        public bool IsSupervisor { get; set; }


        //list of computers and list of training programs
        public Computer Computer { get; set; }

        public ComputerEmployee ComputerEmployee {get; set;}
        public List<TrainingProgram> TrainingPrograms { get; set; } = new List<TrainingProgram>();
       
    }
}
