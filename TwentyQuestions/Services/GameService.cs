using Microsoft.EntityFrameworkCore;
using TwentyQuestions.Data;
using TwentyQuestions.Models;

namespace TwentyQuestions.Services;

public class GameService(GameContext context)
{
    private readonly GameContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public event Action<GameState?>? GameChanged;

    public async Task<GameState> StartGame(string userName, string characterName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("User name cannot be empty.", nameof(userName));

        if (string.IsNullOrWhiteSpace(characterName))
            throw new ArgumentException("Character name cannot be empty.", nameof(characterName));

        var game = new GameState(userName, characterName);
        _context.Games.Add(game);
        await _context.SaveChangesAsync();

        Notify(game);
        return game;
    }

    public async Task<IEnumerable<GameState>> GetCurrentGames()
    {
        return await _context.Games.AsNoTracking().Where(g => !g.IsOver).ToListAsync();
    }

    public async Task<GameState> GetGame(Guid gameId)
    {
        var game = await _context.Games.AsNoTracking().FirstOrDefaultAsync(g => g.Id == gameId);
        return game ?? throw new Exception($"Failed to get game with Id {gameId}. Game not found.");
    }
    
    private void Notify(GameState? state)
    {
        GameChanged?.Invoke(state);
    }
    
    public async Task<GameState> JoinGame(Guid gameId, string userName)
    {
        var game = await _context.Games
            .Include(g => g.Questions)
            .FirstOrDefaultAsync(g => g.Id == gameId);
        if (game is null)
            throw new Exception($"Game not found with Id {gameId}");

        game.JoinGame(userName);
        await _context.SaveChangesAsync();

        Notify(game);
        return game;
    }

    public async Task<GameState> AskQuestion(Guid gameId, string question)
    {
        _context.ChangeTracker.Clear();

        var game = await _context.Games
            .Include(g => g.Questions)
            .FirstOrDefaultAsync(g => g.Id == gameId);
        if (game is null)
            throw new Exception($"Game not found with Id {gameId}");

        game.AskQuestion(question);
        await _context.SaveChangesAsync();

        Notify(game);
        return game;
    }

    public async Task<GameState> AnswerQuestion(Guid gameId, Answer answer)
    {
        _context.ChangeTracker.Clear();

        var game = await _context.Games
            .Include(g => g.Questions)
            .FirstOrDefaultAsync(g => g.Id == gameId);
        if (game is null)
            throw new Exception($"Game not found with Id {gameId}");

        var questionId = game.QuestionAwaitingAnswerId();
        if (questionId is null)
            throw new Exception("There is no question awaiting an answer in the current game");

        game.AnswerQuestion(questionId.Value, answer);
        await _context.SaveChangesAsync();

        Notify(game);
        return game;
    }

    public async Task<GameState> WinGame(Guid gameId)
    {
        _context.ChangeTracker.Clear();

        var game = await _context.Games
            .Include(g => g.Questions)
            .FirstOrDefaultAsync(g => g.Id == gameId);
        if (game is null)
            throw new Exception($"Game not found with Id {gameId}");

        var questionId = game.QuestionAwaitingAnswerId();
        if (questionId is not null)
            game.AnswerQuestion(questionId.Value, Answer.Yes);

        game.WinGame();
        await _context.SaveChangesAsync();

        Notify(game);
        return game;
    }

    public async Task<GameState> AbandonGame(Guid gameId)
    {
        _context.ChangeTracker.Clear();

        var game = await _context.Games
            .Include(g => g.Questions)
            .FirstOrDefaultAsync(g => g.Id == gameId);
        if (game is null)
            throw new Exception($"Game not found with Id {gameId}");

        game.GiveUp();
        await _context.SaveChangesAsync();

        Notify(game);
        return game;
    }
    
    public async Task<List<string>> GetFrequentAnswers()
    {
        int threshold = Math.Max(1, await _context.Games.CountAsync() / 1000);
        return await _context.Games
            .GroupBy(g => g.CharacterName)
            .Where(g => g.Count() > threshold)
            .Select(g => g.Key)
            .ToListAsync();
    }
}