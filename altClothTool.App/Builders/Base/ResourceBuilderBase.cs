using System.Collections.Generic;
using altClothTool.App.Contracts;
using RageLib.GTA5.ResourceWrappers.PC.Meta.Structures;
using RageLib.Resources.GTA5.PC.GameFiles;
using RageLib.Resources.GTA5.PC.Meta;

namespace altClothTool.App.Builders.Base
{
    public abstract class ResourceBuilderBase
        : IClothesResourceBuilder
    {
        protected readonly string[] Prefixes = { "mp_m_", "mp_f_" };
        protected readonly string[] FolderNames = { "ped_male", "ped_female" };

        protected string GenerateShopMetaContent(ClothData.Sex targetSex, string collectionName)
        {
            string targetName = targetSex == ClothData.Sex.Male ? "mp_m_freemode_01" : "mp_f_freemode_01";
            string dlcName = (targetSex == ClothData.Sex.Male ? "mp_m_" : "mp_f_") + collectionName;
            string character = targetSex == ClothData.Sex.Male ? "SCR_CHAR_MULTIPLAYER" : "SCR_CHAR_MULTIPLAYER_F";
            return $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<ShopPedApparel>
	<pedName>{targetName}</pedName>
	<dlcName>{dlcName}</dlcName>
	<fullDlcName>{targetName}_{dlcName}</fullDlcName>
	<eCharacter>{character}</eCharacter>
	<creatureMetaData>MP_CreatureMetadata_{collectionName}</creatureMetaData>
	<pedOutfits>
	</pedOutfits>
	<pedComponents>
	</pedComponents>
	<pedProps>
	</pedProps>
</ShopPedApparel>";
        }

        protected void UpdateYmtComponentTextureBindings(MUnk_3538495220[] componentTextureBindings, YmtPedDefinitionFile ymt)
        {
            int arrIndex = 0;
            for (int i = 0; i < componentTextureBindings.Length; ++i)
            {
                if (componentTextureBindings[i] != null)
                {
                    byte id = (byte) arrIndex++;
                    ymt.Unk_376833625.Unk_2996560424.SetByte(i, id);
                }

                ymt.Unk_376833625.Components[(Unk_884254308) i] = componentTextureBindings[i];
            }
        }

        private List<MUnk_254518642> GetTexDataForCloth(ClothData clothData)
        {
            List<MUnk_254518642> items = new List<MUnk_254518642>();
            for (int i = 0; i < clothData.Textures.Count; i++)
            {
                byte texId = GetTexIdByDrawableType(clothData, i);
                MUnk_254518642 texture = new MUnk_254518642
                {
                    TexId = texId
                };
                items.Add(texture);
            }
            return items;
        }

        /// TODO DURTY: verify if its really based on index? Shouldnt be like this actually, because its connected to skin tone or something
        private static byte GetTexIdByDrawableType(ClothData clothData, int index)
        {
            byte texId = (byte) index;
            switch (clothData.DrawableType)
            {
                case ClothNameResolver.DrawableTypes.Legs:
                    texId = 1;
                    break;
                case ClothNameResolver.DrawableTypes.Shoes:
                    texId = 0;
                    break;
            }
            return texId;
        }

        protected MUnk_94549140 GenerateYmtPedPropItem(YmtPedDefinitionFile ymt, Unk_2834549053 anchor, ClothData clothData)
        {
            var item = new MUnk_94549140(ymt.Unk_376833625.PropInfo)
            {
                AnchorId = (byte) anchor,
                TexData = GetTexDataForCloth(clothData)
            };
            
            // Get or create linked anchor
            var anchorProps = ymt.Unk_376833625.PropInfo.AAnchors.Find(e => e.Anchor == anchor);

            if (anchorProps == null)
            {
                anchorProps = new MCAnchorProps(ymt.Unk_376833625.PropInfo)
                {
                    Anchor = anchor,
                    PropsMap = {[item] = (byte) item.TexData.Count}
                };

                ymt.Unk_376833625.PropInfo.AAnchors.Add(anchorProps);
            }
            else
            {
                anchorProps.PropsMap[item] = (byte) item.TexData.Count;
            }
            return item;
        }
        
        protected void GetClothPostfixes(ClothData clothData, out string ytdPostfix, out string yddPostfix)
        {
            yddPostfix = clothData.MainPath.EndsWith("_u.ydd") ? "u" : "r";
            ytdPostfix = clothData.MainPath.EndsWith("_u.ydd") ? "uni" : "whi";

            switch (clothData.DrawableType)
            {
                case ClothNameResolver.DrawableTypes.Legs:
                    yddPostfix = "r";
                    ytdPostfix = "whi";
                    break;
                case ClothNameResolver.DrawableTypes.Mask:
                    yddPostfix = "r";
                    ytdPostfix = "whi";
                    break;
                case ClothNameResolver.DrawableTypes.Shoes:
                    yddPostfix = "r";
                    ytdPostfix = "uni";
                    break;
            }
        }

        protected MCComponentInfo GenerateYmtPedComponentItem(ClothData clothData, ref MUnk_3538495220[] componentTextureBindings)
        {
            byte componentTypeId = clothData.GetComponentTypeId();
            if (componentTextureBindings[componentTypeId] == null)
                componentTextureBindings[componentTypeId] = new MUnk_3538495220();

            byte nextPropMask = 17;
            switch (componentTypeId)
            {
                case 2:
                case 7:
                    nextPropMask = 11;
                    break;
                case 5:
                case 8:
                    nextPropMask = 65;
                    break;
                case 9:
                    nextPropMask = 1;
                    break;
                case 10:
                    nextPropMask = 5;
                    break;
                case 11:
                    nextPropMask = 1;
                    break;
                default:
                    break;
            }

            MUnk_1535046754 textureDescription = new MUnk_1535046754
            {
                PropMask = nextPropMask,
                Unk_2806194106 = (byte) (clothData.FirstPersonModelPath != "" ? 1 : 0),
                ClothData =
                {
                    Unk_2828247905 = 0
                }
            };

            byte texId = (byte) (clothData.MainPath.EndsWith("_u.ydd") ? 0 : 1);

            if (clothData.DrawableType == ClothNameResolver.DrawableTypes.Mask)
            {
                texId = 1;
            }

            foreach (string texPath in clothData.Textures)
            {
                MUnk_1036962405 texInfo = new MUnk_1036962405
                {
                    Distribution = 255,
                    TexId = texId
                };
                textureDescription.ATexData.Add(texInfo);
            }

            componentTextureBindings[componentTypeId].Unk_1756136273.Add(textureDescription);

            byte componentTextureLocalId = (byte) (componentTextureBindings[componentTypeId].Unk_1756136273.Count - 1);

            return new MCComponentInfo
            {
                Unk_802196719 = 0,
                Unk_4233133352 = 0,
                Unk_128864925 =
                {
                    b0 = (byte) (clothData.PedComponentFlags.unkFlag1 ? 1 : 0),
                    b1 = (byte) (clothData.PedComponentFlags.unkFlag2 ? 1 : 0),
                    b2 = (byte) (clothData.PedComponentFlags.unkFlag3 ? 1 : 0),
                    b3 = (byte) (clothData.PedComponentFlags.unkFlag4 ? 1 : 0),
                    b4 = (byte) (clothData.PedComponentFlags.isHighHeels ? 1 : 0)
                },
                Flags = 0,
                Inclusions = 0,
                Exclusions = 0,
                Unk_1613922652 = 0,
                Unk_2114993291 = 0,
                Unk_3509540765 = componentTypeId,
                Unk_4196345791 = componentTextureLocalId
            };
        }

        protected YmtPedDefinitionFile CreateYmtPedDefinitionFile(string ymtName, out MUnk_3538495220[] componentTextureBindings, out int[] componentIndexes, out int[] propIndexes)
        {
            //Male YMT generating
            YmtPedDefinitionFile ymt = new YmtPedDefinitionFile
            {
                metaYmtName = ymtName,
                Unk_376833625 = {DlcName = RageLib.Hash.Jenkins.Hash(ymtName)}
            };
                    
            componentTextureBindings = new MUnk_3538495220[]{ null, null, null, null, null, null, null, null, null, null, null, null };
            componentIndexes = new []{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            propIndexes = new []{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            //ymt.Unk_376833625.Unk_1235281004 = 0;
            //ymt.Unk_376833625.Unk_4086467184 = 0;
            //ymt.Unk_376833625.Unk_911147899 = 0;
            //ymt.Unk_376833625.Unk_315291935 = 0;
            //ymt.Unk_376833625.Unk_2996560424 = ;

            return ymt;
        }

        public abstract void BuildResource(string outputFolder, string collectionName);
    }
}
