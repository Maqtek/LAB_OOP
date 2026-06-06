using System.Diagnostics;

namespace Task12
{
    internal class Program
    {
        private const int MaxParallelThreads = 4;

        private static readonly Semaphore semaphore = new Semaphore(MaxParallelThreads, MaxParallelThreads);
        private static readonly Mutex totalMutex = new Mutex();
        private static readonly object resultsLocker = new object();

        private static readonly List<NumberSetResult> results = new List<NumberSetResult>();
        private static int totalSum;

        private static void ProcessSet(int setNumber, int[] numbers)
        {
            semaphore.WaitOne();

            try
            {
                int sum = CalculateSum(numbers);
                int threadId = Thread.CurrentThread.ManagedThreadId;

                lock (resultsLocker)
                {
                    results.Add(new NumberSetResult(setNumber, sum, threadId));
                }

                totalMutex.WaitOne();

                try
                {
                    totalSum += sum;
                }
                finally
                {
                    totalMutex.ReleaseMutex();
                }
            }
            finally
            {
                semaphore.Release();
            }
        }

        private static int CalculateSum(int[] numbers)
        {
            int sum = 0;

            for (int i = 0; i < numbers.Length; i++)
            {
                sum += numbers[i];
            }

            return sum;
        }

        private static void Main(string[] args)
        {
            try
            {
                string filePath = Path.Combine(AppContext.BaseDirectory, "Data", "number_sets.txt");
                List<int[]> sets = NumberSetReader.Read(filePath);
                List<Thread> threads = new List<Thread>();

                Stopwatch stopwatch = Stopwatch.StartNew();

                for (int i = 0; i < sets.Count; i++)
                {
                    int setNumber = i + 1;
                    int[] numbers = sets[i];

                    Thread thread = new Thread(() => ProcessSet(setNumber, numbers));
                    threads.Add(thread);
                    thread.Start();
                }

                for (int i = 0; i < threads.Count; i++)
                {
                    threads[i].Join();
                }

                stopwatch.Stop();
                results.Sort((first, second) => first.SetNumber.CompareTo(second.SetNumber));

                Console.WriteLine("Результаты подсчёта:");

                for (int i = 0; i < results.Count; i++)
                {
                    NumberSetResult result = results[i];
                    Console.WriteLine(
                        $"Набор {result.SetNumber}: сумма {result.Sum}, поток {result.ThreadId}");
                }

                Console.WriteLine();
                Console.WriteLine($"Общий итог по всем наборам: {totalSum}");
                Console.WriteLine($"Время выполнения: {stopwatch.ElapsedMilliseconds} мс");
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Ошибка: {exception.Message}");
            }
            finally
            {
                semaphore.Dispose();
                totalMutex.Dispose();
            }
        }
    }
}
