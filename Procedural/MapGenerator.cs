using System;
using System.Threading.Tasks;
using UnityEngine;

namespace PSYEngine.Unity.Procedural
{
    public sealed class MapGenerator : MonoBehaviour
    {
        public int Width = 200;

        public int Height = 200;

        [Tooltip("Number of sign waves to apply")]
        public int Octaves = 3;

        public float Lacunarity = 2f;

        public float Persistance = 0.5f;

        public int Seed = 0;

        public AnimationCurve HeightCurve;

        public UnityEngine.Vector2 Offset = new Vector2(1, 1);

        private float[,] heightMap;

        public float[,] GenerateHeightMap(int seed, float scale)
        {
            if (scale <= 0)
                scale = 0.0001f;

            System.Random random = new System.Random(seed);

            Vector2[] octaveOffsets = new Vector2[Octaves];

            for (int i = 0; i < Octaves; i++)
            {
                float offsetX = random.Next(-100000, 100000) + Offset.x;
                float offsetY = random.Next(-100000, 100000) + Offset.y;
                octaveOffsets[i].Set(offsetX, offsetY);
            }

            float[,] noiseMap = new float[Width, Height];

            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;

            Parallel.For(0, Height, y =>
            {
                for (ushort x = 0; x < Width; x++)
                {
                    float amplitude = 1;
                    float frequency = 1;
                    float noiseHeight = 0;

                    for (ushort i = 0; i < Octaves; i++)
                    {
                        // Control frequency of octaves
                        float sampleX = x / scale * frequency + octaveOffsets[i].x;
                        float sampleY = y / scale * frequency + octaveOffsets[i].y;

                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                        // Control amplitude of octaves
                        noiseHeight += perlinValue * amplitude;

                        // Amplitude = Persistance ^ i
                        amplitude *= Persistance;

                        // Frequency = Lacunarity ^ i
                        frequency *= Lacunarity;
                    }

                    // Determine max height
                    if (noiseHeight > maxNoiseHeight)
                        maxNoiseHeight = noiseHeight;
                    // Determine min height
                    else if (noiseHeight < minNoiseHeight)
                        minNoiseHeight = noiseHeight;

                    noiseMap[x, y] = noiseHeight;
                }
            });

            return noiseMap;
        }

        public Texture2D GenerateTexture(float[,] heightMap)
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