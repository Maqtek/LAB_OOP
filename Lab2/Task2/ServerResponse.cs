namespace Task2
{
    internal class ServerResponse
    {
        public ServerResponse(string serverName, string url, string json)
        {
            ServerName = serverName;
            Url = url;
            Json = json;
        }

        public string ServerName { get; }
        public string Url { get; }
        public string Json { get; }
    }
}
