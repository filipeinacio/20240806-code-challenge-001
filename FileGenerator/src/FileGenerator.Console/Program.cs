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

        if (!long.TryParse(args[0], out long fileSize) || fileSize <= 0)
        {
            Console.WriteLine("Please provide a valid positive number for file size.");
            return;
        }

        string filePath = args[1];

        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        var generator = new GeneratorService();
        generator.GenerateFile(filePath, fileSize);

        stopWatch.Stop();
        // Get the elapsed time as a TimeSpan value.
        TimeSpan ts = stopWatch.Elapsed;

        // Format and display the TimeSpan value.
        string elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
        Console.WriteLine("RunTime " + elapsedTime);
    }
}









