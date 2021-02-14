﻿using HSDRaw;
using HSDRaw.Common;
using HSDRaw.MEX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace HSDRawViewer.Sound
{
    public enum SEM_OP_CODE
    {
        UNKNOWN00,
        SFXID,
        UNKNOWN02,
        UNKNOWN03,
        PRIORITY,
        UNKNOWN05, // Unused
        UNKNOWN06,
        UNKNOWN07,
        CHANNEL,
        UNKNOWN09,
        UNKNOWN0A, // Unused
        UNKNOWN0B, // Unused
        PITCH,
        UNKNOWN0D,
        END,
        LOOP,
        REVERB,
        UNKNOWN11,
        UNKNOWN12, // Unused
        UNKNOWN13, // Unused
        UNKNOWN14,
        UNKNOWN15, // Unused
        NULL = 0xFD
    }

    /// <summary>
    /// Class for help with reading and writing sem files
    /// </summary>
    public class SEM
    {
        /// <summary>
        /// 
        /// </summary>
        private static readonly int NullEntryID = 55;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public static string DecompileSEMScript(byte[] data)
        {
            StringBuilder s = new StringBuilder();
            for (int i = 0; i < data.Length;)
            {
                var op = data[i++];
                i++;
                var val = (short)(((data[i++] & 0xFF) << 8) | ((data[i++] & 0xFF)));
                s.AppendLine($".{(SEM_OP_CODE)op} : {val}");
            }
            return s.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static int CompileSEMScript(string script, out byte[] data)
        {
            var cmd = Regex.Replace(script, @"\s+", string.Empty).Split('.');

            data = new byte[4 * (cmd.Length - 1)];

            int i = 0;
            foreach (var line in cmd)
            {
                var args = line.Split(new string[] { ":" }, StringSplitOptions.None);

                if (args.Length != 2)
                    continue;

                data[i++] = (byte)(SEM_OP_CODE)Enum.Parse(typeof(SEM_OP_CODE), args[0]);
                var d = int.Parse(args[1]);
                data[i++] = (byte)((d >> 16) & 0xFF);
                data[i++] = (byte)((d >> 8) & 0xFF);
                data[i++] = (byte)(d & 0xFF);
            }
            
            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<SEMEntry> ReadSEMFile(string filePath)
        {
            var o = MessageBox.Show("Load Sound Banks(SSM) as well?", "Load SSM Files?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes;
            return ReadSEMFile(filePath, o);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<SEMEntry> ReadSEMFile(string filePath, bool loadSoundBanks, MEX_Data mexData = null)
        {
            var Entries = new List<SEMEntry>();

            Dictionary<int, SSM> indexToSSM = new Dictionary<int, SSM>();

            if(loadSoundBanks)
            {
                foreach(var f in Directory.GetFiles(Path.GetDirectoryName(filePath)))
                {
                    if (f.ToLower().EndsWith(".ssm"))
                    {
                        var ssm = new SSM();
                        ssm.Open(f);
                        if(!indexToSSM.ContainsKey(ssm.StartIndex))
                            indexToSSM.Add(ssm.StartIndex, ssm);
                    }
                }
            }

            //if (!loadSoundBanks)
            //    return Entries;

            using (BinaryReaderExt r = new BinaryReaderExt(new FileStream(filePath, FileMode.Open)))
            {
                r.BigEndian = true;

                r.Seek(8);
                var entryCount = r.ReadInt32();

                var offsetTableStart = r.Position + (entryCount + 1) * 4;

                for (uint i = 0; i < entryCount; i++)
                {
                    SEMEntry e = new SEMEntry();
                    Entries.Add(e);

                    r.Seek(0x0C + i * 4);
                    var startIndex = r.ReadInt32();
                    var endIndex = r.ReadInt32();

                    var ssmStartIndex = int.MaxValue;
                    for (uint j = 0; j < endIndex - startIndex; j++)
                    {
                        SEMScript s = new SEMScript();
                        r.Seek((uint)(offsetTableStart + startIndex * 4 + j * 4));
                        var dataOffsetStart = r.ReadUInt32();
                        var dataOffsetEnd = r.ReadUInt32();

                        if (dataOffsetEnd == 0)
                            dataOffsetEnd = (uint)r.Length;

                        r.Seek(dataOffsetStart);
                        s.CommandData = r.ReadBytes((int)(dataOffsetEnd - dataOffsetStart));
                        e.AddScript(s);

                        ssmStartIndex = Math.Min(ssmStartIndex, s.SoundCommandIndex);
                    }
                    
                    if (loadSoundBanks && indexToSSM.ContainsKey(ssmStartIndex))
                    {
                        e.SoundBank = indexToSSM[ssmStartIndex];

                        if (mexData != null)
                        {
                            var index = mexData.SSMTable.SSM_SSMFiles.Array.ToList().FindIndex(s=>s.Value.Equals(e.SoundBank.Name));
                            if(index != -1)
                            {
                                e.SoundBank.GroupFlags = mexData.SSMTable.SSM_LookupTable[index].EntireFlag;
                                e.SoundBank.Flag = mexData.SSMTable.SSM_BufferSizes[index].Flag;
                            }
                        }

                        foreach (var v in e.Scripts)
                            v.SoundCommandIndex -= ssmStartIndex;
                    }
                }
            }
            return Entries;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static byte[] GeneratePaddedBuffer(int size, byte value)
        {
            byte[] b = new byte[size];
            for (int i = 0; i < b.Length; i++)
                b[i] = value;
            return b;
        }

        /// <summary>
        /// Generates and saves a SEM file
        /// </summary>
        public static void SaveSEMFile(string path, List<SEMEntry> Entries, MEX_Data mexData)
        {
            if(mexData != null)
            {
                mexData.SSMTable.SSM_SSMFiles.Array = new HSD_String[0];
                mexData.SSMTable.SSM_BufferSizes.Array = new MEX_SSMSizeAndFlags[0];
                mexData.SSMTable.SSM_LookupTable.Array = new MEX_SSMLookup[0];
                
                mexData.SSMTable.SSM_BufferSizes.Set(Entries.Count, new MEX_SSMSizeAndFlags());// blank entry at end
                mexData.SSMTable.SSM_LookupTable.Set(Entries.Count, new MEX_SSMLookup());// blank entry at beginning

                // generate runtime struct
                mexData.MetaData.NumOfSSMs = Entries.Count;

                HSDStruct rtTable = new HSDStruct(6 * 4);

                rtTable.SetReferenceStruct(0x00, new HSDStruct(GeneratePaddedBuffer(0x180, 0x01)));
                rtTable.SetReferenceStruct(0x04, new HSDStruct(GeneratePaddedBuffer(Entries.Count * 4, 0x02)));
                rtTable.SetReferenceStruct(0x08, new HSDStruct(GeneratePaddedBuffer(Entries.Count * 4, 0x03)));
                rtTable.SetReferenceStruct(0x0C, new HSDStruct(GeneratePaddedBuffer(Entries.Count * 4, 0x04)));
                rtTable.SetReferenceStruct(0x10, new HSDStruct(GeneratePaddedBuffer(Entries.Count * 4, 0x05)));
                rtTable.SetReferenceStruct(0x14, new HSDStruct(GeneratePaddedBuffer(Entries.Count * 4, 0x06)));
                
                mexData.SSMTable._s.SetReferenceStruct(0x0C, rtTable);
            }

            var soundOffset = 0;
            using (BinaryWriterExt w = new BinaryWriterExt(new FileStream(path, FileMode.Create)))
            {
                w.BigEndian = true;

                w.Write(0);
                w.Write(0);
                w.Write(Entries.Count);
                int index = 0;
                foreach (var e in Entries)
                {
                    w.Write(index);
                    index += e.Scripts.Length;
                }
                w.Write(index);

                var offset = w.BaseStream.Position + 4 * index + 4;
                var dataindex = 0;

                foreach (var e in Entries)
                {
                    foreach (var v in e.Scripts)
                    {
                        w.Write((int)(offset + dataindex));
                        dataindex += v.CommandData.Length;
                    }
                }

                w.Write(0);

                int entryIndex = -1;
                foreach (var e in Entries)
                {
                    entryIndex++;
                    // fix sound offset ids
                    if (e.SoundBank != null)
                    {
                        // set start offset in sem and save
                        e.SoundBank.StartIndex = soundOffset;
                        int bufSize;
                        e.SoundBank.Save(Path.GetDirectoryName(path) + "\\" + e.SoundBank.Name, out bufSize);

                        if(mexData != null)
                        {
                            mexData.SSMTable.SSM_SSMFiles.Set(entryIndex, new HSD_String() { Value = e.SoundBank.Name });
                            mexData.SSMTable.SSM_BufferSizes.Set(entryIndex, new MEX_SSMSizeAndFlags() { Flag = e.SoundBank.Flag, SSMFileSize = bufSize});
                            var lu = new MEX_SSMLookup();
                            lu._s.SetInt32(0x00, e.SoundBank.GroupFlags);
                            mexData.SSMTable.SSM_LookupTable.Set(entryIndex, lu);
                        }

                        // add sound offset
                        foreach (var v in e.Scripts)
                            v.SoundCommandIndex += soundOffset;

                        foreach (var v in e.Scripts)
                            w.Write(v.CommandData);

                        //return to normal
                        foreach (var v in e.Scripts)
                            v.SoundCommandIndex -= soundOffset;

                        soundOffset += e.SoundBank.Sounds.Length;
                    }
                    else
                    {
                        foreach (var v in e.Scripts)
                        {
                            w.Write(v.CommandData);
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        private static void DumpUniqueCommands(string path, IEnumerable<SEMEntry> entries)
        {
            Dictionary<byte, List<int>> commands = new Dictionary<byte, List<int>>();

            foreach (var e in entries)
            {
                foreach (var s in e.Scripts)
                {
                    int p = 0;
                    while (p < s.CommandData.Length)
                    {
                        var cmd = s.CommandData[p++];
                        var value = ((s.CommandData[p++] & 0xFF) << 16)
                        | ((s.CommandData[p++] & 0xFF) << 8)
                        | ((s.CommandData[p++] & 0xFF));

                        if (!commands.ContainsKey(cmd))
                            commands.Add(cmd, new List<int>());

                        if (!commands[cmd].Contains(value))
                            commands[cmd].Add(value);
                    }
                }
            }


            // Dump command Info
            foreach (var k in commands)
            {
                using (StreamWriter w = new StreamWriter(new FileStream(path + "\\" + k.Key.ToString("X2") + ".txt", FileMode.Create)))
                {
                    k.Value.Sort();
                    foreach (var v in k.Value)
                        w.WriteLine(v);
                }
            }
        }
    }

}
