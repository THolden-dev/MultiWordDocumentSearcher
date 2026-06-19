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
    private Dictionary<string, int> MatchCounts = new Dictionary<string, int>();
    private Dictionary<string, List<int>> IndexList = new Dictionary<string, List<int>>();
    private int NumberOfCharsToDisplay = 100;

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

    private void AddEntryGraphic(string FileName, int MatchCount, int row)
    {

        Console.WriteLine("got here");
        RowDefinition rowDef = new RowDefinition();
        rowDef.Height = GridLength.Auto;

        ResultsGrid.RowDefinitions.Add(rowDef);

        TextBlock fileNameBlock = new TextBlock();
        fileNameBlock.Text = FileName;

        var ViewButton = new Button
        {
            Content = "View",
            Tag = FileName,
        };

        ViewButton.Click += view_Hits;

        TextBlock matchCountsBlock = new TextBlock();
        matchCountsBlock.Text = MatchCount.ToString();

      

        Grid.SetRow(fileNameBlock, row);
        Grid.SetColumn(fileNameBlock, 0);

        Grid.SetRow(matchCountsBlock, row);
        Grid.SetColumn(matchCountsBlock, 1);

        Grid.SetRow(ViewButton, row);
        Grid.SetColumn(ViewButton, 2);

        ResultsGrid.Children.Add(fileNameBlock);
        ResultsGrid.Children.Add(matchCountsBlock);
        ResultsGrid.Children.Add(ViewButton);

        
    }

    public void backButton(object sender, RoutedEventArgs e)
    {
        
        ViewResultsGrid.Children.Clear();
        ViewResultPanel.IsVisible = false;
        ResultsGrid.IsVisible = true;
    }

    public void view_Hits(object sender, RoutedEventArgs e)
    {
        ViewResultsGrid.Children.Clear();
        ResultsGrid.IsVisible = false;
        ViewResultPanel.IsVisible = true;
        var button = (Button)sender!;
        string FileName = (string)button.Tag!;

        WordDocument DocToSearch = new WordDocument("SearchArea/" + FileName);
        string TextToSearch = DocToSearch.ExtractText();
        List <int> Indices = IndexList[FileName];
        int row = 0;
        for (int i = 0; i < Indices.Count; i++)
        {
            int UpperRange = Indices[i] + NumberOfCharsToDisplay / 2;
            int LowerRange = Indices[i] - NumberOfCharsToDisplay / 2;
            bool RangeExceedsRight = UpperRange >= TextToSearch.Length;
            bool RangeExceedsLeft = LowerRange < 0;
            string SearchRangeString = "";
            if (RangeExceedsRight && !RangeExceedsLeft)
            {
                SearchRangeString = TextToSearch.Substring(LowerRange,UpperRange - TextToSearch.Length);
            }
            else if (!RangeExceedsRight && RangeExceedsLeft)
            {
                SearchRangeString = TextToSearch.Substring(0,NumberOfCharsToDisplay);
            }
            else if (RangeExceedsRight && RangeExceedsLeft)
            {
                Console.WriteLine("I AM PRINTING EVERYTHING");
                SearchRangeString = TextToSearch;
            }
            else
            {
                SearchRangeString = TextToSearch.Substring(LowerRange,NumberOfCharsToDisplay);
            }
            RowDefinition rowDef = new RowDefinition();
            rowDef.Height = GridLength.Auto;

            ViewResultsGrid.RowDefinitions.Add(rowDef);

            TextBlock IndexBlock = new TextBlock();
            IndexBlock.Text = "" + Indices[i];

            TextBlock AbstractBlock = new TextBlock();
            AbstractBlock.Text = SearchRangeString;

            Grid.SetRow(IndexBlock, row);
            Grid.SetColumn(IndexBlock, 0);

            Grid.SetRow(AbstractBlock, row);
            Grid.SetColumn(AbstractBlock, 1);

            ViewResultsGrid.Children.Add(IndexBlock);
            ViewResultsGrid.Children.Add(AbstractBlock);
            row++;

        }
    }

    public void button_test(object sender, RoutedEventArgs e)
    {
        ResultsGrid.Children.Clear();
        ResultsGrid.IsVisible = true;
        ViewResultPanel.IsVisible = false;
        ViewResultsGrid.Children.Clear();

        string TermToSearch = SearchTermTextBox.Text;
        Console.WriteLine("Clicked");
        //Default value is 100
        NumberOfCharsToDisplay = 100;

        if (VerifyInteger(CharacterLengthTextBox.Text))
        {
            Console.WriteLine("Verification successful");
            NumberOfCharsToDisplay = Int32.Parse(CharacterLengthTextBox.Text);
        }

        DirectoryInfo Directories = new DirectoryInfo("SearchArea");
        FileInfo [] Files = Directories.GetFiles();
        MatchCounts = new Dictionary<string, int>();
        IndexList = new Dictionary<string, List<int>>();

        int RowCount = 0;
        foreach(FileInfo FileInDir in Files)
        {           
            WordDocument DocToSearch = new WordDocument("SearchArea/" + FileInDir.Name);
            string TextToSearch = DocToSearch.ExtractText();
            List <int> Indices = SearchWordInDoc(TextToSearch, TermToSearch);
            MatchCounts.Add(FileInDir.Name, Indices.Count);
            IndexList.Add(FileInDir.Name, Indices);
            AddEntryGraphic(FileInDir.Name,Indices.Count,RowCount);
            RowCount++;
        }
        //Console.WriteLine("GOT PAST HERE");
        
        
    }
}