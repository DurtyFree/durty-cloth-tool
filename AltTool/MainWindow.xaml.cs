using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static AltTool.ClothData;

namespace AltTool
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static TextBlock _statusTextBlock;
        private static ProgressBar _statusProgress;
        private static ClothData _selectedCloth;

        public static ProjectBuild ProjectBuildWindow;
        public static ObservableCollection<ClothData> Clothes;

        public MainWindow()
        {
            InitializeComponent();

            _statusTextBlock = (TextBlock)FindName("currentStatusBar");
            _statusProgress = (ProgressBar)FindName("currentProgress");

            Clothes = new ObservableCollection<ClothData>();
            clothesListBox.ItemsSource = Clothes;
        }

        public static void SetStatus(string status)
        {
            _statusTextBlock.Text = status;
        }

        public static void SetProgress(double progress)
        {
            if (progress > 1)
                progress = 1;
            if (progress < 0)
                progress = 0;

            _statusProgress.Value = _statusProgress.Maximum * progress;
        }

        private void AddMaleClothes_Click(object sender, RoutedEventArgs e)
        {
            ProjectController.Instance().AddFiles(Sex.Male);
        }

        private void AddFemaleClothes_Click(object sender, RoutedEventArgs e)
        {
            ProjectController.Instance().AddFiles(Sex.Female);
        }

        private void RemoveUnderCursor_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCloth == null) 
                return;

            var clothes = Clothes.OrderBy(x => x.Name, new AlphanumericComparer()).ToList();

            Clothes.Clear();

            foreach(var cloth in clothes)
            {
                if(cloth != _selectedCloth)
                {
                    Clothes.Add(cloth);
                }
            }

            _selectedCloth = null;
            editGroupBox.Visibility = Visibility.Collapsed;
        }

        private void ClothesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count > 0)
            {
                _selectedCloth = (ClothData)e.AddedItems[0];

                if (_selectedCloth != null)
                {
                    clothEditWindow.Visibility = Visibility.Collapsed;
                    pedPropEditWindow.Visibility = Visibility.Collapsed;
                    editGroupBox.Visibility = Visibility.Visible;

                    if (_selectedCloth.IsComponent())
                    {
                        clothEditWindow.Visibility = Visibility.Visible;
                        drawableName.Text = _selectedCloth.Name;

                        texturesList.ItemsSource = _selectedCloth.Textures;
                        fpModelPath.Text = _selectedCloth.FpModelPath != "" ? _selectedCloth.FpModelPath : "Not selected...";

                        unkFlag1Check.IsChecked = _selectedCloth.componentFlags.unkFlag1;
                        unkFlag2Check.IsChecked = _selectedCloth.componentFlags.unkFlag2;
                        unkFlag3Check.IsChecked = _selectedCloth.componentFlags.unkFlag3;
                        unkFlag4Check.IsChecked = _selectedCloth.componentFlags.unkFlag4;
                        isHighHeelsCheck.IsChecked = _selectedCloth.componentFlags.isHighHeels;
                    }
                    else
                    {
                        pedPropEditWindow.Visibility = Visibility.Visible;
                        drawableName.Text = _selectedCloth.Name;
                        pedPropName.Text = _selectedCloth.Name;

                        pedPropTexturesList.ItemsSource = _selectedCloth.Textures;

                        pedPropFlag1.IsChecked = _selectedCloth.pedPropFlags.unkFlag1;
                        pedPropFlag2.IsChecked = _selectedCloth.pedPropFlags.unkFlag2;
                        pedPropFlag3.IsChecked = _selectedCloth.pedPropFlags.unkFlag3;
                        pedPropFlag4.IsChecked = _selectedCloth.pedPropFlags.unkFlag4;
                        pedPropFlag5.IsChecked = _selectedCloth.pedPropFlags.unkFlag5;
                    }
                }
            }
        }

        private void DrawableName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(_selectedCloth != null)
            {
                _selectedCloth.Name = drawableName.Text;
            }
        }

        private void NewProjectButton_Click(object sender, RoutedEventArgs e)
        {
            Clothes.Clear();
        }

        private void OpenProjectButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                CheckFileExists = true,
                Filter = "altV cloth JSON (*.altv-cloth.json)|*.altv-cloth.json",
                FilterIndex = 1,
                DefaultExt = "altv-cloth.json"
            };

            if (openFileDialog.ShowDialog() != true)
                return;

            foreach (string filename in openFileDialog.FileNames)
            {
                ProjectBuilder.LoadProject(filename);
            }
        }

        private void SaveProjectButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "altV cloth JSON (*.altv-cloth.json)|*.altv-cloth.json",
                FilterIndex = 1,
                DefaultExt = "altv-cloth.json"
            };

            if (saveFileDialog.ShowDialog() != true) 
                return;

            foreach (string filename in saveFileDialog.FileNames)
            {
                ProjectBuilder.BuildProject(filename);
            }
        }

        private void AddTexture_Click(object sender, RoutedEventArgs e)
        {
            if(_selectedCloth != null)
                ProjectController.Instance().AddTexture(_selectedCloth);
        }

        private void RemoveTexture_Click(object sender, RoutedEventArgs e)
        {
            if(texturesList.SelectedItem != null)
            {
                ((ObservableCollection<string>)texturesList.ItemsSource).Remove((string)texturesList.SelectedItem);
            }
        }

        private void BuildProjectButton_Click(object sender, RoutedEventArgs e)
        {
            ProjectBuildWindow = new ProjectBuild();
            ProjectBuildWindow.Show();
        }

        private void UnkFlag1Check_Checked(object sender, RoutedEventArgs e)
        {
            if (_selectedCloth != null)
                _selectedCloth.componentFlags.unkFlag1 = unkFlag1Check.IsChecked.GetValueOrDefault(false);
        }

        private void UnkFlag2Check_Checked(object sender, RoutedEventArgs e)
        {
            if (_selectedCloth != null)
                _selectedCloth.componentFlags.unkFlag2 = unkFlag2Check.IsChecked.GetValueOrDefault(false);
        }

        private void UnkFlag3Check_Checked(object sender, RoutedEventArgs e)
        {
            if (_selectedCloth != null)
                _selectedCloth.componentFlags.unkFlag3 = unkFlag3Check.IsChecked.GetValueOrDefault(false);
        }

        private void UnkFlag4Check_Checked(object sender, RoutedEventArgs e)
        {
            if (_selectedCloth != null)
                _selectedCloth.componentFlags.unkFlag4 = unkFlag4Check.IsChecked.GetValueOrDefault(false);
        }

        private void IsHighHeelsCheck_Checked(object sender, RoutedEventArgs e)
        {
            if (_selectedCloth != null)
                _selectedCloth.componentFlags.isHighHeels = isHighHeelsCheck.IsChecked.GetValueOrDefault(false);
        }

        private void ClearFPModel_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCloth != null)
                _selectedCloth.FpModelPath = "";
            fpModelPath.Text = "Not selected...";
        }

        private void SelectFPModel_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCloth != null)
                ProjectController.Instance().SetFPModel(_selectedCloth);
            fpModelPath.Text = _selectedCloth.FpModelPath != "" ? _selectedCloth.FpModelPath : "Not selected...";
        }

        private void PedPropName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_selectedCloth != null)
            {
                _selectedCloth.Name = drawableName.Text;
            }
        }

        private void PedPropFlag1_Checked(object sender, RoutedEventArgs e)
        {
            if (_selectedCloth != null)
                _selectedCloth.pedPropFlags.unkFlag1 = unkFlag1Check.IsChecked.GetValueOrDefault(false);
        }

        private void PedPropFlag2_Checked(object sender, RoutedEventArgs e)
        {
            if (_selectedCloth != null)
                _selectedCloth.pedPropFlags.unkFlag2 = unkFlag1Check.IsChecked.GetValueOrDefault(false);
        }

        private void PedPropFlag3_Checked(object sender, RoutedEventArgs e)
        {
            if (_selectedCloth != null)
                _selectedCloth.pedPropFlags.unkFlag3 = unkFlag1Check.IsChecked.GetValueOrDefault(false);
        }

        private void PedPropFlag4_Checked(object sender, RoutedEventArgs e)
        {
            if (_selectedCloth != null)
                _selectedCloth.pedPropFlags.unkFlag4 = unkFlag1Check.IsChecked.GetValueOrDefault(false);
        }

        private void PedPropFlag5_Checked(object sender, RoutedEventArgs e)
        {
            if (_selectedCloth != null)
                _selectedCloth.pedPropFlags.unkFlag5 = unkFlag1Check.IsChecked.GetValueOrDefault(false);
        }
    }
}
