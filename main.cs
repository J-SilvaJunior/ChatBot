using System;
using OpenAI_API;
using System.Threading.Tasks;
using System.IO;

class Program
{
    public static async Task Main(string[] args)
    {

        string apiKey = new Func<string>(() =>
        {
            using (StreamReader reader = new("apiKey.txt"))
            {
                try
                {
                    return reader.ReadLine();
                }
                catch (Exception)
                {
                    throw new Exception(
                        "A chave da API não está presente!\n" +
                        "Coloque a sua chave da API na pasta do executável."
                    );
                }
            }
        })();

        OpenAIAPI openAi = new(apiKey);
        Console.WriteLine(
          "ChatGPT:\n" +
          "Olá, como posso ajudar?\n" +
          "(CTRL+C para sair do chat)"
        );
        string arquivoAtual = $"{DateTime.Now.ToString("dd-MM-yyyy HH-mm-ss")}.txt";


        openAi.Chat.CreateConversation(
          new OpenAI_API.Chat.ChatRequest()
          {
              Model = OpenAI_API.Models.Model.GPT4,
              Temperature = 0.1
          }
        );

        if (!Directory.Exists($"chatlog"))
            Directory.CreateDirectory($"chatlog");
        while (true)
        {
            StreamWriter wtr = new(@$"chatlog\{arquivoAtual}", true, System.Text.Encoding.UTF8);
            string prompt = "";

            while (string.IsNullOrEmpty(prompt))
            {
                Console.WriteLine("Você:");
                prompt = Console.ReadLine();

                await wtr.WriteLineAsync("Você:");
                await wtr.WriteLineAsync(prompt);

                Console.WriteLine("\nGerando resposta...\n");
            }
            var result = await openAi.Chat.CreateChatCompletionAsync(prompt);

            Console.Write("IA:");
            Console.WriteLine($"{result.Choices[0].Message.Content}\n");

            await wtr.WriteLineAsync("\nIA:");
            await wtr.WriteLineAsync(result.Choices[0].Message.Content + '\n');

            wtr.Close();
        }
    }
}