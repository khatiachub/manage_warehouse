using manage_warehouse.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Oracle.ManagedDataAccess.Client;

namespace manage_warehouse.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ManagePackage _managepack;
        private readonly IConfiguration _configuration;


        public ProductController(ManagePackage managepackage, IConfiguration configuration)
        {
            _managepack = managepackage;
            _configuration = configuration;
        }

        [HttpPost("EntryProduct")]
        [Authorize(Roles = "operator")]

        public IActionResult EntryProduct([FromBody] ProductModel model)
        {
            try
            {
                var newproduct = _managepack.EntryProduct(model);
                if (newproduct)
                {
                    return Ok(new { message = "product added successfully" });
                }
                else
                {
                    return StatusCode(500, "Failed to add product.");
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


        [HttpPost("ExitProduct")]
        [Authorize(Roles = "operator")]

        public IActionResult ExitProduct([FromBody] ProductModel model)
        {
            try
            {
                var newproduct = _managepack.ExitProduct(model);
                if (newproduct)
                {
                    return Ok(new { message = "product exited successfully" });
                }
                else
                {
                    return StatusCode(500, "Failed to exit product.");
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
                return StatusCode(500, $"Error exiting product: {ex.Message}");
            }
        }


        [HttpGet("GetAllEntryProducts/{id}")]
        [Authorize(Roles = "manager")]

        public IActionResult GetEntryProducts(int id)
        {
            try
            {
                var prod = _managepack.GetAllEntryProducts(id);
                if (prod != null)
                {
                    return Ok(prod);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error getting products: {ex.Message}");
            }
        }


        [HttpGet("GetAllExitProducts/{id}")]
        [Authorize(Roles = "manager")]

        public IActionResult GetExitProducts(int id)
        {
            try
            {
                var prod = _managepack.GetAllExitProducts(id);
                if (prod != null)
                {
                    return Ok(prod);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error getting products: {ex.Message}");
            }
        }

        [HttpGet("GetEntryProduct/{id}")]
        //[Authorize(Roles = "operator")]

        public IActionResult GetEntryProduct(int id)
        {
            try
            {
                var prod = _managepack.GetEntryProduct(id);
                if (prod != null)
                {
                    return Ok(prod);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error getting products: {ex.Message}");
            }
        }

        [HttpGet("GetExitProduct/{id}")]
       // [Authorize(Roles = "operator")]
        public IActionResult GetExitProduct(int id)
        {
            try
            {
                var prod = _managepack.GetExitProduct(id);
                if (prod != null)
                {
                    return Ok(prod);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error getting products: {ex.Message}");
            }
        }


        [HttpPut("UpdateEntryProduct/{id}")]
        [Authorize(Roles = "operator")]

        public IActionResult UpdateEntryProduct([FromBody] ProductModel model, int id)
        {
            try
            {
                var prod = _managepack.UpdateEntryProduct(model, id);
                if (prod)
                {
                    return Ok(prod);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updateing product: {ex.Message}");
            }
        }

        [HttpPut("UpdateExitProduct/{id}")]
        [Authorize(Roles = "operator")]

        public IActionResult UpdateExitProduct([FromBody] ProductModel model, int id)
        {
            try
            {
                var prod = _managepack.UpdateExitProduct(model, id);
                if (prod)
                {
                    return Ok(prod);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updateing product: {ex.Message}");
            }
        }


        [HttpGet("GetAllCurrentBalance/{id}")]
        [Authorize(Roles = "manager")]
        public IActionResult GetAllcurrentBalance(int id)
        {
            try
            {
                var balance = _managepack.GetAllCurrentBalance(id);
                if (balance != null)
                {
                    return Ok(balance);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error getting balance: {ex.Message}");
            }
        }

        [HttpGet("GetCurrentBalance/{id}")]
        [Authorize(Roles = "operator")]
        public IActionResult GetCurrentBalance(int id)
        {
            try
            {
                var balance = _managepack.GetCurrentBalanace(id);
                if (balance != null)
                {
                    return Ok(balance);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error getting current balance: {ex.Message}");
            }
        }


        [HttpPost("AddWarehouse")]
        [Authorize(Roles = "manager")]

        public IActionResult AddWarehouse([FromBody] WarehouseModel model)
        {
            try
            {
                var newwarehouse = _managepack.AddWarehouse(model);
                if (newwarehouse)
                {
                    return Ok(new { message = "warehouse added successfully" });
                }
                else
                {
                    return StatusCode(500, "Failed to add warehouse.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error adding warehouse: {ex.Message}");
            }
        }


        [HttpPut("UpdateWarehouse/{id}")]
        [Authorize(Roles = "manager")]

        public IActionResult UpdateWarehouse([FromBody] WarehouseModel model, int id)
        {
            try
            {
                var warehouse = _managepack.UpdateWarehouse(model, id);
                if (warehouse)
                {
                    return Ok(warehouse);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updateing warehouse: {ex.Message}");
            }
        }


        [HttpGet("GetWarehouses/{id}")]
        public IActionResult GetWarehouses(int id)
        {
            try
            {
                var war = _managepack.GetWarehouses(id);
                if (war != null)
                {
                    return Ok(war);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error getting warehoouses: {ex.Message}");
            }
        }
        [HttpGet("GetWarehouseForUser/{id}")]
        public IActionResult GetWarehouseForUser(int id)
        {
            try
            {
                var war = _managepack.GetCompanyWarehouse(id);
                if (war != null)
                {
                    return Ok(war);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error getting warehoouses: {ex.Message}");
            }
        }
    }
}
