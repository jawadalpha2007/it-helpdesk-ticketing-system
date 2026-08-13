using ITHelpDesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITHelpDesk.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Priority> Priorities { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketComment> TicketComments { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<TicketAttachment> TicketAttachments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Map C# PascalCase properties to your actual SQL camelCase columns

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Roles");
                entity.Property(r => r.Id).HasColumnName("id");
                entity.Property(r => r.RoleName).HasColumnName("roleName");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.Property(u => u.Id).HasColumnName("id");
                entity.Property(u => u.RoleId).HasColumnName("roleId");
                entity.Property(u => u.FullName).HasColumnName("fullName");
                entity.Property(u => u.Email).HasColumnName("email");
                entity.Property(u => u.PasswordHash).HasColumnName("passwordHash");
                entity.Property(u => u.IsActive).HasColumnName("isActive");
                entity.Property(u => u.CreatedAt).HasColumnName("createdAt");

                entity.HasOne(u => u.Role)
                      .WithMany()
                      .HasForeignKey(u => u.RoleId);
            });
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories");
                entity.Property(c => c.Id).HasColumnName("id");
                entity.Property(c => c.CategoryName).HasColumnName("categoryName");
            });

            modelBuilder.Entity<Priority>(entity =>
            {
                entity.ToTable("Priorities");
                entity.Property(p => p.Id).HasColumnName("id");
                entity.Property(p => p.PriorityName).HasColumnName("priorityName");
            });

            modelBuilder.Entity<Status>(entity =>
            {
                entity.ToTable("Statuses");
                entity.Property(s => s.Id).HasColumnName("id");
                entity.Property(s => s.StatusName).HasColumnName("statusName");
            });

            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.ToTable("Tickets");
                entity.Property(t => t.Id).HasColumnName("id");
                entity.Property(t => t.TicketReference).HasColumnName("ticketReference");
                entity.Property(t => t.Title).HasColumnName("title");
                entity.Property(t => t.Description).HasColumnName("description");
                entity.Property(t => t.CategoryId).HasColumnName("categoryId");
                entity.Property(t => t.PriorityId).HasColumnName("priorityId");
                entity.Property(t => t.StatusId).HasColumnName("statusId");
                entity.Property(t => t.CreatedBy).HasColumnName("createdBy");
                entity.Property(t => t.AssignedTo).HasColumnName("assignedTo");
                entity.Property(t => t.CreatedAt).HasColumnName("createdAt");
                entity.Property(t => t.UpdatedAt).HasColumnName("updatedAt");
                entity.Property(t => t.ResolvedAt).HasColumnName("resolvedAt");

                entity.HasOne(t => t.Category)
                      .WithMany()
                      .HasForeignKey(t => t.CategoryId);

                entity.HasOne(t => t.Priority)
                      .WithMany()
                      .HasForeignKey(t => t.PriorityId);

                entity.HasOne(t => t.Status)
                      .WithMany()
                      .HasForeignKey(t => t.StatusId);

                // Two SEPARATE relationships to User — this is the tricky part
                entity.HasOne(t => t.CreatedByUser)
                      .WithMany()
                      .HasForeignKey(t => t.CreatedBy)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.AssignedToUser)
                      .WithMany()
                      .HasForeignKey(t => t.AssignedTo)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<TicketComment>(entity =>
            {
                entity.ToTable("TicketComments");
                entity.Property(c => c.Id).HasColumnName("id");
                entity.Property(c => c.TicketId).HasColumnName("ticketId");
                entity.Property(c => c.UserId).HasColumnName("userId");
                entity.Property(c => c.CommentText).HasColumnName("commentText");
                entity.Property(c => c.IsInternal).HasColumnName("isInternal");
                entity.Property(c => c.CreatedAt).HasColumnName("createdAt");

                entity.HasOne(c => c.Ticket)
                      .WithMany()
                      .HasForeignKey(c => c.TicketId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.User)
                      .WithMany()
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<ActivityLog>(entity =>
            {
                entity.ToTable("ActivityLogs");
                entity.Property(a => a.Id).HasColumnName("id");
                entity.Property(a => a.UserId).HasColumnName("userId");
                entity.Property(a => a.Action).HasColumnName("action");
                entity.Property(a => a.EntityType).HasColumnName("entityType");
                entity.Property(a => a.EntityId).HasColumnName("entityId");
                entity.Property(a => a.CreatedAt).HasColumnName("createdAt");

                entity.HasOne(a => a.User)
                      .WithMany()
                      .HasForeignKey(a => a.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<TicketAttachment>(entity =>
            {
                entity.ToTable("TicketAttachments");
                entity.Property(a => a.Id).HasColumnName("id");
                entity.Property(a => a.TicketId).HasColumnName("ticketId");
                entity.Property(a => a.UploadedBy).HasColumnName("uploadedBy");
                entity.Property(a => a.FileName).HasColumnName("fileName");
                entity.Property(a => a.FilePath).HasColumnName("filePath");
                entity.Property(a => a.FileSize).HasColumnName("fileSize");
                entity.Property(a => a.FileType).HasColumnName("fileType");
                entity.Property(a => a.UploadedAt).HasColumnName("uploadedAt");

                entity.HasOne(a => a.Ticket)
                      .WithMany()
                      .HasForeignKey(a => a.TicketId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.UploadedByUser)
                      .WithMany()
                      .HasForeignKey(a => a.UploadedBy)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notifications");
                entity.Property(n => n.Id).HasColumnName("id");
                entity.Property(n => n.UserId).HasColumnName("userId");
                entity.Property(n => n.TicketId).HasColumnName("ticketId");
                entity.Property(n => n.Message).HasColumnName("message");
                entity.Property(n => n.IsRead).HasColumnName("isRead");
                entity.Property(n => n.CreatedAt).HasColumnName("createdAt");

                entity.HasOne(n => n.User)
                      .WithMany()
                      .HasForeignKey(n => n.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(n => n.Ticket)
                      .WithMany()
                      .HasForeignKey(n => n.TicketId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
