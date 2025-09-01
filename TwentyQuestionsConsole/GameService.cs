namespace TwentyQuestionsConsole;

public class GameService
{
    private static Dictionary<Guid, GameState> GamesPlayed { get; } = new();
    
    public static GameState StartGame()
    {
        var game = new GameState();
        GamesPlayed.Add(game.Id, game);
        return game;
    }

    public static GameState WinGame(string character)
    {
        var game = GetCurrentGame();
        if (game is null)
            throw new ArgumentException($"No current game found, did you ask all 20 questiosns already?");
        game.WinGame($"Is it {character}?", Answer.Yes, character);
        return game;
    }
    
    public static List<string> GetFrequentAnswers()
    {
        // return a list of all the character names that have been used more than once per 1000 times in previous games
        int threshold = Math.Max(1, GamesPlayed.Count / 1000);
        return GamesPlayed
            .GroupBy(g => g.Value.CharacterName ?? "")
            .Where(g => g.Count() > threshold)
            .Select(g => g.Key)
            .ToList();
    }

    public static GameState GetGameState(Guid gameId)
    {
        return GamesPlayed[gameId];
    }

    public static bool IsGameOver(Guid gameId)
    {
        return GamesPlayed.ContainsKey(gameId);
    }
    
    public static void RegisterQuestion(Guid gameId, string question, Answer answer)
    {
        if (!GamesPlayed.TryGetValue(gameId, out var game))
            throw new ArgumentException("Game not found with the specified game id");
        game.RegisterQuestion(question, answer);
    }
    
    public static void RegisterQuestion(string question, Answer answer)
    {
        var game = GetCurrentGame();
        if (game is null)
            throw new ArgumentException($"No current game found");
        game.RegisterQuestion(question, answer);
    }

    public static GameState? GetCurrentGame()
    {
        return GamesPlayed.Values.FirstOrDefault(g => !g.IsOver);
    }

    public static void AbandonGame(Guid gameId, string character)
    {
        if (!GamesPlayed.TryGetValue(gameId, out var game))
            throw new ArgumentException("Game not found with the specified game id");

        if (game.IsOver)
            throw new InvalidOperationException("The game is already over, so the user cannot abandon it");
        
        game.GiveUp(character);
    }

    public static bool AbandonGame(string character)
    {
        var currentGame = GetCurrentGame();
        if (currentGame is null) return false; // no current game to abandon

        AbandonGame(currentGame.Id, character);
        return true;
    }
    
    
}