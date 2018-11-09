using UnityEngine;

namespace PSYEngine.Procedural.Unity
{
    internal struct MeshData
    {
        public Vector3[] Vertices;
        public int[] Triangles;
        public Vector2[] UVs;

        public int Index => tIndex;

        private int tIndex;

        internal MeshData(int width, int height)
        {
            if (width * height >= 65000)
                throw new UnityException("Mesh.vertices is too large. A mesh may not have more than 65000 vertices");
            
            this.Vertices = new Vector3[width * height];
            this.Triangles = new int[(width - 1) * (height - 1) * 6];
            this.UVs = new Vector2[width * height];
            this.tIndex = 0;
        }

        public void AddTriangle(int a, int b, int c)
        {
            Triangles[tIndex] = a;
            Triangles[tIndex + 1] = b;
            Triangles[tIndex + 2] = c;
            tIndex += 3;
        }

        public Mesh CreateMesh()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = this.Vertices;
            mesh.triangles = this.Triangles;
            mesh.uv = this.UVs;

            mesh.RecalculateNormals();

            return mesh;
        }

        public void UpdateMesh(ref Mesh mesh)
        {
            mesh.vertices = this.Vertices;
            mesh.triangles = this.Triangles;
            mesh.uv = this.UVs;

            mesh.RecalculateNormals();
        }
    }
}