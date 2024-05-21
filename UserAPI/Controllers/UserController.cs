using System;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using UserAPI.Models;
using UserAPI.Services;
using Microsoft.Extensions.Logging;

namespace userAPI.Controllers
{
    [Controller]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly MongoDBService _mongoDBService;

        public UserController(MongoDBService mongoDBService, ILogger<UserController> logger)
        {
            _mongoDBService = mongoDBService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<List<User>> Get()
        {
            return await _mongoDBService.GetAsync();
        }


        [HttpGet("{id}/Activities")]
        public async Task<ActionResult<List<int>>> GetUserActivities(string id)
        {
            var ActivityIds = await _mongoDBService.GetActivityListByUserIdAsync(id);
            if (ActivityIds == null)
                return NotFound();

            return Ok(ActivityIds);
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<User>> GetUserByEmail(string email)
        {
            var user = await _mongoDBService.GetUserByEmailAsync(email);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] User user)
        {
            try
            {
                // Valideer de invoer
                if (user == null || string.IsNullOrEmpty(user.email) || !IsValidEmail(user.email))
                {
                    return BadRequest(new { message = "Invalid email address" });
                }

                await _mongoDBService.CreateAsync(user);
                return CreatedAtAction(nameof(Get), new { id = user.id }, user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error saving user data", error = ex.Message });
            }
        }

        // E-mailvalidatie
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }




        [HttpPut("{id}")]
        public async Task<IActionResult> AddActivity(string id, [FromBody] int activityID)
        {
            try
            {
                // Log the received values
                _logger.LogInformation("Received request to add activity. User ID: {UserId}, Activity ID: {ActivityId}", id, activityID);

                // Call the method to add activity
                await _mongoDBService.AddToActivityAsync(id, activityID);

                // Return success response
                return NoContent();
            }
            catch (Exception ex)
            {
                // Log any exceptions
                _logger.LogError(ex, "An error occurred while adding activity.");

                // Return error response
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
