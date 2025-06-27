using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project8.Backend.Models
{
    public class UserExam
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int ExamId { get; set; }

        [ForeignKey("ExamId")]
        public Exam Exam { get; set; } = null!;

        public DateTime SubmittedAt { get; set; }

        public int Score { get; set; }

        public ICollection<UserAnswer> Answers { get; set; } = new List<UserAnswer>();
    }
}
