using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Ookii.Dialogs.Wpf;

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

            try
            {
                switch (resType)
                {
                    case TargetResourceType.AltV:
                        ResourceBuilder.BuildResourceAltv(OutputFolder, CollectionName);
                        MessageBox.Show("alt:V Resource built!");
                        break;

                    case TargetResourceType.Single:
                        ResourceBuilder.BuildResourceSingle(OutputFolder, CollectionName);
                        MessageBox.Show("Singleplayer Resource built!");
                        break;

                    case TargetResourceType.FiveM:
                        ResourceBuilder.BuildResourceFiveM(OutputFolder, CollectionName);
                        MessageBox.Show("FiveM Resource built!");
                        break;
                }
            }
            catch (Exception exception)
            {
                ShowExceptionErrorDialog(exception);
            }
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

        private void ShowExceptionErrorDialog(Exception exception)
        {
            var reportErrorButton = new TaskDialogButton("Report error");
            var taskDialog = new TaskDialog
            {
                WindowTitle = "",
                CollapsedControlText = "See error details",
                ExpandedControlText = "Close error details",
                Content = "Building cloth resource failed. Please report this error at https://github.com/DurtyFree/altv-cloth-tool",
                Buttons =
                {
                    reportErrorButton,
                    new TaskDialogButton("Close")
                    {
                        ButtonType = ButtonType.Close
                    },
                },
                ExpandedInformation = exception.ToString(),
                MainIcon = TaskDialogIcon.Error,
                MainInstruction = "Unknown error occured"
            };
            var pressedButton = taskDialog.ShowDialog(this);
            if (pressedButton == reportErrorButton)
            {
                var issueBody = "I have the following error:\n" + exception +
                                "\n\nCloth files: [Please provide cloth files (ydd, ytd) and cloth project file here]";
                var issueTitle = "Exception error";
                Process.Start($"https://github.com/DurtyFree/altv-cloth-tool/issues/new?body={Uri.EscapeDataString(issueBody)}&title={Uri.EscapeDataString(issueTitle)}");
            }
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
