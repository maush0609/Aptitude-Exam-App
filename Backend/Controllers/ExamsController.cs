using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project8.Backend.Dtos;
using Project8.Backend.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Project8.Backend.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ExamsController : ControllerBase
    {
        private readonly AdminService _adminService;

        public ExamsController(AdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllExams()
        {
            var exams = await _adminService.GetAllExamDtosAsync();
            return Ok(exams);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetExam(int id)
        {
            var exam = await _adminService.GetExamDtoByIdAsync(id);
            return exam == null ? NotFound() : Ok(exam);
        }

         [HttpPost]
public async Task<IActionResult> CreateExam([FromBody] CreateExamDto dto)
{
    var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
    if (string.IsNullOrEmpty(userId))
        return Unauthorized("User ID not found.");

    var createdExam = await _adminService.CreateExamAsync(dto, userId);
    if (createdExam == null)
    return BadRequest("Failed to create exam.");
    return CreatedAtAction(nameof(GetExam), new { id = createdExam.Id }, createdExam);
}


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExam(int id, [FromBody] UpdateExamDto dto)
        {
            if (id != dto.Id)
                return BadRequest("Exam ID in URL does not match DTO.");

            var updatedExam = await _adminService.UpdateExamAsync(dto);
            return updatedExam == null ? NotFound() : Ok(updatedExam);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExam(int id)
        {
            var result = await _adminService.DeleteExamAsync(id);
            return result ? NoContent() : NotFound();
        }

        [HttpPost("{examId}/questions/{questionId}")]
        public async Task<IActionResult> AddQuestionToExam(int examId, int questionId, [FromQuery] int? order = null)
        {
            var result = await _adminService.AddQuestionToExamAsync(examId, questionId, order);
            return result 
    ? Ok(new { message = "Question added." }) 
    : BadRequest(new { message = "Invalid exam or question ID." });

        }

        [HttpDelete("{examId}/questions/{questionId}")]
        public async Task<IActionResult> RemoveQuestionFromExam(int examId, int questionId)
        {
            var result = await _adminService.RemoveQuestionFromExamAsync(examId, questionId);
            return result ? NoContent() : BadRequest("Question not found in exam.");
        }

        [HttpPut("{examId}/questions/{questionId}/order")]
        public async Task<IActionResult> UpdateQuestionOrder(int examId, int questionId, [FromQuery] int newOrder)
        {
            var result = await _adminService.UpdateQuestionOrderInExamAsync(examId, questionId, newOrder);
            return result ? Ok("Order updated.") : BadRequest("Failed to update order.");
        }
    }
}
