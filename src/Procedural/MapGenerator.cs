using System;
using System.Threading.Tasks;
using UnityEngine;

/** Comments
 * [Simanto] Unity is not Thread safe!!!
 */
namespace PSYEngine.Procedural.Unity
{
    public partial class MapGenerator : MonoBehaviour
    {
        [Tooltip("Width of noise map")]
        public int Width = 200;

        [Tooltip("Height of noise map")]
        public int Height = 200;

        [Tooltip("Number of sign waves to apply")]
        public int Octaves = 3;

        [Tooltip("Detail scale")]
        public float Lacunarity = 2f;

        [Tooltip("Fine Tuner")]
        public float Persistance = 0.5f;

        [Tooltip("Random identifier")]
        public int Seed = 0;

        public float Scale = 0f;

        [Tooltip("Don't know. Don't ask")]
        public Vector2[] Offsets = new Vector2[]
        {
            new Vector2(0, 0),
            new Vector2(0, 0)
        };

        [Tooltip("Controls the curve of scale to apply")]
        public AnimationCurve HeightCurve;

        readonly Procedural.MapGenerator instance = new Procedural.MapGenerator();

        // Need to include Height Curve to the algorithm
        /// <summary>
        /// Generates a heightmap in 2D array
        /// </summary>
        /// <param name="seed">Seed</param>
        /// <param name="scale">Scale to apply on the noise</param>
        /// <returns>2D Float array of generated height map</returns>
        public float[,] GenerateHeightMap(int seed, float scale)
            => this.instance.GenerateHeightmap(this.Width, this.Height, seed, scale, this.Octaves, this.Persistance, this.Lacunarity, this.Offsets.ToSystemVectors());

        /// <summary>
        /// Generates a <see cref="Texture2D"/> from the height map
        /// </summary>
        /// <param name="heightMap">2D <see cref="float"/> noise map</param>
        public Texture2D GenerateTexture2D(float[,] heightMap)
        {
            // Color map to convert to from height map
            Color[] colorMap = new Color[Width * Height];

            // Create new color map based on noise map
            Parallel.For(0, Height, y =>
            {
                for (ushort x = 0; x < Width; x++)
                    colorMap[(y * Width) + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            });

            Texture2D texture = new Texture2D(Width, Height);

            // Assigns color to texture. Extremely slow, thus 'Task'
            texture.SetPixels(colorMap);
            texture.Apply();

            return texture;
        }

        // [Untested]
        /// <summary>
        /// Generate a mesh out of the 
        /// </summary>
        /// <param name="heightMap"></param>
        /// <param name="heightScale"></param>
        /// <param name="heightCurve"></param>
        /// <returns></returns>
        public Mesh GenerateMesh(float[,] heightMap, float heightScale, AnimationCurve heightCurve)
        {
            int Width = heightMap.GetLength(0);
            int Height = heightMap.GetLength(1);
            float topLeftX = (Width - 1) / -2f;
            float topLeftZ = (Height - 1) / 2f;

            MeshData meshData = new MeshData(Width, Height);
            int vertIndex = 0;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    meshData.Vertices[vertIndex] = new Vector3(topLeftX + x, heightMap[x, y] * heightScale, topLeftZ - y);
                    meshData.UVs[vertIndex] = new Vector2(x / (float)Width, y / (float)Height);

                    if (x < Width - 1 && y < Height - 1)
                    {
                        meshData.AddTriangle(vertIndex, vertIndex + Width + 1, vertIndex + Width);
                        meshData.AddTriangle(vertIndex + Width + 1, vertIndex, vertIndex + 1);
                    }
                    vertIndex++;
                }
            }

            return meshData.CreateMesh();
        }
    }
}