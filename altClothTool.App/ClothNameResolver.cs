using System;
using System.IO;

namespace altClothTool.App
{
    public class ClothNameResolver
    {
        public enum ClothTypes
        {
            Component,
            PedProp
        }

        public enum DrawableTypes
        {
            Head,
            Mask,
            Hair,
            Body,
            Legs,
            Bag,
            Shoes,
            Accessories,
            Undershirt,
            Armor,
            Decal,
            Top,
            PropHead,
            PropEyes,
            PropEars,
            PropMouth,
            PropLHand,
            PropRHand,
            PropLWrist,
            PropRWrist,
            PropHip,
            PropLFoot,
            PropRFoot,
            PropUnk1,
            PropUnk2,
            COUNT
        }

        public ClothTypes ClothType { get; }
        public DrawableTypes DrawableType { get; }
        public string BindedNumber { get; }
        public string Postfix { get; } = "";
        public bool IsVariation { get; }

        public ClothNameResolver(string filename)
        {
            string[] parts = Path.GetFileNameWithoutExtension(filename).Split('_');
            if (parts.Length < 3)
                throw new Exception("Wrong drawable name");

            if(parts[0].ToLower() == "p")
            {
                ClothType = ClothTypes.PedProp;

                string drName = parts[1].ToLower();
                switch(drName)
                {
                    case "head": DrawableType = DrawableTypes.PropHead; break;
                    case "eyes": DrawableType = DrawableTypes.PropEyes; break;
                    case "ears": DrawableType = DrawableTypes.PropEars; break;
                    case "mouth": DrawableType = DrawableTypes.PropMouth; break;
                    case "lhand": DrawableType = DrawableTypes.PropLHand; break;
                    case "rhand": DrawableType = DrawableTypes.PropRHand; break;
                    case "lwrist": DrawableType = DrawableTypes.PropLWrist; break;
                    case "rwrist": DrawableType = DrawableTypes.PropRWrist; break;
                    case "hip": DrawableType = DrawableTypes.PropHip; break;
                    case "lfoot": DrawableType = DrawableTypes.PropLFoot; break;
                    case "rfoot": DrawableType = DrawableTypes.PropRFoot; break;
                    case "unk1": DrawableType = DrawableTypes.PropUnk1; break;
                    case "unk2": DrawableType = DrawableTypes.PropUnk2; break;
                    default: break;
                }

                BindedNumber = parts[2];
            }
            else
            {
                ClothType = ClothTypes.Component;

                string drName = parts[0].ToLower();
                switch(drName)
                {
                    case "head": DrawableType = DrawableTypes.Head; break;
                    case "berd": DrawableType = DrawableTypes.Mask; break;
                    case "hair": DrawableType = DrawableTypes.Hair; break;
                    case "uppr": DrawableType = DrawableTypes.Body; break;
                    case "lowr": DrawableType = DrawableTypes.Legs; break;
                    case "hand": DrawableType = DrawableTypes.Bag; break;
                    case "feet": DrawableType = DrawableTypes.Shoes; break;
                    case "teef": DrawableType = DrawableTypes.Accessories; break;
                    case "accs": DrawableType = DrawableTypes.Undershirt; break;
                    case "task": DrawableType = DrawableTypes.Armor; break;
                    case "decl": DrawableType = DrawableTypes.Decal; break;
                    case "jbib": DrawableType = DrawableTypes.Top; break;
                    default: break;
                }

                BindedNumber = parts[1];
                Postfix = parts[2].ToLower();
                if (parts.Length > 3)
                    IsVariation = true;
            }
        }

        public override string ToString()
        {
            return ClothType + " " + DrawableType + " " + BindedNumber;
        }

        public static string DrawableTypeToString(DrawableTypes types)
        {
            switch(types)
            {
                case DrawableTypes.Head: return "head";
                case DrawableTypes.Mask: return "berd";
                case DrawableTypes.Hair: return "hair";
                case DrawableTypes.Body: return "uppr";
                case DrawableTypes.Legs: return "lowr";
                case DrawableTypes.Bag: return "hand";
                case DrawableTypes.Shoes: return "feet";
                case DrawableTypes.Accessories: return "teef";
                case DrawableTypes.Undershirt: return "accs";
                case DrawableTypes.Armor: return "task";
                case DrawableTypes.Decal: return "decl";
                case DrawableTypes.Top: return "jbib";
                case DrawableTypes.PropHead: return "p_head";
                case DrawableTypes.PropEyes: return "p_eyes";
                case DrawableTypes.PropEars: return "p_ears";
                case DrawableTypes.PropMouth: return "p_mouth";
                case DrawableTypes.PropLHand: return "p_lhand";
                case DrawableTypes.PropRHand: return "p_rhand";
                case DrawableTypes.PropLWrist: return "p_lwrist";
                case DrawableTypes.PropRWrist: return "p_rwrist";
                case DrawableTypes.PropHip: return "p_hip";
                case DrawableTypes.PropLFoot: return "p_lfoot";
                case DrawableTypes.PropRFoot: return "p_rfoot";
                case DrawableTypes.PropUnk1: return "p_unk1";
                case DrawableTypes.PropUnk2: return "p_unk2";
                default: return "";
            }
        }
    }
}
