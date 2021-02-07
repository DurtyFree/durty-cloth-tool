using System.Collections.Generic;
using System.IO;
using RageLib.GTA5.ResourceWrappers.PC.Meta.Structures;
using RageLib.Resources.GTA5.PC.GameFiles;
using RageLib.Resources.GTA5.PC.Meta;

namespace altClothTool.App.Builders
{
    internal class AltvResourceBuilder
        : ResourceBuilderBase
    {
        public override void BuildResource(string outputFolder, string collectionName)
        {
            List<string> streamCfgMetas = new List<string>();
            List<string> streamCfgIncludes = new List<string>();

            for(int sexNr = 0; sexNr < 2; ++sexNr)
            {
                //Male YMT generating
                YmtPedDefinitionFile ymt = new YmtPedDefinitionFile
                {
                    metaYmtName = Prefixes[sexNr] + collectionName,
                    Unk_376833625 = {DlcName = RageLib.Hash.Jenkins.Hash(Prefixes[sexNr] + collectionName)}
                };
                
                MUnk_3538495220[] componentTextureBindings = { null, null, null, null, null, null, null, null, null, null, null, null };
                int[] componentIndexes = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                int[] propIndexes      = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                //ymt.Unk_376833625.Unk_1235281004 = 0;
                //ymt.Unk_376833625.Unk_4086467184 = 0;
                //ymt.Unk_376833625.Unk_911147899 = 0;
                //ymt.Unk_376833625.Unk_315291935 = 0;
                //ymt.Unk_376833625.Unk_2996560424 = ;

                bool isAnyClothAdded = false;
                bool isAnyPropAdded  = false;

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
                            Directory.CreateDirectory(outputFolder + "\\stream");
                            Directory.CreateDirectory(outputFolder + "\\stream\\" + FolderNames[sexNr] + ".rpf");
                            Directory.CreateDirectory(outputFolder + "\\stream\\" + FolderNames[sexNr] + ".rpf\\" + Prefixes[sexNr] + "freemode_01_" + Prefixes[sexNr] + collectionName);
                        }

                        int currentComponentIndex = componentIndexes[componentTypeId]++;
                        string componentNumerics = currentComponentIndex.ToString().PadLeft(3, '0');
                        string prefix = clothData.GetPrefix();

                        clothData.SetComponentNumerics(componentNumerics, currentComponentIndex);

                        File.Copy(clothData.MainPath, outputFolder + "\\stream\\" + FolderNames[sexNr] + ".rpf\\" + Prefixes[sexNr] + "freemode_01_" + Prefixes[sexNr] + collectionName + "\\" + prefix + "_" + componentNumerics + "_" + yddPostfix + ".ydd");

                        char offsetLetter = 'a';
                        for (int i = 0; i < clothData.Textures.Count; ++i)
                            File.Copy(clothData.Textures[i], outputFolder + "\\stream\\" + FolderNames[sexNr] + ".rpf\\" + Prefixes[sexNr] + "freemode_01_" + Prefixes[sexNr] + collectionName + "\\" + prefix + "_diff_" + componentNumerics + "_" + (char)(offsetLetter + i) + "_" + ytdPostfix + ".ytd");

                        if (clothData.FirstPersonModelPath != "")
                            File.Copy(clothData.FirstPersonModelPath, outputFolder + "\\stream\\" + FolderNames[sexNr] + ".rpf\\" + Prefixes[sexNr] + "freemode_01_" + Prefixes[sexNr] + collectionName + "\\" + prefix + "_" + componentNumerics + "_" + yddPostfix + "_1.ydd");
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
                            Directory.CreateDirectory(outputFolder + "\\stream");
                            Directory.CreateDirectory(outputFolder + "\\stream\\" + FolderNames[sexNr] + "_p.rpf");
                            Directory.CreateDirectory(outputFolder + "\\stream\\" + FolderNames[sexNr] + "_p.rpf\\" + Prefixes[sexNr] + "freemode_01_p_" + Prefixes[sexNr] + collectionName);
                        }

                        int currentPropIndex = propIndexes[(byte)anchor]++;

                        string componentNumerics = currentPropIndex.ToString().PadLeft(3, '0');
                        string prefix = clothData.GetPrefix();

                        clothData.SetComponentNumerics(componentNumerics, currentPropIndex);

                        File.Copy(clothData.MainPath, outputFolder + "\\stream\\" + FolderNames[sexNr] + "_p.rpf\\" + Prefixes[sexNr] + "freemode_01_p_" + Prefixes[sexNr] + collectionName + "\\" + prefix + "_" + componentNumerics + ".ydd", true);

                        char offsetLetter = 'a';
                        for (int i = 0; i < clothData.Textures.Count; ++i)
                        {
                            File.Copy(clothData.Textures[i], outputFolder + "\\stream\\" + FolderNames[sexNr] + "_p.rpf\\" + Prefixes[sexNr] + "freemode_01_p_" + Prefixes[sexNr] + collectionName + "\\" + prefix + "_diff_" + componentNumerics + "_" + (char)(offsetLetter + i) + ".ytd", true);
                        }
                    }
                }
                
                if (isAnyClothAdded)
                {
                    UpdateYmtComponentTextureBindings(componentTextureBindings, ymt);
                    ymt.Save(outputFolder + "\\stream\\" + FolderNames[sexNr] + ".rpf\\" + Prefixes[sexNr] + "freemode_01_" + Prefixes[sexNr] + collectionName + ".ymt");
                }

                if(isAnyPropAdded)
                {
                    streamCfgIncludes.Add("stream/" + FolderNames[sexNr] + "_p.rpf/*");
                }

                if (isAnyClothAdded || isAnyPropAdded)
                {
                    File.WriteAllText(outputFolder + "\\stream\\" + Prefixes[sexNr] + "freemode_01_" + Prefixes[sexNr] + collectionName + ".meta", GenerateShopMetaContent((ClothData.Sex)sexNr, collectionName));
                    streamCfgMetas.Add("stream/" + Prefixes[sexNr] + "freemode_01_" + Prefixes[sexNr] + collectionName + ".meta: SHOP_PED_APPAREL_META_FILE");

                    streamCfgIncludes.Add("stream/" + FolderNames[sexNr] + ".rpf/*");
                }
            }

            File.WriteAllText(outputFolder + "\\stream.cfg", GenerateAltvStreamCfgContent(streamCfgIncludes, streamCfgMetas));
            File.WriteAllText(outputFolder + "\\resource.cfg", GenerateAltvResourceCfgContent());
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
