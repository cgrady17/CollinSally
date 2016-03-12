using System.ComponentModel.DataAnnotations;

namespace CollinSally.Web.ViewModels
{
    public class RsvpAttendeeViewModel
    {
        [Display(Name = "Attendee Name")]
        [Required]
        [StringLength(150, MinimumLength = 5)]
        public string Name { get; set; }

        [Display(Name = "Email Address")]
        [Required]
        [EmailAddress]
        [StringLength(75, MinimumLength = 5)]
        public string EmailAddress { get; set; }
    }
}