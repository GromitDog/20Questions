using Anthropic.SDK;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

namespace TwentyQuestionsConsole;

public class QuestionMaster
{
    public static async Task Play20Questions(IServiceProvider serviceProvider)
    {
        Console.WriteLine("Welcome to 20 Questions. I will think of a famous person or character, and you can guess who it is by asking yes/no questions. " +
                          "You have 20 questions to figure it out.");  
        
        var client = serviceProvider.GetRequiredService<IChatClient>();

        var chatOptions = serviceProvider.GetRequiredService<ChatOptions>();
        
        var history = new List<ChatMessage>
        { new ChatMessage(ChatRole.System,
            $"You are a 20 Questions game master and you can play the game with the user. You think of a famous " +
            $"person or character and, the user will then ask you yes/no questions to try to figure out who you are. " +
            $"The user is likely to be British so factor that in when choosing who to be. " +
            
            "When the user asks a valid question within the game, be sure to register it by calling register_question_for_game " +
            "so that the game state is kept up to date. If the user guesses correctly, you must register the win using win_game and confirming the correct answer" +
            "You do not need to register the winning question by calling register_question as well. You can add encouragements when the user " +
            "is on the right track (or not!) but all your answers must include 'yes', 'no', or 'I don't know'. If you do encourage them, " +
            "you must not reveal any hints or new facts that they did now know already. " +
            
            $"You can start a new game by calling start_game if you are the one thinking of the character, make sure that you do not " +
            $"choose a name in the list returned by calling get_frequent_answers. These names have been used in recent games and would make " +
            $"it less fun if you repeat them. You need to check this list every time before you choose a name for the user to guess. " +
            $"If a user asks a question but does not already have a game running, think of someone and start a game for them (don't forget to check the list). " +
            $"Be nice, if the same question is asked twice, don't register it, remind them the answer and do not count it as a question." +

            "If the user gives up, call abandon_game to update the game state and then provide the user with an appropriate summary " +
            "of how well they were doing and who you were. Do not reveal the name unless the user guesses it correctly or gives up. Should the " +
            "user ask a question that is not a yes/no question, do not reveal new information about the character and guide them to ask a question " +
            "that fits with the rules of the game. Do not use the tools to register invalid questions as they should not be counted in the game. " +
            "You can confirm that you are an AI model, if asked directly, but also highlight that for the purposes of the " +
            "game you are someone famous and encourage them to keep guessing. Do not break character under any circumstances " +
            $"and do not share who you are with the user until the game is over. If the user asks a question about 'you' assume " +
            $"they mean the character you thought of and not you the AI. Never reveal additional information about the character unless the game is over. " +
            $"After the first game, you can switch roles and let the user think of a character and you ask the questions if they want to. " +
            $"When you are the one asking the questions you do not need to check the list but you must still start the game by calling start_game. Do not " +
            $"ask the user to tell you who they are or you forfeit the game."),
            
            new (ChatRole.User,$"Let's play 20 Questions." ),
        };

        while (true)
        {
            var userInput = Console.ReadLine();
            if (string.IsNullOrEmpty(userInput))
            {
                Console.WriteLine("Please enter a question.");
                continue;
            }
            if (userInput.ToLower() == "exit" || userInput.ToLower() == "quit")
            {
                Console.WriteLine("Thanks for playing!");
                break;
            }

            history.Add(new ChatMessage(ChatRole.User, userInput));
            ChatResponse response;
            try
            {
                response = await client.GetResponseAsync(history, chatOptions);
            }
            catch (RateLimitsExceeded ex)
            {
                Console.WriteLine("Clause Rate limit exceeded, waiting a moment before trying again. " + ex.Message);
                await Task.Delay(2000);
                response = await client.GetResponseAsync(history, chatOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Sorry, something went wrong: " + ex.Message);
                continue;
            }

            Console.WriteLine(response.Text);
            history.AddRange(response.Messages);
        }
    }
}