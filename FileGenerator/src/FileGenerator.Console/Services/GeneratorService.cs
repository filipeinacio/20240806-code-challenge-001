using System.Text;

namespace FileGenerator.Console.Services;

public class GeneratorService
{
    public void GenerateFile(string filePath, long fileSize)
    {
        // Determine reservoir size based on file size
        const int averageLineLength = 100; // Estimated average line length in bytes
        int reservoirSize = (int)(fileSize / (averageLineLength * 1000)); // Set as a fraction of the total size

        if (reservoirSize < 1)
        {
            reservoirSize = 1; // Ensure at least one line can be stored
        }

        var reservoir = new List<string>(reservoirSize);

        long currentSize = 0;
        const int maxWordsPerSentence = 100; // maximum number of words in a sentence

        using (var writer = new StreamWriter(filePath))
        {
            while (currentSize < fileSize)
            {
                // Generate a random int64
                long randomInt = Random.Shared.NextInt64(long.MinValue, long.MaxValue);

                // Generate a human-readable sentence
                string randomSentence = SentenceGenerator.GenerateRandom(maxWordsPerSentence);

                // Construct the line
                string line = $"{randomInt}. {randomSentence}";

                // Occasionally add duplicate lines
                if (Random.Shared.NextDouble() < 0.1 && reservoir.Count > 0)
                {
                    // Use a previously generated line to introduce duplicates
                    line = reservoir[Random.Shared.Next(reservoir.Count)];
                }
                else
                {
                    // Store the line for future duplication
                    // Add the current line to the reservoir with reservoir sampling logic
                    if (reservoir.Count < reservoirSize)
                    {
                        reservoir.Add(line);
                    }
                    else
                    {
                        int index = Random.Shared.Next(reservoir.Count());
                        if (index < reservoirSize)
                        {
                            reservoir[index] = line;
                        }
                    }
                }

                // Write the line to the file
                writer.WriteLine(line);

                // Update current size
                currentSize += Encoding.UTF8.GetByteCount(line + Environment.NewLine);

                // Check if we should also duplicate random text parts within the line
                if (Random.Shared.NextDouble() < 0.05)
                {
                    randomInt = Random.Shared.NextInt64(long.MinValue, long.MaxValue);
                    
                    string reservoirLine = reservoir[Random.Shared.Next(reservoir.Count)];
                    int index = reservoirLine.IndexOf(". ", StringComparison.Ordinal);
                    
                    string duplicateSentence = reservoirLine.Substring(index + 2);
                    writer.WriteLine($"{randomInt}. {duplicateSentence}");
                    
                    currentSize += Encoding.UTF8.GetByteCount($"{randomInt}. {duplicateSentence}" + Environment.NewLine);
                }
            }
        }
        
        System.Console.WriteLine($"File '{filePath}' generated with size approximately {currentSize} bytes.");
    }
}