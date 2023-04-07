using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WordleSolver.Pages
{
    public class Letter
    {
        public char Character { get; set; }
        public string Color { get; set; } = "green";
    }

    public class Word
    {
        public List<Letter> Letters { get; set; } = new List<Letter>();
    }

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
                    string inputValue = Request.Form[$"word-{i}-letter-{j}"].ToString().Trim();
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
            var possibleWords = _wordDictionaryService.WordsHashSet.ToList();

            foreach (var word in words)
            {
                for (int i = possibleWords.Count - 1; i >= 0; i--)
                {
                    var possibleWord = possibleWords[i];
                    bool isMatch = true;

                    for (int j = 0; j < 5; j++)
                    {
                        var letter = word.Letters[j];
                        char possibleLetter = possibleWord[j];

                        if (letter.Color == "green" && letter.Character != possibleLetter)
                        {
                            isMatch = false;
                            break;
                        }
                        else if (letter.Color == "yellow" && !word.Letters.Any(l => l.Character == possibleLetter))
                        {
                            isMatch = false;
                            break;
                        }
                        else if (letter.Color == "darkgrey" && word.Letters.Any(l => l.Character == possibleLetter))
                        {
                            isMatch = false;
                            break;
                        }
                    }

                    if (!isMatch)
                    {
                        possibleWords.RemoveAt(i);
                    }
                }
            }

            // Group the words by the number of common letters and order by the count in descending order
            var groupedWords = possibleWords.GroupBy(x => x)
                                             .Select(group => new
                                             {
                                                 Word = group.Key,
                                                 Count = group.Count()
                                             })
                                             .OrderByDescending(x => x.Count)
                                             .Take(10)
                                             .Select(x => x.Word)
                                             .ToList();

            return groupedWords;
        }
    }
}