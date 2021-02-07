using System.Collections.Generic;
using RageLib.GTA5.ResourceWrappers.PC.Meta.Structures;
using RageLib.Resources.GTA5.PC.GameFiles;
using RageLib.Resources.GTA5.PC.Meta;

namespace altClothTool.App.Builders.Base
{
    internal abstract class MultiplayerResourceBuilderBase
        : ResourceBuilderBase
    {
        public override void BuildResource(string outputFolder, string collectionName)
        {
            OnResourceBuildingStarted(outputFolder);

            for(int sexNr = 0; sexNr < 2; ++sexNr)
            {
                YmtPedDefinitionFile ymt = CreateYmtPedDefinitionFile(Prefixes[sexNr] + collectionName, 
                    out var componentTextureBindings, 
                    out var componentIndexes, 
                    out var propIndexes);

                bool isAnyClothAdded = false;
                bool isAnyPropAdded = false;

                foreach (ClothData clothData in MainWindow.Clothes)
                {
                    if (clothData.IsComponent())
                    {
                        if (clothData.Textures.Count <= 0 || (int) clothData.TargetSex != sexNr) 
                            continue;

                        var componentItemInfo = GenerateYmtPedComponentItem(clothData, ref componentTextureBindings);
                        ymt.Unk_376833625.CompInfos.Add(componentItemInfo);

                        var componentTypeId = componentItemInfo.Unk_3509540765;
                        GetClothPostfixes(clothData, out var ytdPostfix, out var yddPostfix);

                        if (!isAnyClothAdded)
                        {
                            isAnyClothAdded = true;
                            OnFirstClothAddedToResource(outputFolder, sexNr, collectionName);
                        }

                        int currentComponentIndex = componentIndexes[componentTypeId]++;

                        string componentNumerics = currentComponentIndex.ToString().PadLeft(3, '0');
                        string prefix = clothData.GetPrefix();

                        clothData.SetComponentNumerics(componentNumerics, currentComponentIndex);

                        CopyClothModelToResource(clothData, sexNr, outputFolder, collectionName, componentNumerics, prefix, yddPostfix);
                        
                        char offsetLetter = 'a';
                        for (int i = 0; i < clothData.Textures.Count; ++i)
                        {
                            CopyClothTextureToResource(clothData.Textures[i], sexNr, outputFolder, collectionName, componentNumerics, prefix, ytdPostfix, (char)(offsetLetter + i));
                        }

                        if (!string.IsNullOrEmpty(clothData.FirstPersonModelPath))
                        {
                            CopyClothFirstPersonModelToResource(clothData.FirstPersonModelPath, sexNr, outputFolder, collectionName, componentNumerics, prefix, yddPostfix);
                        }
                    }
                    else
                    {
                        if (clothData.Textures.Count <= 0 || (int) clothData.TargetSex != sexNr) 
                            continue;

                        Unk_2834549053 anchor = (Unk_2834549053)clothData.GetPedPropTypeId();
                        var defs = ymt.Unk_376833625.PropInfo.Props[anchor] ?? new List<MUnk_94549140>();
                        var item = GenerateYmtPedPropItem(ymt, anchor, clothData);
                        defs.Add(item);

                        if (!isAnyPropAdded)
                        {
                            isAnyPropAdded = true;
                            OnFirstPropAddedToResource(outputFolder, sexNr, collectionName);
                        }

                        int currentPropIndex = propIndexes[(byte)anchor]++;

                        string componentNumerics = currentPropIndex.ToString().PadLeft(3, '0');
                        string prefix = clothData.GetPrefix();

                        clothData.SetComponentNumerics(componentNumerics, currentPropIndex);

                        CopyPropModelToResource(clothData, sexNr, outputFolder, collectionName, componentNumerics, prefix);
                        
                        char offsetLetter = 'a';
                        for (int i = 0; i < clothData.Textures.Count; ++i)
                        {
                            CopyPropTextureToResource(clothData.Textures[i], sexNr, outputFolder, collectionName, componentNumerics, prefix, (char)(offsetLetter + i));
                        }
                    }
                }
                
                if (isAnyClothAdded)
                {
                    UpdateYmtComponentTextureBindings(componentTextureBindings, ymt);
                    var clothYmtFilePath = GetClothYmtFilePath(outputFolder, sexNr, collectionName);
                    ymt.Save(clothYmtFilePath);
                }
                
                OnResourceClothDataFinished(outputFolder, sexNr, collectionName, isAnyClothAdded, isAnyPropAdded);
            }

            OnResourceBuildingFinished(outputFolder);
        }
        
        protected abstract string GetClothYmtFilePath(string outputFolder, int sexNr, string collectionName);

        protected abstract void CopyPropTextureToResource(string propTextureFilePath, int sexNr, string outputFolder,
            string collectionName, string componentNumerics, string prefix, char offsetLetter);

        protected abstract void CopyPropModelToResource(ClothData propClothData, int sexNr, string outputFolder,
            string collectionName, string componentNumerics, string prefix);

        protected abstract void CopyClothFirstPersonModelToResource(string clothDataFirstPersonModelPath, int sexNr,
            string outputFolder, string collectionName, string componentNumerics, string prefix, string yddPostfix);

        protected abstract void CopyClothTextureToResource(string clothTextureFilePath, int sexNr, string outputFolder,
            string collectionName, string componentNumerics, string prefix, string ytdPostfix, char offsetLetter);

        protected abstract void CopyClothModelToResource(ClothData clothData, int sexNr, string outputFolder,
            string collectionName, string componentNumerics, string prefix, string yddPostfix);

        protected virtual void OnResourceClothDataFinished(string outputFolder, int sexNr, string collectionName, bool isAnyClothAdded, bool isAnyPropAdded)
        {

        }

        protected virtual void OnFirstPropAddedToResource(string outputFolder, int sexNr, string collectionName)
        {

        }

        protected virtual void OnFirstClothAddedToResource(string outputFolder, int sexNr, string collectionName)
        {

        }

        protected virtual void OnResourceBuildingStarted(string outputFolder)
        {

        }

        protected virtual void OnResourceBuildingFinished(string outputFolder)
        {

        }
    }
}
