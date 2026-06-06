using System.Diagnostics;
using Task11.Counter;
using Task11.Monitor;
using Task11.Mutex;
using Task11.Semaphore;

namespace Task11
{
    internal class Program
    {
        const int START = 1;
        const int END = 10000;

        static void CountPrimeNumbers(int start, int end, ICounter counter)
        {
            for (int i = start; i <= end; i++)
            {
                bool isPrime = true;

                if (i < 2)
                    continue;

                for (int j = 2; j <= Math.Sqrt(i); j++)
                {
                    if (i % j == 0)
                    {
                        isPrime = false;
                        break;
                    }
                }

                Console.WriteLine($"[Поток {Thread.CurrentThread.ManagedThreadId}] проверка числа {i}");

                if (isPrime)
                {
                    counter.Increment();

                    Console.WriteLine($"[Поток {Thread.CurrentThread.ManagedThreadId}] найдено простое число: {i}");
                }
            }
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Выберите версию синхронизации:");
            Console.WriteLine("1 - Monitor");
            Console.WriteLine("2 - Mutex");
            Console.WriteLine("3 - Semaphore");
            Console.Write("Ваш выбор: ");

            string? choice = Console.ReadLine();
            ICounter counter;

            switch (choice)
            {
                case "1":
                    counter = new MonitorCounter();
                    Console.WriteLine("Выбрана версия Monitor.");
                    break;

                case "2":
                    counter = new MutexCounter();
                    Console.WriteLine("Выбрана версия Mutex.");
                    break;

                case "3":
                    counter = new SemaphoreCounter();
                    Console.WriteLine("Выбрана версия Semaphore.");
                    break;

                default:
                    Console.WriteLine("Ошибка: необходимо выбрать 1, 2 или 3.");
                    return;
            }

            Console.WriteLine();

            Stopwatch stopwatch = Stopwatch.StartNew();

            Thread thread1 = new Thread(() => CountPrimeNumbers(START, 2500, counter));
            Thread thread2 = new Thread(() => CountPrimeNumbers(2501, 5000, counter));
            Thread thread3 = new Thread(() => CountPrimeNumbers(5001, 7500, counter));
            Thread thread4 = new Thread(() => CountPrimeNumbers(7501, END, counter));

            thread1.Start();
            thread2.Start();
            thread3.Start();
            thread4.Start();

            thread1.Join();
            thread2.Join();
            thread3.Join();
            thread4.Join();

            stopwatch.Stop();

            Console.WriteLine();
            Console.WriteLine($"Всего найдено простых чисел: {counter.Value}");
            Console.WriteLine($"Время выполнения: {stopwatch.ElapsedMilliseconds} мс");
        }
    }
}
