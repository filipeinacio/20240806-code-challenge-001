using System.Diagnostics;
using FileSorter.Console.Services;

namespace FileSorter.Console;

class Program
{
    static async Task<int> Main(string[] args)
    {
        if (args.Length != 2)
        {
            System.Console.WriteLine("Usage: ExternalMergeSort <input file path> <output file path>");
            return 0;
        }

        var inputFilePath = args[0];
        var outputFilePath = args[1];
        
        
        var stopWatch = new Stopwatch();
        stopWatch.Start();

        var sorter = new FileSorterService();
        await sorter.SortTo(inputFilePath, outputFilePath);

        stopWatch.Stop();
        // Get the elapsed time as a TimeSpan value.
        var ts = stopWatch.Elapsed;

        // Format and display the TimeSpan value.
        var elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
        System.Console.WriteLine($"RunTime {elapsedTime}");

        return 0;
    }
}