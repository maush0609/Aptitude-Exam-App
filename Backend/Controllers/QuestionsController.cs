using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project8.Backend.Dtos;
using Project8.Backend.Models;
using Project8.Backend.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Project8.Backend.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly AdminService _adminService;

        public QuestionsController(AdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllQuestions()
        {
            var questions = await _adminService.GetAllQuestionsAsync();
            return Ok(questions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuestion(int id)
        {
            var question = await _adminService.GetQuestionByIdAsync(id);
            if (question == null) return NotFound();
            return Ok(question);
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuestion([FromBody] CreateQuestionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            dto.CreatedById = userId;

            var createdQuestion = await _adminService.CreateQuestionAsync(dto);
            return CreatedAtAction(nameof(GetQuestion), new { id = createdQuestion.Id }, createdQuestion);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuestion(int id, [FromBody] UpdateQuestionDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID in URL and payload do not match.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedQuestion = await _adminService.UpdateQuestionAsync(dto);
            if (updatedQuestion == null)
                return NotFound();

            return Ok(updatedQuestion);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var success = await _adminService.DeleteQuestionAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}
