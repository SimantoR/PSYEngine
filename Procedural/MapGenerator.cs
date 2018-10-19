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
    }
}