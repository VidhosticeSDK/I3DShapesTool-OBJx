using System.Linq;
using I3DShapesTool.Lib.Model;
using I3DShapesTool.Lib.Tools;
using System.IO;

namespace I3DShapesTool.Lib.Export
{
    public class WavefrontObj : IExporter
    {
        private string Name { get; }

        private string GeometryName { get; }

        private float Scale { get; }

        private I3DTri[] Triangles { get; set; }

        private I3DVector[] Positions { get; set; }

        private I3DVector[] Normals { get; set; }

        private I3DUV[] UVs { get; set; }
        private I3DUV[] UVs2 { get; set; }
        private I3DUV[] UVs3 { get; set; }
        private I3DUV[] UVs4 { get; set; }

        private I3DVector4[] Colors { get; set; }

        public WavefrontObj(I3DShape shape, string name, float scale = 1)
        {
            Scale = scale;

            string geomname = shape.Name;
            if(geomname.EndsWith("Shape"))
                geomname = geomname.Substring(0, geomname.Length - 5);

            Name = name;
            GeometryName = geomname;
            Positions = shape.Positions;
            Triangles = shape.Triangles;

            if(shape.Normals != null)
                Normals = shape.Normals;

            if(shape.UVSets.Length > 0)
                UVs = shape.UVSets[0];

            if(shape.UVSets.Length > 1)
                UVs2 = shape.UVSets[1];

            if(shape.UVSets.Length > 2)
                UVs3 = shape.UVSets[2];

            if(shape.UVSets.Length > 3)
                UVs4 = shape.UVSets[3];

            if(shape.VertexColor != null)
                Colors = shape.VertexColor;
        }

        /// <summary>
        /// Transforms the vertices of this obj using the transformation matrix
        /// </summary>
        /// <param name="t">Transformation matrix</param>
        public void Transform(Transform t)
        {
            Positions = Positions.Select(v => t * v).ToArray();
        }

        /// <summary>
        /// Merges the vertex data of another WavefrontObj into this
        /// </summary>
        /// <param name="newObj"></param>
        public void Merge(WavefrontObj newObj)
        {
            uint oldVertCnt = (uint)Positions.Length;

            I3DVector[] newPos = new I3DVector[Positions.Length + newObj.Positions.Length];
            Positions.CopyTo(newPos, 0);
            newObj.Positions.CopyTo(newPos, oldVertCnt);

            if(Normals != null || newObj.Normals != null)
            {
                I3DVector[] newNorm = new I3DVector[(Normals?.Length ?? 0) + (newObj.Normals?.Length ?? 0)];
                Normals?.CopyTo(newNorm, 0);
                newObj.Normals?.CopyTo(newNorm, Normals?.Length ?? 0);
                Normals = newNorm;
            }

            if(UVs != null || newObj.UVs != null)
            {
                I3DUV[] newUV = new I3DUV[(UVs?.Length ?? 0) + (newObj.UVs?.Length ?? 0)];
                UVs?.CopyTo(newUV, 0);
                newObj.UVs?.CopyTo(newUV, UVs?.Length ?? 0);
                UVs = newUV;
            }

            if(UVs2 != null || newObj.UVs2 != null)
            {
                I3DUV[] newUV2 = new I3DUV[(UVs2?.Length ?? 0) + (newObj.UVs2?.Length ?? 0)];
                UVs2?.CopyTo(newUV2, 0);
                newObj.UVs2?.CopyTo(newUV2, UVs2?.Length ?? 0);
                UVs2 = newUV2;
            }

            if(UVs3 != null || newObj.UVs3 != null)
            {
                I3DUV[] newUV3 = new I3DUV[(UVs3?.Length ?? 0) + (newObj.UVs3?.Length ?? 0)];
                UVs3?.CopyTo(newUV3, 0);
                newObj.UVs3?.CopyTo(newUV3, UVs3?.Length ?? 0);
                UVs3 = newUV3;
            }

            if(UVs4 != null || newObj.UVs4 != null)
            {
                I3DUV[] newUV4 = new I3DUV[(UVs4?.Length ?? 0) + (newObj.UVs4?.Length ?? 0)];
                UVs4?.CopyTo(newUV4, 0);
                newObj.UVs4?.CopyTo(newUV4, UVs4?.Length ?? 0);
                UVs4 = newUV4;
            }

            if(Colors != null || newObj.Colors != null)
            {
                I3DVector4[] newColor = new I3DVector4[(Colors?.Length ?? 0) + (newObj.Colors?.Length ?? 0)];
                Colors?.CopyTo(newColor, 0);
                newObj.Colors?.CopyTo(newColor, Colors?.Length ?? 0);
                Colors = newColor;
            }

            I3DTri[] newTris = new I3DTri[Triangles.Length + newObj.Triangles.Length];
            Triangles.CopyTo(newTris, 0);
            for(int i = 0; i < newObj.Triangles.Length; i++)
            {
                I3DTri newObjTr = newObj.Triangles[i];
                newTris[Triangles.Length + i] = new I3DTri(newObjTr.P1Idx + oldVertCnt, newObjTr.P2Idx + oldVertCnt, newObjTr.P3Idx + oldVertCnt);
            }

            Positions = newPos;
            Triangles = newTris;
        }

        private void WriteHeader(StreamWriter s)
        {
            s.WriteLine("# Wavefront OBJ file");
            s.WriteLine("# Creator: I3DShapesTool by Donkie");
            s.WriteLine("# Name: {0:G}", Name);
            s.WriteLine("# Scale: {0:F}", Scale);
        }

        private static void WriteGroup(StreamWriter s, string groupName)
        {
            s.WriteLine("g {0:G}", groupName);
        }

        private static void WriteSmoothing(StreamWriter s, bool smoothOn)
        {
            s.WriteLine("s {0:G}", smoothOn ? "on" : "off");
        }

        private void WriteVertex(StreamWriter s, I3DVector vec)
        {
            s.WriteLine("v {0:F6} {1:F6} {2:F6}", vec.X * Scale, vec.Y * Scale, vec.Z * Scale);
        }

        private void WriteVertexWithColor(StreamWriter s, I3DVector vec, I3DVector4 col)
        {
            s.WriteLine("v {0:F6} {1:F6} {2:F6} {3:F6} {4:F6} {5:F6}", vec.X * Scale, vec.Y * Scale, vec.Z * Scale, col.X, col.Y, col.Z);
        }

        private static void WriteUV(StreamWriter s, I3DUV uv)
        {
            s.WriteLine("vt {0:F6} {1:F6}", uv.U, uv.V);
        }

        private static void WriteUV2(StreamWriter s, I3DUV uv)
        {
            s.WriteLine("vt2 {0:F6} {1:F6}", uv.U, uv.V);
        }

        private static void WriteUV3(StreamWriter s, I3DUV uv)
        {
            s.WriteLine("vt3 {0:F6} {1:F6}", uv.U, uv.V);
        }

        private static void WriteUV4(StreamWriter s, I3DUV uv)
        {
            s.WriteLine("vt4 {0:F6} {1:F6}", uv.U, uv.V);
        }

        private static void WriteNormal(StreamWriter s, I3DVector vec)
        {
            s.WriteLine("vn {0:F6} {1:F6} {2:F6}", vec.X, vec.Y, vec.Z);
        }

        private void WriteTriangleFace(StreamWriter s, uint idx)
        {
            s.Write("{0:F0}", idx);

            if(UVs != null)
                s.Write("/{0:F0}", idx);
            else if(Normals != null)
                s.Write('/');

            if(Normals != null)
                s.Write("/{0:F0}", idx);
        }

        private void WriteTriangle(StreamWriter s, I3DTri tri)
        {
            s.Write("f ");
            WriteTriangleFace(s, tri.P1Idx);
            s.Write(" ");
            WriteTriangleFace(s, tri.P2Idx);
            s.Write(" ");
            WriteTriangleFace(s, tri.P3Idx);
            s.WriteLine();
        }

        /// <summary>
        /// Writes the .obj data to a stream
        /// </summary>
        /// <param name="stream">The stream</param>
        public void Export(Stream stream)
        {
            using(StreamWriter s = new InvariantStreamWriter(stream))
            {
                WriteHeader(s);
                s.WriteLine();
                WriteGroup(s, "default");
                s.WriteLine();

                if(Colors == null)
                {
                    foreach(I3DVector t in Positions)
                    {
                        WriteVertex(s, t);
                    }
                }
                else
                {
                    for( int i = 0; i < Positions.Length; i++)
                    {
                        WriteVertexWithColor(s, Positions[i], Colors[i]);
                    }
                }

                if(UVs != null)
                {
                    foreach(I3DUV t in UVs)
                    {
                        WriteUV(s, t);
                    }
                }
                if(UVs2 != null)
                {
                    foreach(I3DUV t in UVs2)
                    {
                        WriteUV2(s, t);
                    }
                }
                if(UVs3 != null)
                {
                    foreach(I3DUV t in UVs3)
                    {
                        WriteUV3(s, t);
                    }
                }
                if(UVs4 != null)
                {
                    foreach(I3DUV t in UVs4)
                    {
                        WriteUV4(s, t);
                    }
                }
                if(Normals != null)
                {
                    foreach(I3DVector t in Normals)
                    {
                        WriteNormal(s, t);
                    }
                }
                WriteSmoothing(s, false);
                WriteGroup(s, GeometryName);
                foreach(I3DTri t in Triangles)
                {
                    WriteTriangle(s, t);
                }
            }
        }
    }
}
