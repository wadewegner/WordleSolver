using WordleSolver.Models;

namespace WordleSolver.Services
{
    public class WordleSolver
    {
        private IDictionary<int, char> GreenLetters;
        private IDictionary<int, char> YellowLetters;
        private IList<char> DarkgreyLetters;
        private WordDictionaryService _wordDictionaryService;

        public WordleSolver(WordDictionaryService wordDictionaryService)
        {
            _wordDictionaryService = wordDictionaryService;
        }

        public List<string> GetTop10LikelyWords(List<Word> words)
        {
            // Retrieve the list of possible words from the dictionary service
            var possibleWords = _wordDictionaryService.WordsHashSet.ToList();

            // Create dictionaries to store the known green, yellow, and darkgrey letters with their positions
            GreenLetters = new Dictionary<int, char>();
            YellowLetters = new Dictionary<int, char>();
            DarkgreyLetters = new List<char>();

            // Iterate through the input words and populate the dictionaries
            for (int i = 0; i < words.Count; i++)
            {
                for (int j = 0; j < words[i].Letters.Count; j++)
                {
                    char character = words[i].Letters[j].Character;
                    string color = words[i].Letters[j].Color;

                    if (color == "green")
                    {
                        GreenLetters[j] = character;
                    }
                    else if (color == "yellow")
                    {
                        YellowLetters[j] = character;
                    }
                    else if (color == "darkgrey")
                    {
                        DarkgreyLetters.Add(character);
                    }
                }
            }

            // Filter the possible words based on the green letters, yellow letters, and excluding darkgrey letters
            possibleWords = possibleWords.Where(word =>
            {
                foreach (var greenLetter in GreenLetters)
                {
                    if (word.Length <= greenLetter.Key || word[greenLetter.Key] != greenLetter.Value)
                    {
                        return false;
                    }
                }

                foreach (var yellowLetter in YellowLetters)
                {
                    if (!word.Contains(yellowLetter.Value))
                    {
                        return false;
                    }

                    if (word[yellowLetter.Key] == yellowLetter.Value)
                    {
                        return false;
                    }
                }

                foreach (var darkgreyLetter in DarkgreyLetters)
                {
                    if (word.Contains(darkgreyLetter))
                    {
                        return false;
                    }
                }

                return true;
            }).ToList();

            // Calculate the frequency of letters used in the possibleWords list
            Dictionary<char, int> letterFrequency = new Dictionary<char, int>();

            foreach (var word in possibleWords)
            {
                foreach (var letter in word)
                {
                    if (letterFrequency.ContainsKey(letter))
                    {
                        letterFrequency[letter]++;
                    }
                    else
                    {
                        letterFrequency.Add(letter, 1);
                    }
                }
            }

            // Sort the possibleWords list based on the frequency of letters used
            possibleWords = possibleWords.OrderByDescending(word =>
            {
                int sumFrequency = 0;

                foreach (var letter in word)
                {
                    sumFrequency += letterFrequency[letter];
                }

                return (double)sumFrequency / word.Length;
            }).ToList();

            // Return the top 10 most likely words
            return possibleWords.Take(10).ToList();
        }
    }
}