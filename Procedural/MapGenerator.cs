using System.Threading.Tasks;
using UnityEngine;

namespace PSYEngine.Unity.Procedural
{
    /// <summary>
    /// Manages map generations
    /// </summary>
    public class MapGenerator
    {
        public NoiseConfiguration Configuration { get { return _config; } }

        private NoiseConfiguration _config { get; set; }

        /// <summary>
        /// Create a new instance of <see cref="MapGenerator"/>
        /// </summary>
        /// <param name="configuration"><see cref="Procedural.NoiseConfiguration"/> configuration for Noisemap generation</param>
        public MapGenerator(NoiseConfiguration configuration) => this._config = configuration;

        /// <summary>
        /// Generates height map data
        /// </summary>
        /// <param name="seed">Seed to use for generation</param>
        /// <param name="scale">Scale to apply on genareted noise</param>
        /// <returns>Returns <see cref="Buffer2D{T}"/> array of height data</returns>
        /// <exception cref="System.InsufficientMemoryException"/>
        public float[,] GenerateNoiseMap(int seed, float scale)
        {
            if (scale <= 0)
                scale = 0.0001f;

            System.Random random = new System.Random(seed);

            Vector2[] octaveOffsets = new Vector2[_config.Octaves];

            for (int i = 0; i < _config.Octaves; i++)
            {
                float offsetX = random.Next(-100000, 100000) + _config.Offset.x;
                float offsetY = random.Next(-100000, 100000) + _config.Offset.y;
                octaveOffsets[i].Set(offsetX, offsetY);
            }

            float[,] noiseMap = new float[_config.Width, _config.Height];

            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;

            Parallel.For(0, _config.Height, y =>
            {
                for (ushort x = 0; x < _config.Width; x++)
                {
                    float amplitude = 1;
                    float frequency = 1;
                    float noiseHeight = 0;

                    for (ushort i = 0; i < _config.Octaves; i++)
                    {
                        // Control frequency of octaves
                        float sampleX = x / scale * frequency + octaveOffsets[i].x;
                        float sampleY = y / scale * frequency + octaveOffsets[i].y;

                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                        // Control amplitude of octaves
                        noiseHeight += perlinValue * amplitude;

                        // Amplitude = Persistance ^ i
                        amplitude *= _config.Persistance;

                        // Frequency = Lacunarity ^ i
                        frequency *= _config.Lacunarity;
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