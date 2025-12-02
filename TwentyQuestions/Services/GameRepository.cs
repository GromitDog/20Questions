using TwentyQuestionsConsole;

namespace TwentyQuestions.Services;

public class GameRepository
{
    private readonly Dictionary<Guid, GameState> _gamesPlayed = new();

    public void Add(GameState game) => _gamesPlayed.Add(game.Id, game);
    
    public GameState? GetCurrentGame() => _gamesPlayed.Values.FirstOrDefault(g => !g.IsOver);
    
    public GameState? GetById(Guid id) => _gamesPlayed.GetValueOrDefault(id);

}