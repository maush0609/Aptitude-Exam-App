namespace Project8.Backend.Dtos
{
    public class ActiveExamDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
    }

    public class SubmitAnswerDto
    {
        public int QuestionId { get; set; }
        public string UserAnswer { get; set; } = string.Empty;
    }

    public class SubmitExamDto
    {
        public int ExamId { get; set; }
        public List<SubmitAnswerDto> Answers { get; set; } = new();
    }

    public class ExamResultDto
    {
        public int UserExamId { get; set; }
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
    }

    public class ReviewAnswerDto
    {
        public string QuestionText { get; set; } = string.Empty;
        public string GivenAnswer { get; set; } = string.Empty;
        public string CorrectAnswer { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }

    public class ExamReviewDto
    {
        public ExamResultDto Result { get; set; } = new();
        public List<ReviewAnswerDto> Questions { get; set; } = new();
    }
}
