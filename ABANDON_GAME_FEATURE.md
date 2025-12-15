# Abandon Game Feature

## Overview
The 20 Questions game now includes abandon/give up functionality that allows players to end a game early.

## How to Use

### Console Application
When playing the game in the console, you can abandon the game at any time by typing phrases like:
- "I give up"
- "give up"
- "abandon game"
- "I quit" (this also exits the program)

The AI game master will:
1. Call the `abandon_game` function to update the game state
2. Reveal who they were thinking of
3. Provide a summary of how well you were doing

### Technical Details
The abandon functionality is implemented through:
- **GameService.AbandonGame(string character)**: Abandons the current active game
- **GameService.AbandonGame(Guid gameId, string character)**: Abandons a specific game by ID
- **GameState.GiveUp(string characterName)**: Marks a game as over and records the character name

The AI is instructed to recognize abandonment requests and handle them gracefully by calling the appropriate service method.

## Game State Updates
When a game is abandoned:
- The game's `IsOver` property is set to `true`
- The `CharacterName` property is set to the character that was being thought of
- The `IsWon` property remains `false` (game was not won)
- The game is no longer returned by `GetCurrentGame()`

## Examples

```csharp
// Start a game
var game = GameService.StartGame();

// User gives up
GameService.AbandonGame("Harry Potter");

// Game is now over
Console.WriteLine(game.IsOver); // True
Console.WriteLine(game.CharacterName); // "Harry Potter"
Console.WriteLine(game.IsWon); // False
```

## Future Enhancements
- Multiplayer support with notifications to both players when a game is abandoned
- Web UI using Radzen Blazor components
- Game abandonment statistics and tracking
