using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace altClothTool.App
{
    public partial class ProjectBuild : Window
    {
        public enum TargetResourceType
        {
            AltV,
            Single,
            FiveM
        }
        public string OutputFolder = "";
        public string CollectionName = "";

        public ProjectBuild()
        {
            InitializeComponent();
        }

        private void BuildButton_Click(object sender, RoutedEventArgs e)
        {
            TargetResourceType resType = TargetResourceType.AltV;

            if (isSinglePlayerRadio.IsChecked == true)
                resType = TargetResourceType.Single;
            else if (isFivemResourceRadio.IsChecked == true)
                resType = TargetResourceType.FiveM;

            CollectionName = collectionNameText.Text;

            if (FilePathHasInvalidChars(OutputFolder))
            {
                MessageBox.Show("Output folder path contains invalid characters.\nPlease choose another output location.");
                StatusController.SetStatus("Error: Invalid build output folder.");
                return;
            }

            new ClothesResourceBuilderFactory().BuildResource(resType, OutputFolder, CollectionName);
        }

        private bool FilePathHasInvalidChars(string path)
        {
            bool ret = false;
            if(!string.IsNullOrEmpty(path))
            {
                try
                {
                    string fileName = System.IO.Path.GetFileName(path);
                    string fileDirectory = System.IO.Path.GetDirectoryName(path);
                }
                catch (ArgumentException)
                {
                    ret = true;
                }
            }
            return ret;
        }
        
        private void SelectFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            if (!dialog.ShowDialog(this).GetValueOrDefault()) 
                return;

            OutputFolder = dialog.SelectedPath;
            outFolderPathText.Content = OutputFolder;
        }

        private void ValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[A-Za-z_]$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void CollectionNameText_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }
    }
}
