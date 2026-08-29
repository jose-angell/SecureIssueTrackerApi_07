using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecureIssueTrackerApi_07.Domain;

namespace SecureIssueTrackerApi_07.Infrastructure
{
    public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.ToTable("tickets");

            builder.HasKey(ticket => ticket.Id);

            builder.Property(ticket => ticket.Title)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(ticket => ticket.Description)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(ticket => ticket.Status)
                .IsRequired();

            builder.Property(ticket => ticket.Priority)
                .IsRequired();

            builder.Property(ticket => ticket.CreatedByUserId)
                .IsRequired();

            builder.Property(ticket => ticket.AssignedToUserId)
                .IsRequired(false);

            builder.Property(ticket => ticket.CreatedAt)
           .IsRequired();

            builder.Property(ticket => ticket.UpdatedAt)
                .IsRequired(false);

            builder.Property(ticket => ticket.ClosedAt)
                .IsRequired(false);

            builder.HasOne(ticket => ticket.CreatedByUser)
                .WithMany(user => user.CreatedTickets)
                .HasForeignKey(ticket => ticket.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ticket => ticket.AssignedToUser)
                .WithMany(user => user.AssignedTickets)
                .HasForeignKey(ticket => ticket.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
