namespace Task11
{
    public class MonitorCounter : ICounter
    {
        private readonly object locker = new object();
        private int value;

        public int Value
        {
            get
            {
                lock (locker)
                {
                    return value;
                }
            }
        }

        public void Increment()
        {
            lock (locker)
            {
                value++;
            }
        }
    }
}
