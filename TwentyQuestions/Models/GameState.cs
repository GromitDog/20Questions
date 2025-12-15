namespace TwentyQuestions.Models;

public class GameState(string answererUserName, string characterName)
{
    // primary constructor
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string CharacterName { get; set; } = characterName;

    public string AnswererUserName { get; set; } = answererUserName;
    
    public string? AskerUserName { get; set; }
    
    const int MaxQuestions  = 20;
    public int QuestionsAnswered => Questions.Count(q => q.Answer is not null && q.Answer != Answer.InvalidQuestion);
    public List<Question> Questions { get; } = [];
    public bool IsOver { get; private set; }
    public bool IsWon { get; set; }
    
    public void JoinGame(string askerUserName)
    {
        if (AskerUserName is not null)
            throw new InvalidOperationException("The game is already being played by two players. You cannot join it.");
        
        AskerUserName = askerUserName;
    }
    
    public void AskQuestion(string question)
    {
        if (IsOver)
            throw new InvalidOperationException("The game is already over, you cannot register additional questions and answers");

        if (Questions.Any(q => q.Answer is null))
            throw new InvalidOperationException("You must wait for an answer to the previous question before asking a new one.");

        Questions.Add(new Question(){QuestionText = question, GameStateId = Id});

        if (QuestionsAnswered >= MaxQuestions) IsOver = true;
    }
        
    public void AnswerQuestion(Guid questionId, Answer answer)
    {
        if (IsOver)
            throw new InvalidOperationException("The game is already over, you cannot register additional questions and answers");
        
        var question = Questions.FirstOrDefault(q => q.QuestionId == questionId);
        
        if (question is null)
            throw new ArgumentException("The question with the specified ID was not found in the current game.");
        
        question.Answer = answer;
       
        if (QuestionsAnswered >= MaxQuestions) IsOver = true;
    }

    public void WinGame()
    {
        if (IsOver)
            throw new InvalidOperationException("The game is already over, it cannot now be won");
        IsWon = true;
        IsOver = true;
    }

    public void GiveUp()
    {
        if (IsOver)
            throw new InvalidOperationException("The game is already over, so the user cannot give up");
        IsOver = true;
    }
    
    public Guid? QuestionAwaitingAnswerId()
    {
        return Questions.FirstOrDefault(q => q.Answer is null)?.QuestionId;
    }
}

public enum Answer
{
    Yes,
    No,
    DontKnow,
    InvalidQuestion
}