using ThreadMutex = System.Threading.Mutex;

namespace Task11
{
    public class MutexCounter : ICounter
    {
        private readonly ThreadMutex mutex = new ThreadMutex();
        private int value;

        public int Value
        {
            get
            {
                mutex.WaitOne();

                try
                {
                    return value;
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
        }

        public void Increment()
        {
            mutex.WaitOne();

            try
            {
                value++;
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }
    }
}
