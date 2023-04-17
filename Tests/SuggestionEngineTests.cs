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
        public void GetMostLikelyWords_ShouldReturnCorrectResults_Collection1()
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
            var result = suggestionEngine.GetMostLikelyWords(words);

            // check if the result is not null nor empty
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();

            // check that the result does not contain the word "torot"
            result.Should().NotContain("torot");

            result.Should().Contain("NOINT".ToLower());
            result.Should().Contain("OOBIT".ToLower());
            result.Should().Contain("POINT".ToLower());
            result.Should().Contain("POUPT".ToLower());
            result.Should().Contain("JOINT".ToLower());
            result.Should().Contain("MOUNT".ToLower());
            result.Should().Contain("POYNT".ToLower());
            result.Should().Contain("COUNT".ToLower());
            result.Should().Contain("FOUNT".ToLower());
            result.Should().Contain("POKIT".ToLower());
            result.Should().Contain("COMPT".ToLower());
            result.Should().Contain("VOMIT".ToLower());
        }

        [Fact]
        public void GetMostLikelyWords_ShouldReturnCorrectResults_Collection2()
        {
            var suggestionEngine = new SuggestionEngine(_wordDictionaryService);

            // construct the words with the correct colors
            var wordDetails = new[]
            {
                ("soare", "dddyy"),
                ("dreer", "yddgg"),
                ("eider", "ddggg")
            };

            // convert the word details to a list of words
            var words = EngineHelper.ConstructWords(wordDetails);
            // get the most likely words
            var result = suggestionEngine.GetMostLikelyWords(words);

            // check if the result is not null nor empty
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();

            // check that the result does not contain the word "torot"
            result.Should().NotContain("torot");

            result.Should().Contain("RUDER".ToLower());
            result.Should().Contain("UDDER".ToLower());
            result.Should().Contain("CEDER".ToLower());
            result.Should().Contain("HEDER".ToLower());
            result.Should().Contain("NUDER".ToLower());
            result.Should().Contain("UNDER".ToLower());
            result.Should().Contain("CYDER".ToLower());
        }

        [Fact]
        public void GetMostLikelyWords_ShouldReturnCorrectResults_Collection3()
        {
            var suggestionEngine = new SuggestionEngine(_wordDictionaryService);

            // construct the words with the correct colors
            var wordDetails = new[]
            {
                ("irate", "dddyy"),
                ("shown", "dddyd"),
                ("lucky", "ydddd")
            };

            // convert the word details to a list of words
            var words = EngineHelper.ConstructWords(wordDetails);
            // get the most likely words
            var result = suggestionEngine.GetMostLikelyWords(words);

            // check if the result is not null nor empty
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();

            // check that the result does not contain the word "jewel" or "dwell" or "wedel"
            result.Should().NotContain("jewel");
            result.Should().NotContain("dwell");
            result.Should().NotContain("wedel");

            result.Should().Contain("tewel");
            result.Should().Contain("tweel");
            result.Should().Contain("dwelt");
        }
    }
}