using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AltTool
{
    class ProjectController
    {

        static ProjectController singleton = null;
        public static ProjectController Instance()
        {
            if(singleton == null)
                singleton = new ProjectController();
            return singleton;
        }

        public void AddFiles(ClothData.Sex targetSex)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckFileExists = true;
            openFileDialog.Filter = "Clothes geometry (*.ydd)|*.ydd";
            openFileDialog.FilterIndex = 1;
            openFileDialog.DefaultExt = "ydd";
            openFileDialog.Multiselect = true;
            openFileDialog.Title = "Adding " + ((targetSex == ClothData.Sex.Male) ? "male" : "female") + " clothes";
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    string baseFileName = Path.GetFileName(filename);
                    ClothNameResolver cData = new ClothNameResolver(baseFileName);

                    if(!cData.isVariation)
                    {
                        ClothData nextCloth = new ClothData(filename, cData.clothType, cData.drawableType, cData.bindedNumber, cData.postfix, targetSex);
                        
                        if(cData.clothType == ClothNameResolver.Type.Component)
                        {
                            nextCloth.SearchForFPModel();
                            nextCloth.SearchForTextures();

                            var _clothes = MainWindow.clothes.ToList();
                            _clothes.Add(nextCloth);
                            _clothes = _clothes.OrderBy(x => x.Name, new AlphanumericComparer()).ToList();
                            MainWindow.clothes.Clear();

                            foreach(var cloth in _clothes)
                            {
                                MainWindow.clothes.Add(cloth);
                            }

                            StatusController.SetStatus(nextCloth.ToString() + " added (FP model found: " + (nextCloth.fpModelPath != "" ? "Yes" : "No") + ", Textures: " + (nextCloth.textures.Count) + "). Total: " + MainWindow.clothes.Count);
                        }
                        else
                        {
                            nextCloth.SearchForTextures();

                            var _clothes = MainWindow.clothes.ToList();
                            _clothes.Add(nextCloth);
                            _clothes = _clothes.OrderBy(x => x.Name, new AlphanumericComparer()).ToList();
                            MainWindow.clothes.Clear();

                            foreach (var cloth in _clothes)
                            {
                                MainWindow.clothes.Add(cloth);
                            }

                            StatusController.SetStatus(nextCloth.ToString() + " added, Textures: " + (nextCloth.textures.Count) + "). Total: " + MainWindow.clothes.Count);
                        }
                    }
                    else
                        StatusController.SetStatus("Item " + baseFileName + " can't be added. Looks like it's variant of another item");
                }
            }
        }

        public void AddTexture(ClothData cloth)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckFileExists = true;
            openFileDialog.Filter = "Clothes texture (*.ytd)|*.ytd";
            openFileDialog.FilterIndex = 1;
            openFileDialog.DefaultExt = "ytd";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    if(filename.EndsWith(".ytd"))
                    {
                        cloth.AddTexture(filename);
                    }
                }
            }
        }

        public void SetFPModel(ClothData cloth)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckFileExists = true;
            openFileDialog.Filter = "Clothes drawable (*.ydd)|*.ydd";
            openFileDialog.FilterIndex = 1;
            openFileDialog.DefaultExt = "ydd";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    if (filename.EndsWith(".ydd"))
                    {
                        cloth.SetFPModel(filename);
                    }
                }
            }
        }
    }
}
