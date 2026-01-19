//using FluentEmail.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using role_play.DTOs;
using role_play.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.RegularExpressions;
using Resend;
using BC = BCrypt.Net.BCrypt;


namespace role_play.Controllers
{
    [Route("api/[controller]")]
    public class Users(UserContext context, IConfiguration configuration, IResend _resend) : ControllerBase
    {
        private readonly UserContext _context = context;
        private readonly IResend resend = _resend;
        private readonly IConfiguration _configuration = configuration;


        [HttpPost("new-user")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest user)
        { 
            // 1. check if user is null
            if (user == null)
            {
                return BadRequest("User data is required.");
            }

            //return Ok(user);


            // Validate PlainPassword (from JSON)
            if (string.IsNullOrEmpty(user.Password))
            {
                return BadRequest("Password is required.");
            }

            //3.Check for duplicate username/ email BEFORE processing
                   if (!string.IsNullOrEmpty(user.Username) &&
            await _context.Users.AnyAsync(u => u.Username == user.Username))
                {
                    return BadRequest("Username already exists.");
                }

            //if (_context.Users.Any(u => u.Email == user.Email))
            //{
            //    return BadRequest("Email already exists.");
            //}

            // 4. Validate password pattern
            const string StrongPasswordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";

            if (!Regex.IsMatch(user.Password, StrongPasswordPattern))
            {
                return BadRequest("Password must be at least 8 characters long and include at least one uppercase letter, one lowercase letter, one digit, and one special character (@$!%*?&).");
            }

            try
            {
                // 5. Hash the password
                user.Password = BC.HashPassword(user.Password.Trim());

                // 7. timestamps
                user.CreatedAt = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;

                // Set new verification fields
                user.IsVerified = false; // User starts as unverified
                user.OTPCode = "555555"; // Generate 6-digit OTP
                user.OTPExpiry = DateTime.UtcNow.AddMinutes(15); // OTP valid for 15 mins

                // Map CreateUserRequest to User entity
                var newUser = new User
                {
                    FullName = user.FullName,
                    Username = user.Username,
                    Email = user.Email,
                    Password = user.Password,
                    Role = user.Role,
                    IsVerified = user.IsVerified,
                    OTPCode = user.OTPCode,
                    OTPExpiry = user.OTPExpiry,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                };

                // to database
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                // 9. Return success
                var response = new UserResponse
                {
                    Id = newUser.Id,
                    FullName = newUser.FullName,
                    Username = newUser.Username,
                    Email = newUser.Email,
                    Role = newUser.Role,
                };

               await VerificationEmailSendingAsync(newUser.Email, newUser.Username);  

                Console.WriteLine("Touched the email sending func in main flow and email sent");
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Don't throw, return proper error
                return StatusCode(500, $"Error creating user: {ex.Message}");
            }
        }

     
        public async Task HandleEmailSendingAsync(string to, string otpCode, string username)
        {
            try
            {
                Console.WriteLine($"Attempting to send email to: {to}");

                var message = new EmailMessage();
                message.From = "Acme <onboarding@resend.dev>";
                message.To.Add(to);
                message.Subject = "Your OTP Code";
                message.HtmlBody = $@"
            <h2>Hello {username},</h2>
            <p>Your OTP code is: <strong>{otpCode}</strong></p>
            <p>This code will expire in 10 minutes.</p>
        ";

                await _resend.EmailSendAsync(message);

                Console.WriteLine("Email sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email sending failed: {ex.Message}");
                throw;
            }
        }

        public async Task VerificationEmailSendingAsync(
            string to,
            string username)
        {
            try
            {
                Console.WriteLine($"Sending verification success email to: {to}");

                var message = new EmailMessage();
                message.From = "Acme <onboarding@resend.dev>";
                message.To.Add(to);
                message.Subject = $"Welcome to Leah {username}";
                message.HtmlBody = $@"
                <div style='font-family: Arial, sans-serif; padding: 20px;'>
                    <h2>Welcome to Leah once more, {username}!</h2>
                    <p>Your email has been successfully verified. We can't wait to see the exciting things we can do together!</p>
                    <p>You can now log in to your account.</p>
                </div>";

                await _resend.EmailSendAsync(message);

                Console.WriteLine("Welcome Email sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Verification email failed: {ex.Message}");
            }
        }
        
        }
    }

