using System.Text;

namespace FileGenerator.Console.Services;

public static class SentenceGenerator
{
    public static string GenerateRandom(int maxWordsPerSentence)
    {
        var words = new string[]
        {
            "the", "quick", "brown", "fox", "jumps", "over", "lazy", "dog",
            "lorem", "ipsum", "dolor", "sit", "amet", "consectetur", "adipiscing", "elit",
            "sed", "do", "eiusmod", "tempor", "incididunt", "ut", "labore", "et", "dolore",
            "magna", "aliqua", "bonjour", "ça", "va", "merci", "gracias", "hola",
            "niño", "mundo", "château", "fête", "ballet", "café", "tête-à-tête",
            "über", "groß", "façade", "résumé", "jalapeño", "naïve", "élite",
            "mañana", "año", "señor", "señora", "tortilla", "sushi", "sakura", "konnichiwa",
            "ありがとう", "さようなら", "こんにちは", "안녕하세요", "감사합니다", "再见", "你好", "谢谢",
            "Добрый", "день", "спасибо", "пожалуйста", "Привет", "こんにちは",
            "Γειά", "σας", "ευχαριστώ", "пожалуйста", "مرحبا", "شكرا"
        };
        
        Random.Shared.Shuffle(words);

        const int minValue = 5;
        var maxValue = maxWordsPerSentence + 1 > minValue ? maxWordsPerSentence + 1 : minValue+1;
        var wordCount = Random.Shared.Next(minValue, maxValue);
        var sentence = new StringBuilder();

        for (var i = 0; i < wordCount; i++)
        {
            if (i > 0)
                sentence.Append(' ');
            sentence.Append(words[Random.Shared.Next(words.Length)]);
        }

        // Capitalize the first letter of the sentence and add a period at the end.
        sentence[0] = char.ToUpper(sentence[0]);
        sentence.Append('.');

        return sentence.ToString();
    }
}