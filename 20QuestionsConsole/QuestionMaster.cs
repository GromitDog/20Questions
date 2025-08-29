using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

namespace _20QuestionsConsole;

public class QuestionMaster
{
    public static async Task<string> Play20Questions(IServiceProvider serviceProvider, List<string> forbiddenAnswers)
    {
        Console.WriteLine("Welcome to 20 Questions. I will think of a famous person or character, and you can guess who it is by asking yes/no questions. " +
                          "You have 20 questions to figure it out. Hang on while I think of someone you'll never guess!");  
        var client = serviceProvider.GetRequiredService<IChatClient>();

        var chatOptions = serviceProvider.GetRequiredService<ChatOptions>();
        
        int turnsSinceLastSummary = 0;
        const int SUMMARY_INTERVAL = 5;
        
        var history = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, $"Think of a famous person or character that most British people will have heard of and know what they are famous for. " +
                                             $"Do not pick anyone who has recently been publicly disgraced {string.Join(" or ", forbiddenAnswers)}." ),
            new (ChatRole.User, "Who are you? Respond with just the name of the person or character you have thought of.")
        };
       
        var answer = await client.GetResponseAsync(history, chatOptions);
        history.Add(new ChatMessage(ChatRole.Assistant, answer.Text));
        history.Clear();
        history.Add(new ChatMessage(ChatRole.System,
            $"You are a 20 Questions game master and in this game you are {answer.Text}.  The " +
            "user will try to guess who you are by asking yes/no questions. You can be encouraging when the user " +
            "is on the right track but all your answers must only ever start with 'yes', 'no', or 'I don't know'. " +
            "If you do encourage them, you must not reveal any hints or new facts that they did now know already. " +
            "If the user guesses the name correctly, respond with 'correct'. If the user gives up, respond with " +
            "an appropriate summary of how well they were doing and who you were. Do not reveal the name unless the user guesses it correctly or gives up. Should the " +
            "user ask a question that is not a yes/no question, respond with something that ends 'Please ask a yes/no question.'. " +
            "You can confirm that you are an AI model, if asked directly, but also highlight that for the purposes of the " +
            "game you are someone famous and encourage them to keep guessing. Do not break character under any circumstances and do not share who you are with the user."));
        
        Console.WriteLine("Ok, I have thought of a famous person. Start asking yes/no questions to guess who it is.");
        var guesses = 1;

        while (guesses <= 20)
        {
            Console.WriteLine($"Question {guesses}: ");
            var userInput = Console.ReadLine();
            if (string.IsNullOrEmpty(userInput))
            {
                Console.WriteLine("Please enter a question.");
                continue;
            }

            // validate the question is a yes/no question
            var questionCheck = new List<ChatMessage>
            {
                new(ChatRole.User, $"can answer yes or no to this question? only respond with 'yes' or 'no' : {userInput}")
            };
            var questionCheckResponse = await client.GetResponseAsync(questionCheck, chatOptions);
    
            if (questionCheckResponse.Text.Trim().ToLower() != "yes")
            {
                Console.WriteLine("Please ask a yes/no question.");
                continue;
            }
            history.Add(new ChatMessage(ChatRole.User, userInput));
            var response = await client.GetResponseAsync(history, chatOptions);
            
            Console.WriteLine(response.Text);
            history.AddRange(response.Messages);
    
            if (response.Text.Trim().ToLower() == "correct")
            {
                Console.WriteLine($"Well done, you guessed correctly in {guesses} guesses!");
                return answer.Text;
            }
            
            guesses++;
        }
        
        Console.WriteLine($"I'm sorry, you've used all 20 questions, I was {answer}. Better luck next time!");
        return answer.Text;
        
        
    }
}