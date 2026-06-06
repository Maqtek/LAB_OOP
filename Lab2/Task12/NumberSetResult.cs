namespace Task12
{
    internal class NumberSetResult
    {
        public NumberSetResult(int setNumber, int sum, int threadId)
        {
            SetNumber = setNumber;
            Sum = sum;
            ThreadId = threadId;
        }

        public int SetNumber { get; }
        public int Sum { get; }
        public int ThreadId { get; }
    }
}
