using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project8.Backend.Dtos;
using Project8.Backend.Services;

namespace Project8.Backend.Controllers
{
    [Authorize]
    [Route("api/user-exam")]
    [ApiController]
    public class UserExamController : ControllerBase
    {
        private readonly UserExamService _svc;
        public UserExamController(UserExamService svc) => _svc = svc;

        [HttpGet("active")]
        public async Task<IActionResult> GetActive() =>
            Ok(await _svc.GetActiveExamsAsync());

        [HttpGet("questions/{examId}")]
        public async Task<IActionResult> GetQuestions(int examId) =>
            Ok(await _svc.GetQuestionsForExamAsync(examId));

        [HttpPost("submit")]
        public async Task<IActionResult> Submit([FromBody] SubmitExamDto dto)
        {
            var result = await _svc.SubmitExamAsync(User, dto);
            return CreatedAtAction(nameof(Review), new { userExamId = result.UserExamId }, result);
        }

        [HttpGet("exam/{examId}")]
        public async Task<IActionResult> GetExamWithQuestions(int examId)
        {
            var data = await _svc.GetExamWithQuestionsAsync(examId); 
            return data == null ? NotFound() : Ok(data);
        }

        [HttpGet("review/{userExamId}")]
        public async Task<IActionResult> Review(int userExamId)
        {
            var review = await _svc.GetReviewAsync(User, userExamId);
            return review != null ? Ok(review) : NotFound();
        }
    }
}
