using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RichTextControls.ExampleApp
{
    public sealed partial class CodeControlTestView : UserControl
    {
        private Debouncer _debouncedParse = new Debouncer(TimeSpan.FromMilliseconds(500));

        public CodeControlTestView()
        {
            InitializeComponent();

            _debouncedParse.Action += (__, _) => CodeHighlightedPreviewTextBlock.Code = CodeSourceTextBox.Text;
            Loaded += CodeControlTestView_Loaded;
        }

        public IEnumerable<HighlightLanguage> LanguageOptions
        {
            get
            {
                foreach (var lang in Enum.GetValues(typeof(HighlightLanguage)))
                {
                    yield return (HighlightLanguage)lang;
                }
            }
        }

        private async void CodeControlTestView_Loaded(object sender, RoutedEventArgs e)
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                return;

            Loaded -= CodeControlTestView_Loaded;

            var exampleFile = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(@"Assets\Code\example.cs");

            await LoadFromFileAsync(exampleFile);
        }

        private async void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker()
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            picker.FileTypeFilter.Add("*");

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                await LoadFromFileAsync(file);
            }
        }

        private async Task LoadFromFileAsync(StorageFile file)
        {
            try
            {
                CodeSourceTextBox.Text = await FileIO.ReadTextAsync(file);
                HighlightLanguage language = HighlightLanguage.PlainText;
                switch (file.FileType)
                {
                    case ".js":
                    case ".jsx":
                        language = HighlightLanguage.JavaScript;
                        break;
                    case ".json":
                        language = HighlightLanguage.JSON;
                        break;
                    case ".cs":
                        language = HighlightLanguage.CSharp;
                        break;
                    case ".html":
                    case ".htm":
                    case ".xml":
                    case ".xaml":
                    case ".xsd":
                    case ".xhtml":
                        language = HighlightLanguage.XML;
                        break;
                    case ".py":
                        language = HighlightLanguage.Python;
                        break;
                    case ".java":
                        language = HighlightLanguage.Java;
                        break;
                    case ".css":
                        language = HighlightLanguage.CSS;
                        break;
                    case ".php":
                        language = HighlightLanguage.PHP;
                        break;
                    case ".rb":
                        language = HighlightLanguage.Ruby;
                        break;
                    case ".cpp":
                        language = HighlightLanguage.CPlusPlus;
                        break;
                    case ".sql":
                        language = HighlightLanguage.SQL;
                        break;
                }

                LanguageSelectionComboBox.SelectedIndex = LanguageOptions.ToList().IndexOf(language);
            }
            catch (Exception ex)
            {
                CodeSourceTextBox.Text = $"Unable to read the file: <em>{ex.Message}</em>";
            }
        }

        private void CodeSourceTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _debouncedParse.Hit();
        }

        private void LanguageSelectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CodeHighlightedPreviewTextBlock.HighlightLanguage = (HighlightLanguage)LanguageSelectionComboBox.SelectedItem;
        }
    }
}
