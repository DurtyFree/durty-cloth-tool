using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AltTool
{
    public class ClothData: INotifyPropertyChanged
    {
        public ClothNameResolver.Type clothType;
        public ClothNameResolver.DrawableType drawableType;

        public struct ComponentFlags
        {
            public bool unkFlag1;
            public bool unkFlag2;
            public bool unkFlag3;
            public bool unkFlag4;
            public bool isHighHeels;
        }

        public struct PedPropFlags
        {
            public bool unkFlag1;
            public bool unkFlag2;
            public bool unkFlag3;
            public bool unkFlag4;
            public bool unkFlag5;
        }

        public enum Sex
        {
            Male,
            Female
        }

        static int[] idsOffset = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        static char offsetLetter = 'a';
        static string[] sexIcons = { "👨🏻", "👩🏻" };
        static string[] typeIcons = { "🧥", "👓" };

        public string mainPath = "";
        string origNumerics = "";
        string postfix = "";

        public ComponentFlags componentFlags;
        public PedPropFlags pedPropFlags;

        public string fpModelPath;
        public ObservableCollection<string> textures = new ObservableCollection<string>();

        public Sex targetSex;

        public string name = "";

        public string Icon
        {
            get
            {
                return sexIcons[(int)targetSex];
            }
        }

        public string Type
        {
            get
            {
                return typeIcons[(int)clothType];
            }
        }

        private int _currentComponentIndex;
        public int CurrentComponentIndex
        {
            get => _currentComponentIndex;
            set
            {
                _currentComponentIndex = value;
                OnPropertyChanged("DisplayName");
            }
        }

        private string _componentNumerics;
        public string ComponentNumerics
        {
            get => _componentNumerics;
            set
            {
                _componentNumerics = value;
                OnPropertyChanged("DisplayName");
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged("DisplayName");
            }
        }

        public string DisplayName => $"{name} (ID: {_currentComponentIndex}) ({_componentNumerics})";

        public ClothData()
        {

        }

        public ClothData(string path, ClothNameResolver.Type _type, ClothNameResolver.DrawableType _drawableType, string numeric, string _postfix, Sex sex)
        {
            if (!File.Exists(path))
                throw new Exception("YDD file not found");

            clothType = _type;
            drawableType = _drawableType;
            origNumerics = numeric;

            targetSex = sex;
            postfix = _postfix;

            name = drawableType.ToString() + "_" + origNumerics;

            mainPath = path;
        }

        public void SearchForFPModel()
        {
            string rootPath = Path.GetDirectoryName(mainPath);
            string fileName = Path.GetFileNameWithoutExtension(mainPath);
            string relPath = rootPath + "\\" + fileName + "_1.ydd";
            if (File.Exists(relPath))
                fpModelPath = relPath;
            else
                fpModelPath = "";
        }

        public void SetFPModel(string path)
        {
            fpModelPath = path;
        }

        public void SearchForTextures()
        {
            textures.Clear();
            string rootPath = Path.GetDirectoryName(mainPath);

            if(IsComponent())
            {
                for (int i = 0; ; ++i)
                {
                    string relPath = rootPath + "\\" + ClothNameResolver.DrawableTypeToString(drawableType) + "_diff_" + origNumerics + "_" + (char)(offsetLetter + i) + "_uni.ytd";
                    if (!File.Exists(relPath))
                        break;
                    textures.Add(relPath);
                }
                for (int i = 0; ; ++i)
                {
                    string relPath = rootPath + "\\" + ClothNameResolver.DrawableTypeToString(drawableType) + "_diff_" + origNumerics + "_" + (char)(offsetLetter + i) + "_whi.ytd";
                    if (!File.Exists(relPath))
                        break;
                    textures.Add(relPath);
                }
            }
            else
            {
                for (int i = 0; ; ++i)
                {
                    string relPath = rootPath + "\\" + ClothNameResolver.DrawableTypeToString(drawableType) + "_diff_" + origNumerics + "_" + (char)(offsetLetter + i) + ".ytd";
                    if (!File.Exists(relPath))
                        break;
                    textures.Add(relPath);
                }
            }
        }

        public void AddTexture(string path)
        {
            if(!textures.Contains(path))
                textures.Add(path);
        }

        public override string ToString()
        {
            return sexIcons[(int)targetSex] + " " + name;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public bool IsComponent()
        {
            if (drawableType <= ClothNameResolver.DrawableType.Top)
                return true;
            return false;
        }

        public byte GetComponentTypeID()
        {
            if(IsComponent())
                return (byte)drawableType;
            return 255;
        }

        public bool IsPedProp()
        {
            return !IsComponent();
        }

        public byte GetPedPropTypeID()
        {
            if (IsPedProp())
                return (byte)((int)drawableType - (int)ClothNameResolver.DrawableType.PropHead);
            return 255;
        }

        public string GetPrefix()
        {
            return ClothNameResolver.DrawableTypeToString(drawableType);
        }

        public void SetComponentNumerics(string componentNumerics, int currentComponentIndex)
        {
            ComponentNumerics = componentNumerics;
            CurrentComponentIndex = currentComponentIndex;
        }
    }
}
