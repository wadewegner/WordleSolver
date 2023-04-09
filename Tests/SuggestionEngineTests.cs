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

            var wordDetails = new[]
            {
                ("dealt", "ddddg"),
                ("stott", "ddydg"),
                ("torot", "dgddg")
            };

            var words = EngineHelper.ConstructWords(wordDetails);
            var result = suggestionEngine.GetTop10LikelyWords(words);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Should().NotContain("TOROT");
        }
    }
}