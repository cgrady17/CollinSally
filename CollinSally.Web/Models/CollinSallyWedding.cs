namespace CollinSally.Web.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class CollinSallyWedding : DbContext
    {
        public CollinSallyWedding()
            : base("name=CollinSallyWedding")
        {
        }

        public virtual DbSet<RSVP> RSVPs { get; set; }
        public virtual DbSet<RSVPAttendee> RSVPAttendees { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RSVP>()
                .HasMany(e => e.RSVPAttendees)
                .WithRequired(e => e.RSVP)
                .HasForeignKey(e => e.RSVP_ID)
                .WillCascadeOnDelete(false);
        }
    }
}
