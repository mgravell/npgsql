using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Npgsql.Benchmarks;

[Config(typeof(Config))]
public class CommandCreationBenchmarks
{
    [Params(0,1,5,10,20)]
    public int ParameterCount { get; set; }

    [Benchmark]
    public void CreateCommand()
    {
        using var cmd = new NpgsqlCommand("""select * from orders""");
        var paramCount = ParameterCount;
        for (var i = 0; i < paramCount; i++)
        {
            cmd.Parameters.Add(new NpgsqlParameter<int> { TypedValue = i });
        }
        if (cmd.Parameters.Count != paramCount)
        {
            Throw();
        }

        [MethodImpl(MethodImplOptions.NoInlining), DoesNotReturn]
        static void Throw() => throw new InvalidOperationException("Incorrect parameter count detected");
    }

    class Config : ManualConfig
    {
        public Config()
        {
            AddColumn(StatisticColumn.OperationsPerSecond);
            AddDiagnoser(MemoryDiagnoser.Default);
        }
    }
}
