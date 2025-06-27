namespace Project8.Backend.Models
{
    public class ExamQuestion
    {
        public int ExamId { get; set; }
        public required Exam Exam { get; set; }
        
        public int QuestionId { get; set; }
        public required Question Question { get; set; }
        
        public int Order { get; set; } 
    }
}