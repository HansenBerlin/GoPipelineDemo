using System;

namespace GoPipelineDemo
{
    public class Program
    {
        static void Main(string[] args)
        {
            var calc = new Calculator();
            Console.WriteLine(calc.Add(2, 2));
            Console.WriteLine(calc.CheckPrime(3));
            Console.WriteLine(calc.CheckPrime(13));
            Console.WriteLine(calc.CheckPrime(305175781));
            Console.WriteLine(calc.CheckPrime(1));
            Console.WriteLine(calc.CheckPrime(12));
            Console.WriteLine(calc.CheckPrime(10000000));
            
        }
    }
}

