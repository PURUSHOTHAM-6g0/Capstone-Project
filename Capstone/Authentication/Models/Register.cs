using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Authentication.Models
{
    public class Register:IdentityUser
    {
        //[Required]
        //[DataType(DataType.Password)]
        //public string Password {  get; set; }
        [Required]
        public string EmployeeId { get; set; }
        [Required]
        public string Country { get; set; }
        public string TimeZone { get; set; }
    }
}
