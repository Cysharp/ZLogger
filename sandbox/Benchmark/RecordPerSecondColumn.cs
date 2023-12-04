using System.Reflection;
using Perfolizer.Horology;

namespace Benchmark;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

public class LogWritesPerSecondAttribute : ColumnConfigBaseAttribute
{
    public LogWritesPerSecondAttribute() : base(new LogWritesPerSecond())
    {
    }
}

public class LogWritesPerSecond : IColumn
{
    public string Id => nameof(LogWritesPerSecond);

    public string ColumnName => "recs/sec";

    public bool AlwaysShow => true;

    public ColumnCategory Category => ColumnCategory.Custom;

    public int PriorityInCategory => 0;

    public bool IsNumeric => true;

    public UnitType UnitType => UnitType.Size;

    public string Legend => "log records written per second";

    public string GetValue(Summary summary, BenchmarkCase benchmarkCase)
    {
        var nField = benchmarkCase.Descriptor.Type.GetField("N", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        if (nField != null)
        {
            var n = (int)nField.GetValue(null)!;
            var statistics = summary[benchmarkCase].ResultStatistics;
            var mean = TimeInterval.FromNanoseconds(statistics.Mean);
            return (1.0 / (mean.ToSeconds() / n)).ToString("F3");
        }
        return "-";
    }

    public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style)
    {
        return GetValue(summary, benchmarkCase);
    }

    public bool IsAvailable(Summary summary)
    {
        return true;
    }

    public bool IsDefault(Summary summary, BenchmarkCase benchmarkCase)
    {
        return false;
    }
}