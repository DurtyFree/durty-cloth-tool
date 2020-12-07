using RageLib.Archives;
using RageLib.GTA5.Archives;
using RageLib.GTA5.ArchiveWrappers;
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

        public static string GenerateResourceLua(List<string> metas)
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
            manifestContent += "fx_version 'bodacious'\n";
            manifestContent += "game { 'gta5' }\n\n";
            manifestContent += $"files {{\n{filesText}\n}}\n\n{metasText}";
            return manifestContent;
        }

        public static string GenerateContentXML(string collectionName, bool hasMale, bool hasFemale, bool hasMaleProps, bool hasFemaleProps)
        {
            string str = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<CDataFileMgr__ContentsOfDataFileXml>
  <disabledFiles />
  <includedXmlFiles />
  <includedDataFiles />
  <dataFiles>
";

            if(hasMale)
            {
                str += $@"      <Item>
      <filename>dlc_{collectionName}:/common/data/mp_m_freemode_01_mp_m_{collectionName}.meta</filename>
      <fileType>SHOP_PED_APPAREL_META_FILE</fileType>
      <overlay value=""false"" />
      <disabled value=""true"" />
      <persistent value=""false"" />
    </Item>
    <Item>
      <filename>dlc_{collectionName}:/%PLATFORM%/models/cdimages/{collectionName}_male.rpf</filename>
      <fileType>RPF_FILE</fileType>
      <overlay value=""false"" />
      <disabled value=""true"" />
      <persistent value=""true"" />
    </Item>
";
            }

            if(hasFemale)
            {
                str += $@"    <Item>
      <filename>dlc_{collectionName}:/common/data/mp_f_freemode_01_mp_f_{collectionName}.meta</filename>
      <fileType>SHOP_PED_APPAREL_META_FILE</fileType>
      <overlay value=""false"" />
      <disabled value=""true"" />
      <persistent value=""false"" />
    </Item>
    <Item>
      <filename>dlc_{collectionName}:/%PLATFORM%/models/cdimages/{collectionName}_female.rpf</filename>
      <fileType>RPF_FILE</fileType>
      <overlay value=""false"" />
      <disabled value=""true"" />
      <persistent value=""true"" />
    </Item>
";
            }

            if (hasMaleProps)
            {
                str += $@"      <Item>
      <filename>dlc_{collectionName}:/%PLATFORM%/models/cdimages/{collectionName}_male_p.rpf</filename>
      <fileType>RPF_FILE</fileType>
      <overlay value=""false"" />
      <disabled value=""true"" />
      <persistent value=""true"" />
    </Item>
";
            }

            if (hasFemaleProps)
            {
                str += $@"    <Item>
      <filename>dlc_{collectionName}:/%PLATFORM%/models/cdimages/{collectionName}_female_p.rpf</filename>
      <fileType>RPF_FILE</fileType>
      <overlay value=""false"" />
      <disabled value=""true"" />
      <persistent value=""true"" />
    </Item>
";
            }

            str += $@"</dataFiles>
  <contentChangeSets>
    <Item>
      <changeSetName>{collectionName.ToUpper()}_AUTOGEN</changeSetName>
      <mapChangeSetData />
      <filesToInvalidate />
      <filesToDisable />
      <filesToEnable>
";
            if(hasMale)
            {
                str += $"    <Item>dlc_{collectionName}:/common/data/mp_m_freemode_01_mp_m_{collectionName}.meta</Item>\n";
                str += $"    <Item>dlc_{collectionName}:/%PLATFORM%/models/cdimages/{collectionName}_male.rpf</Item>\n";
            }

            if(hasFemale)
            {
                str += $"    <Item>dlc_{collectionName}:/common/data/mp_f_freemode_01_mp_f_{collectionName}.meta</Item>\n";
                str += $"    <Item>dlc_{collectionName}:/%PLATFORM%/models/cdimages/{collectionName}_female.rpf</Item>\n";
            }

            if (hasMaleProps)
            {
                str += $"    <Item>dlc_{collectionName}:/%PLATFORM%/models/cdimages/{collectionName}_male_p.rpf</Item>\n";
            }

            if (hasFemaleProps)
            {
                str += $"    <Item>dlc_{collectionName}:/%PLATFORM%/models/cdimages/{collectionName}_female_p.rpf</Item>\n";
            }

            str += $@"      </filesToEnable>
    </Item>
  </contentChangeSets>
  <patchFiles />
</CDataFileMgr__ContentsOfDataFileXml>";

            return str;
        }

        public static string GenerateSetup2XML(string collectionName, int order = 1000)
        {
            return $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<SSetupData>
    <deviceName>dlc_{collectionName}</deviceName>
    <datFile>content.xml</datFile>
    <timeStamp>07/07/2077 07:07:07</timeStamp>
    <nameHash>{collectionName}</nameHash>
    <contentChangeSets />
    <contentChangeSetGroups>
        <Item>
            <NameHash>GROUP_STARTUP</NameHash>
            <ContentChangeSets>
                <Item>{collectionName.ToUpper()}_AUTOGEN</Item>
            </ContentChangeSets>
        </Item>
    </contentChangeSetGroups>
    <startupScript />
    <scriptCallstackSize value=""0"" />
    <type>EXTRACONTENT_COMPAT_PACK</type>
    <order value=""1000"" />
    <minorOrder value=""0"" />
    <isLevelPack value=""false"" />
    <dependencyPackHash />
    <requiredVersion />
    <subPackCount value=""0"" />
</SSetupData>";
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
                int[] propIndexes      = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                //ymt.Unk_376833625.Unk_1235281004 = 0;
                //ymt.Unk_376833625.Unk_4086467184 = 0;
                //ymt.Unk_376833625.Unk_911147899 = 0;
                //ymt.Unk_376833625.Unk_315291935 = 0;
                //ymt.Unk_376833625.Unk_2996560424 = ;

                bool isAnyClothAdded = false;
                bool isAnyPropAdded  = false;

                foreach (ClothData cd in MainWindow.clothes)
                {
                    if (cd.IsComponent())
                    {

                        byte componentTypeID = cd.GetComponentTypeID();

                        if (cd.textures.Count > 0 && (int)cd.targetSex == sexNr)
                        {
                            YmtPedDefinitionFile targetYmt = ymt;

                            if (componentTextureBindings[componentTypeID] == null)
                                componentTextureBindings[componentTypeID] = new MUnk_3538495220();

                            MUnk_1535046754 textureDescription = new MUnk_1535046754();

                            byte nextPropMask = 17;

                            switch (componentTypeID)
                            {
                                case 2:
                                case 7:
                                    nextPropMask = 11; break;
                                case 5:
                                case 8:
                                    nextPropMask = 65; break;
                                case 9:
                                    nextPropMask = 1; break;
                                case 10:
                                    nextPropMask = 5; break;
                                case 11:
                                    nextPropMask = 1; break;
                                default:
                                    break;
                            }

                            textureDescription.PropMask = nextPropMask;
                            textureDescription.Unk_2806194106 = (byte)(cd.fpModelPath != "" ? 1 : 0);

                            byte texId = (byte)(cd.mainPath.EndsWith("_u.ydd") ? 0 : 1);
                            string postfix = cd.mainPath.EndsWith("_u.ydd") ? "u" : "r";
                            string ytdPostfix = cd.mainPath.EndsWith("_u.ydd") ? "uni" : "whi";
                            
                            if((cd.drawableType == ClothNameResolver.DrawableType.Shoes) || (cd.drawableType == ClothNameResolver.DrawableType.Accessories))
                            {
                                postfix = "r";
                                ytdPostfix = "uni";
                            }   

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
                            for (int i = 0; i < cd.textures.Count; ++i)
                                File.Copy(cd.textures[i], outputFolder + "\\stream\\" + folderNames[sexNr] + ".rpf\\" + prefixes[sexNr] + "freemode_01_" + prefixes[sexNr] + collectionName + "\\" + prefix + "_diff_" + componentNumerics + "_" + (char)(offsetLetter + i) + "_" + ytdPostfix + ".ytd");

                            if (cd.fpModelPath != "")
                                File.Copy(cd.fpModelPath, outputFolder + "\\stream\\" + folderNames[sexNr] + ".rpf\\" + prefixes[sexNr] + "freemode_01_" + prefixes[sexNr] + collectionName + "\\" + prefix + "_" + componentNumerics + "_" + postfix + "_1.ydd");
                        }
                    }
                    else
                    {
                        Unk_2834549053 anchor = (Unk_2834549053)cd.GetPedPropTypeID();

                        if (cd.textures.Count > 0 && (int)cd.targetSex == sexNr)
                        {
                            YmtPedDefinitionFile targetYmt = ymt;

                            var defs = ymt.Unk_376833625.PropInfo.Props[anchor] ?? new List<MUnk_94549140>();
                            var item = new MUnk_94549140(ymt.Unk_376833625.PropInfo);

                            item.AnchorId = (byte)anchor;

                            for (int i = 0; i < cd.textures.Count; i++)
                            {
                                var texture = new MUnk_254518642();
                                texture.TexId = (byte)i;
                                item.TexData.Add(texture);
                            }

                            // Get or create linked anchor
                            var aanchor = ymt.Unk_376833625.PropInfo.AAnchors.Find(e => e.Anchor == anchor);

                            if (aanchor == null)
                            {
                                aanchor = new MCAnchorProps(ymt.Unk_376833625.PropInfo)
                                {
                                    Anchor = anchor
                                };

                                aanchor.PropsMap[item] = (byte)item.TexData.Count;

                                ymt.Unk_376833625.PropInfo.AAnchors.Add(aanchor);
                            }
                            else
                            {
                                aanchor.PropsMap[item] = (byte)item.TexData.Count;
                            }

                            defs.Add(item);

                            if (!isAnyPropAdded)
                            {
                                isAnyPropAdded = true;
                                Directory.CreateDirectory(outputFolder + "\\stream");
                                Directory.CreateDirectory(outputFolder + "\\stream\\" + folderNames[sexNr] + "_p.rpf");
                                Directory.CreateDirectory(outputFolder + "\\stream\\" + folderNames[sexNr] + "_p.rpf\\" + prefixes[sexNr] + "freemode_01_p_" + prefixes[sexNr] + collectionName);
                            }

                            int currentPropIndex = propIndexes[(byte)anchor]++;

                            string componentNumerics = currentPropIndex.ToString().PadLeft(3, '0');
                            string prefix = cd.GetPrefix();

                            var ydr = new YdrFile();

                            File.Copy(cd.mainPath, outputFolder + "\\stream\\" + folderNames[sexNr] + "_p.rpf\\" + prefixes[sexNr] + "freemode_01_p_" + prefixes[sexNr] + collectionName + "\\" + prefix + "_" + componentNumerics + ".ydd", true);

                            char offsetLetter = 'a';
                            for (int i = 0; i < cd.textures.Count; ++i)
                            {
                                File.Copy(cd.textures[i], outputFolder + "\\stream\\" + folderNames[sexNr] + "_p.rpf\\" + prefixes[sexNr] + "freemode_01_p_" + prefixes[sexNr] + collectionName + "\\" + prefix + "_diff_" + componentNumerics + "_" + (char)(offsetLetter + i) + ".ytd", true);
                            }
                        }
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
                }

                if(isAnyPropAdded)
                {
                    streamCfgIncludes.Add("stream/" + folderNames[sexNr] + "_p.rpf");
                }

                if (isAnyClothAdded || isAnyPropAdded)
                {
                    File.WriteAllText(outputFolder + "\\stream\\" + prefixes[sexNr] + "freemode_01_" + prefixes[sexNr] + collectionName + ".meta", GenerateShopMeta((Sex)sexNr, collectionName));
                    streamCfgMetas.Add("stream/" + prefixes[sexNr] + "freemode_01_" + prefixes[sexNr] + collectionName + ".meta: SHOP_PED_APPAREL_META_FILE");
                    ymt.Save(outputFolder + "\\stream\\" + folderNames[sexNr] + ".rpf\\" + prefixes[sexNr] + "freemode_01_" + prefixes[sexNr] + collectionName + ".ymt");
                    streamCfgIncludes.Add("stream/" + folderNames[sexNr] + ".rpf");
                }

            }

            File.WriteAllText(outputFolder + "\\stream.cfg", GenerateStreamCfg(streamCfgIncludes, streamCfgMetas));
            File.WriteAllText(outputFolder + "\\resource.cfg", GenerateResourceCfg());

            MessageBox.Show("Resource built!");
        }

        public static void BuildResourceSingle(string outputFolder, string collectionName)
        {
            Utils.EnsureKeys();

            using (RageArchiveWrapper7 rpf = RageArchiveWrapper7.Create(outputFolder + @"\dlc.rpf"))
            {
                rpf.archive_.Encryption = RageArchiveEncryption7.NG;

                var dir  = rpf.Root.CreateDirectory();
                dir.Name = "common";

                var dataDir = dir.CreateDirectory();
                dataDir.Name = "data";

                dir = rpf.Root.CreateDirectory();
                dir.Name = "x64";

                dir = dir.CreateDirectory();
                dir.Name = "models";

                var cdimagesDir = dir.CreateDirectory();
                cdimagesDir.Name = "cdimages";

                RageArchiveWrapper7 currComponentRpf = null;
                IArchiveDirectory   currComponentDir = null;

                RageArchiveWrapper7 currPropRpf = null;
                IArchiveDirectory   currPropDir = null;

                bool hasMale        = false;
                bool hasFemale      = false;
                bool hasMaleProps   = false;
                bool hasFemaleProps = false;

                for (int sexNr = 0; sexNr < 2; ++sexNr)
                {
                    //Male YMT generating
                    YmtPedDefinitionFile ymt = new YmtPedDefinitionFile();

                    ymt.metaYmtName = prefixes[sexNr] + collectionName;
                    ymt.Unk_376833625.DlcName = RageLib.Hash.Jenkins.Hash(prefixes[sexNr] + collectionName);

                    MUnk_3538495220[] componentTextureBindings = { null, null, null, null, null, null, null, null, null, null, null, null };
                    int[] componentIndexes = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    int[] propIndexes = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                    //ymt.Unk_376833625.Unk_1235281004 = 0;
                    //ymt.Unk_376833625.Unk_4086467184 = 0;
                    //ymt.Unk_376833625.Unk_911147899 = 0;
                    //ymt.Unk_376833625.Unk_315291935 = 0;
                    //ymt.Unk_376833625.Unk_2996560424 = ;

                    bool isAnyClothAdded = false;
                    bool isAnyPropAdded  = false;

                    foreach (ClothData cd in MainWindow.clothes)
                    {
                        if (cd.IsComponent())
                        {
                            byte componentTypeID = cd.GetComponentTypeID();

                            if (cd.textures.Count > 0 && (int)cd.targetSex == sexNr)
                            {
                                YmtPedDefinitionFile targetYmt = ymt;

                                if (componentTextureBindings[componentTypeID] == null)
                                    componentTextureBindings[componentTypeID] = new MUnk_3538495220();

                                MUnk_1535046754 textureDescription = new MUnk_1535046754();

                                byte nextPropMask = 17;

                                switch (componentTypeID)
                                {
                                    case 2:
                                    case 7:
                                        nextPropMask = 11; break;
                                    case 5:
                                    case 8:
                                        nextPropMask = 65; break;
                                    case 9:
                                        nextPropMask = 1; break;
                                    case 10:
                                        nextPropMask = 5; break;
                                    case 11:
                                        nextPropMask = 1; break;
                                    default:
                                        break;
                                }

                                textureDescription.PropMask = nextPropMask;
                                textureDescription.Unk_2806194106 = (byte)(cd.fpModelPath != "" ? 1 : 0);

                                byte texId = (byte)(cd.mainPath.EndsWith("_u.ydd") ? 0 : 1);
                                string postfix = cd.mainPath.EndsWith("_u.ydd") ? "u" : "r";
                                string ytdPostfix = cd.mainPath.EndsWith("_u.ydd") ? "uni" : "whi";

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

                                    var ms = new MemoryStream();

                                    currComponentRpf                     = RageArchiveWrapper7.Create(ms, folderNames[sexNr].Replace("ped_", collectionName + "_") + ".rpf");
                                    currComponentRpf.archive_.Encryption = RageArchiveEncryption7.NG;
                                    currComponentDir                     = currComponentRpf.Root.CreateDirectory();
                                    currComponentDir.Name                = prefixes[sexNr] + "freemode_01_" + prefixes[sexNr] + collectionName;
                                }

                                int currentComponentIndex = componentIndexes[componentTypeID]++;

                                string componentNumerics = currentComponentIndex.ToString().PadLeft(3, '0');
                                string prefix = cd.GetPrefix();

                                var resource = currComponentDir.CreateResourceFile();
                                resource.Name = prefix + "_" + componentNumerics + "_" + postfix + ".ydd";
                                resource.Import(cd.mainPath);

                                char offsetLetter = 'a';

                                for (int i = 0; i < cd.textures.Count; ++i)
                                {
                                    resource = currComponentDir.CreateResourceFile();
                                    resource.Name = prefix + "_diff_" + componentNumerics + "_" + (char)(offsetLetter + i) + "_" + ytdPostfix + ".ytd";
                                    resource.Import(cd.textures[i]);
                                }

                                if (cd.fpModelPath != "")
                                {
                                    resource = currComponentDir.CreateResourceFile();
                                    resource.Name = prefix + "_" + componentNumerics + "_" + postfix + "_1.ydd";
                                    resource.Import(cd.fpModelPath);
                                }

                            }
                        }
                        else
                        {
                            Unk_2834549053 anchor = (Unk_2834549053)cd.GetPedPropTypeID();

                            if (cd.textures.Count > 0 && (int)cd.targetSex == sexNr)
                            {
                                YmtPedDefinitionFile targetYmt = ymt;

                                var defs = ymt.Unk_376833625.PropInfo.Props[anchor] ?? new List<MUnk_94549140>();
                                var item = new MUnk_94549140(ymt.Unk_376833625.PropInfo);

                                item.AnchorId = (byte)anchor;

                                for (int i = 0; i < cd.textures.Count; i++)
                                {
                                    var texture = new MUnk_254518642();
                                    texture.TexId = (byte)i;
                                    item.TexData.Add(texture);
                                }

                                // Get or create linked anchor
                                var aanchor = ymt.Unk_376833625.PropInfo.AAnchors.Find(e => e.Anchor == anchor);

                                if (aanchor == null)
                                {
                                    aanchor = new MCAnchorProps(ymt.Unk_376833625.PropInfo)
                                    {
                                        Anchor = anchor
                                    };

                                    aanchor.PropsMap[item] = (byte)item.TexData.Count;

                                    ymt.Unk_376833625.PropInfo.AAnchors.Add(aanchor);
                                }
                                else
                                {
                                    aanchor.PropsMap[item] = (byte)item.TexData.Count;
                                }

                                defs.Add(item);

                                if (!isAnyPropAdded)
                                {
                                    isAnyPropAdded = true;

                                    var ms = new MemoryStream();

                                    currPropRpf = RageArchiveWrapper7.Create(ms, folderNames[sexNr].Replace("ped_", collectionName + "_") + "_p.rpf");
                                    currPropRpf.archive_.Encryption = RageArchiveEncryption7.NG;
                                    currPropDir = currPropRpf.Root.CreateDirectory();
                                    currPropDir.Name = prefixes[sexNr] + "freemode_01_p_" + prefixes[sexNr] + collectionName;
                                }

                                int currentPropIndex = propIndexes[(byte)anchor]++;

                                string componentNumerics = currentPropIndex.ToString().PadLeft(3, '0');
                                string prefix = cd.GetPrefix();

                                var resource = currPropDir.CreateResourceFile();
                                resource.Name = prefix + "_" + componentNumerics + ".ydd";
                                resource.Import(cd.mainPath);

                                char offsetLetter = 'a';
                                for (int i = 0; i < cd.textures.Count; ++i)
                                {
                                    resource = currPropDir.CreateResourceFile();
                                    resource.Name = prefix + "_diff_" + componentNumerics + "_" + (char)(offsetLetter + i) + ".ytd";
                                    resource.Import(cd.textures[i]);
                                }
                            }
                        }
                    }

                    if (isAnyClothAdded)
                    {
                        if (sexNr == 0)
                            hasMale = true;
                        else if (sexNr == 1)
                            hasFemale = true;

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
                    }

                    if (isAnyClothAdded || isAnyPropAdded)
                    {
                        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(GenerateShopMeta((Sex)sexNr, collectionName))))
                        {
                            var binFile = dataDir.CreateBinaryFile();
                            binFile.Name = prefixes[sexNr] + "freemode_01_" + prefixes[sexNr] + collectionName + ".meta";
                            binFile.Import(stream);
                        }
                        currComponentRpf.Flush();

                        var binRpfFile = cdimagesDir.CreateBinaryFile();
                        binRpfFile.Name = folderNames[sexNr].Replace("ped_", collectionName + "_") + ".rpf";
                        binRpfFile.Import(currComponentRpf.archive_.BaseStream);

                        currComponentRpf.Dispose();
                    }

                    if (isAnyPropAdded)
                    {
                        if (sexNr == 0)
                            hasMaleProps = true;
                        else if (sexNr == 1)
                            hasFemaleProps = true;

                        currPropRpf.Flush();

                        var binRpfFile = cdimagesDir.CreateBinaryFile();
                        binRpfFile.Name = folderNames[sexNr].Replace("ped_", collectionName + "_") + "_p.rpf";
                        binRpfFile.Import(currPropRpf.archive_.BaseStream);

                        currPropRpf.Dispose();
                    }
                }

                using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(GenerateContentXML(collectionName, hasMale, hasFemale, hasMaleProps, hasFemaleProps))))
                {
                    var binFile = rpf.Root.CreateBinaryFile();
                    binFile.Name = "content.xml";
                    binFile.Import(stream);
                }

                using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(GenerateSetup2XML(collectionName))))
                {
                    var binFile = rpf.Root.CreateBinaryFile();
                    binFile.Name = "setup2.xml";
                    binFile.Import(stream);
                }

                rpf.Flush();
                rpf.Dispose();

                MessageBox.Show("Resource built!");
            }
        }

        public static void BuildResourceFivem(string outputFolder, string collectionName)
        {
            List<string> resourceLUAMetas = new List<string>();

            for(int sexNr = 0; sexNr < 2; ++sexNr)
            {
                //Male YMT generating
                YmtPedDefinitionFile ymt = new YmtPedDefinitionFile();

                ymt.metaYmtName = prefixes[sexNr] + collectionName;
                ymt.Unk_376833625.DlcName = RageLib.Hash.Jenkins.Hash(prefixes[sexNr] + collectionName);

                MUnk_3538495220[] componentTextureBindings = { null, null, null, null, null, null, null, null, null, null, null, null };
                int[] componentIndexes = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                int[] propIndexes      = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                //ymt.Unk_376833625.Unk_1235281004 = 0;
                //ymt.Unk_376833625.Unk_4086467184 = 0;
                //ymt.Unk_376833625.Unk_911147899 = 0;
                //ymt.Unk_376833625.Unk_315291935 = 0;
                //ymt.Unk_376833625.Unk_2996560424 = ;

                bool isAnyClothAdded = false;
                bool isAnyPropAdded = false;

                foreach (ClothData cd in MainWindow.clothes)
                {
                    if (cd.IsComponent())
                    {
                        byte componentTypeID = cd.GetComponentTypeID();

                        if (cd.textures.Count > 0 && (int)cd.targetSex == sexNr)
                        {
                            YmtPedDefinitionFile targetYmt = ymt;

                            if (componentTextureBindings[componentTypeID] == null)
                                componentTextureBindings[componentTypeID] = new MUnk_3538495220();

                            MUnk_1535046754 textureDescription = new MUnk_1535046754();

                            byte nextPropMask = 17;

                            switch (componentTypeID)
                            {
                                case 2:
                                case 7:
                                    nextPropMask = 11; break;
                                case 5:
                                case 8:
                                    nextPropMask = 65; break;
                                case 9:
                                    nextPropMask = 1; break;
                                case 10:
                                    nextPropMask = 5; break;
                                case 11:
                                    nextPropMask = 1; break;
                                default:
                                    break;
                            }

                            textureDescription.PropMask = nextPropMask;
                            textureDescription.Unk_2806194106 = (byte)(cd.fpModelPath != "" ? 1 : 0);

                            byte texId = (byte)(cd.mainPath.EndsWith("_u.ydd") ? 0 : 1);
                            string postfix = cd.mainPath.EndsWith("_u.ydd") ? "u" : "r";
                            string ytdPostfix = cd.mainPath.EndsWith("_u.ydd") ? "uni" : "whi";

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
                                Directory.CreateDirectory(outputFolder + "\\stream\\" + prefixes[sexNr] + "freemode_01_" + prefixes[sexNr] + collectionName);
                            }

                            int currentComponentIndex = componentIndexes[componentTypeID]++;

                            string componentNumerics = currentComponentIndex.ToString().PadLeft(3, '0');
                            string prefix = cd.GetPrefix();

                            File.Copy(cd.mainPath, outputFolder + "\\stream\\" + prefixes[sexNr] + "freemode_01_" + prefixes[sexNr] + collectionName + "\\" + prefixes[sexNr] + "freemode_01_" + prefixes[sexNr] + collectionName + "^" + prefix + "_" + componentNumerics + "_" + postfix + ".ydd", true);

                            char offsetLetter = 'a';
                            for (int i = 0; i < cd.textures.Count; ++i)
                                File.Copy(cd.textures[i], outputFolder + "\\stream\\" + prefixes[sexNr] + "freemode_01_" + prefixes[sexNr] + collectionName + "\\" + prefixes[sexNr] + "freemode_01_" + prefixes[sexNr] + collectionName + "^" + prefix + "_diff_" + componentNumerics + "_" + (char)(offsetLetter + i) + "_" + ytdPostfix + ".ytd", true);

                            if (cd.fpModelPath != "")
                                File.Copy(cd.fpModelPath, outputFolder + "\\stream\\" + prefixes[sexNr] + "freemode_01_" + prefixes[sexNr] + collectionName + "\\" + prefixes[sexNr] + "freemode_01_" + prefixes[sexNr] + collectionName + "^" + prefix + "_" + componentNumerics + "_" + postfix + "_1.ydd", true);
                        }
                    }
                    else
                    {
                        Unk_2834549053 anchor = (Unk_2834549053)cd.GetPedPropTypeID();

                        if (cd.textures.Count > 0 && (int)cd.targetSex == sexNr)
                        {
                            YmtPedDefinitionFile targetYmt = ymt;

                            var defs = ymt.Unk_376833625.PropInfo.Props[anchor] ?? new List<MUnk_94549140>();
                            var item = new MUnk_94549140(ymt.Unk_376833625.PropInfo);

                            item.AnchorId = (byte)anchor;

                            for (int i = 0; i < cd.textures.Count; i++)
                            {
                                var texture = new MUnk_254518642();
                                texture.TexId = (byte)i;
                                item.TexData.Add(texture);
                            }

                            // Get or create linked anchor
                            var aanchor = ymt.Unk_376833625.PropInfo.AAnchors.Find(e => e.Anchor == anchor);

                            if (aanchor == null)
                            {
                                aanchor = new MCAnchorProps(ymt.Unk_376833625.PropInfo)
                                {
                                    Anchor = anchor
                                };

                                aanchor.PropsMap[item] = (byte)item.TexData.Count;

                                ymt.Unk_376833625.PropInfo.AAnchors.Add(aanchor);
                            }
                            else
                            {
                                aanchor.PropsMap[item] = (byte)item.TexData.Count;
                            }

                            defs.Add(item);

                            if (!isAnyPropAdded)
                            {
                                isAnyPropAdded = true;
                                Directory.CreateDirectory(outputFolder + "\\stream");
                                Directory.CreateDirectory(outputFolder + "\\stream\\" + prefixes[sexNr] + "freemode_01_p_" + prefixes[sexNr] + collectionName);
                            }

                            int currentPropIndex = propIndexes[(byte)anchor]++;

                            string componentNumerics = currentPropIndex.ToString().PadLeft(3, '0');
                            string prefix = cd.GetPrefix();

                            File.Copy(cd.mainPath, outputFolder + "\\stream\\" + prefixes[sexNr] + "freemode_01_p_" + prefixes[sexNr] + collectionName + "\\" + prefixes[sexNr] + "freemode_01_p_" + prefixes[sexNr] + collectionName + "^" + prefix + "_" + componentNumerics + ".ydd", true);

                            char offsetLetter = 'a';
                            for (int i = 0; i < cd.textures.Count; ++i)
                            {
                                File.Copy(cd.textures[i], outputFolder + "\\stream\\" + prefixes[sexNr] + "freemode_01_p_" + prefixes[sexNr] + collectionName + "\\" + prefixes[sexNr] + "freemode_01_p_" + prefixes[sexNr] + collectionName + "^" + prefix + "_diff_" + componentNumerics + "_" + (char)(offsetLetter + i) + ".ytd", true);
                            }
                        }
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
                }

                if(isAnyClothAdded || isAnyPropAdded)
                {
                    ymt.Save(outputFolder + "\\stream\\" + prefixes[sexNr] + "freemode_01_" + prefixes[sexNr] + collectionName + ".ymt");
                    File.WriteAllText(outputFolder + "\\" + prefixes[sexNr] + "freemode_01_" + prefixes[sexNr] + collectionName + ".meta", GenerateShopMeta((Sex)sexNr, collectionName));
                    resourceLUAMetas.Add(prefixes[sexNr] + "freemode_01_" + prefixes[sexNr] + collectionName + ".meta");
                }
            }

            File.WriteAllText(outputFolder + "\\fxmanifest.lua", GenerateResourceLua(resourceLUAMetas));

            MessageBox.Show("Resource built!");
        }
    }
}
