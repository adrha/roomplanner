using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RoomPlanner.App.Models.InputModels
{
    public class RoomReservationInputModel : IValidatableObject
    {
        [DisplayName("Room")]
        public Guid RoomId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy HH:mm}")]
        public DateTime From { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy HH:mm}")]
        public DateTime To { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Subject { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> results = new List<ValidationResult>();

            if(To < DateTime.Now)
            {
                results.Add(new ValidationResult("Booking end-date cannot be in the past"));
            }

            if(To <= From)
            {
                results.Add(new ValidationResult("Booking end-date must be after start-date"));
            }

            return results;
        }
    }
}
