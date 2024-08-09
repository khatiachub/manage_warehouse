using manage_warehouse.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating user: {ex.Message}");
            }
        }


        [HttpPost("ExitProduct")]
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
            catch (Exception ex)
            {
                return StatusCode(500, $"Error exiting product: {ex.Message}");
            }
        }


        [HttpGet("GetAllEntryProducts")]
        //[Authorize(Roles = "admin")]

        public IActionResult GetEntryProducts()
        {
            try
            {
                var prod = _managepack.GetAllEntryProducts();
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


        [HttpGet("GetAllExitProducts")]
        //[Authorize(Roles = "admin")]

        public IActionResult GetExitProducts()
        {
            try
            {
                var prod = _managepack.GetAllExitProducts();
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
        //[Authorize(Roles = "admin")]

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
        //[Authorize(Roles = "admin")]
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
        //[Authorize(Roles = "admin")]

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
        //[Authorize(Roles = "admin")]

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


        [HttpGet("GetAllCurrentBalance")]
        //[Authorize(Roles = "admin")]
        public IActionResult GetAllcurrentBalance()
        {
            try
            {
                var balance = _managepack.GetAllCurrentBalance();
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
        //[Authorize(Roles = "admin")]
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
        //[Authorize(Roles = "admin")]

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


        [HttpGet("GetWarehouses")]
        //[Authorize(Roles = "admin")]
        public IActionResult GetWarehouses()
        {
            try
            {
                var war = _managepack.GetWarehouses();
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
