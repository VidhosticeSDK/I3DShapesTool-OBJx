﻿using System;
using System.IO;
using I3DShapesTool.Lib.Tools;
using I3DShapesTool.Lib.Tools.Extensions;

namespace I3DShapesTool.Lib.Model
{
    public class I3DShape : I3DPart
    {
        private const int VERSION_PRECOMPUTED4DDATA = 4; // Not sure of exact version here

        public float BoundingVolumeX { get; private set; }
        public float BoundingVolumeY { get; private set; }
        public float BoundingVolumeZ { get; private set; }
        public float BoundingVolumeR { get; private set; }

        /// <summary>
        /// Number of triangle corners
        /// </summary>
        public uint CornerCount { get; private set; }
        public uint NumExtraStuff { get; private set; }
        /// <summary>
        /// Number of unique vertices
        /// </summary>
        public uint VertexCount { get; private set; }
        public I3DShapeOptions Options { get; private set; }
        public I3DShapeExtra[] ExtraStuff { get; private set; }
        public bool ZeroBasedIndicesInRawData { get; private set; }
        public I3DTri[] Triangles { get; private set; }
        public I3DVector[] Positions { get; private set; }
        public I3DVector[]? Normals { get; private set; }
        public I3DVector4[]? Some4DData { get; private set; }
        public I3DUV[][] UVSets { get; private set; }
        public I3DVector4[]? Unknown4DVect { get; private set; }
        public byte[]? UnknownData1_1 { get; private set; }
        public byte[]? UnknownData1_2 { get; private set; }
        public float[]? UnknownData2 { get; private set; }
        public I3DShapeExtra2[] UnknownData3 { get; private set; }

#nullable disable
        public I3DShape(byte[] rawData, Endian endian, int version)
            : base(ShapeType.Shape, rawData, endian, version)
        {
        }
#nullable restore

        protected override void ReadContents(BinaryReader reader)
        {
            BoundingVolumeX = reader.ReadSingle();
            BoundingVolumeY = reader.ReadSingle();
            BoundingVolumeZ = reader.ReadSingle();
            BoundingVolumeR = reader.ReadSingle();
            CornerCount = reader.ReadUInt32();
            NumExtraStuff = reader.ReadUInt32();
            VertexCount = reader.ReadUInt32();
            Options = (I3DShapeOptions)reader.ReadUInt32();
            ExtraStuff = new I3DShapeExtra[NumExtraStuff];
            for(var i = 0; i < NumExtraStuff; i++)
            {
                ExtraStuff[i] = new I3DShapeExtra(reader, Version, Options);
            }

            //if (CornerCount > 65536) // This has a special case in the loading which is not RE yet
            //    throw new NotImplementedException("Models with more than 65536 vertices is not supported at the moment");

            ZeroBasedIndicesInRawData = false;
            Triangles = new I3DTri[CornerCount / 3];
            for (var i = 0; i < CornerCount / 3; i++)
            {
                Triangles[i] = new I3DTri(reader);

                if (Triangles[i].P1Idx == 0 || Triangles[i].P2Idx == 0 || Triangles[i].P3Idx == 0)
                    ZeroBasedIndicesInRawData = true;
            }

            // Convert to 1-based indices if it's detected that it is a zero-based index
            if (ZeroBasedIndicesInRawData)
            {
                foreach (var t in Triangles)
                {
                    t.P1Idx += 1;
                    t.P2Idx += 1;
                    t.P3Idx += 1;
                }
            }

            // TODO: figure out the exact logic for this
            //if (Version < 4) // Could be 5 as well
                reader.BaseStream.Align(4);

            Positions = new I3DVector[VertexCount];
            for (var i = 0; i < VertexCount; i++)
            {
                Positions[i] = new I3DVector(reader);
            }

            if (Options.HasFlag(I3DShapeOptions.HasNormals))
            {
                Normals = new I3DVector[VertexCount];
                for (var i = 0; i < VertexCount; i++)
                {
                    Normals[i] = new I3DVector(reader);
                }
            }

            if (Version >= VERSION_PRECOMPUTED4DDATA && Options.HasFlag(I3DShapeOptions.HasPrecomputed4DVectorData))
            {
                Some4DData = new I3DVector4[VertexCount];
                for (var i = 0; i < VertexCount; i++)
                {
                    Some4DData[i] = new I3DVector4(reader);
                }
            }

            UVSets = new I3DUV[4][];
            for(int uvSet = 0; uvSet < 4; uvSet++)
            {
                if(Options.HasFlag((I3DShapeOptions)((uint)I3DShapeOptions.HasUV1 << uvSet)))
                {
                    var uvs = new I3DUV[VertexCount];
                    for(var i = 0; i < VertexCount; i++)
                    {
                        uvs[i] = new I3DUV(reader, Version);
                    }
                    UVSets[uvSet] = uvs;
                }
            }

            if (Options.HasFlag(I3DShapeOptions.HasUnknown4DVect))
            {
                Unknown4DVect = new I3DVector4[VertexCount];
                for(var i = 0; i < VertexCount; i++)
                {
                    Unknown4DVect[i] = new I3DVector4(reader);
                }
            }

            // I have no idea what this data is, let alone how to store it properly
            if (Options.HasFlag(I3DShapeOptions.HasUnknownData1))
            {
                bool isSingleValue = Options.HasFlag(I3DShapeOptions.UnknownData1IsSingleValue);
                int numValues = isSingleValue ? 1 : 4;

                if (!isSingleValue)
                {
                    UnknownData1_1 = reader.ReadBytes((int)(4 * VertexCount * numValues));
                }
                UnknownData1_2 = reader.ReadBytes((int)(VertexCount * numValues));
            }

            if (Options.HasFlag(I3DShapeOptions.HasUnknownData2))
            {
                UnknownData2 = new float[VertexCount];
                for (var i = 0; i < VertexCount; i++)
                {
                    UnknownData2[i] = reader.ReadSingle();
                }
            }

            /*
            TODO: Implement this if necessary
            if (Version < 5 && !Options.HasFlag(I3DShapeOptions.HasPrecomputed4DVectorData))
            {
                Some4DData = Compute4DData(Vertices, Normals, FirstNonNullUVMap);
            }
            */

            /*
            TODO: Implement this if necessary
            if (Version < 6)
            {
                foreach(var extra in ExtraStuff)
                {
                    // Loop over the 4 UV floats in extra and set them based on some math done on the vertex and UV set data
                }
            }
            */

            uint numUnknownData3 = reader.ReadUInt32();
            UnknownData3 = new I3DShapeExtra2[numUnknownData3];
            for(int i = 0; i < numUnknownData3; i++)
            {
                UnknownData3[i] = new I3DShapeExtra2(reader);
            }

            if (reader.BaseStream.Position != reader.BaseStream.Length)
                throw new DecodeException("Failed to read the entire shape data");
        }

        protected override void WriteContents(BinaryWriter writer)
        {
            writer.Write(BoundingVolumeX);
            writer.Write(BoundingVolumeY);
            writer.Write(BoundingVolumeZ);
            writer.Write(BoundingVolumeR);
            writer.Write(CornerCount);
            writer.Write((uint)ExtraStuff.Length);
            writer.Write(VertexCount);
            writer.Write((uint)Options);
            foreach (var extra in ExtraStuff)
                extra.Write(writer, Version, Options);

            foreach (var tri in Triangles)
                tri.Write(writer, ZeroBasedIndicesInRawData);

            writer.Align(4);

            foreach (var pos in Positions)
                pos.Write(writer);

            if (Options.HasFlag(I3DShapeOptions.HasNormals))
            {
                if (Normals == null)
                    throw new InvalidOperationException("Options say we have normals but Normals field is null");

                foreach (var norm in Normals)
                    norm.Write(writer);
            }

            if (Version >= VERSION_PRECOMPUTED4DDATA && Options.HasFlag(I3DShapeOptions.HasPrecomputed4DVectorData))
            {
                if (Some4DData == null)
                    throw new InvalidOperationException("Options say we have 4d data but Some4DData field is null");

                foreach (var vec in Some4DData)
                    vec.Write(writer);
            }

            for (int uvSet = 0; uvSet < 4; uvSet++)
            {
                if (Options.HasFlag((I3DShapeOptions)((uint)I3DShapeOptions.HasUV1 << uvSet)))
                {
                    if (UVSets[uvSet] == null)
                        throw new InvalidOperationException($"Options say we have UV set {uvSet + 1} but UVSets[{uvSet}] is null");

                    foreach (var uv in UVSets[uvSet])
                        uv.Write(writer, Version);
                }
            }

            if (Options.HasFlag(I3DShapeOptions.HasUnknown4DVect))
            {
                if (Unknown4DVect == null)
                    throw new InvalidOperationException("Options say we have Unknown4DVect but Unknown4DVect field is null");

                foreach (var vec in Unknown4DVect)
                    vec.Write(writer);
            }

            if (Options.HasFlag(I3DShapeOptions.HasUnknownData1))
            {
                if (UnknownData1_2 == null)
                    throw new InvalidOperationException("Options say we have UnknownData1 but UnknownData1_2 field is null");

                bool isSingleValue = Options.HasFlag(I3DShapeOptions.UnknownData1IsSingleValue);
                if (!isSingleValue)
                {
                    if (UnknownData1_2 == null)
                        throw new InvalidOperationException("Options say we have UnknownData1 not single value but UnknownData1_1 field is null");

                    writer.Write(UnknownData1_1);
                }
                writer.Write(UnknownData1_2);
            }

            if (Options.HasFlag(I3DShapeOptions.HasUnknownData2))
            {
                if (UnknownData2 == null)
                    throw new InvalidOperationException("Options say we have UnknownData2 but UnknownData2 field is null");

                foreach (var v in UnknownData2)
                    writer.Write(v);
            }

            writer.Write((uint)UnknownData3.Length);
            foreach (var data in UnknownData3)
                data.Write(writer);
        }

        public override string ToString()
        {
            return $"I3DShape #{Id} V{Version} {Name}";
        }
    }
}