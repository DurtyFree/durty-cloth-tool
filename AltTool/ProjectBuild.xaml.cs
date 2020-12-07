using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AltTool
{
    /// <summary>
    /// Логика взаимодействия для ProjectBuild.xaml
    /// </summary>
    public partial class ProjectBuild : Window
    {
        public enum TargetResourceType
        {
            Altv,
            Single,
            Fivem
        }
        public string OutputFolder = "";
        public string CollectionName = "";

        public ProjectBuild()
        {
            InitializeComponent();
        }

        private void BuildButton_Click(object sender, RoutedEventArgs e)
        {
            TargetResourceType resType = TargetResourceType.Altv;

            if (isSinglePlayerRadio.IsChecked == true)
                resType = TargetResourceType.Single;
            else if (isFivemResourceRadio.IsChecked == true)
                resType = TargetResourceType.Fivem;

            CollectionName = collectionNameText.Text;

            switch (resType)
            {
                case TargetResourceType.Altv:
                    ResourceBuilder.BuildResourceAltv(OutputFolder, CollectionName);
                    break;

                case TargetResourceType.Single:
                    ResourceBuilder.BuildResourceSingle(OutputFolder, CollectionName);
                    break;

                case TargetResourceType.Fivem:
                    ResourceBuilder.BuildResourceFivem(OutputFolder, CollectionName);
                    break;
            }
        }

        private void SelectFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            if (dialog.ShowDialog(this).GetValueOrDefault())
            {
                OutputFolder = dialog.SelectedPath;
                outFolderPathText.Content = OutputFolder;
            }
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
