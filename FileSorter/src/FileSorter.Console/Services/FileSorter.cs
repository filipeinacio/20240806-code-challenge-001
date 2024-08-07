using System.Text;

namespace FileSorter.Console.Services;

    public class FileSorterService
{
    public async Task SortTo(string inputFilePath, string outputFilePath)
    {
        const int chunkSize = 100000; // Number of lines per chunk
        var tempFiles = new List<string>();
        var maxDegreeOfParallelism = Environment.ProcessorCount;

        try
        {
            using (var reader = new StreamReader(new BufferedStream(new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 8192 * 4)), Encoding.UTF8))
            {
                var tasks = new List<Task>();
                while (!reader.EndOfStream)
                {
                    var lines = new List<string>(chunkSize);
                    for (var i = 0; i < chunkSize && !reader.EndOfStream; i++)
                    {
                        var line = await reader.ReadLineAsync() ?? string.Empty;
                        if (!string.IsNullOrEmpty(line))
                        {
                            lines.Add(line);
                        }
                    }

                    // Process each chunk in parallel
                    tasks.Add(Task.Run(async () =>
                    {
                        var sortedLines = lines.AsParallel()
                                               .Select(line =>
                                               {
                                                   var parts = line.Split([". "], 2, StringSplitOptions.None);
                                                   var number = long.Parse(parts[0]);
                                                   var sentence = parts[1];
                                                   return new { Number = number, Sentence = sentence, Original = line };
                                               })
                                               .OrderBy(x => x.Sentence)
                                               .ThenBy(x => x.Number)
                                               .Select(x => x.Original)
                                               .ToList();

                        var tempFileName = Path.GetTempFileName();
                        await using (var writer = new StreamWriter(new BufferedStream(new FileStream(tempFileName, FileMode.Create, FileAccess.Write, FileShare.None, 8192, FileOptions.WriteThrough)), Encoding.UTF8))
                        {
                            foreach (var line in sortedLines)
                            {
                                await writer.WriteLineAsync(line);
                            }
                        }
                        lock (tempFiles)
                        {
                            tempFiles.Add(tempFileName);
                        }
                    }));

                    if (tasks.Count < maxDegreeOfParallelism) continue;
                    
                    await Task.WhenAll(tasks);
                    tasks.Clear();
                }

                await Task.WhenAll(tasks);
            }

            await MergeSortedFiles(tempFiles, outputFilePath);
        }
        finally
        {
            // Clean up temporary files
            foreach (var file in tempFiles)
            {
                File.Delete(file);
            }
        }
    }

    private static async Task MergeSortedFiles(List<string> sortedFiles, string outputFilePath)
    {
        var readers = sortedFiles.Select(filePath =>
            new StreamReader(new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 8192), Encoding.UTF8)
        ).ToList();

        // Using a priority queue implemented as a sorted dictionary
        var priorityQueue = new SortedDictionary<string, Queue<int>>();

        await using (var writer = new StreamWriter(new BufferedStream(new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, FileOptions.WriteThrough)), Encoding.UTF8))
        {
            for (var i = 0; i < readers.Count; i++)
            {
                if (readers[i].EndOfStream) continue;
                
                var line = await readers[i].ReadLineAsync() ?? string.Empty;

                if (string.IsNullOrEmpty(line)) continue;
                
                if (!priorityQueue.TryGetValue(line, out var indices))
                {
                    indices = new Queue<int>();
                    priorityQueue[line] = indices;
                }
                indices.Enqueue(i);
            }

            while (priorityQueue.Count > 0)
            {
                var (line, value) = priorityQueue.First();
                var fileIndex = value.Dequeue();

                if (value.Count == 0)
                {
                    priorityQueue.Remove(line);
                }

                await writer.WriteLineAsync(line);

                if (readers[fileIndex].EndOfStream) continue;
                
                var nextLine = await readers[fileIndex].ReadLineAsync() ?? string.Empty;
                    
                if (string.IsNullOrEmpty(nextLine)) continue;
                    
                if (!priorityQueue.TryGetValue(nextLine, out var indices))
                {
                    indices = new Queue<int>();
                    priorityQueue[nextLine] = indices;
                }
                indices.Enqueue(fileIndex);
            }
        }

        foreach (var reader in readers)
        {
            reader.Dispose();
        }
    }
}

