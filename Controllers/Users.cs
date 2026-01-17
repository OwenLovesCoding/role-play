using Microsoft.AspNetCore.Mvc;
using role_play.Models;

namespace role_play.Controllers
{
    [Route("api/[controller]")]
    public class Users(UserContext context): ControllerBase
    {
        private readonly UserContext _context = context;


        [HttpGet("all-users")]
        public IActionResult GetUsers()
        {
            var users = _context.Users.ToList();
           
            return Ok(users);
        }
    }
}
