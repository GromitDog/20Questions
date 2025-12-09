namespace TwentyQuestions.Models;

public class Question
{
    public Guid QuestionId { get; set; } = Guid.NewGuid();
    public Guid GameStateId { get; set; }
    public required string QuestionText { get; set; }
    public Answer? Answer { get; set; }
}