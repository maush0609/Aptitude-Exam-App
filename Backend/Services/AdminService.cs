using Microsoft.EntityFrameworkCore;
using Project8.Backend.Data;
using Project8.Backend.Dtos;
using Project8.Backend.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project8.Backend.Services
{
    public class AdminService
    {
        private readonly ApplicationDbContext _context;
        public AdminService(ApplicationDbContext context) => _context = context;

        public async Task<Question> CreateQuestionAsync(CreateQuestionDto dto)
        {
            var q = new Question
            {
                Text = dto.Text,
                OptionA = dto.OptionA,
                OptionB = dto.OptionB,
                OptionC = dto.OptionC,
                OptionD = dto.OptionD,
                CorrectAnswer = dto.CorrectAnswer,
                Category = dto.Category ?? string.Empty,
                DifficultyLevel = dto.DifficultyLevel,
                CreatedById = dto.CreatedById,
                CreatedBy = null! 
            };
            _context.Questions.Add(q);
            await _context.SaveChangesAsync();
            return q;
        }

        public async Task<List<Question>> GetAllQuestionsAsync()
        {
            return await _context.Questions
                .Include(q => q.CreatedBy)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();
        }

        public async Task<Question?> GetQuestionByIdAsync(int id)
        {
            return await _context.Questions
                .Include(q => q.CreatedBy)
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<Question?> UpdateQuestionAsync(UpdateQuestionDto dto)
        {
            var q = await _context.Questions.FindAsync(dto.Id);
            if (q is null) return null;

            q.Text = dto.Text;
            q.OptionA = dto.OptionA;
            q.OptionB = dto.OptionB;
            q.OptionC = dto.OptionC;
            q.OptionD = dto.OptionD;
            q.CorrectAnswer = dto.CorrectAnswer;
            q.Category = dto.Category ?? string.Empty;
            q.DifficultyLevel = dto.DifficultyLevel;

            await _context.SaveChangesAsync();
            return q;
        }

        public async Task<bool> DeleteQuestionAsync(int id)
        {
            var q = await _context.Questions.FindAsync(id);
            if (q is null) return false;

            var inUse = await _context.ExamQuestions.AnyAsync(eq => eq.QuestionId == id);
            if (inUse) return false;

            _context.Questions.Remove(q);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ExamDetailDto?> CreateExamAsync(CreateExamDto dto, string userId)
        {
            var exam = new Exam
            {
                Title = dto.Title,
                Description = dto.Description,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                DurationMinutes = dto.DurationMinutes,
                IsPublished = dto.IsPublished,
                CreatedById = userId,
                CreatedBy = null!, // <-- fix for CS9035
                ExamQuestions = new List<ExamQuestion>()
            };

            _context.Exams.Add(exam);
            await _context.SaveChangesAsync();

            return await GetExamDtoByIdAsync(exam.Id);
        }

        public async Task<List<ExamSummaryDto>> GetAllExamDtosAsync()
        {
            return await _context.Exams
                .Include(e => e.ExamQuestions)
                .Select(e => new ExamSummaryDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    IsPublished = e.IsPublished,
                    QuestionCount = e.ExamQuestions.Count
                }).ToListAsync();
        }

        public async Task<ExamDetailDto?> GetExamDtoByIdAsync(int id)
        {
            var exam = await _context.Exams
                .Include(e => e.CreatedBy)
                .Include(e => e.ExamQuestions.OrderBy(eq => eq.Order))
                .ThenInclude(eq => eq.Question)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (exam == null) return null;

            return new ExamDetailDto
            {
                Id = exam.Id,
                Title = exam.Title,
                Description = exam.Description,
                StartTime = exam.StartTime,
                EndTime = exam.EndTime,
                DurationMinutes = exam.DurationMinutes,
                IsPublished = exam.IsPublished,
                CreatedBy = exam.CreatedBy?.UserName ?? "Unknown", // <-- fix for CS8601
                Questions = exam.ExamQuestions.Select(eq => new QuestionDto
                {
                    Id = eq.QuestionId,
                    Text = eq.Question.Text,
                    Order = eq.Order
                }).ToList()
            };
        }

        public async Task<ExamDetailDto?> UpdateExamAsync(UpdateExamDto dto)
        {
            var exam = await _context.Exams.FindAsync(dto.Id);
            if (exam == null) return null;

            exam.Title = dto.Title;
            exam.Description = dto.Description;
            exam.StartTime = dto.StartTime;
            exam.EndTime = dto.EndTime;
            exam.DurationMinutes = dto.DurationMinutes;
            exam.IsPublished = dto.IsPublished;

            await _context.SaveChangesAsync();
            return await GetExamDtoByIdAsync(dto.Id);
        }

        public async Task<bool> DeleteExamAsync(int id)
        {
            var exam = await _context.Exams.Include(e => e.ExamQuestions)
                                           .FirstOrDefaultAsync(e => e.Id == id);
            if (exam is null) return false;

            _context.ExamQuestions.RemoveRange(exam.ExamQuestions);
            _context.Exams.Remove(exam);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddQuestionToExamAsync(int examId, int questionId, int? order = null)
        {
            var exam = await _context.Exams.FindAsync(examId);
            var question = await _context.Questions.FindAsync(questionId);
            if (exam == null || question == null) return false;

            if (!order.HasValue)
            {
                var maxOrder = await _context.ExamQuestions
                    .Where(eq => eq.ExamId == examId)
                    .MaxAsync(eq => (int?)eq.Order) ?? 0;
                order = maxOrder + 1;
            }

            _context.ExamQuestions.Add(new ExamQuestion
            {
                ExamId = examId,
                QuestionId = questionId,
                Order = order.Value,
                Exam = exam,
                Question = question
            });
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveQuestionFromExamAsync(int examId, int questionId)
        {
            var eq = await _context.ExamQuestions
                .FirstOrDefaultAsync(x => x.ExamId == examId && x.QuestionId == questionId);
            if (eq is null) return false;

            _context.ExamQuestions.Remove(eq);
            await _context.SaveChangesAsync();
            await ReorderExamQuestionsAsync(examId);
            return true;
        }

        public async Task<bool> ReorderExamQuestionsAsync(int examId)
        {
            var list = await _context.ExamQuestions
                .Where(x => x.ExamId == examId)
                .OrderBy(x => x.Order)
                .ToListAsync();
            for (var i = 0; i < list.Count; i++)
                list[i].Order = i + 1;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateQuestionOrderInExamAsync(int examId, int questionId, int newOrder)
        {
            var eq = await _context.ExamQuestions
                .FirstOrDefaultAsync(x => x.ExamId == examId && x.QuestionId == questionId);
            if (eq == null) return false;

            var oldOrder = eq.Order;
            if (newOrder < oldOrder)
            {
                await _context.ExamQuestions
                    .Where(x => x.ExamId == examId && x.Order >= newOrder && x.Order < oldOrder)
                    .ForEachAsync(x => x.Order++);
            }
            else if (newOrder > oldOrder)
            {
                await _context.ExamQuestions
                    .Where(x => x.ExamId == examId && x.Order > oldOrder && x.Order <= newOrder)
                    .ForEachAsync(x => x.Order--);
            }

            eq.Order = newOrder;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
