using FluentEmail.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using role_play.DTOs;
using role_play.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.RegularExpressions;
using BC = BCrypt.Net.BCrypt;

namespace role_play.Controllers
{
    [Route("api/[controller]")]
    public class Users(UserContext context, IConfiguration configuration, IFluentEmail email) : ControllerBase
    {
        private readonly UserContext _context = context;
        private readonly IFluentEmail _email = email;
        private readonly IConfiguration _configuration = configuration;


        [HttpGet("all-users")]
        public IActionResult GetUsers()
        {
            var users = _context.Users.ToList();

            return Ok(users);
        }

        [HttpPost("new-user")]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        { 
            // 1. check if user is null
            if (user == null)
            {
                return BadRequest("User data is required.");
            }


            // 2. Check if Password is null or empty
            if (string.IsNullOrEmpty(user.Password))
            {
                return BadRequest("Password is required.");
            }

            // 3. Check for duplicate username/email BEFORE processing
            if (_context.Users.Any(u => u.Username == user.Username))
            {
                return BadRequest("Username already exists.");
            }

            if (_context.Users.Any(u => u.Email == user.Email))
            {
                return BadRequest("Email already exists.");
            }

            // 4. Validate password pattern
            const string StrongPasswordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";

            if (!Regex.IsMatch(user.Password, StrongPasswordPattern))
            {
                return BadRequest("Password must be at least 8 characters long and include at least one uppercase letter, one lowercase letter, one digit, and one special character (@$!%*?&).");
            }

            try
            {
                // 5. Hash the password
                user.PasswordHash = BC.HashPassword(user.Password.Trim());

                // 6. Clear plain password
                user.Password = null!;

                // 7. timestamps
                user.CreatedAt = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;


                //const otpCode = Math.Ran
                // Set new verification fields
                user.IsVerified = false; // User starts as unverified
                user.OTPCode = GenerateOTP(); // Generate 6-digit OTP
                user.OTPExpiry = DateTime.UtcNow.AddMinutes(15); // OTP valid for 15 mins

                // Set timestamps
                user.CreatedAt = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;

                // to database
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // 9. Return success
                var response = new UserResponse
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role,
                };



                HandleEmailSending(user.Email,
                    user.OTPCode,
                    user.Username
                );

                Console.WriteLine("Touched the email sending func in main flow and email sent");
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Don't throw, return proper error
                return StatusCode(500, $"Error creating user: {ex.Message}");
            }
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (user.IsVerified)
            {
                return BadRequest("Email is already verified.");
            }

            if (user.OTPCode != request.OTPCode)
            {
                return BadRequest("Invalid verification code.");
            }

            if (user.OTPExpiry < DateTime.UtcNow)
            {
                return BadRequest("Verification code has expired.");
            }

            // Mark as verified and clear OTP
            user.IsVerified = true;
            user.OTPCode = null;
            user.OTPExpiry = null;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            
            VerificationEmailSending(user.Email,
                    user.Username
                );

            return Ok(new
            {
                success = true,
                message = "Email verified successfully!",
                user = new
                {
                    user.Id,
                    user.Username,
                    user.Email,
                    user.IsVerified
                }
            });
        }

        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOTP([FromBody] ResendOtpRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (user.IsVerified)
            {
                return BadRequest("Email is already verified.");
            }

            // Generate new OTP
            user.OTPCode = GenerateOTP();
            user.OTPExpiry = DateTime.UtcNow.AddMinutes(15);
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Send new verification email
            HandleEmailSending(user.Email, user.OTPCode, user.Username);

            return Ok(new
            {
                success = true,
                message = "New verification code sent to your email."
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !BC.Verify(request.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid email or password.");
            }

            if (!user.IsVerified)
            {
                return BadRequest(new
                {
                    error = "Email not verified",
                    message = "Please verify your email before logging in.",
                    email = user.Email,
                    canResend = true
                });
            }

            // Return user data (excluding password)
            return Ok(new
            {
                user.Id,
                user.Username,
                user.Email,
                user.FullName,
                user.Role,
                user.IsVerified,
                user.CreatedAt
            });
        }

        private static string GenerateOTP()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString(); // 6-digit code
        }

        public async void HandleEmailSending(
            string to,
            string otpCode,
            string user
            //string subject,
             //string items
        )
        {
            try
            {
                Console.WriteLine("Touched the sending email func");

                var responses = await _email
               .To(to.Trim())
              .Subject($"Verify Your Email - {to}")
            .Body($@"
                <div style='font-family: Arial, sans-serif; padding: 20px;'>
                    <h2>Welcome to Leah, {user}!</h2>
                    <p>Thank you for registering. Please use the code below to verify your email:</p>
                    <div style='background: #f0f0f0; padding: 15px; margin: 20px 0; font-size: 24px; font-weight: bold; letter-spacing: 5px; text-align: center;'>
                        {otpCode}
                    </div>
                    <p>This code will expire in 15 minutes.</p>
                    <p>If you didn't create this account, please ignore this email.</p>
                </div>", true)
               .SendAsync();

                Console.WriteLine("Email sent successfully.");

            }
            catch (Exception ex)
            {
                //return ex.Message;
            }
        }

        public async void VerificationEmailSending(
            string to,
            string user
            //string subject,
             //string items
        )
        {
            try
            {
                Console.WriteLine("Touched the verification email sending");

                var responses = await _email
               .To(to.Trim())
              .Subject($"Your email has been Verified - {to}")
            .Body($@"
                <div style='font-family: Arial, sans-serif; padding: 20px;'>
                    <h2>Welcome to Leah once more, {user}!</h2>
                    <p>Thank you for registering. We can not wait to see the exciting things we can do together</p>
                </div>", true)
               .SendAsync();

                Console.WriteLine("Verification Successful Email sent successfully.");

            }
            catch (Exception ex)
            {
                //return ex.Message;
            }
        }
    }
}
