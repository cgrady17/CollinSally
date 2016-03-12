namespace CollinSally.Web.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RSVP")]
    public partial class RSVP
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RSVP()
        {
            RSVPAttendees = new HashSet<RSVPAttendee>();
        }

        public int ID { get; set; }

        [Required]
        [StringLength(75)]
        public string EmailAddress { get; set; }

        public bool Attending { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RSVPAttendee> RSVPAttendees { get; set; }
    }
}
