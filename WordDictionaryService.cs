using System.Collections.Generic;
using System.IO;

public class WordDictionaryService
{
    public HashSet<string> WordsHashSet { get; private set; }

    public WordDictionaryService()
    {
        string filePath = "words.txt";
        WordsHashSet = LoadWordsFromFile(filePath);
    }

    private HashSet<string> LoadWordsFromFile(string filePath)
    {
        HashSet<string> wordsHashSet = new HashSet<string>();

        try
        {
            string allWords = File.ReadAllText(filePath);
            string[] wordsArray = allWords.Split(' ');

            foreach (string word in wordsArray)
            {
                wordsHashSet.Add(word);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading file: {ex.Message}");
        }

        return wordsHashSet;
    }
}