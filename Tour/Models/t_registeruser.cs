using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVC.Models
{
    public class t_registeruser
    {
        [Display(Name = "UserID")]
        public int? c_userid { get; set; }

        [Display(Name = "UserName")]
        [Required(ErrorMessage = "Invalid UserName")]
        public string? c_username { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[a-z][a-zA-Z0-9._%+-]*@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",

        ErrorMessage = "Enter a valid Email address")]
        public string? c_email { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Password must contain Uppercase, Lowercase, Number and Special character")]
        public string? c_password { get; set; }


        [Display(Name = "Confirm Password")]
        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("c_password", ErrorMessage = "Password and Confirm Password do not match")]
        public string? c_cpassword { get; set; }

        [Display(Name = "Date Of BoD")]
        [Required(ErrorMessage = "Invalid BoD")]
        public string? c_date { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage = "Invalid State")]
        public string? c_state { get; set; }

        [Display(Name = "City")]
        [Required(ErrorMessage = "Invalid City")]
        public string? c_city { get; set; }

        [Display(Name = "Gender")]
        [Required(ErrorMessage = "Invalid Gender")]
        public string? c_gender { get; set; }

        [Display(Name = "Mobile")]
        [Required(ErrorMessage = "Invalid Mobile")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Enter To Number")]
        public string? c_mobile { get; set; }

        [Display(Name = "ProfileImage")]
        public string? c_image { get; set; }

        public IFormFile? ProfileImage { get; set; }

        public string? c_role { get; set; }
    }
}