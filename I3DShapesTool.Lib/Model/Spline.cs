using I3DShapesTool.Lib.Container;
using System;
using System.IO;
using System.Linq;

namespace I3DShapesTool.Lib.Model
{
    public class Spline : I3DPart
    {
        public uint UnknownFlags1 { get; set; } = 0;                    //  0: form="open"      1: form="closed"
        public uint UnknownFlags2 { get; set; } = 0;                    //  Spline Attributes - optional parameter for control points (use unknown)

        public I3DVector[] Points { get; set; } = new I3DVector[0];

        public override EntityType Type => EntityType.Spline;

        public Spline()
        {
        }

        protected override void ReadContents(BinaryReader binaryReader, short fileVersion)
        {
            UnknownFlags1 = binaryReader.ReadUInt32();

            int pointCount = binaryReader.ReadInt32();
            Points = Enumerable.Range(0, pointCount)
                .Select(index => new I3DVector(binaryReader))
                .ToArray();

            if(fileVersion >= 10)
            {
                UnknownFlags2 = binaryReader.ReadUInt32();

                if (UnknownFlags2 != 0)
                {
                    throw new DecodeException("Spline contains attributes - decoding is not yet supported.");
                }
            }

/*
            Console.WriteLine("-------------------\\");
            Console.WriteLine($"fileVersion: {fileVersion}");
            Console.WriteLine($"UnknownFlags1: {UnknownFlags1}");
            Console.WriteLine($"UnknownFlags2: {UnknownFlags2}");
            Console.WriteLine("-------------------/");
*/

            if(binaryReader.BaseStream.Position != binaryReader.BaseStream.Length)
            {
                throw new DecodeException("Failed to read to all of spline data.");
            }
        }

        protected override void WriteContents(BinaryWriter writer, short fileVersion)
        {
            writer.Write(UnknownFlags1);
            writer.Write(Points.Length);
            foreach(I3DVector point in Points)
                point.Write(writer);
            writer.Write(UnknownFlags2);
        }
    }
}
