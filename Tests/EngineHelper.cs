using System.Collections.Generic;
using System.Linq;
using WordleSolver.Models;

namespace WordleSolver.Tests
{
    public static class EngineHelper
    {
        public static List<Word> ConstructWords((string word, string colorCodes)[] wordDetails)
        {
            var colorMap = new Dictionary<char, string>
            {
                { 'd', "darkgrey" },
                { 'g', "green" },
                { 'y', "yellow" }
            };

            var words = wordDetails.Select(details =>
            {
                var letters = details.word.Zip(details.colorCodes, (c, colorCode) => new Letter
                {
                    Character = c,
                    Color = colorMap[colorCode]
                }).ToList();
                return new Word { Letters = letters };
            }).ToList();

            return words;
        }
    }
}