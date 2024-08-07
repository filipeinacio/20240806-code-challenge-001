using System.Diagnostics;
using FileGenerator.Console.Services;


class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Usage: FileGenerator <file size in bytes> <output file path>");
            return;
        }

        if (!long.TryParse(args[0], out var fileSize) || fileSize <= 0)
        {
            Console.WriteLine("Please provide a valid positive number for file size.");
            return;
        }

        var filePath = args[1];

        var stopWatch = new Stopwatch();
        stopWatch.Start();

        var generator = new GeneratorService();
        generator.GenerateFile(filePath, fileSize);

        stopWatch.Stop();
        // Get the elapsed time as a TimeSpan value.
        var ts = stopWatch.Elapsed;

        // Format and display the TimeSpan value.
        var elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
        Console.WriteLine("RunTime " + elapsedTime);
    }
}









