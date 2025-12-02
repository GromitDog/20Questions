using TwentyQuestionsConsole;

namespace TwentyQuestions.Services;

public class GameService(HttpClient http)
{
    private readonly HttpClient _http = http ?? throw new ArgumentNullException(nameof(http));
    
    public event Action<GameState?>? GameChanged;
    private GameState? _current;

    public async Task<GameState> StartGame(string userName, string characterName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("User name cannot be empty.", nameof(userName));
        
        if (string.IsNullOrWhiteSpace(characterName))
            throw new ArgumentException("Character name cannot be empty.", nameof(characterName));
        
        if (_current is not null && !_current.IsOver)
            throw new InvalidOperationException("A game is already in progress. Finish or abandon the current game before starting a new one.");
        
        var response = await _http.PostAsJsonAsync("api/Game/Start", new
        {
            UserName = userName,
            CharacterName = characterName
        });

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to start game. {response.StatusCode}: {response.ReasonPhrase}");
        }

        var gameState = await response.Content.ReadFromJsonAsync<GameState>();
        if (gameState is null) throw new Exception("Failed to start game. Invalid game state received.");

        Notify(gameState);
        return gameState;
    }
    
    public async Task<GameState?> GetCurrentGame()
    { 
        var response = await _http.GetAsync("api/Game/Current");
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to find current game, invalid game state received. {response.StatusCode}: {response.ReasonPhrase}");
        }

        if (response.Content.Headers.ContentLength == 0)
        {
            return null;
        }
        
        return await response.Content.ReadFromJsonAsync<GameState?>();
    }
    
    protected void Notify(GameState? state)
    {
        _current = state;
        GameChanged?.Invoke(state);
    }
}