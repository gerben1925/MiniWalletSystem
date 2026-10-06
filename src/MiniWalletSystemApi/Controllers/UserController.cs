using Microsoft.AspNetCore.Mvc;
using MiniWalletSystemApi.Interfaces.Services;
using MiniWalletSystemApi.Payloads.Requests;

namespace MiniWalletSystemApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController  : ControllerBase
    {
        private readonly IUserService _userService;
        
        public  UserController(IUserService userService)
        {
            _userService = userService;
        }
        
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterNewUser([FromBody] NewUserRequest newUserRequest)
        {
            var result = await _userService.RegisterAsync(newUserRequest);
            if (result > 0)
            {
                return Ok(new  { Message = "User created successfully." });
            }

            return StatusCode(500, new  { Message = "An error occurred while creating user." });
        }
        
    }    
}
