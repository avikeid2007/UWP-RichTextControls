using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RichTextControls.ExampleApp
{
    public sealed partial class HtmlControlTestView : UserControl
    {
        private Debouncer _debouncedParseHtml = new Debouncer(TimeSpan.FromMilliseconds(500));

        public HtmlControlTestView()
        {
            InitializeComponent();

            _debouncedParseHtml.Action += (_, __) => HtmlPreviewTextBlock.Html = HtmlSourceTextBox.Text;

            Loaded += HtmlControlTestView_Loaded;
        }

        private async void HtmlControlTestView_Loaded(object sender, RoutedEventArgs e)
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                return;

            Loaded -= HtmlControlTestView_Loaded;

            var exampleFile = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(@"Assets\Code\example.html");

            await LoadFromFileAsync(exampleFile);
        }

        private async void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker()
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            picker.FileTypeFilter.Add(".txt");
            picker.FileTypeFilter.Add(".html");

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
                HtmlSourceTextBox.Text = await FileIO.ReadTextAsync(file);
            }
            catch (Exception ex)
            {
                HtmlSourceTextBox.Text = $"Unable to read the file: <em>{ex.Message}</em>";
            }
        }

        private void HtmlSourceTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _debouncedParseHtml.Hit();
        }
    }
}
