﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace I3DShapesTool
{
    class Program
    {
        private static void Main(string[] args)
        {

            string path = args.Length > 0 ? args[0] : @"D:\SteamLibrary\steamapps\common\Farming Simulator 2013\data\vehicles\balers\kroneBigPack1290.i3d.shapes";
            List<I3DShape> shapes = new List<I3DShape>();

            using (FileStream fs = File.OpenRead(path))
            {
                string fileName = Path.GetFileName(fs.Name) ?? "N/A";
                Console.WriteLine("Loading file: " + fileName);

                byte seed = (byte)fs.ReadInt16L();
                Console.WriteLine("File Seed: " + seed);

                short version = fs.ReadInt16L();
                Console.WriteLine("File Version: " + version);

                if (version != 2)
                    throw new NotSupportedException("Unsupported version");

                Console.WriteLine();
                
                using (I3DDecryptorStream dfs = new I3DDecryptorStream(fs, seed))
                {
                    int itemCount = dfs.ReadInt32L();
                    Console.WriteLine("Found " + itemCount + " items");
                    Console.WriteLine();
                    for (int i = 0; i < itemCount; i++)
                    {
                        Console.WriteLine("Loading item " + (i + 1));

                        int type = dfs.ReadInt32L();
                        Console.WriteLine("Type: " + type);
                        int size = dfs.ReadInt32L();
                        Console.WriteLine("Size: " + size);
                        byte[] data = dfs.ReadBytes(size);

                        I3DShape shape;

                        using (MemoryStream ms = new MemoryStream(data))
                        {
                            using (BigEndianBinaryReader br = new BigEndianBinaryReader(ms))
                            {
                                shape = new I3DShape(br);
                            }
                        }

                        shapes.Add(shape);

                        string folder = Path.Combine(@"C:\Users\Daniel\Desktop\", fileName);
                        Directory.CreateDirectory(folder);

                        string filename = shape.Name + ".obj";

                        File.WriteAllBytes(Path.Combine(folder, CleanFileName(filename)), data);

                        Console.WriteLine();
                    }
                }
            }

            Console.Read();
        }

        /// <summary>
        /// https://stackoverflow.com/a/7393722/2911165
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static string CleanFileName(string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
        }
    }
}
