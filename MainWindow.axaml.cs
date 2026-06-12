using Avalonia.Controls;
using System;
using Avalonia.Interactivity;
using Avalonia;
using IronWord;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;
using System.IO;

namespace WordDocumentSearcher;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public static List<int> SearchWordInDoc(string TextToSearch, string TermToSearch)
    {
        string searchTerm = TermToSearch;
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

        return Indices;
    } 

    public static void OutputSearchResults(List <int> Indices, string TextToSearch)
    {
        int CharDisplayRange = 100;

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

    public void button_test(object sender, RoutedEventArgs e)
    {
        DirectoryInfo Directories = new DirectoryInfo("SearchArea");
        FileInfo [] Files = Directories.GetFiles();
        foreach(FileInfo FileInDir in Files)
        {           
            WordDocument DocToSearch = new WordDocument("SearchArea/" + FileInDir.Name);
            string TextToSearch = DocToSearch.ExtractText();
            List <int> Indices = SearchWordInDoc(TextToSearch, "However");
            OutputSearchResults(Indices, TextToSearch);
        }
        
    }
}