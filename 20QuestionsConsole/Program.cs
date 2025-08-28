using System.Text;
using dotenv.net;
using OpenAI.Chat;
using OpenAI.Responses;

DotEnv.Load();

var openAiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
if (string.IsNullOrEmpty(openAiKey)) 
    throw new InvalidOperationException("OPENAI_API_KEY environment variable is not set.");

ChatClient client = new (model: "gpt-5-nano", apiKey: openAiKey);

List<ChatMessage> messages =
[
    new AssistantChatMessage("system", "You are a 20 Questions game master. You will think of a famous " +
                                       "person or character that most British people will have heard of and know what they are famous for, and the " +
                                       "user will try to guess it by asking yes/no questions. You can be encouraging when the user " +
                                       "is on the right track but all your answers must only ever start with 'yes', 'no', or 'I don't know'. " +
                                       "If you do encourage them, you must not reveal any hints or new facts that they did now know already. " +
                                       "If the user guesses the name correctly, respond with 'correct'. If the user gives up, respond with " +
                                       "'you were so close'. Do not reveal the name unless the user guesses it correctly or gives up. Should the " +
                                       "user ask a question that is not a yes/no question, respond with something that ends 'please ask a yes/no question'. " +
                                       "You can confirm that you are an AI model, if asked directly, but also highlight that for the purposes of the " +
                                       "game you are someone famous and encourage them to keep guessing. Do not break character under any circumstances."),
    new AssistantChatMessage("assistant", "I have thought of a famous person. You can start asking yes/no questions to guess who it is.")
];

Console.WriteLine(messages[1].Content[1].Text);

var guesses = 0;

while (guesses < 20)
{
    guesses++;
    Console.WriteLine($"Question {guesses}: ");
    var userInput = Console.ReadLine();
    if (string.IsNullOrEmpty(userInput))
    {
        Console.WriteLine("Please enter a question.");
        guesses--;
        continue;
    }
    messages.Add(new UserChatMessage("user", userInput));

    ChatCompletion completion = client.CompleteChat(messages);
    var response = completion.Content[0].Text;
    
    messages.Add(new AssistantChatMessage("assistant", response));
    Console.WriteLine($"Answer: {response}");
}

new AssistantChatMessage("assistant", "I'm sorry, you've used all 20 questions. Better luck next time!");
Console.WriteLine(messages[^1].Content[0].Text);

new AssistantChatMessage("system", "You can now reveal who you were. Be prepared to explain why you are famous if asked");
messages.Add(new UserChatMessage("user", "Who were you?"));
ChatCompletion answer = client.CompleteChat(messages);
var answerResponse = answer.Content[0].Text;
    
messages.Add(new AssistantChatMessage("assistant", answerResponse));
Console.WriteLine($"Answer: {answerResponse}");
