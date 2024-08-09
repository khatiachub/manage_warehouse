using manage_warehouse.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace manage_warehouse.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ManagePackage _managepack;
        private readonly IConfiguration _configuration;


        public UserController(ManagePackage managepackage, IConfiguration configuration)
        {
            _managepack = managepackage;
            _configuration = configuration;
        }


        [HttpPost("CreateCompany")]
        public IActionResult CreateEmployee([FromBody] CompanyRegisterModel model)
        {
            try
            {
                var newcompany= _managepack.RegisterCompany(model);
                if (newcompany)
                {
                    return Ok(new { message = "company created successfully" });
                }
                else
                {
                    return StatusCode(500, "Failed to create company.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating user: {ex.Message}");
            }
        }

        [HttpPost("CreateUser")]
        public IActionResult CreateUser([FromBody] UserRegisterModel model)
        {
            try
            {
                var newcompany = _managepack.RegisterUser(model);
                if (newcompany)
                {
                    return Ok(new { message = "company created successfully" });
                }
                else
                {
                    return StatusCode(500, "Failed to create company.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating user: {ex.Message}");
            }
        }

        private string GenerateNewJsonWebToken(List<Claim> claims)
        {
            var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var tokenObject = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.MaxValue,
                claims: claims,
                signingCredentials: new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256)
            );
            string token = new JwtSecurityTokenHandler().WriteToken(tokenObject);
            return token;
        }

        [HttpPost("LoginUser")]
        public IActionResult LoginUser([FromBody] UserLoginModel model)
        {
            try
            {
                var user= _managepack.LoginUser(model);
                if (user?.id!=null)
                {
                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Email, user.username),
                        new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                        new Claim("JWTID", Guid.NewGuid().ToString()),
                        new Claim(ClaimTypes.Role, user.role)
                    };
                    var token = GenerateNewJsonWebToken(authClaims);
                    return Ok(new { message = "login was successfull",token });
                }
                else
                {
                    return StatusCode(500, "Failed to login.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating user: {ex.Message}");
            }
        }

        [HttpGet("GetUsers/{id}")]
        //[Authorize(Roles = "admin")]

        public IActionResult GetUsers(int id)
        {
            try
            {
                var user = _managepack.GetUsers(id);
                if (user != null)
                {
                    return Ok(user);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating user: {ex.Message}");
            }
        }
        [HttpPut("UpdateUser/{id}")]
        //[Authorize(Roles = "admin")]

        public IActionResult Update([FromBody] UserRegisterModel model, int id)
        {
            try
            {
                var user = _managepack.UpdateUser(model, id);
                if (user)
                {
                    return Ok(user);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating user: {ex.Message}");
            }
        }
    }
}
