﻿public class ClassToTest
{
    public static int FibStatic1(int i, int acc)
    {
        if (i == 0)
        {
            return acc;
        }

        return FibStatic1(i - 1, acc + i);
    }

    public static int FibStatic2(int i, int acc)
    {
        var w = 100;
        var x = 200;
        var y = 300;
        var z = 400;

        if (i == 0)
        {
            return acc;
        }

        return FibStatic2(i - 1, acc + i);
    }

    public int Fib1(int i, int acc)
    {
        if (i == 0)
        {
            return acc;
        }

        return Fib1(i - 1, acc + i);
    }

    public int Fib2(int i, int acc)
    {
        var w = 100;
        var x = 200;
        var y = 300;
        var z = 400;

        if (i == 0)
        {
            return acc;
        }

        return Fib2(i - 1, acc + i);
    }

    public virtual int FibVirtual1(int i, int acc)
    {
        if (i == 0)
        {
            return acc;
        }

        return FibVirtual1(i - 1, acc + i);
    }

    public virtual int FibVirtual2(int i, int acc)
    {
        var w = 100;
        var x = 200;
        var y = 300;
        var z = 400;

        if (i == 0)
        {
            return acc;
        }

        return FibVirtual2(i - 1, acc + i);
    }
}