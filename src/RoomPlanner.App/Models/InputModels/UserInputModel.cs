using System.ComponentModel.DataAnnotations;

namespace RoomPlanner.App.Models.InputModels
{
    public class UserInputModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Name")]
        public string FirstName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Lastname")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone number")]
        public string? PhoneNumber { get; set; } 

        [Display(Name = "Book")]
        public bool BookRole { get; set; }

        [Display(Name = "Cleaning")]
        public bool CleanPlanRole { get; set; }

        [Display(Name = "Admin")]
        public bool AdminRole { get; set; }
    }
}
