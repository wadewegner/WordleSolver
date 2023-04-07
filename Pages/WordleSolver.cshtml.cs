using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace WordleSolver.Pages
{
    public class WordleSolverModel : PageModel
    {
        [BindProperty]
        public List<string> Words { get; set; } = new List<string>();

        public string ResultMessage { get; set; }

        public void OnGet()
        {
        }

        public void OnPost()
        {
            // Collect the data from the form into a list of six words
            for (int i = 0; i < 6; i++)
            {
                string word = "";
                for (int j = 0; j < 5; j++)
                {
                    // Use Request.Form to get the value of each input by its name attribute
                    string letter = Request.Form[$"word-{i}-letter-{j}"];
                    word += letter;
                }
                Words.Add(word);
            }

            // Prepare the result message
            ResultMessage = "Words: " + string.Join(", ", Words);
        }
   }
}