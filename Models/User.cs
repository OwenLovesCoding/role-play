using Microsoft.EntityFrameworkCore;
using role_play.Controllers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace role_play.Models
{
    public class UserContext : DbContext //needs constructor
    {
        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } // Plural

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // unique index for Email
            //modelBuilder.Entity<User>()
            //    .HasIndex(u => u.Email)
            //    .IsUnique();
        }
    }
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string FullName { get; set; } = null!;

        [Required]
        public string Username { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        // ✅ ONLY ONE password property!
        [Required]
        [JsonIgnore]  // Don't return hash in JSON responses
        public string Password { get; set; } = null!;  // This stores the HASH

        // ❌ NO PasswordHash property!

        public bool IsVerified { get; set; } = false;
        public string? OTPCode { get; set; }
        public DateTime? OTPExpiry { get; set; }
        public string Role { get; set; } = "User";
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}