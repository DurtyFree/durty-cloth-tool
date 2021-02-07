using System.Collections.Generic;
using System.IO;
using System.Text;
using altClothTool.App.Builders.Base;
using RageLib.Archives;
using RageLib.GTA5.Archives;
using RageLib.GTA5.ArchiveWrappers;
using RageLib.GTA5.ResourceWrappers.PC.Meta.Structures;
using RageLib.Resources.GTA5.PC.GameFiles;
using RageLib.Resources.GTA5.PC.Meta;
using static altClothTool.App.ClothData;

namespace altClothTool.App.Builders
{
    public class SingleplayerResourceBuilder
        : ResourceBuilderBase
    {
        private string GenerateSingleplayerContentXml(string collectionName, bool hasMale, bool hasFemale, bool hasMaleProps, bool hasFemaleProps)
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

        private static string GenerateSingleplayerSetup2Xml(string collectionName, int order = 1000)
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
        
        public override void BuildResource(string outputFolder, string collectionName)
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
                    YmtPedDefinitionFile ymt = CreateYmtPedDefinitionFile(Prefixes[sexNr] + collectionName, 
                        out var componentTextureBindings,
                        out var componentIndexes, 
                        out var propIndexes);

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

                                var ms = new MemoryStream();

                                currComponentRpf                     = RageArchiveWrapper7.Create(ms, FolderNames[sexNr].Replace("ped_", collectionName + "_") + ".rpf");
                                currComponentRpf.archive_.Encryption = RageArchiveEncryption7.NG;
                                currComponentDir                     = currComponentRpf.Root.CreateDirectory();
                                currComponentDir.Name                = Prefixes[sexNr] + "freemode_01_" + Prefixes[sexNr] + collectionName;
                            }

                            int currentComponentIndex = componentIndexes[componentTypeId]++;

                            string componentNumerics = currentComponentIndex.ToString().PadLeft(3, '0');
                            string prefix = clothData.GetPrefix();

                            clothData.SetComponentNumerics(componentNumerics, currentComponentIndex);

                            var resource = currComponentDir.CreateResourceFile();
                            resource.Name = prefix + "_" + componentNumerics + "_" + yddPostfix + ".ydd";
                            resource.Import(clothData.MainPath);

                            char offsetLetter = 'a';
                            for (int i = 0; i < clothData.Textures.Count; ++i)
                            {
                                resource = currComponentDir.CreateResourceFile();
                                resource.Name = prefix + "_diff_" + componentNumerics + "_" + (char)(offsetLetter + i) + "_" + ytdPostfix + ".ytd";
                                resource.Import(clothData.Textures[i]);
                            }

                            if (!string.IsNullOrEmpty(clothData.FirstPersonModelPath))
                            {
                                resource = currComponentDir.CreateResourceFile();
                                resource.Name = prefix + "_" + componentNumerics + "_" + yddPostfix + "_1.ydd";
                                resource.Import(clothData.FirstPersonModelPath);
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

                                var ms = new MemoryStream();

                                currPropRpf = RageArchiveWrapper7.Create(ms, FolderNames[sexNr].Replace("ped_", collectionName + "_") + "_p.rpf");
                                currPropRpf.archive_.Encryption = RageArchiveEncryption7.NG;
                                currPropDir = currPropRpf.Root.CreateDirectory();
                                currPropDir.Name = Prefixes[sexNr] + "freemode_01_p_" + Prefixes[sexNr] + collectionName;
                            }

                            int currentPropIndex = propIndexes[(byte)anchor]++;

                            string componentNumerics = currentPropIndex.ToString().PadLeft(3, '0');
                            string prefix = clothData.GetPrefix();

                            clothData.SetComponentNumerics(componentNumerics, currentPropIndex);

                            var resource = currPropDir.CreateResourceFile();
                            resource.Name = prefix + "_" + componentNumerics + ".ydd";
                            resource.Import(clothData.MainPath);

                            char offsetLetter = 'a';
                            for (int i = 0; i < clothData.Textures.Count; ++i)
                            {
                                resource = currPropDir.CreateResourceFile();
                                resource.Name = prefix + "_diff_" + componentNumerics + "_" + (char)(offsetLetter + i) + ".ytd";
                                resource.Import(clothData.Textures[i]);
                            }
                        }
                    }

                    if (isAnyClothAdded)
                    {
                        if (sexNr == 0)
                            hasMale = true;
                        else if (sexNr == 1)
                            hasFemale = true;

                        UpdateYmtComponentTextureBindings(componentTextureBindings, ymt);
                    }

                    if (isAnyClothAdded || isAnyPropAdded)
                    {
                        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(GenerateShopMetaContent((Sex)sexNr, collectionName))))
                        {
                            var binFile = dataDir.CreateBinaryFile();
                            binFile.Name = Prefixes[sexNr] + "freemode_01_" + Prefixes[sexNr] + collectionName + ".meta";
                            binFile.Import(stream);
                        }
                        currComponentRpf.Flush();

                        var binRpfFile = cdimagesDir.CreateBinaryFile();
                        binRpfFile.Name = FolderNames[sexNr].Replace("ped_", collectionName + "_") + ".rpf";
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
                        binRpfFile.Name = FolderNames[sexNr].Replace("ped_", collectionName + "_") + "_p.rpf";
                        binRpfFile.Import(currPropRpf.archive_.BaseStream);

                        currPropRpf.Dispose();
                    }
                }

                using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(GenerateSingleplayerContentXml(collectionName, hasMale, hasFemale, hasMaleProps, hasFemaleProps))))
                {
                    var binFile = rpf.Root.CreateBinaryFile();
                    binFile.Name = "content.xml";
                    binFile.Import(stream);
                }

                using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(GenerateSingleplayerSetup2Xml(collectionName))))
                {
                    var binFile = rpf.Root.CreateBinaryFile();
                    binFile.Name = "setup2.xml";
                    binFile.Import(stream);
                }

                rpf.Flush();
                rpf.Dispose();
            }
        }
    }
}