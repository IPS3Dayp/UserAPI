using System;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using UserAPI.Models;
using UserAPI.Services;

namespace userAPI.Controllers
{
    [Controller]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly MongoDBService _mongoDBService;

        public UserController(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
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
                await _mongoDBService.CreateAsync(user);
                return CreatedAtAction(nameof(Get), new { id = user.id }, user);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = "Error saving user data", error = ex.Message });
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> AddActivity(string id, [FromBody] int activityID)
        {
            await _mongoDBService.AddToActivityAsync(id, activityID);
            return NoContent();
        }
    }
}
