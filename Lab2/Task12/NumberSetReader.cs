namespace Task12
{
    internal static class NumberSetReader
    {
        private const int SetsCount = 15;
        private const int NumbersInSet = 100;

        public static List<int[]> Read(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);

            if (lines.Length != SetsCount)
            {
                throw new InvalidOperationException("Файл должен содержать 15 наборов чисел.");
            }

            List<int[]> sets = new List<int[]>();

            for (int i = 0; i < lines.Length; i++)
            {
                string[] parts = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != NumbersInSet)
                {
                    throw new InvalidOperationException($"Набор {i + 1} должен содержать 100 чисел.");
                }

                int[] numbers = new int[NumbersInSet];

                for (int j = 0; j < parts.Length; j++)
                {
                    int number = int.Parse(parts[j]);

                    if (number < 1 || number > 100)
                    {
                        throw new InvalidOperationException("Числа должны находиться в диапазоне от 1 до 100.");
                    }

                    numbers[j] = number;
                }

                sets.Add(numbers);
            }

            return sets;
        }
    }
}
