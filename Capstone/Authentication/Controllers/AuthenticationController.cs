using Authentication.Contracts;
using Authentication.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthentication _authentication;
        public AuthenticationController(IAuthentication authentication)
        {
            _authentication = authentication;
        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] Login model)
        {
            var user = await _authentication.CheckLoginCred(model);
            if (user != null)
            {
                return Ok(user);
            }
            return Unauthorized();
        }
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] Register model)
        {
            var userExists = await _authentication.UserRegister(model);
            if (userExists.Status == "Error")
            {
                return StatusCode(StatusCodes.Status500InternalServerError, userExists);
            }
            return Ok(userExists);
        }
        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] Register model)
        {
            var userExists = await _authentication.AdminRegister(model);
            if (userExists.Status == "Error")
            {
                return StatusCode(StatusCodes.Status500InternalServerError, userExists);
            }
            return Ok(userExists);

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(string id)
        {
            var getEmployee=await _authentication.GetEmployeeById(id);
            if(getEmployee==null) return BadRequest();
            return Ok(getEmployee);
        }
    }
}

