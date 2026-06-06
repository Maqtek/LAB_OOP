using ThreadSemaphore = System.Threading.Semaphore;
using Task11.Counter;

namespace Task11.Semaphore
{
    public class SemaphoreCounter : ICounter
    {
        private readonly ThreadSemaphore semaphore = new ThreadSemaphore(1, 1);
        private int value;

        public int Value
        {
            get
            {
                semaphore.WaitOne();

                try
                {
                    return value;
                }
                finally
                {
                    semaphore.Release();
                }
            }
        }

        public void Increment()
        {
            semaphore.WaitOne();

            try
            {
                value++;
            }
            finally
            {
                semaphore.Release();
            }
        }
    }

}
