namespace TwentyQuestionsConsole;

public class GameState()
{
    // primary constructor
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string? CharacterName { get; private set; } = null; // this will not be known until the game is won or abandoned
    
    const int MaxQuestions  = 20;
    public int QuestionsAsked => Questions.Count;
    public Dictionary<string, Answer> Questions { get; } = new();
    public bool IsOver { get; private set; }

    public bool IsWon { get; set; }

    public void RegisterQuestion(string question, Answer answer)
    {
        if (IsOver)
            throw new InvalidOperationException("The game is already over, you cannot register additional questions and answers");
        
        Questions[question] = answer;
       
        if (QuestionsAsked >= MaxQuestions) IsOver = true;
    }

    public void WinGame(string question, Answer answer, string characterName)
    {
        if (IsOver)
            throw new InvalidOperationException("The game is already over, it cannot now be won");
        CharacterName = characterName;
        Questions.Add(question, answer);
        IsWon = true;
        IsOver = true;
    }

    public void GiveUp(string characterName)
    {
        if (IsOver)
            throw new InvalidOperationException("The game is already over, so the user cannot give up");
        CharacterName = characterName;
        IsOver = true;
    }
}

public enum Answer
{
    Yes,
    No,
    DontKnow
}