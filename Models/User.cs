using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using BC = BCrypt.Net.BCrypt;
using System;
using System.Text.RegularExpressions;

namespace role_play.Models
{
    public class User
    {

        private static readonly string[] Roles = new[] { "Admin", "User", "Guest" };
        private string username = null!;
        private string password = null!;
        private string email = null!;
        private string fullName = null!;

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("id")]
        public required string FullName { get {
            
                //get first letter
                string firstLetter = char.ToUpper(fullName[0]).ToString();

            }
            set {
                if (!value.Trim().Contains(" ")) {
                    throw new ArgumentException("Full name must contain at least a first name and a last name.");
                }

                //fullName = value.Trim();
                string sourceName = Regex.Replace(value.Trim(), @"\s+", " ");

                string[] nameParts = sourceName.Split(' ');
                if (nameParts.Length < 2)
                {
                    throw new ArgumentException("Full name must contain at least a first name and a last name.");
                }

                nameParts[0][0].ToString().ToUpper();
                nameParts[2][0].ToString().ToUpper();
                
                nameParts.Join(" ") = fullName;


            } }
        [BsonElement("fullName")]
        public required string Username { get {
            return username;
            } set{
                if (value.Length < 3)
                {
                    throw new ArgumentException("Username must be at least 3 characters long.");
                }
                username = value.Trim();
            }
        }
        [BsonElement("email")]
        public required string Email
        {
            get
            {
                return email;
            }
            set
            {
                const string EmailPattern = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$";
                if (!Regex.IsMatch(value, EmailPattern, RegexOptions.IgnoreCase))
                {
                    throw new ArgumentException("Invalid email format.");
                }
                email = value.Trim();
            }
        }
        
        [BsonElement("password")]
        public required string Password { get { 
                return password.Trim();
            }
            set
            {
                const string StrongPasswordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";

                if (!Regex.IsMatch(value, StrongPasswordPattern))
                {
                    throw new ArgumentException("Password must be at least 8 characters long and include at least one uppercase letter, one lowercase letter, one digit, and one special character.");
                } else if (value.Length < 6)
                {
                    throw new ArgumentException("Password must be at least 6 characters long.");
                }
                password = BC.HashPassword(value.Trim());
            }
        }
        


        [BsonElement("role")]
        public string Role { get; set; } = Roles[1]; 
    }
}
