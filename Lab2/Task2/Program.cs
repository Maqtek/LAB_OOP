using System.Diagnostics;

namespace Task2
{
    internal class Program
    {
        private static readonly List<ServerEndpoint> endpoints = new List<ServerEndpoint>
        {
            new ServerEndpoint("JSONPlaceholder", "https://jsonplaceholder.typicode.com/posts/1"),
            new ServerEndpoint("Dog API", "https://dog.ceo/api/breeds/image/random"),
            new ServerEndpoint("CatFact", "https://catfact.ninja/fact")
        };

        private static void Main(string[] args)
        {
            Console.WriteLine("Выберите версию выполнения запросов:");
            Console.WriteLine("1 - Синхронная");
            Console.WriteLine("2 - Асинхронная");
            Console.Write("Ваш выбор: ");

            string? choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        RunSynchronousVersion();
                        break;

                    case "2":
                        RunAsynchronousVersion().GetAwaiter().GetResult();
                        break;

                    default:
                        Console.WriteLine("Ошибка: необходимо выбрать 1 или 2.");
                        break;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Ошибка выполнения запросов: {exception.Message}");
            }
        }

        private static void RunSynchronousVersion()
        {
            Console.WriteLine();
            Console.WriteLine("Синхронная версия.");

            Stopwatch stopwatch = Stopwatch.StartNew();
            List<ServerResponse> responses = new List<ServerResponse>();

            using (HttpClient client = new HttpClient())
            {
                for (int i = 0; i < endpoints.Count; i++)
                {
                    responses.Add(SendSynchronousRequest(client, endpoints[i]));
                }
            }

            stopwatch.Stop();

            PrintResponses(responses);
            Console.WriteLine($"Общее время работы: {stopwatch.ElapsedMilliseconds} мс");
        }

        private static ServerResponse SendSynchronousRequest(HttpClient client, ServerEndpoint endpoint)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, endpoint.Url))
            using (HttpResponseMessage response = client.Send(request))
            {
                EnsureSuccessResponse(response, endpoint);

                using (Stream stream = response.Content.ReadAsStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    string json = reader.ReadToEnd();
                    return new ServerResponse(endpoint.Name, endpoint.Url, json);
                }
            }
        }

        private static async Task RunAsynchronousVersion()
        {
            Console.WriteLine();
            Console.WriteLine("Асинхронная версия.");

            Stopwatch stopwatch = Stopwatch.StartNew();
            List<Task<ServerResponse>> tasks = new List<Task<ServerResponse>>();

            using (HttpClient client = new HttpClient())
            {
                for (int i = 0; i < endpoints.Count; i++)
                {
                    tasks.Add(SendAsynchronousRequest(client, endpoints[i]));
                }

                ServerResponse[] responses = await Task.WhenAll(tasks);

                stopwatch.Stop();

                PrintResponses(new List<ServerResponse>(responses));
                Console.WriteLine($"Общее время работы: {stopwatch.ElapsedMilliseconds} мс");
            }
        }

        private static async Task<ServerResponse> SendAsynchronousRequest(
            HttpClient client,
            ServerEndpoint endpoint)
        {
            using (HttpResponseMessage response = await client.GetAsync(endpoint.Url))
            {
                EnsureSuccessResponse(response, endpoint);

                string json = await response.Content.ReadAsStringAsync();
                return new ServerResponse(endpoint.Name, endpoint.Url, json);
            }
        }

        private static void EnsureSuccessResponse(HttpResponseMessage response, ServerEndpoint endpoint)
        {
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    $"Сервер {endpoint.Name} вернул ошибку {(int)response.StatusCode} ({response.ReasonPhrase}).");
            }
        }

        private static void PrintResponses(List<ServerResponse> responses)
        {
            Console.WriteLine();

            for (int i = 0; i < responses.Count; i++)
            {
                ServerResponse response = responses[i];

                Console.WriteLine($"Сервер: {response.ServerName}");
                Console.WriteLine($"URL: {response.Url}");
                Console.WriteLine(response.Json);
                Console.WriteLine();
            }
        }
    }
}
