namespace Task2
{
    internal class ServerEndpoint
    {
        public ServerEndpoint(string name, string url)
        {
            Name = name;
            Url = url;
        }

        public string Name { get; }
        public string Url { get; }
    }
}
