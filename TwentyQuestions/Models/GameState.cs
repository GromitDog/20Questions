namespace TwentyQuestionsConsole;

public class GameState(string answererUserName, string characterName)
{
    // primary constructor
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string CharacterName { get; set; } = characterName;

    public string AnswererUserName { get; set; } = answererUserName;
    
    public string? AskerUserName { get; set; }
    
    const int MaxQuestions  = 20;
    public int QuestionsAsked => Questions.Count;
    public Dictionary<string, Answer> Questions { get; } = new();
    public bool IsOver { get; private set; }
    public bool IsWon { get; set; }
    
    public void JoinGame(string askerUserName)
    {
        if (AskerUserName is not null)
            throw new InvalidOperationException("The game is already being played by two players. You cannot join it.");
        
        AskerUserName = askerUserName;
    }
    
    public void RegisterQuestion(string question, Answer answer)
    {
        if (IsOver)
            throw new InvalidOperationException("The game is already over, you cannot register additional questions and answers");
        
        Questions[question] = answer;
       
        if (QuestionsAsked >= MaxQuestions) IsOver = true;
    }

    public void WinGame(string question, Answer answer)
    {
        if (IsOver)
            throw new InvalidOperationException("The game is already over, it cannot now be won");
        Questions.Add(question, answer);
        IsWon = true;
        IsOver = true;
    }

    public void GiveUp()
    {
        if (IsOver)
            throw new InvalidOperationException("The game is already over, so the user cannot give up");
        IsOver = true;
    }
}

public enum Answer
{
    Yes,
    No,
    DontKnow
}