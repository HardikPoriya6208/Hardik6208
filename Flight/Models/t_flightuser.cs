using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Flight.Models
{
    public class t_flightuser
    {
        [Display(Name = "UserId")]
        public int? c_userid { get; set; }

        [Display(Name = "Username")]
        public string? c_username { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
       ErrorMessage = "Enter a valid Email address")]
        public string? c_email { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
      ErrorMessage = "Password must contain Uppercase, Lowercase, Number and Special character")]
        public string? c_password { get; set; }

        [Display(Name = "Mobile")]
        [Required(ErrorMessage = "Invalid Mobile")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Enter To Number")]
        public string? c_mobile { get; set; }

        [Display(Name = "ProFileImage")]
        public string? c_image { get; set; }

        public IFormFile? ProfileImage { get; set; }

        public string? c_role { get; set; }

    }
}