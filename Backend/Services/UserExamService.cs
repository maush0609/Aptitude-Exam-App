using Microsoft.EntityFrameworkCore;
using Project8.Backend.Data;
using Project8.Backend.Dtos;
using Project8.Backend.Models;
using System.Security.Claims;

namespace Project8.Backend.Services
{
    public class UserExamService
    {
        private readonly ApplicationDbContext _context;
        public UserExamService(ApplicationDbContext context) => _context = context;

        public async Task<List<ActiveExamDto>> GetActiveExamsAsync()
        {
            return await _context.Exams
                .Where(e => e.IsPublished)
                .Select(e => new ActiveExamDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    DurationMinutes = e.DurationMinutes
                }).ToListAsync();
        }

        public async Task<List<Question>> GetQuestionsForExamAsync(int examId)
        {
            return await _context.ExamQuestions
                .Where(eq => eq.ExamId == examId)
                .OrderBy(eq => eq.Order)
                .Select(eq => eq.Question)
                .ToListAsync();
        }

        public async Task<ExamResultDto> SubmitExamAsync(ClaimsPrincipal user, SubmitExamDto dto)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var ue = new UserExam
            {
                UserId = userId,
                ExamId = dto.ExamId,
                SubmittedAt = DateTime.UtcNow,
                Score = 0,
                Answers = new List<UserAnswer>()
            };
            _context.UserExams.Add(ue);
            await _context.SaveChangesAsync();

            int score = 0;

            var questions = await _context.Questions
                .Where(q => dto.Answers.Select(a => a.QuestionId).Contains(q.Id))
                .ToDictionaryAsync(q => q.Id, q => q.CorrectAnswer.ToString().Trim().ToLower());

            foreach (var a in dto.Answers)
            {
                string givenAns = (a.UserAnswer ?? "").Trim().ToLower();
                bool correct = questions.TryGetValue(a.QuestionId, out var correctAns) && correctAns == givenAns;

                if (correct) score++;

                var userAnswer = new UserAnswer
                {
                    UserExamId = ue.Id,
                    QuestionId = a.QuestionId,
                    GivenAnswer = a.UserAnswer ?? "",
                    IsCorrect = correct
                };

                ue.Answers.Add(userAnswer);
            }

            ue.Score = score;
            await _context.SaveChangesAsync();

            int total = await _context.ExamQuestions.CountAsync(eq => eq.ExamId == dto.ExamId);
            return new ExamResultDto { UserExamId = ue.Id, Score = score, TotalQuestions = total };
        }

        public async Task<ExamWithQuestionsDto?> GetExamWithQuestionsAsync(int examId) 
        {
            var exam = await _context.Exams
                .Include(e => e.ExamQuestions)
                    .ThenInclude(eq => eq.Question)
                .FirstOrDefaultAsync(e => e.Id == examId && e.IsPublished);

            if (exam == null)
                return null;

            var questions = exam.ExamQuestions
                .OrderBy(eq => eq.Order)
                .Select(eq => new Question1Dto
                {
                    Id = eq.Question.Id,
                    Text = eq.Question.Text,
                    OptionA = eq.Question.OptionA,
                    OptionB = eq.Question.OptionB,
                    OptionC = eq.Question.OptionC,
                    OptionD = eq.Question.OptionD
                }).ToList();

            return new ExamWithQuestionsDto
            {
                DurationMinutes = exam.DurationMinutes,
                Questions = questions
            };
        }

        public async Task<ExamReviewDto?> GetReviewAsync(ClaimsPrincipal user, int userExamId)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var ue = await _context.UserExams
                .Include(x => x.Answers).ThenInclude(a => a.Question)
                .Include(x => x.Exam)
                .FirstOrDefaultAsync(x => x.Id == userExamId && x.UserId == userId);

            if (ue == null) return null;

            return new ExamReviewDto
            {
                Result = new ExamResultDto
                {
                    UserExamId = ue.Id,
                    Score = ue.Score,
                    TotalQuestions = ue.Answers.Count
                },
                Questions = ue.Answers.Select(a => new ReviewAnswerDto
                {
                    QuestionText = a.Question.Text,
                    GivenAnswer = a.GivenAnswer,
                    CorrectAnswer = a.Question.CorrectAnswer.ToString(),
                    IsCorrect = a.IsCorrect
                }).ToList()
            };
        }
    }
}
