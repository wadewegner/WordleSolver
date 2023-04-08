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
        public Dictionary<int, List<char>> DarkgreyLetters { get; set; } // Updated to store a list of characters

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
            DarkgreyLetters = new Dictionary<int, List<char>>(); // Updated to store a list of characters

            Console.WriteLine("----------");

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
                        if (!DarkgreyLetters.ContainsKey(j))
                        {
                            DarkgreyLetters[j] = new List<char>();
                        }

                        DarkgreyLetters[j].Add(character); // Add the character to the list
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
                        return false;
                    }
                }

                foreach (var yellowLetter in YellowLetters)
                {
                    // check to see if the character is in the word; if not, exclude it
                    if (!word.Contains(yellowLetter.Value))
                    {
                        return false;
                    }

                    // check to see if this character is in this position; if so, exclude it
                    if (word[yellowLetter.Key] == yellowLetter.Value)
                    {
                        return false;
                    }
                }

                foreach (var darkgreyLetter in DarkgreyLetters)
                {
                    foreach (var character in darkgreyLetter.Value)
                    {
                        // Check if the current character is in the word and not green in a different position
                        if (word.Contains(character) && !GreenLetters.Values.Contains(character))
                        {
                            // Exclude the word if the character is found and not green elsewhere
                            return false;
                        }
                    }
                }

                foreach (var darkgreyLetter in DarkgreyLetters)
                {
                    var position = darkgreyLetter.Key;
                    var greyCharacters = darkgreyLetter.Value;

                    // If there's a grey color character in the same position as the word being evaluated, exclude the word
                    if (greyCharacters.Contains(word[position]))
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
            return possibleWords.ToList();
        }
    }
}