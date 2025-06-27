using System.ComponentModel.DataAnnotations;

namespace Project8.Backend.Models
{
    public class Question
    {
        public int Id { get; set; }

        [Required]
        public string Text { get; set; } = string.Empty;
        
        [Required]
        public string OptionA { get; set; } = string.Empty;
        
        [Required]
        public string OptionB { get; set; } = string.Empty;
        
        [Required]
        public string OptionC { get; set; } = string.Empty;
        
        [Required]
        public string OptionD { get; set; } = string.Empty;
        
        [Required]
        public char CorrectAnswer { get; set; } 
        
        public string Category { get; set; } = string.Empty;
        public int DifficultyLevel { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedById { get; set; } = string.Empty;
        public required User CreatedBy { get; set; }
    }
}
