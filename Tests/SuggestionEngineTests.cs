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
            // check if the wordlist is not null and has the correct number of words
            wordList.Should().NotBeNull();
            wordList.Should().NotBeEmpty();
            wordList.Should().HaveCount(14855);
        }

        [Fact]
        public void GetTop10LikelyWords_ShouldReturnCorrectResults_WhenProvidedWithGreenAndYellowLetters()
        {
            var suggestionEngine = new SuggestionEngine(_wordDictionaryService);

            // construct the words with the correct colors
            var wordDetails = new[]
            {
                ("dealt", "ddddg"),
                ("stott", "ddydg"),
                ("torot", "dgddg")
            };

            // convert the word details to a list of words
            var words = EngineHelper.ConstructWords(wordDetails);
            // get the most likely words
            var result = suggestionEngine.GetTop10LikelyWords(words);

            // check if the result is not null nor empty
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();

            // check that the result does not contain the word "torot"
            result.Should().NotContain("TOROT");
        }
    }
}