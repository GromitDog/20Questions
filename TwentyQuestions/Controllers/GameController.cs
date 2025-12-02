using Microsoft.AspNetCore.Mvc;
using TwentyQuestions.Services;
using TwentyQuestionsConsole;

namespace TwentyQuestions.Controllers;

[ApiController]
[Route("api/Game")]
public class GameController : ControllerBase
{
    private readonly GameRepository _repository;
    
    public GameController(GameRepository repository)
    {
        _repository = repository;
    }
    

    [HttpPost("Start")]
    public IActionResult StartGame([FromBody] StartGameRequest request)
    {
        try
        {
            var game = new GameState(request.UserName, request.CharacterName);
            _repository.Add(game);
            return Ok(game);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpGet("Current")]
    public IActionResult GetCurrentGame()
    {
        return Ok(_repository.GetCurrentGame());
    }

    
//    [HttpPut("JoinGame/{gameId:guid}")]
//    public static GameState JoinGame(Guid gameId, string userName)
//    {        
//        if (!GamesPlayed.TryGetValue(gameId, out var game))
//           throw new ArgumentException("Game not found with the specified game id");
//        
//        game.JoinGame(userName);
//        return game;
//    }
//    
//    [HttpPut("Join")]
//    public static GameState JoinGame( string userName)
//    {        
//        var game = GetCurrentGame();
//        if (game is null)
//            throw new ArgumentException("Game not found with the specified game id");
//        
//        game.JoinGame(userName);
//        return game;
//    }
//
//    [HttpPut("Win")]
//    public static GameState WinGame(string question, Answer answer)
//    {
//        var game = GetCurrentGame();
//        if (game is null)
//            throw new ArgumentException($"No current game found, did you ask all 20 questions already?");
//        game.WinGame(question, answer);
//        return game;
//    }
//    
//    public static List<string> GetFrequentAnswers()
//    {
//        // return a list of all the character names that have been used more than once per 1000 times in previous games
//        int threshold = Math.Max(1, GamesPlayed.Count / 1000);
//        return GamesPlayed
//            .GroupBy(g => g.Value.CharacterName)
//            .Where(g => g.Count() > threshold)
//            .Select(g => g.Key)
//            .ToList();
//    }
//
//    [HttpGet("{gameId}")]
//    public static GameState GetGameState(Guid gameId)
//    {
//        return GamesPlayed[gameId];
//    }
//
//    [HttpGet("IsOver/{gameId}")]
//    public static bool IsGameOver(Guid gameId)
//    {
//        return GamesPlayed.ContainsKey(gameId);
//    }
//    
//    [HttpGet("AskInGame/{gameId:guid}")]
//    public static void RegisterQuestion(Guid gameId, string question, Answer answer)
//    {
//        if (!GamesPlayed.TryGetValue(gameId, out var game))
//            throw new ArgumentException("Game not found with the specified game id");
//        game.RegisterQuestion(question, answer);
//    }
//    
//    [HttpGet("Ask")]
//    public static void RegisterQuestion(string question, Answer answer)
//    {
//        var game = GetCurrentGame();
//        if (game is null)
//            throw new ArgumentException($"No current game found");
//        game.RegisterQuestion(question, answer);
//    }

//    [HttpPut("AbandonGame/{gameId:guid}")]
//    public static void AbandonGame(Guid gameId)
//    {
//        if (!GamesPlayed.TryGetValue(gameId, out var game))
//            throw new ArgumentException("Game not found with the specified game id");
//
//        if (game.IsOver)
//            throw new InvalidOperationException("The game is already over, so the user cannot abandon it");
//        
//        game.GiveUp();
//    }
//
//    [HttpPut("Abandon")]
//    public static bool AbandonGame()
//    {
//        var currentGame = GetCurrentGame();
//        if (currentGame is null) return false; // no current game to abandon
//
//        AbandonGame(currentGame.Id);
//        return true;
//    }
    
    
}

public class StartGameRequest
{
    public string UserName { get; set; } = "";
    public string CharacterName { get; set; } = "";
}