namespace IndependentWork12;

public static class ComputeOperations
{
    public static bool IsPrime(int n)
    {
        if (n < 2)
        {
            return false;
        }

        if (n % 2 == 0)
        {
            return n == 2;
        }

        int limit = (int)Math.Sqrt(n);
        for (int i = 3; i <= limit; i += 2)
        {
            if (n % i == 0)
            {
                return false;
            }
        }

        return true;
    }

    public static List<int> GenerateNumbers(int count, int maxValue, int seed = 42)
    {
        var rnd = new Random(seed);
        var numbers = new List<int>(count);

        for (int i = 0; i < count; i++)
        {
            numbers.Add(rnd.Next(1, maxValue));
        }

        return numbers;
    }
}