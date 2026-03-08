using System.ComponentModel.DataAnnotations;

namespace MOM_Project.Models
{
    public class MeetingMemberModel
    {
        public int MeetingMemberID { get; set; }

        [Required(ErrorMessage = "Meeting Name is required")]
        [StringLength(100)]

        public int MeetingID { get; set; }

        public int StaffID { get; set; }

        public string? MeetingTypeName { get; set; }

        public string MeetingName { get; set; }

        [Required(ErrorMessage = "Staff Name is required")]
        [StringLength(100)]
        public string StaffName { get; set; }

        [Required(ErrorMessage = "Attendance status is required")]
        public bool? IsPresent { get; set; }

        [StringLength(250, ErrorMessage = "Max 250 characters allowed")]
        public string? Remarks { get; set; }

        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
}
