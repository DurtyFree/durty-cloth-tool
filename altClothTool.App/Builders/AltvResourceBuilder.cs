using System.Collections.Generic;
using System.IO;
using altClothTool.App.Builders.Base;

namespace altClothTool.App.Builders
{
    internal class AltvResourceBuilder
        : MultiplayerResourceBuilderBase
    {
        private readonly List<string> _streamCfgMetas = new List<string>();
        private readonly List<string> _streamCfgIncludes = new List<string>();
        
        #region Resource Props

        protected override void OnFirstPropAddedToResource(string outputFolder, int sexNr, string collectionName)
        {
            Directory.CreateDirectory($"{outputFolder}\\stream");
            Directory.CreateDirectory($"{outputFolder}\\stream\\{FolderNames[sexNr]}_p.rpf");
            Directory.CreateDirectory($"{outputFolder}\\stream\\{FolderNames[sexNr]}_p.rpf\\{Prefixes[sexNr]}freemode_01_p_{Prefixes[sexNr]}{collectionName}");
        }

        protected override void CopyPropTextureToResource(string propTextureFilePath, int sexNr, string outputFolder, string collectionName,
            string componentNumerics, string prefix, char offsetLetter)
        {
            string targetFilePath = $"{outputFolder}\\stream\\{FolderNames[sexNr]}_p.rpf\\{Prefixes[sexNr]}freemode_01_p_{Prefixes[sexNr]}{collectionName}\\{prefix}_diff_{componentNumerics}_{offsetLetter}.ytd";
            File.Copy(propTextureFilePath, targetFilePath, true);
        }

        protected override void CopyPropModelToResource(ClothData propClothData, int sexNr, string outputFolder, string collectionName,
            string componentNumerics, string prefix)
        {
            string targetFilePath = $"{outputFolder}\\stream\\{FolderNames[sexNr]}_p.rpf\\{Prefixes[sexNr]}freemode_01_p_{Prefixes[sexNr]}{collectionName}\\{prefix}_{componentNumerics}.ydd";
            File.Copy(propClothData.MainPath, targetFilePath, true);
        }

        #endregion

        #region Resource Clothes

        protected override string GetClothYmtFilePath(string outputFolder, int sexNr, string collectionName)
        {
            return $"{outputFolder}\\stream\\{FolderNames[sexNr]}.rpf\\{Prefixes[sexNr]}freemode_01_{Prefixes[sexNr]}{collectionName}.ymt";
        }

        protected override void OnFirstClothAddedToResource(string outputFolder, int sexNr, string collectionName)
        {
            Directory.CreateDirectory($"{outputFolder}\\stream");
            Directory.CreateDirectory($"{outputFolder}\\stream\\{FolderNames[sexNr]}.rpf");
            Directory.CreateDirectory($"{outputFolder}\\stream\\{FolderNames[sexNr]}.rpf\\{Prefixes[sexNr]}freemode_01_{Prefixes[sexNr]}{collectionName}");
        }

        protected override void CopyClothFirstPersonModelToResource(string clothDataFirstPersonModelPath, int sexNr, string outputFolder,
            string collectionName, string componentNumerics, string prefix, string yddPostfix)
        {
            string targetFilePath = $"{outputFolder}\\stream\\{FolderNames[sexNr]}.rpf\\{Prefixes[sexNr]}freemode_01_{Prefixes[sexNr]}{collectionName}\\{prefix}_{componentNumerics}_{yddPostfix}_1.ydd";
            File.Copy(clothDataFirstPersonModelPath, targetFilePath);
        }

        protected override void CopyClothTextureToResource(string clothTextureFilePath, int sexNr, string outputFolder, string collectionName,
            string componentNumerics, string prefix, string ytdPostfix, char offsetLetter)
        {
            string targetFilePath = $"{outputFolder}\\stream\\{FolderNames[sexNr]}.rpf\\{Prefixes[sexNr]}freemode_01_{Prefixes[sexNr]}{collectionName}\\{prefix}_diff_{componentNumerics}_{offsetLetter}_{ytdPostfix}.ytd";
            File.Copy(clothTextureFilePath, targetFilePath);
        }

        protected override void CopyClothModelToResource(ClothData clothData, int sexNr, string outputFolder, string collectionName,
            string componentNumerics, string prefix, string yddPostfix)
        {
            string targetFilePath = $"{outputFolder}\\stream\\{FolderNames[sexNr]}.rpf\\{Prefixes[sexNr]}freemode_01_{Prefixes[sexNr]}{collectionName}\\{prefix}_{componentNumerics}_{yddPostfix}.ydd";
            File.Copy(clothData.MainPath, targetFilePath);
        }

        #endregion
        
        protected override void OnResourceClothDataFinished(string outputFolder, int sexNr, string collectionName, bool isAnyClothAdded, bool isAnyPropAdded)
        {
            if (isAnyPropAdded)
            {
                _streamCfgIncludes.Add($"stream/{FolderNames[sexNr]}_p.rpf/*");
            }

            if (isAnyClothAdded || isAnyPropAdded)
            {
                string shopMetaFilePath = $"{outputFolder}\\stream\\{Prefixes[sexNr]}freemode_01_{Prefixes[sexNr]}{collectionName}.meta";
                File.WriteAllText(shopMetaFilePath, GenerateShopMetaContent((ClothData.Sex)sexNr, collectionName));
                
                _streamCfgMetas.Add($"stream/{Prefixes[sexNr]}freemode_01_{Prefixes[sexNr]}{collectionName}.meta: SHOP_PED_APPAREL_META_FILE");
                _streamCfgIncludes.Add($"stream/{FolderNames[sexNr]}.rpf/*");
            }
        }

        protected override void OnResourceBuildingFinished(string outputFolder)
        {
            File.WriteAllText($"{outputFolder}\\stream.cfg", GenerateAltvStreamCfgContent(_streamCfgIncludes, _streamCfgMetas));
            File.WriteAllText($"{outputFolder}\\resource.cfg", GenerateAltvResourceCfgContent());
        }

        private string GenerateAltvStreamCfgContent(List<string> files, List<string> metas)
        {
            string filesText = "";
            for(int i = 0; i < files.Count; ++i)
            {
                if (i != 0)
                    filesText += "\n";
                filesText += "  " + files[i];
            }

            string metasText = "";
            for(int i = 0; i < metas.Count; ++i)
            {
                if (i != 0)
                    metasText += "\n";
                metasText += "  " + metas[i];
            }

            return $"files: [\n{filesText}\n]\nmeta: {{\n{metasText}\n}}\n";
        }

        private string GenerateAltvResourceCfgContent()
        {
            return "type: dlc,\nmain: stream.cfg,\nclient-files: [\n  stream/*\n]\n";
        }
    }
}
