using manage_warehouse.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Oracle.ManagedDataAccess.Client;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace manage_warehouse.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController :MainController
    {
        private readonly ManagePackage _managepack;

        public UserController(ManagePackage managepackage, IConfiguration configuration):base(configuration) 
        {
            _managepack = managepackage;
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
            catch (OracleException ex)
            {
                if (ex.Number == 20001)
                {
                    return StatusCode(20001, $"Oracle error occurred: {ex.Message}");
                }
                else
                {
                    return StatusCode(500, $"Oracle error occurred: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating user: {ex.Message}");
            }
        }

        [HttpPost("CreateUser")]
        [Authorize(Roles = "admin")]

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
            catch (OracleException ex)
            {
                if (ex.Number == 20001)
                {
                    return StatusCode(20001, $"Oracle error occurred: {ex.Message}");
                }
                else
                {
                    return StatusCode(500, $"Oracle error occurred: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating user: {ex.Message}");
            }
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
                    return Ok(new { message = "login was successfull",token,user.id,user.role });
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
        [Authorize(Roles = "admin")]

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
        [Authorize(Roles = "admin")]

        public IActionResult Update([FromBody] EditUserModel model, int id)
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
