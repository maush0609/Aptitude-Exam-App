public class ExamWithQuestionsDto
{
    public int DurationMinutes { get; set; }
    public List<Question1Dto> Questions { get; set; } = new();
}

public class Question1Dto
{
    public int Id { get; set; }
    public string Text { get; set; } = "";
    public string OptionA { get; set; } = "";
    public string OptionB { get; set; } = "";
    public string OptionC { get; set; } = "";
    public string OptionD { get; set; } = "";
}
