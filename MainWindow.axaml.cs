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

        // Set initial size after window is opened
        Opened += OnOpened;

        // Update whenever the window size changes
        SizeChanged += OnSizeChanged;
    }

    private void OnOpened(object? sender, System.EventArgs e)
    {
        UpdateResultsHeight();
    }

    private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        UpdateResultsHeight();
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

    public static bool VerifyInteger(string StringToVerify)
    {
        foreach (char Character in StringToVerify)
        {
            if (Character > '9' || Character < '0')
            {
                return false;
            }
        }

        return true;
    }

    private void UpdateResultsHeight()
    {
        if (ResultsScrollSection != null)
        {
            // Limit results area to 50% of the window height
            ResultsScrollSection.MaxHeight = Bounds.Height * 0.5;

        }
    }
    public void button_test(object sender, RoutedEventArgs e)
    {
        string TermToSearch = SearchTermTextBox.Text;
        Console.WriteLine("Clicked");
        //Default value is 100
        int NumberOfCharsToDisplay = 100;

        if (VerifyInteger(CharacterLengthTextBox.Text))
        {
            Console.WriteLine("Verification successful");
            NumberOfCharsToDisplay = Int32.Parse(CharacterLengthTextBox.Text);
        }

        DirectoryInfo Directories = new DirectoryInfo("SearchArea");
        FileInfo [] Files = Directories.GetFiles();
        Dictionary<string, int> MatchCounts = new Dictionary<string, int>();

        foreach(FileInfo FileInDir in Files)
        {           
            WordDocument DocToSearch = new WordDocument("SearchArea/" + FileInDir.Name);
            string TextToSearch = DocToSearch.ExtractText();
            List <int> Indices = SearchWordInDoc(TextToSearch, TermToSearch);
            MatchCounts.Add(FileInDir.Name, Indices.Count);
            OutputSearchResults(Indices, TextToSearch);
        }
        
    }
}