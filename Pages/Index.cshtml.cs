﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

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
        [BindProperty]
        public List<Word> Words { get; set; } = new List<Word>();

        public string ResultMessage { get; set; }

        public void OnGet()
        {
        }

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
                Words.Add(word);
            }
        }
   }
}