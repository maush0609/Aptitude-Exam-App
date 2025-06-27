namespace Project8.Backend.Dtos
{
    public class CreateExamDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int DurationMinutes { get; set; }
        public bool IsPublished { get; set; } = false;
    }

    public class UpdateExamDto : CreateExamDto
    {
        public int Id { get; set; }
    }

    public class ExamSummaryDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int QuestionCount { get; set; }
        public bool IsPublished { get; set; }
    }

    public class ExamDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int DurationMinutes { get; set; }
        public bool IsPublished { get; set; }
        public string CreatedBy { get; set; } = string.Empty;

        public List<QuestionDto> Questions { get; set; } = new();
    }

    public class QuestionDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public int Order { get; set; }
    }
}
