using Xunit;
using FluentAssertions;
using WordleSolver.Services;
using WordleSolver.Models;

namespace WordleSolver.Tests
{
    public class SuggestionEngineTests
    {
        private readonly WordDictionaryService _wordDictionaryService;
        private List<string> wordList;

        public SuggestionEngineTests()
        {
            // Initialize WordDictionaryService
            _wordDictionaryService = new WordDictionaryService();
            wordList = _wordDictionaryService.WordsHashSet.ToList();
        }

        [Fact]
        public void GetWordleWords_ShouldReturnCorrectResults()
        {
            wordList.Should().NotBeNull();
            wordList.Should().NotBeEmpty();
            wordList.Should().HaveCount(14855);
        }

        [Fact]
        public void GetTop10LikelyWords_ShouldReturnCorrectResults_WhenProvidedWithGreenAndYellowLetters()
        {
            var suggestionEngine = new SuggestionEngine(_wordDictionaryService);

            var words = new List<Word>
            {
                new Word
                {
                    Letters = new List<Letter>
                    {
                        new Letter { Character = 'd', Color = "darkgrey" },
                        new Letter { Character = 'e', Color = "darkgrey" },
                        new Letter { Character = 'a', Color = "darkgrey" },
                        new Letter { Character = 'l', Color = "darkgrey" },
                        new Letter { Character = 't', Color = "green" }
                    }
                },
                new Word 
                {
                    Letters = new List<Letter>
                    {
                        new Letter { Character = 's', Color = "darkgrey" },
                        new Letter { Character = 't', Color = "darkgrey" },
                        new Letter { Character = 'o', Color = "yellow" },
                        new Letter { Character = 't', Color = "darkgrey" },
                        new Letter { Character = 't', Color = "green" }
                    }
                },
                new Word 
                {
                    Letters = new List<Letter>
                    {
                        new Letter { Character = 't', Color = "darkgrey" },
                        new Letter { Character = 'o', Color = "green" },
                        new Letter { Character = 'r', Color = "darkgrey" },
                        new Letter { Character = 'o', Color = "darkgrey" },
                        new Letter { Character = 't', Color = "green" }
                    }
                }
            };

            // Act
            var result = suggestionEngine.GetTop10LikelyWords(words);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Should().NotContain("TOROT");

            // Add more assertions to verify the correctness of the result
        }
    }
}