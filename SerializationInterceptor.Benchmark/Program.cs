using System;

namespace SerializationInterceptor.Benchmark
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WarmUp();
            NewtonsoftJsonBenchmark.DoBenchmark();
            SystemTextJsonBenchmark.DoBenchmark();
            Console.ReadKey();
        }

        private static void WarmUp()
        {
            for (int i = 0; i < 100000; i++) new Random().Next();
        }
    }
}
