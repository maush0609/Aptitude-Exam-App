using System.ComponentModel.DataAnnotations;

namespace Project8.Backend.Models
{
    public class Exam
    {
        public int Id { get; set; }
        
        [Required]
        public string Title { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public DateTime StartTime { get; set; }
        
        [Required]
        public DateTime EndTime { get; set; }
        
        public int DurationMinutes { get; set; }
        public bool IsPublished { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedById { get; set; } = string.Empty;
        public required User CreatedBy { get; set; }
        
        public required ICollection<ExamQuestion> ExamQuestions { get; set; }
    }
}