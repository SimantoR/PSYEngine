namespace PSYEngine.Unity.Procedural
{
    /// <summary>
    /// A struct containing configuration for noise map generator
    /// </summary>
    public struct NoiseConfiguration
    {
        public int Width { get; set; }

        public int Height { get; set; }

        /// <summary>
        /// Number of individual layers of noise
        /// </summary>
        public int Octaves { get; set; }

        /// <summary>
        /// Control for the frequency of each octave
        /// </summary>
        public float Lacunarity { get; set; }

        /// <summary>
        /// Control for decrease in amplitude of each octave
        /// </summary>
        public float Persistance { get; set; }

        public UnityEngine.Vector2 Offset { get; set; }
    }
}
