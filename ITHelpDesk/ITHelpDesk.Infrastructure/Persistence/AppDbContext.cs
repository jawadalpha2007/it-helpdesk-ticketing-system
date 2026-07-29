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
        }
    }
}
