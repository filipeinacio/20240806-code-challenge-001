using System.Diagnostics;
using FileSorter.Console.Services;

class Program
{
    static async Task<int> Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Usage: ExternalMergeSort <input file path> <output file path>");
            return 0;
        }

        string inputFilePath = args[0];
        string outputFilePath = args[1];
        
        
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        var sorter = new FileSorterService();
        await sorter.SortTo(inputFilePath, outputFilePath);

        stopWatch.Stop();
        // Get the elapsed time as a TimeSpan value.
        TimeSpan ts = stopWatch.Elapsed;

        // Format and display the TimeSpan value.
        string elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
        Console.WriteLine("RunTime " + elapsedTime);

        return 0;
    }
}
