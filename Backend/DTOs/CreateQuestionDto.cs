using System;
using System.ComponentModel.DataAnnotations;
namespace Project8.Backend.Dtos
{
    public class CreateQuestionDto
    {
        [Required] public string Text { get; set; } = string.Empty;
        [Required] public string OptionA { get; set; } = string.Empty;
        [Required] public string OptionB { get; set; } = string.Empty;
        [Required] public string OptionC { get; set; } = string.Empty;
        [Required] public string OptionD { get; set; } = string.Empty;
        [Required] public char CorrectAnswer { get; set; }
        public string? Category { get; set; }
        public int DifficultyLevel { get; set; }
        [Required] public string CreatedById { get; set; } = string.Empty;
    }
}
