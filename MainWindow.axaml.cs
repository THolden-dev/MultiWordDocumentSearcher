using Avalonia.Controls;
using System;
using Avalonia.Interactivity;
using Avalonia;
using IronWord;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;

namespace WordDocumentSearcher;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public static List<int> SearchWordInDoc(string FilePath, string TermToSearch)
    {
        WordDocument DocToSearch = new WordDocument("Test2.docx");
        string TextToSearch = DocToSearch.ExtractText();
        string searchTerm = TermTosearch;
        List<int> Indices = new();

        int startIndex = 0;
        int index = 0;
        while (index != -1)
        {
            index = TextToSearch.IndexOf(
                searchTerm,
                startIndex,
                StringComparison.OrdinalIgnoreCase);

            if (index >= 0)
            {
                Indices.Add(index);
            }

            // Move past this match
            startIndex = index + searchTerm.Length;
        }
    } 

    public void button_test(object sender, RoutedEventArgs e)
    {
        WordDocument DocToSearch = new WordDocument("Test2.docx");
        string TextToSearch = DocToSearch.ExtractText();
        string searchTerm = "However";
        int CharDisplayRange = 300;
        List<int> Indices = new();

        int startIndex = 0;
        int index = 0;
        while (index != -1)
        {
            index = TextToSearch.IndexOf(
                searchTerm,
                startIndex,
                StringComparison.OrdinalIgnoreCase);

            if (index >= 0)
            {
                Indices.Add(index);
            }

            // Move past this match
            startIndex = index + searchTerm.Length;
        }
        for (int i = 0; i < Indices.Count; i++)
        {
            int UpperRange = Indices[i] + CharDisplayRange / 2;
            int LowerRange = Indices[i] - CharDisplayRange / 2;
            bool RangeExceedsRight = UpperRange >= TextToSearch.Length;
            bool RangeExceedsLeft = LowerRange < 0;
            string SearchRangeString = "";
            if (RangeExceedsRight && !RangeExceedsLeft)
            {
                SearchRangeString = TextToSearch.Substring(LowerRange,UpperRange - TextToSearch.Length);
            }
            else if (!RangeExceedsRight && RangeExceedsLeft)
            {
                SearchRangeString = TextToSearch.Substring(0,CharDisplayRange);
            }
            else if (RangeExceedsRight && RangeExceedsLeft)
            {
                Console.WriteLine("I AM PRINTING EVERYTHING");
                SearchRangeString = TextToSearch;
            }
            else
            {
                SearchRangeString = TextToSearch.Substring(LowerRange,CharDisplayRange);
            }
            Console.WriteLine("------------------");
            Console.WriteLine(SearchRangeString);
        }
    }
}