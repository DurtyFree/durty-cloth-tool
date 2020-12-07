using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace AltTool
{
    public class ClothData
        : INotifyPropertyChanged
    {
        public readonly ClothNameResolver.ClothTypes ClothType;
        public readonly ClothNameResolver.DrawableTypes DrawableType;

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

        private static int[] _idsOffset = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private static char _offsetLetter = 'a';
        private static readonly string[] SexIcons = { "👨🏻", "👩🏻" };
        private static readonly string[] TypeIcons = { "🧥", "👓" };
        private string _origNumerics = "";
        private string _postfix = "";

        public string MainPath = "";
        public ComponentFlags componentFlags;
        public PedPropFlags pedPropFlags;
        public string FpModelPath;
        public ObservableCollection<string> Textures = new ObservableCollection<string>();
        public Sex TargetSex;
        public string Icon => SexIcons[(int)TargetSex];
        public string Type => TypeIcons[(int)ClothType];

        private int _currentComponentIndex;
        private int CurrentComponentIndex
        {
            get => _currentComponentIndex;
            set
            {
                _currentComponentIndex = value;
                OnPropertyChanged("DisplayName");
            }
        }

        private string _componentNumerics;
        private string ComponentNumerics
        {
            get => _componentNumerics;
            set
            {
                _componentNumerics = value;
                OnPropertyChanged("DisplayName");
            }
        }

        private string _name = "";
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged("DisplayName");
            }
        }

        public string DisplayName => $"{_name} (ID: {_currentComponentIndex}) ({_componentNumerics})";

        public ClothData()
        { }

        public ClothData(string path, ClothNameResolver.ClothTypes clothType, ClothNameResolver.DrawableTypes drawableType, string numeric, string postfix, Sex sex)
        {
            if (!File.Exists(path))
                throw new Exception("YDD file not found");
            
            _origNumerics = numeric;
            _postfix = postfix;

            ClothType = clothType;
            DrawableType = drawableType;
            Name = DrawableType + "_" + _origNumerics;
            TargetSex = sex;
            MainPath = path;
        }

        public void SearchForFPModel()
        {
            string rootPath = Path.GetDirectoryName(MainPath);
            string fileName = Path.GetFileNameWithoutExtension(MainPath);
            string relPath = rootPath + "\\" + fileName + "_1.ydd";
            FpModelPath = File.Exists(relPath) ? relPath : "";
        }

        public void SetFPModel(string path)
        {
            FpModelPath = path;
        }

        public void SearchForTextures()
        {
            Textures.Clear();
            string rootPath = Path.GetDirectoryName(MainPath);

            if(IsComponent())
            {
                for (int i = 0; ; ++i)
                {
                    string relPath = rootPath + "\\" + ClothNameResolver.DrawableTypeToString(DrawableType) + "_diff_" + _origNumerics + "_" + (char)(_offsetLetter + i) + "_uni.ytd";
                    if (!File.Exists(relPath))
                        break;
                    Textures.Add(relPath);
                }
                for (int i = 0; ; ++i)
                {
                    string relPath = rootPath + "\\" + ClothNameResolver.DrawableTypeToString(DrawableType) + "_diff_" + _origNumerics + "_" + (char)(_offsetLetter + i) + "_whi.ytd";
                    if (!File.Exists(relPath))
                        break;
                    Textures.Add(relPath);
                }
            }
            else
            {
                for (int i = 0; ; ++i)
                {
                    string relPath = rootPath + "\\" + ClothNameResolver.DrawableTypeToString(DrawableType) + "_diff_" + _origNumerics + "_" + (char)(_offsetLetter + i) + ".ytd";
                    if (!File.Exists(relPath))
                        break;
                    Textures.Add(relPath);
                }
            }
        }

        public void AddTexture(string path)
        {
            if(!Textures.Contains(path))
                Textures.Add(path);
        }

        public override string ToString()
        {
            return SexIcons[(int)TargetSex] + " " + _name;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool IsComponent()
        {
            if (DrawableType <= ClothNameResolver.DrawableTypes.Top)
                return true;
            return false;
        }

        public byte GetComponentTypeID()
        {
            if(IsComponent())
                return (byte)DrawableType;
            return 255;
        }

        public bool IsPedProp()
        {
            return !IsComponent();
        }

        public byte GetPedPropTypeID()
        {
            if (IsPedProp())
                return (byte)((int)DrawableType - (int)ClothNameResolver.DrawableTypes.PropHead);
            return 255;
        }

        public string GetPrefix()
        {
            return ClothNameResolver.DrawableTypeToString(DrawableType);
        }

        public void SetComponentNumerics(string componentNumerics, int currentComponentIndex)
        {
            ComponentNumerics = componentNumerics;
            CurrentComponentIndex = currentComponentIndex;
        }
    }
}
