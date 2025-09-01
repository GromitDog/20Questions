using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

namespace TwentyQuestionsConsole;

public static class FunctionRegistry
{
    public static IEnumerable<AITool> GetTools(this IServiceProvider sp)
    {

        var gameService = sp.GetRequiredService<GameService>();

        var previousAnswersFn = typeof(GameService)
            .GetMethod(nameof(GameService.GetFrequentAnswers), [])!;
        
        yield return AIFunctionFactory.Create(
            previousAnswersFn,
            gameService,
            new AIFunctionFactoryOptions
            {
                Name = "get_frequent_answers",
                Description = "Returns a list of all the character names that have been used more than once per 1000 times in previous games. These will not be permitted for use in new games.",
            });
        
        var startGameFn = typeof(GameService)
            .GetMethod(nameof(GameService.StartGame),
                [])!;
        yield return AIFunctionFactory.Create(
            startGameFn,
            gameService,
            new AIFunctionFactoryOptions
            {
                Name = "start_game",
                Description = "Starts a new game of Twenty Questions for the specified user name, where the user has chosen the answer and the AI needs to guess it. " +
                              "If the user already has a game in progress, no new game will be started and an error will be returned. You can check if a user has a game in progress by calling the get_current_game function. " +
                              "You can abandon a current game by calling the abandon_current_game function and then call this function again to start a new game.",
            });
        
        var getGameStateFn = typeof(GameService)
            .GetMethod(nameof(GameService.GetGameState),
                [typeof(Guid)])!;

        yield return AIFunctionFactory.Create(
            getGameStateFn,
            gameService,
            new AIFunctionFactoryOptions
            {
                Name = "get_game_state",
                Description = "Gets the state of the game with the specified game id",
            });

        var isGameOverFn = typeof(GameService)
            .GetMethod(nameof(GameService.IsGameOver),
                [typeof(Guid)])!;

        yield return AIFunctionFactory.Create(
            isGameOverFn,
            gameService,
            new AIFunctionFactoryOptions
            {
                Name = "is_game_over",
                Description = "Checks if the game with the specified game id is over",
            });

        var getCurentGameFn = typeof(GameService)
            .GetMethod(nameof(GameService.GetCurrentGame),
                [])!;

        yield return AIFunctionFactory.Create(
            getCurentGameFn,
            gameService,
            new AIFunctionFactoryOptions
            {
                Name = "get_current_game",
                Description = "Gets the state of the current active game for the specified user name, or null if there is no active game. If there is not current game, you can start a new game by calling the start_game function",
            });

        var abandonSpecificGame = typeof(GameService)
            .GetMethod(nameof(GameService.AbandonGame),
                [typeof(Guid), typeof(string)])!;

        yield return AIFunctionFactory.Create(
            abandonSpecificGame,
            gameService,
            new AIFunctionFactoryOptions
            {
                Name = "abandon_game_id",
                Description = "Finds the game with the specified game id and abandons it, allowing the user to start a new game. If the game is already over, an error will be returned. Confirms the character name the user or ai was thinking of.",
            });

        var abandonGameFn = typeof(GameService)
            .GetMethod(nameof(GameService.AbandonGame),
                [typeof(string)])!;

        yield return AIFunctionFactory.Create(
            abandonGameFn,
            gameService,
            new AIFunctionFactoryOptions
            {
                Name = "abandon_game",
                Description = "Checks if the user had a game in progress and abandons it if they do, allowing the user to start a new game. Returns true if a game was found and abandoned or false if no active game was found. Confirms the character name the user or ai was thinking of.",
            });

        var registerGameQuestionFn = typeof(GameService)
            .GetMethod(nameof(GameService.RegisterQuestion),
                [typeof(Guid), typeof(string), typeof(Answer)])!;

        yield return AIFunctionFactory.Create(
            registerGameQuestionFn,
            gameService,
            new AIFunctionFactoryOptions
            {
                Name = "register_question_for_game",
                Description = "Registers the specified question and answer for the game with the specified game id and updates the game state accordingly. If the game is over, an error will be returned.",
            });
        

        var registerQuestionFn = typeof(GameService)
            .GetMethod(nameof(GameService.RegisterQuestion),
                [typeof(string), typeof(Answer)])!;

        yield return AIFunctionFactory.Create(
            registerQuestionFn,
            gameService,
            new AIFunctionFactoryOptions
            {
                Name = "register_question",
                Description = "Registers the specified question and answer for the current game and updates the game state accordingly. If the user does not have an active game, an error will be returned.",
            });
        
        
        var winGameFn = typeof(GameService)
            .GetMethod(nameof(GameService.WinGame),
                [typeof(string)])!;

        yield return AIFunctionFactory.Create(
            winGameFn,
            gameService,
            new AIFunctionFactoryOptions
            {
                Name = "win_game",
                Description = "Updates the game state with the fact the game has been won. Confirms the correct answer.",
            });
    }
}