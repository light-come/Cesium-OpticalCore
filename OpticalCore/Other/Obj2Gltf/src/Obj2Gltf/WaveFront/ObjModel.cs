﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace Arctron.Obj2Gltf.WaveFront
{
    /// <summary>
    ///  represents an obj file model
    /// </summary>
    public class ObjModel
    {
        public string Name { get; set; }
        /// <summary>
        /// obj used mat file path
        /// </summary>
        public string MatFilename { get; set; }
        /// <summary>
        /// vertices coordinates list
        /// </summary>
        public List<Vec3> Vertices { get; set; } = new List<Vec3>();
        /// <summary>
        /// vertices normal list
        /// </summary>
        public List<Vec3> Normals { get; set; } = new List<Vec3>();
        /// <summary>
        /// vertices texture coordinates list
        /// </summary>
        public List<Vec2> Uvs { get; set; } = new List<Vec2>();
        /// <summary>
        /// grouped geometries
        /// </summary>
        public List<Geometry> Geometries { get; set; } = new List<Geometry>();
        /// <summary>
        /// mat list from mat file
        /// </summary>
        public List<Material> Materials { get; set; } = new List<Material>();
        /// <summary>
        /// write obj file
        /// </summary>
        /// <param name="writer"></param>
        public void Write(StreamWriter writer)
        {
            writer.WriteLine("# File generated by Arctron BIMClient");
            if (!String.IsNullOrEmpty(MatFilename))
            {
                writer.WriteLine($"mtllib {MatFilename}");
            }
            var vs = String.Join(Environment.NewLine, Vertices.Select(v => $"v {v.X} {v.Y} {v.Z}"));
            writer.WriteLine(vs);
            writer.Flush();
            var ts = String.Join(Environment.NewLine, Uvs.Select(t => $"vt {t.U} {t.V}"));
            writer.WriteLine(ts);
            writer.Flush();
            var ns = String.Join(Environment.NewLine, Normals.Select(n => $"vn {n.X} {n.Y} {n.Z}"));
            writer.WriteLine(ns);
            writer.Flush();
            foreach (var g in Geometries)
            {
                g.Write(writer);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="level">will generate level^3 models</param>
        /// <returns></returns>
        public List<ObjModel> Split(int level)
        {
            if (level <= 1)
            {
                return new List<ObjModel> { this };
            }
            var box = GetBounding();
            var boxes = box.Split(level);
            var geoes = new List<Geometry>[boxes.Count];
            var pnts = new List<int>[boxes.Count];
            var normals = new List<int>[boxes.Count];
            var uvs = new List<int>[boxes.Count];
            for (var i = 0; i < geoes.Length; i++)
            {
                geoes[i] = new List<Geometry>();
                pnts[i] = new List<int>();
                normals[i] = new List<int>();
                uvs[i] = new List<int>();
            }
            foreach(var g in Geometries)
            {
                var geoBox = GetBoxIndex(g, boxes);
                var index = geoBox.Index;
                var gg = AddGeo(g, geoBox, pnts[index], normals[index], uvs[index]);
                geoes[index].Add(gg);
            }
            var objModels = new List<ObjModel>();
            for(var i = 0;i< geoes.Length;i++)
            {
                if (geoes[i].Count == 0) continue;
                var m = new ObjModel { Geometries = geoes[i], Name = Name + "_" + objModels.Count,
                    MatFilename = MatFilename, Materials = Materials };
                if (m.Vertices == null) m.Vertices = new List<Vec3>();
                var ps = pnts[i];
                foreach(var v in ps)
                {
                    m.Vertices.Add(Vertices[v - 1]);
                }
                if (m.Normals == null) m.Normals = new List<Vec3>();
                var ns = normals[i];
                foreach(var n in ns)
                {
                    m.Normals.Add(Normals[n - 1]);
                }
                if (m.Uvs == null) m.Uvs = new List<Vec2>();
                var ts = uvs[i];
                foreach(var t in ts)
                {
                    m.Uvs.Add(Uvs[t - 1]);
                }
                objModels.Add(m);
            }
            return objModels;
        }

        private static FaceVertex GetVertex(FaceVertex v, Dictionary<int, int> pnts, 
            Dictionary<int, int> normals, Dictionary<int, int> uvs)
        {
            var v1p = v.V;
            var v1n = v.N;
            var v1t = v.T;
            if (v1p > 0)
            {
                v1p = pnts[v1p];
            }
            if (v1n > 0)
            {
                v1n = normals[v1n];
            }
            if (v1t > 0)
            {
                v1t = uvs[v1t];
            }
            return new FaceVertex(v1p, v1t, v1n);
        }

        private Geometry AddGeo(Geometry g, GeomBox box, 
            List<int> pnts, List<int> normals, List<int> uvs)
        {
            var gg = new Geometry { Id = g.Id };

            var pntList = box.Pnts; // new List<int>(); // 
            var normList = box.Norms; // new List<int>(); // 
            var uvList = box.Uvs; // new List<int>(); // 

            //if (pntList.Count == 0)
            //{
            //    foreach (var f in g.Faces)
            //    {
            //        foreach (var t in f.Triangles)
            //        {
            //            var v1 = t.V1;
            //            if (!pntList.Contains(v1.V))
            //            {
            //                pntList.Add(v1.V);
            //            }
            //            if (v1.N > 0 && !normList.Contains(v1.N))
            //            {
            //                normList.Add(v1.N);
            //            }
            //            if (v1.T > 0 && !uvList.Contains(v1.T))
            //            {
            //                uvList.Add(v1.T);
            //            }
            //            var v2 = t.V2;
            //            if (!pntList.Contains(v2.V))
            //            {
            //                pntList.Add(v2.V);
            //            }
            //            if (v2.N > 0 && !normList.Contains(v2.N))
            //            {
            //                normList.Add(v2.N);
            //            }
            //            if (v2.T > 0 && !uvList.Contains(v2.T))
            //            {
            //                uvList.Add(v2.T);
            //            }
            //            var v3 = t.V3;
            //            if (!pntList.Contains(v3.V))
            //            {
            //                pntList.Add(v3.V);
            //            }
            //            if (v3.N > 0 && !normList.Contains(v3.N))
            //            {
            //                normList.Add(v3.N);
            //            }
            //            if (v3.T > 0 && !uvList.Contains(v3.T))
            //            {
            //                uvList.Add(v3.T);
            //            }
            //        }

            //    }
            //}
            

            var pntDict = new Dictionary<int, int>();
            var normDict = new Dictionary<int, int>();
            var uvDict = new Dictionary<int, int>();

            foreach (var p in pntList)
            {
                var index = pnts.IndexOf(p);
                if (index == -1)
                {
                    index = pnts.Count;
                    pnts.Add(p);
                }
                pntDict.Add(p, index + 1);
            }

            foreach (var n in normList)
            {
                var index = normals.IndexOf(n);
                if (index == -1)
                {
                    index = normals.Count;
                    normals.Add(n);
                }
                normDict.Add(n, index + 1);
            }

            foreach (var t in uvList)
            {
                var index = uvs.IndexOf(t);
                if (index == -1)
                {
                    index = uvs.Count;
                    uvs.Add(t);
                }
                uvDict.Add(t, index + 1);
            }


            foreach (var f in g.Faces)
            {
                var ff = new Face { MatName = f.MatName };                

                foreach (var t in f.Triangles)
                {
                    var v1 = GetVertex(t.V1, pntDict, normDict, uvDict);
                    var v2 = GetVertex(t.V2, pntDict, normDict, uvDict);
                    var v3 = GetVertex(t.V3, pntDict, normDict, uvDict);
                    var fv = new FaceTriangle(v1, v2, v3);
                    ff.Triangles.Add(fv);
                }

                gg.Faces.Add(ff);
            }

            return gg;
        }

        class GeomBox
        {
            public int Index { get; set; } = -1;

            public Vec3 Center { get; set; }

            public SortedSet<int> Pnts { get; set; }

            public SortedSet<int> Norms { get; set; }

            public SortedSet<int> Uvs { get; set; }
        }

        private GeomBox GetBoxIndex(Geometry g, IList<BoundingBox> boxes)
        {
            var gCenter = GetCenter(g);
            for(var i = 0;i<boxes.Count;i++)
            {
                if (boxes[i].IsIn(gCenter.Center))
                {
                    gCenter.Index = i;
                    return gCenter;
                }
            }
            return gCenter;
        }

        private GeomBox GetCenter(Geometry g)
        {
            var ps = new SortedSet<int>();
            var ns = new SortedSet<int>();
            var ts = new SortedSet<int>();
            var sumX = 0.0;
            var sumY = 0.0;
            var sumZ = 0.0;
            foreach (var f in g.Faces)
            {
                foreach(var t in f.Triangles)
                {
                    if (!ps.Contains(t.V1.V))
                    {
                        var v = Vertices[t.V1.V-1];
                        sumX += v.X;
                        sumY += v.Y;
                        sumZ += v.Z;
                        ps.Add(t.V1.V);
                    }                    
                    if (!ps.Contains(t.V2.V))
                    {
                        var v = Vertices[t.V2.V - 1];
                        sumX += v.X;
                        sumY += v.Y;
                        sumZ += v.Z;
                        ps.Add(t.V2.V);
                    }                    
                    if (!ps.Contains(t.V3.V))
                    {
                        var v = Vertices[t.V3.V - 1];
                        sumX += v.X;
                        sumY += v.Y;
                        sumZ += v.Z;
                        ps.Add(t.V3.V);
                    }


                    if (t.V1.N > 0 && !ns.Contains(t.V1.N))
                    {
                        ns.Add(t.V1.N);
                    }
                    if (t.V1.T > 0 && !ts.Contains(t.V1.T))
                    {
                        ts.Add(t.V1.T);
                    }
                    if (t.V2.N > 0 && !ns.Contains(t.V2.N))
                    {
                        ns.Add(t.V2.N);
                    }
                    if (t.V2.T > 0 && !ts.Contains(t.V2.T))
                    {
                        ts.Add(t.V2.T);
                    }
                    if (t.V3.N > 0 && !ns.Contains(t.V3.N))
                    {
                        ns.Add(t.V3.N);
                    }
                    if (t.V3.T > 0 && !ts.Contains(t.V3.T))
                    {
                        ts.Add(t.V3.T);
                    }

                }
            }
            
            var x = sumX / ps.Count;
            var y = sumY / ps.Count;
            var z = sumZ / ps.Count;
            return new GeomBox
            {
                Center = new Vec3(x, y, z),
                Pnts = ps,
                Norms = ns,
                Uvs = ts
            };
        }

        public BoundingBox GetBounding()
        {
            var box = new BoundingBox();
            foreach(var v in Vertices)
            {
                var x = v.X;                
                if (box.X.Min > x)
                {
                    box.X.Min = x;
                } else if (box.X.Max < x)
                {
                    box.X.Max = x;
                }
                var y = v.Y;
                if (box.Y.Min > y)
                {
                    box.Y.Min = y;
                } else if (box.Y.Max < y)
                {
                    box.Y.Max = y;
                }
                var z = v.Z;
                if (box.Z.Min > z)
                {
                    box.Z.Min = z;
                } else if (box.Z.Max < z)
                {
                    box.Z.Max = z;
                }
            }
            return box;
        }
    }

    /// <summary>
    /// geometry with face meshes
    /// http://paulbourke.net/dataformats/obj/
    /// http://www.fileformat.info/format/wavefrontobj/egff.htm
    /// </summary>
    public class Geometry
    {
        /// <summary>
        /// group name
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// meshes
        /// </summary>
        public List<Face> Faces { get; set; } = new List<Face>();
        /// <summary>
        /// write geometry
        /// </summary>
        /// <param name="writer"></param>
        public void Write(StreamWriter writer)
        {
            writer.WriteLine($"g {Id}");
            writer.WriteLine($"s off");
            foreach (var f in Faces)
            {                
                f.Write(writer);
            }
        }
    }
}