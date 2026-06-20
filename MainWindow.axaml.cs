using Avalonia.Controls;
using System;
using Avalonia.Interactivity;
using Avalonia;
using IronWord;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;
using System.IO;
using Avalonia.Platform.Storage;
using System.Threading.Tasks;
using Avalonia.Media;

namespace WordDocumentSearcher;

public partial class MainWindow : Window
{
    private Dictionary<string, int> MatchCounts = new Dictionary<string, int>();
    private Dictionary<string, List<int>> IndexList = new Dictionary<string, List<int>>();
    private string BasePath = "";
    private int NumberOfCharsToDisplay = 100;
    private int RowCount = 0;

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

        Grid EntryGrid = new Grid
        {
            ColumnDefinitions = ColumnDefinitions.Parse("*,*,*")
        };

        var EntryBorder = new Border
        {
            Background = Brushes.White,
            BorderBrush = Brushes.Transparent,
            CornerRadius = new CornerRadius(3),
            Child = EntryGrid,

            BoxShadow = BoxShadows.Parse("5 5 10 0 DarkGray")
        };

        EntryBorder.Margin = new Thickness(5, 0, 10, 5);


        Grid.SetRow(EntryBorder,row);

        Grid.SetRow(fileNameBlock, 0);
        Grid.SetColumn(fileNameBlock, 0);

        Grid.SetRow(matchCountsBlock, 0);
        Grid.SetColumn(matchCountsBlock, 1);

        Grid.SetRow(ViewButton, 0);
        Grid.SetColumn(ViewButton, 2);

        EntryGrid.Children.Add(fileNameBlock);
        EntryGrid.Children.Add(matchCountsBlock);
        EntryGrid.Children.Add(ViewButton);

        ResultsGrid.Children.Add(EntryBorder);

        
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

        WordDocument DocToSearch = new WordDocument(Path.Combine(BasePath,FileName));
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
            else if (!RangeExceedsRight && RangeExceedsLeft && TextToSearch.Length > NumberOfCharsToDisplay)
            {
                SearchRangeString = TextToSearch.Substring(0,NumberOfCharsToDisplay);
            }
            else if (RangeExceedsRight && RangeExceedsLeft || !RangeExceedsRight && RangeExceedsLeft && TextToSearch.Length < NumberOfCharsToDisplay)
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
            IndexBlock.Margin = new Thickness(0, 0, 10, 0);
            IndexBlock.Text = "" + Indices[i];

            TextBlock AbstractBlock = new TextBlock();
            AbstractBlock.Text = SearchRangeString;
            AbstractBlock.TextWrapping = TextWrapping.Wrap;

            Grid EntryGrid = new Grid
            {
                ColumnDefinitions = ColumnDefinitions.Parse("Auto,*")
            };

            var EntryBorder = new Border
            {
                Background = Brushes.White,
                BorderBrush = Brushes.Transparent,
                CornerRadius = new CornerRadius(3),
                Child = EntryGrid,

                BoxShadow = BoxShadows.Parse("5 5 10 0 DarkGray")
            };
            EntryBorder.Margin = new Thickness(5, 0, 10, 5);

            Grid.SetRow(EntryBorder,row);

            Grid.SetRow(IndexBlock, 0);
            Grid.SetColumn(IndexBlock, 0);

            Grid.SetRow(AbstractBlock, 0);
            Grid.SetColumn(AbstractBlock, 1);

            EntryGrid.Children.Add(IndexBlock);
            EntryGrid.Children.Add(AbstractBlock);

            ViewResultsGrid.Children.Add(EntryBorder);
            row++;

        }
    }

    private void searchDirectory(FileInfo [] Files, string TermToSearch, string ExtraPath)
    {
        foreach(FileInfo FileInDir in Files)
        {
            Console.WriteLine(FileInDir.Name);
            if (FileInDir.Name.Contains(".docx")){       
                
                bool Opened = true;
                WordDocument DocToSearch = new WordDocument();
                try
                {    
                    DocToSearch = new WordDocument(Path.Combine(BasePath,ExtraPath, FileInDir.Name));
                }
                catch (Exception error)
                {
                    Console.WriteLine("Error opening: " + ExtraPath + FileInDir.Name);
                    Opened = false;
                }
                if (Opened)
                {
                    string TextToSearch = DocToSearch.ExtractText();
                    List <int> Indices = SearchWordInDoc(TextToSearch, TermToSearch);
                    MatchCounts.Add(Path.Combine(ExtraPath, FileInDir.Name), Indices.Count);
                    IndexList.Add(Path.Combine(ExtraPath, FileInDir.Name), Indices);
                    AddEntryGraphic(Path.Combine(ExtraPath, FileInDir.Name),Indices.Count,RowCount);
                    RowCount++;
                }
                
            }
        }
    }

    private void SearchRecSubDir(string PrevDir, DirectoryInfo [] SubDirectiories, string TermToSearch)
    {
        foreach (DirectoryInfo DirTosearch in SubDirectiories)
        {
            FileInfo [] Files = DirTosearch.GetFiles();
            searchDirectory(Files, TermToSearch,Path.Combine(PrevDir,DirTosearch.Name));
            DirectoryInfo Directory = new DirectoryInfo(Path.Combine(BasePath, PrevDir,DirTosearch.Name));
            DirectoryInfo [] SubDirs = Directory.GetDirectories();
            if (SubDirs.Length > 0)
            {
                SearchRecSubDir(Path.Combine(PrevDir,DirTosearch.Name) , SubDirs,TermToSearch);
            }
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

        DirectoryInfo Directories = new DirectoryInfo(BasePath);
        DirectoryInfo [] SubDirectiories = Directories.GetDirectories();
        FileInfo [] Files = Directories.GetFiles(); 
        MatchCounts = new Dictionary<string, int>();
        IndexList = new Dictionary<string, List<int>>();

        RowCount = 0;
        searchDirectory(Files, TermToSearch,"");
        //Console.WriteLine("GOT PAST HERE");
        SearchRecSubDir("", SubDirectiories, TermToSearch);
        
        
    }

    private async Task SelectFolder()
    {
        var folders = await StorageProvider.OpenFolderPickerAsync(
            new FolderPickerOpenOptions
            {
                Title = "Select a Folder",
                AllowMultiple = false
            });

        if (folders.Count > 0)
        {
            BasePath = folders[0].Path.LocalPath;
        }
    }

    private async void SelectFolderButton(object? sender, RoutedEventArgs e)
    {
        await SelectFolder();
    }
}