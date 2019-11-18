using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforceManagement.Models
{
    public class Computer
    {
        [DisplayName("ComputerId")]
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Make { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Manufacturer { get; set; }

        [DisplayName("Computer Info")]
        public string ComputerInfo
        {
            get
            {
                return $"{Id} {Manufacturer} {Make}";
            }
            
        }

        [Required]
        public DateTime PurchaseDate { get; set; }
        public DateTime DecommissionDate { get; set; }
    }
}
