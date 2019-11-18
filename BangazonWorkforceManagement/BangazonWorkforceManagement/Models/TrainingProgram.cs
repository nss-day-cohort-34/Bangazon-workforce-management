using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforceManagement.Models
{
    public class TrainingProgram
    {

        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayName("Start Date")]
        public DateTime? StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
<<<<<<< HEAD
        [DisplayName("End Date")]
=======

>>>>>>> master
        public DateTime? EndDate { get; set; }

        [Required]
        [DisplayName("Max Attendees")]
        public int MaxAttendees { get; set; }

        [DisplayName("Current Attendees")]
        public List<Employee> CurrentAttendees { get; set; } = new List<Employee>();

    }
}
