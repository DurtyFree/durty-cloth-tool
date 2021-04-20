using System.Collections.Generic;
using System.IO;
using altClothTool.App.Builders.Base;

namespace altClothTool.App.Builders
{
    internal class FivemResourceBuilder
        : MultiplayerResourceBuilderBase
    {
        private readonly List<string> _resourceLuaMetas = new List<string>();
        
        #region Resource Props 
        
        protected override void OnFirstPropAddedToResource(string outputFolder, int sexNr, string collectionName)
        {
            Directory.CreateDirectory($"{outputFolder}\\stream");
            Directory.CreateDirectory($"{outputFolder}\\stream\\{Prefixes[sexNr]}freemode_01_p_{Prefixes[sexNr]}{collectionName}");
        }
        
        protected override void CopyPropTextureToResource(string propTextureFilePath, int sexNr, string outputFolder, string collectionName,
            string componentNumerics, string prefix, char offsetLetter)
        {
            string targetFilePath = $"{outputFolder}\\stream\\{Prefixes[sexNr]}freemode_01_p_{Prefixes[sexNr]}{collectionName}\\{Prefixes[sexNr]}freemode_01_p_{Prefixes[sexNr]}{collectionName}^{prefix}_diff_{componentNumerics}_{offsetLetter}.ytd";
            File.Copy(propTextureFilePath, targetFilePath, true);
        }

        protected override void CopyPropModelToResource(ClothData propClothData, int sexNr, string outputFolder, string collectionName,
            string componentNumerics, string prefix)
        {
            string targetFilePath = $"{outputFolder}\\stream\\{Prefixes[sexNr]}freemode_01_p_{Prefixes[sexNr]}{collectionName}\\{Prefixes[sexNr]}freemode_01_p_{Prefixes[sexNr]}{collectionName}^{prefix}_{componentNumerics}.ydd";
            File.Copy(propClothData.MainPath, targetFilePath, true);
        }

        #endregion

        #region Resource Clothes
        
        protected override string GetClothYmtFilePath(string outputFolder, int sexNr, string collectionName)
        {
            return $"{outputFolder}\\stream\\{Prefixes[sexNr]}freemode_01_{Prefixes[sexNr]}{collectionName}.ymt";
        }

        protected override void OnFirstClothAddedToResource(string outputFolder, int sexNr, string collectionName)
        {
            Directory.CreateDirectory(outputFolder + "\\stream");
            Directory.CreateDirectory(outputFolder + "\\stream\\" + Prefixes[sexNr] + "freemode_01_" + Prefixes[sexNr] + collectionName);
        }

        protected override void CopyClothFirstPersonModelToResource(string clothDataFirstPersonModelPath, int sexNr, string outputFolder,
            string collectionName, string componentNumerics, string prefix, string yddPostfix)
        {
            string targetFilePath = $"{outputFolder}\\stream\\{Prefixes[sexNr]}freemode_01_{Prefixes[sexNr]}{collectionName}\\{Prefixes[sexNr]}freemode_01_{Prefixes[sexNr]}{collectionName}^{prefix}_{componentNumerics}_{yddPostfix}_1.ydd";
            File.Copy(clothDataFirstPersonModelPath, targetFilePath, true);
        }

        protected override void CopyClothTextureToResource(string clothTextureFilePath, int sexNr, string outputFolder, string collectionName,
            string componentNumerics, string prefix, string ytdPostfix, char offsetLetter)
        {
            string targetFilePath = $"{outputFolder}\\stream\\{Prefixes[sexNr]}freemode_01_{Prefixes[sexNr]}{collectionName}\\{Prefixes[sexNr]}freemode_01_{Prefixes[sexNr]}{collectionName}^{prefix}_diff_{componentNumerics}_{offsetLetter}_{ytdPostfix}.ytd";
            File.Copy(clothTextureFilePath, targetFilePath, true);
        }

        protected override void CopyClothModelToResource(ClothData clothData, int sexNr, string outputFolder, string collectionName,
            string componentNumerics, string prefix, string yddPostfix)
        {
            string targetFilePath = $"{outputFolder}\\stream\\{Prefixes[sexNr]}freemode_01_{Prefixes[sexNr]}{collectionName}\\{Prefixes[sexNr]}freemode_01_{Prefixes[sexNr]}{collectionName}^{prefix}_{componentNumerics}_{yddPostfix}.ydd";
            File.Copy(clothData.MainPath, targetFilePath, true);
        }

        #endregion
        
        protected override void OnResourceBuildingFinished(string outputFolder)
        {
            File.WriteAllText(outputFolder + "\\fxmanifest.lua", GenerateFiveMResourceLuaContent(_resourceLuaMetas));
        }

        protected override void OnResourceClothDataFinished(string outputFolder, int sexNr, string collectionName, bool isAnyPropAdded, bool isAnyClothAdded)
        {
            if (!isAnyClothAdded && !isAnyPropAdded) 
                return;

            string shopMetaFilePath = $"{outputFolder}\\{Prefixes[sexNr]}freemode_01_{Prefixes[sexNr]}{collectionName}.meta";
            File.WriteAllText(shopMetaFilePath, GenerateShopMetaContent((ClothData.Sex)sexNr, collectionName));

            _resourceLuaMetas.Add(Prefixes[sexNr] + "freemode_01_" + Prefixes[sexNr] + collectionName + ".meta");
        }

        private string GenerateFiveMResourceLuaContent(List<string> metas)
        {
            string filesText = "";
            for (int i = 0; i < metas.Count; ++i)
            {
                if (i != 0)
                    filesText += ",\n";
                filesText += "  '" + metas[i] + "'";
            }

            string metasText = "";
            for (int i = 0; i < metas.Count; ++i)
            {
                if (i != 0)
                    metasText += "\n";
                metasText += "data_file 'SHOP_PED_APPAREL_META_FILE' '" + metas[i] + "'";
            }

            string manifestContent = "-- Generated with AltTool\n\n";
            manifestContent += "fx_version 'cerulean'\n";
            manifestContent += "game { 'gta5' }\n\n";
            manifestContent += $"files {{\n{filesText}\n}}\n\n{metasText}";
            return manifestContent;
        }
    }
}
