using Microsoft.EntityFrameworkCore;
using role_play.Controllers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using BC = BCrypt.Net.BCrypt;

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
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }

    public class User
    {
        private static readonly string[] Roles = new[] { "Admin", "User", "Guest" };
        private string username = null!;
        //private string password = null!; 
        private string email = null!;
        private string fullName = null!;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } // int for primary key (better than string)

        [Required]
        public string FullName
        {
            get => fullName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Full name cannot be empty.");

                string sourceName = value.Trim();
                string pattern = @"^[A-Z][a-zA-Z]{3,}(?:\s+[A-Z][a-zA-Z]{3,})+$";

                if (!Regex.IsMatch(sourceName, pattern))
                {
                    throw new ArgumentException("Name must have first and last name, each starting with a capital letter and at least 4 characters long.");
                }

                fullName = Regex.Replace(sourceName, @"\s+", " ");
            }
        }

        [Required]
        public string Username
        {
            get => username;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Username cannot be empty.");

                if (value.Length < 3)
                {
                    throw new ArgumentException("Username must be at least 3 characters long.");
                }

                if (!Regex.IsMatch(value, @"^[a-zA-Z0-9_]+$"))
                {
                    throw new ArgumentException("Username can only contain letters, numbers, and underscores.");
                }

                username = value.Trim().ToLower();
            }
        }

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email
        {
            get => email;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Email cannot be empty.");

                const string EmailPattern = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$";
                if (!Regex.IsMatch(value, EmailPattern, RegexOptions.IgnoreCase))
                {
                    throw new ArgumentException("Invalid email format.");
                }
                email = value.Trim().ToLower(); // lowercase for consistency
            }
        }

        // This is for INPUT ONLY - not stored in DB
        [Required]
        public string Password { get; set; } = null!;

        // This is what gets stored in DB
        [Required]
        public string PasswordHash { get; set; } = null!;

        public bool IsVerified { get; set; } = false;

        [MaxLength(6)]
        public string? OTPCode { get; set; }

        public DateTime? OTPExpiry { get; set; }

        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = "User"; // Default to "User"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}