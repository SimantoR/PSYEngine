using System;
using System.Numerics;
using System.Threading.Tasks;

#region AssemblyInfo
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(assemblyName: "PSYEngine.Procedural.Unity, PublicKey=0024000004800000940000000602000000240000525341310004000001000100e93e0e9905ccd2f5eddaa10c05066bbdf47ab96ecc5b77817ebda817ce0b61f42a2c1d4196686e3be5b7bab5153f2b10b7a7bbb9771c20cfac8868fe0fca3d5354e8bdd189a243908be4320d5c1d53bedc4ff4b8e129c04db1eff884f318e610dc772e8574d239ef1ed620231febf6b1bcba776f2bd9f542fe5b0a508197e4e3")];
#endregion

namespace PSYEngine.Procedural
{
    internal class MapGenerator
    {
        internal MapGenerator() { }

        internal float[,] GenerateHeightmap(int width, int height, int seed, float scale, int octave, float persistance, float lacunarity, Vector2[] offsets)
        {
            if (scale <= 0)
                scale = 0.0001f;

            Vector2[] octaveOffsets = new Vector2[octave];

            Random random = new Random(seed);

            float offsetX, offsetY;

            for (int i = 0; i < octave; i++)
            {
                offsetX = random.Next(-100_000, 100_000);
                offsetY = random.Next(-100_000, 100_000);
                octaveOffsets[i].Set(offsetX, offsetY);
            }

            float[,] noiseMap = new float[width, height];

            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;

            // Parallel shouldn't work in this context
            Parallel.For(0, height, y =>
            {
                float amplitude;
                float frequency;
                float noiseHeight;
                float sampleX;
                float sampleY;
                float perlinValue;

                for (ushort x = 0; x < width; x++)
                {
                    amplitude = 1f;
                    frequency = 1f;
                    noiseHeight = 0f;

                    for (ushort i = 0; i < octave; i++)
                    {
                        sampleX = x / scale * frequency + octaveOffsets[i].X;
                        sampleY = y / scale * frequency + octaveOffsets[i].Y;

                        perlinValue = PerlinNoise(sampleX, sampleY) * 2 - 1;

                        noiseHeight += perlinValue * amplitude;
                        amplitude *= persistance;
                        frequency *= lacunarity;
                    }

                    if (noiseHeight > maxNoiseHeight)
                        maxNoiseHeight = noiseHeight;
                    else if (noiseHeight < minNoiseHeight)
                        minNoiseHeight = noiseHeight;

                    noiseMap[x, y] = noiseHeight;
                }
            });

            return noiseMap;
        }

        /// <summary>
        /// Generate Perlin noise
        /// </summary>
        /// <param name="x">X coordinate of perlin noise</param>
        /// <param name="y">Y coordinate of perlin noise</param>
        /// <returns><see cref="float"/>value at given coordinates</returns>
        internal float PerlinNoise(float x, float y)
        {
            return 0.0f;
        }

        // Perm used to Perlin noise generation
        private static int[] _perm
        {
            get => new int[]
            {
                151,160,137,91,90,15,
                131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
                190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
                88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
                77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
                102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
                135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
                5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
                223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
                129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
                251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
                49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
                138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180,
                151
            };
        }
    }
}
