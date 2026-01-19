
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LIBRARY.Models
{
    public class t_libraryuser
    {
        [Display(Name = "UserId")]
        public int? c_userid { get; set; }

        [Display(Name = "Username")]
        [Required(ErrorMessage = "Invalid Username")]
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

        [Display(Name = "Department")]
        [Required(ErrorMessage = "Invalid Department")]
        public string? c_department { get; set; }

        [Display(Name = "Status")]
        [Required(ErrorMessage = "Invalid Status")]
        public string? c_status { get; set; }

        [Display(Name = "ProfileImage")]
        public string? c_image { get; set; }

        public IFormFile? ProfileImage { get; set; }

        public string? c_role { get; set; }



    }
}