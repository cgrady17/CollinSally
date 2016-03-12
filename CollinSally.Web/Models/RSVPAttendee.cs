namespace CollinSally.Web.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RSVPAttendee")]
    public partial class RSVPAttendee
    {
        public int ID { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [Required]
        [StringLength(75)]
        public string EmailAddress { get; set; }

        public int RSVP_ID { get; set; }

        public virtual RSVP RSVP { get; set; }
    }
}
