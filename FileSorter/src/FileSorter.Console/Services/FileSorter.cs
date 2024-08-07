using System.Text;

namespace FileSorter.Console.Services;

    public class FileSorterService
{
    public async Task SortTo(string inputFilePath, string outputFilePath)
    {
        int chunkSize = 100000; // Number of lines per chunk
        List<string> tempFiles = new List<string>();
        var maxDegreeOfParallelism = Environment.ProcessorCount;

        try
        {
            using (var reader = new StreamReader(new BufferedStream(new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 8192 * 4)), Encoding.UTF8))
            {
                var tasks = new List<Task>();
                while (!reader.EndOfStream)
                {
                    var lines = new List<string>(chunkSize);
                    for (int i = 0; i < chunkSize && !reader.EndOfStream; i++)
                    {
                        string line = await reader.ReadLineAsync();
                        lines.Add(line);
                    }

                    // Process each chunk in parallel
                    tasks.Add(Task.Run(async () =>
                    {
                        var sortedLines = lines.AsParallel()
                                               .Select(line =>
                                               {
                                                   var parts = line.Split(new[] { ". " }, 2, StringSplitOptions.None);
                                                   Int64 number = Int64.Parse(parts[0]);
                                                   string sentence = parts[1];
                                                   return new { Number = number, Sentence = sentence, Original = line };
                                               })
                                               .OrderBy(x => x.Sentence)
                                               .ThenBy(x => x.Number)
                                               .Select(x => x.Original)
                                               .ToList();

                        string tempFileName = Path.GetTempFileName();
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

                    if (tasks.Count >= maxDegreeOfParallelism)
                    {
                        await Task.WhenAll(tasks);
                        tasks.Clear();
                    }
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

    static async Task MergeSortedFiles(List<string> sortedFiles, string outputFilePath)
    {
        var readers = sortedFiles.Select(filePath =>
            new StreamReader(new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 8192), Encoding.UTF8)
        ).ToList();

        // Using a priority queue implemented as a sorted dictionary
        var priorityQueue = new SortedDictionary<string, Queue<int>>();

        await using (var writer = new StreamWriter(new BufferedStream(new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, FileOptions.WriteThrough)), Encoding.UTF8))
        {
            for (int i = 0; i < readers.Count; i++)
            {
                if (!readers[i].EndOfStream)
                {
                    string line = await readers[i].ReadLineAsync();
                    if (!priorityQueue.TryGetValue(line, out var indices))
                    {
                        indices = new Queue<int>();
                        priorityQueue[line] = indices;
                    }
                    indices.Enqueue(i);
                }
            }

            while (priorityQueue.Count > 0)
            {
                var firstEntry = priorityQueue.First();
                string line = firstEntry.Key;
                int fileIndex = firstEntry.Value.Dequeue();

                if (firstEntry.Value.Count == 0)
                {
                    priorityQueue.Remove(line);
                }

                await writer.WriteLineAsync(line);

                if (!readers[fileIndex].EndOfStream)
                {
                    string nextLine = await readers[fileIndex].ReadLineAsync();
                    if (!priorityQueue.TryGetValue(nextLine, out var indices))
                    {
                        indices = new Queue<int>();
                        priorityQueue[nextLine] = indices;
                    }
                    indices.Enqueue(fileIndex);
                }
            }
        }

        foreach (var reader in readers)
        {
            reader.Dispose();
        }
    }
}

