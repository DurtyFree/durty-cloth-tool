using RageLib.GTA5.ResourceWrappers.PC.Meta.Structures;
using RageLib.Resources.GTA5.PC.GameFiles;
using RageLib.Resources.GTA5.PC.Meta;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static AltTool.ClothData;

namespace AltTool
{
    public class ResourceBuilder
    {
        public static void GenerateYmt()
        {

        }

        public static string GenerateShopMeta(Sex targetSex, string collectionName)
        {
            string targetName = targetSex == Sex.Male ? "mp_m_freemode_01" : "mp_f_freemode_01";
            string dlcName = (targetSex == Sex.Male ? "mp_m_" : "mp_f_") + collectionName;
            string character = (targetSex == Sex.Male ? "SCR_CHAR_MULTIPLAYER" : "SCR_CHAR_MULTIPLAYER_F");
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

        public static string GenerateStreamCfg(List<string> files, List<string> metas)
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

            return $"files: [\n{filesText}/*\n]\nmeta: {{\n{metasText}\n}}\n";
        }

        public static string GenerateResourceCfg()
        {
            return "type: dlc,\nmain: stream.cfg,\nclient-files: [\n  stream/*\n]\n";
        }

        private static string[] prefixes = { "mp_m_", "mp_f_" };
        private static string[] folderNames = { "ped_male", "ped_female" };

        public static void BuildResourceAltv(string outputFolder, string collectionName)
        {
            List<string> streamCfgMetas = new List<string>();
            List<string> streamCfgIncludes = new List<string>();
            for(int sexNr = 0; sexNr < 2; ++sexNr)
            {
                //Male YMT generating
                YmtPedDefinitionFile ymt = new YmtPedDefinitionFile();

                ymt.metaYmtName = prefixes[sexNr] + collectionName;
                ymt.Unk_376833625.DlcName = RageLib.Hash.Jenkins.Hash(prefixes[sexNr] + collectionName);

                MUnk_3538495220[] componentTextureBindings = { null, null, null, null, null, null, null, null, null, null, null, null };
                int[] componentIndexes = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                //ymt.Unk_376833625.Unk_1235281004 = 0;
                //ymt.Unk_376833625.Unk_4086467184 = 0;
                //ymt.Unk_376833625.Unk_911147899 = 0;
                //ymt.Unk_376833625.Unk_315291935 = 0;
                //ymt.Unk_376833625.Unk_2996560424 = ;

                bool isAnyClothAdded = false;

                foreach (ClothData cd in MainWindow.clothes)
                {
                    if (!cd.IsComponent())
                        continue;

                    byte componentTypeID = cd.GetComponentTypeID();

                    if (cd.textures.Count > 0 && (int)cd.targetSex == sexNr)
                    {
                        YmtPedDefinitionFile targetYmt = ymt;

                        if (componentTextureBindings[componentTypeID] == null)
                            componentTextureBindings[componentTypeID] = new MUnk_3538495220();

                        MUnk_1535046754 textureDescription = new MUnk_1535046754();

                        byte nextPropMask = 17;
                        if (componentTypeID == 8 || componentTypeID == 11)
                            nextPropMask = 1;

                        textureDescription.PropMask = nextPropMask;
                        textureDescription.Unk_2806194106 = (byte)(cd.fpModelPath != "" ? 1 : 0);

                        byte texId = (byte)(componentTypeID != 4 ? 0 : 1);
                        string postfix = componentTypeID != 4 ? "u" : "r";
                        string ytdPostfix = componentTypeID != 4 ? "uni" : "whi";

                        foreach (string texPath in cd.textures)
                        {
                            MUnk_1036962405 texInfo = new MUnk_1036962405();
                            texInfo.Distribution = 255;
                            texInfo.TexId = texId;
                            textureDescription.ATexData.Add(texInfo);
                        }

                        textureDescription.ClothData.Unk_2828247905 = 0;

                        componentTextureBindings[componentTypeID].Unk_1756136273.Add(textureDescription);

                        byte componentTextureLocalId = (byte)(componentTextureBindings[componentTypeID].Unk_1756136273.Count - 1);

                        MCComponentInfo componentInfo = new MCComponentInfo();
                        componentInfo.Unk_802196719 = 0;
                        componentInfo.Unk_4233133352 = 0;
                        componentInfo.Unk_128864925.b0 = (byte)(cd.componentFlags.unkFlag1 ? 1 : 0);
                        componentInfo.Unk_128864925.b1 = (byte)(cd.componentFlags.unkFlag2 ? 1 : 0);
                        componentInfo.Unk_128864925.b2 = (byte)(cd.componentFlags.unkFlag3 ? 1 : 0);
                        componentInfo.Unk_128864925.b3 = (byte)(cd.componentFlags.unkFlag4 ? 1 : 0);
                        componentInfo.Unk_128864925.b4 = (byte)(cd.componentFlags.isHighHeels ? 1 : 0);
                        componentInfo.Flags = 0;
                        componentInfo.Inclusions = 0;
                        componentInfo.Exclusions = 0;
                        componentInfo.Unk_1613922652 = 0;
                        componentInfo.Unk_2114993291 = 0;
                        componentInfo.Unk_3509540765 = componentTypeID;
                        componentInfo.Unk_4196345791 = componentTextureLocalId;

                        targetYmt.Unk_376833625.CompInfos.Add(componentInfo);

                        if (!isAnyClothAdded)
                        {
                            isAnyClothAdded = true;
                            Directory.CreateDirectory(outputFolder + "\\stream");
                            Directory.CreateDirectory(outputFolder + "\\stream\\" + folderNames[sexNr] + ".rpf");
                            Directory.CreateDirectory(outputFolder + "\\stream\\" + folderNames[sexNr] + ".rpf\\" + prefixes[sexNr] + "freemode_01_" + prefixes[sexNr] + collectionName);
                        }

                        int currentComponentIndex = componentIndexes[componentTypeID]++;

                        string componentNumerics = currentComponentIndex.ToString().PadLeft(3, '0');
                        string prefix = cd.GetPrefix();

                        File.Copy(cd.mainPath, outputFolder + "\\stream\\" + folderNames[sexNr] + ".rpf\\" + prefixes[sexNr] + "freemode_01_" + prefixes[sexNr] + collectionName + "\\" + prefix + "_" + componentNumerics + "_" + postfix + ".ydd");

                        char offsetLetter = 'a';
                        for(int i = 0; i < cd.textures.Count; ++i)
                            File.Copy(cd.textures[i], outputFolder + "\\stream\\" + folderNames[sexNr] + ".rpf\\" + prefixes[sexNr] + "freemode_01_" + prefixes[sexNr] + collectionName + "\\" + prefix + "_diff_" + componentNumerics + "_" + (char)(offsetLetter + i) + "_" + ytdPostfix+ ".ytd");

                        if (cd.fpModelPath != "")
                            File.Copy(cd.fpModelPath, outputFolder + "\\stream\\" + folderNames[sexNr] + ".rpf\\" + prefixes[sexNr] + "freemode_01_" + prefixes[sexNr] + collectionName + "\\" + prefix + "_" + componentNumerics + "_" + postfix + "_1.ydd");
                    }
                }


                if (isAnyClothAdded)
                {

                    int arrIndex = 0;
                    for (int i = 0; i < componentTextureBindings.Length; ++i)
                    {
                        if (componentTextureBindings[i] != null)
                        {
                            byte id = (byte)(arrIndex++);
                            ymt.Unk_376833625.Unk_2996560424.SetByte(i, id);
                        }

                        ymt.Unk_376833625.Components[(Unk_884254308)i] = componentTextureBindings[i];
                    }

                    File.WriteAllText(outputFolder + "\\stream\\" + prefixes[sexNr] + "freemode_01_" + prefixes[sexNr] + collectionName + ".meta", GenerateShopMeta((Sex)sexNr, collectionName));
                    streamCfgMetas.Add("stream/" + prefixes[sexNr] + "freemode_01_" + prefixes[sexNr] + collectionName + ".meta: SHOP_PED_APPAREL_META_FILE");
                    ymt.Save(outputFolder + "\\stream\\" + folderNames[sexNr] + ".rpf\\" + prefixes[sexNr] + "freemode_01_" + prefixes[sexNr] + collectionName + ".ymt");
                    streamCfgIncludes.Add("stream/" + folderNames[sexNr] + ".rpf/*");
                }
            }
            File.WriteAllText(outputFolder + "\\stream.cfg", GenerateStreamCfg(streamCfgIncludes, streamCfgMetas));
            File.WriteAllText(outputFolder + "\\resource.cfg", GenerateResourceCfg());

            MessageBox.Show("Resource built!");
        }

        public static void BuildResourceSingle(string outputFolder, string collectionName)
        {

        }

        public static void BuildResourceFivem(string outputFolder, string collectionName)
        {

        }
    }
}
