using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WordleSolver.Services;
using WordleSolver.Models;

namespace WordleSolver.Pages
{
    public class IndexModel : PageModel
    {
        private readonly WordDictionaryService _wordDictionaryService;
        public IndexModel(WordDictionaryService wordDictionaryService)
        {
            _wordDictionaryService = wordDictionaryService;
        }

        [BindProperty]
        public List<Word> Words { get; set; } = new List<Word>();
        public List<string> Top10LikelyWords { get; set; }

        public Dictionary<int, char> GreenLetters { get; set; }
        public Dictionary<int, char> YellowLetters { get; set; }
        public Dictionary<int, char> DarkgreyLetters { get; set; }

        public void OnPost()
        {
            // Clear the words list before adding new words
            Words.Clear();

            // Collect the data from the form into a list of six words
            for (int i = 0; i < 6; i++)
            {
                Word word = new Word();
                for (int j = 0; j < 5; j++)
                {
                    // Use Request.Form to get the value of each input by its name attribute
                    string inputValue = Request.Form[$"word-{i}-letter-{j}"].ToString().Trim().ToLower();
                    char letterChar = !string.IsNullOrEmpty(inputValue) ? inputValue[0] : ' ';
                    string letterColor = Request.Form[$"word-{i}-letter-{j}-color"].ToString().Trim();

                    // Save the input values and colors to TempData
                    TempData[$"word-{i}-letter-{j}"] = letterChar;
                    TempData[$"word-{i}-letter-{j}-color"] = letterColor;

                    word.Letters.Add(new Letter { Character = letterChar, Color = letterColor });
                }
                string wordString = new string(word.Letters.Select(l => l.Character).ToArray()).Trim();

                if (!string.IsNullOrEmpty(wordString))
                {
                    Words.Add(word);
                }
            }

            foreach (Word word in Words)
            {
                string wordString = new string(word.Letters.Select(l => l.Character).ToArray());
                bool wordExists = _wordDictionaryService.WordsHashSet.Contains(wordString);
            }

            Top10LikelyWords = GetTop10LikelyWords(Words);
        }

        public List<string> GetTop10LikelyWords(List<Word> words)
        {
            // Retrieve the list of possible words from the dictionary service
            var possibleWords = _wordDictionaryService.WordsHashSet.ToList();

            // Create dictionaries to store the known green, yellow, and darkgrey letters with their positions
            GreenLetters = new Dictionary<int, char>();
            YellowLetters = new Dictionary<int, char>();
            DarkgreyLetters = new Dictionary<int, char>();

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
                        DarkgreyLetters[j] = character;
                    }
                }
            }

            // Filter the possible words based on the green letters
            possibleWords = possibleWords.Where(word =>
            {
                foreach (var greenLetter in GreenLetters)
                {
                    // check to see if the character at the specified position in the word does not match the value; if so, exclude it
                    if (word[greenLetter.Key] != greenLetter.Value)
                    {
                        Console.WriteLine("GREEN: word / key / value: " + word + " / " + greenLetter.Key + " / " + greenLetter.Value);
                        return false;
                    }
                }

                foreach (var yellowLetter in YellowLetters)
                {
                    // check to see if the character is in the word; if not, exclude it
                    if (!word.Contains(yellowLetter.Value))
                    {
                        Console.WriteLine("YELLOW 1: word / key / value: " + word + " / " + yellowLetter.Key + " / " + yellowLetter.Value);
                        return false;
                    }

                    // check to see if this character is in this position; if so, exclude it
                    if (word[yellowLetter.Key] == yellowLetter.Value)
                    {
                        Console.WriteLine("YELLOW 2: word / key / value: " + word + " / " + yellowLetter.Key + " / " + yellowLetter.Value);
                        return false;
                    }
                }

                // foreach (var darkgreyLetter in DarkgreyLetters)
                // {
                //     if (!GreenLetters.Values.Contains(darkgreyLetter.Value) && word.Contains(darkgreyLetter.Value))
                //     {
                //         return false;
                //     }
                // }

                foreach (var darkgreyLetter in DarkgreyLetters)
                {
                    if (word[darkgreyLetter.Key] == darkgreyLetter.Value)
                    {
                        Console.WriteLine("DARKGREY: word / key / value: " + word + " / " + darkgreyLetter.Key + " / " + darkgreyLetter.Value);
                        
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
            return possibleWords.ToList();
        }
    }
}