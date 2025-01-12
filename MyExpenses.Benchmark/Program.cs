using BenchmarkDotNet.Running;

namespace MyExpenses.Benchmark;

public class Program
{
    public static void Main(string[] args)
    {
        // Executes all benchmarks defined in the assembly (all classes marked with [Benchmarks])
        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }
}