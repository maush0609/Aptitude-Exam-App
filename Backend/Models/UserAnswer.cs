using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project8.Backend.Models
{
    public class UserAnswer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserExamId { get; set; }

        [ForeignKey("UserExamId")]
        public UserExam UserExam { get; set; } = null!;

        [Required]
        public int QuestionId { get; set; }

        [ForeignKey("QuestionId")]
        public Question Question { get; set; } = null!;

        public string GivenAnswer { get; set; } = string.Empty;

        public bool IsCorrect { get; set; }
    }
}
