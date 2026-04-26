namespace Vector
{
    class MyVector
    {
        private readonly double DirectionX, DirectionY;

        public MyVector(double x, double y)
        {
            DirectionX = x;
            DirectionY = y;
        }

        public override string ToString()
        {
            return $"({DirectionX}, {DirectionY})";
        }

        public static MyVector operator +(MyVector first, MyVector other)
        {
            return new MyVector(first.DirectionX + other.DirectionX, first.DirectionY + other.DirectionY);
        }

        public static MyVector operator -(MyVector first, MyVector other)
        {
            return new MyVector(first.DirectionX - other.DirectionX, first.DirectionY - other.DirectionY);
        }

        public static double operator *(MyVector first, MyVector other)
        {
            return first.DirectionX * other.DirectionX + first.DirectionY * other.DirectionY;
        }

        public static double operator !(MyVector vector)
        {
            return Math.Sqrt(vector.DirectionX * vector.DirectionX + vector.DirectionY * vector.DirectionY);
        }

        public static bool operator ==(MyVector first, MyVector other)
        {
            return first.DirectionX == other.DirectionX && first.DirectionY == other.DirectionY;
        }

        public static bool operator !=(MyVector first, MyVector other)
        {
            return first.DirectionX != other.DirectionX || first.DirectionY != other.DirectionY;
        }

    }
}

