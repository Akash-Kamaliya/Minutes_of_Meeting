using System.ComponentModel.DataAnnotations;

namespace MOM_Project.Models
{
    public class MeetingsModel
    {
        public int MeetingID { get; set; }

        [Required(ErrorMessage = "Meeting date & time is required")]
        public DateTime MeetingDate { get; set; }

        [Required(ErrorMessage = "Meeting type is required")]
        public int MeetingTypeID { get; set; }

        public string? MeetingTypeName { get; set; }

        [Required(ErrorMessage = "Department is required")]
        public int DepartmentID { get; set; }

        public string? DepartmentName { get; set; }

        [Required(ErrorMessage = "Venue is required")]
        public int MeetingVenueID { get; set; }

        public string? MeetingVenueName { get; set; }

        [StringLength(250, ErrorMessage = "Max 250 characters")]
        public string? MeetingDescription { get; set; }

        public bool? IsCancelled { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }
    }
}