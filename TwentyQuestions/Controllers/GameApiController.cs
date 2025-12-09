using Microsoft.AspNetCore.Mvc;
using TwentyQuestions.Models;
using TwentyQuestions.Services;

namespace TwentyQuestions.Controllers;

[ApiController]
[Route("api/Games")]
public class GameApiController(GameService gameService) : ControllerBase
{    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GameState>>> GetCurrentGames()
    {
        return Ok(await gameService.GetCurrentGames());
    }
       
    [HttpGet("FrequentAnswers")]
    public async Task<ActionResult<List<string>>> GetFrequentAnswers()
    {
        return Ok(await gameService.GetFrequentAnswers());
    }
    
    [HttpGet("GetGame/{gameId:guid}")]
    public async Task<ActionResult<GameState>> GetGame(Guid gameId)
    {
        try
        {
            return Ok(await gameService.GetGame(gameId));
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    
    [HttpPost("Start")]
    public async Task<ActionResult<GameState>> StartGame([FromBody] StartGameRequest request)
    {
        try
        {
            return Ok(await gameService.StartGame(request.UserName, request.CharacterName));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpPut("JoinGame")]
    public async Task<ActionResult<GameState>> JoinGame([FromBody] JoinGameRequest request)
    {
        try
        {
            return Ok(await gameService.JoinGame(request.GameId, request.UserName));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpPut("AskQuestion")]
    public async Task<ActionResult<GameState>> AskQuestion([FromBody] AskQuestionRequest questionRequest)
    {
        try
        {
            return Ok(await gameService.AskQuestion(questionRequest.GameId, questionRequest.Question));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("AnswerQuestion")]
    public async Task<ActionResult<GameState>> AnswerQuestion([FromBody] AnswerQuestionRequest answerRequest)
    {
        try
        {
            return Ok(await gameService.AnswerQuestion(answerRequest.GameId, answerRequest.Answer));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("WinGame")]
    public async Task<ActionResult<GameState>> WinGame([FromBody] Guid gameId)
    {
        try
        {
            return Ok(await gameService.WinGame(gameId));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpPut("AbandonGame")]
    public async Task<ActionResult<GameState>> AbandonGame([FromBody] Guid gameId)
    {
        try
        {
            return Ok(await gameService.AbandonGame(gameId));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("IsOver/{gameId:guid}")]
    public async Task<ActionResult<bool>> IsGameOver(Guid gameId)
    {
        try
        {
            var game = await gameService.GetGame(gameId);
            return Ok(game.IsOver);
        }
        catch
        {
            return Ok(false);
        }
    }
}

public class StartGameRequest
{
    public string UserName { get; set; } = "";
    public string CharacterName { get; set; } = "";
}

public class JoinGameRequest
{
    public Guid GameId { get; set; }
    public string UserName { get; set; } = "";
}

public class AskQuestionRequest
{
    public  Guid GameId { get; set; }
    public string Question { get; set; } = "";
}

public class AnswerQuestionRequest
{
    public Guid GameId { get; set; }
    public Answer Answer { get; set; }
}