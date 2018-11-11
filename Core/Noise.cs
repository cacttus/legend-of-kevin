using System;
using System.Collections.Generic;
using System.Text;

namespace Core
{
    //https://stackoverflow.com/questions/8659351/2d-perlin-noise
    /// implements improved Perlin noise in 2D. 
    /// Transcribed from http://www.siafoo.net/snippet/144?nolinenos#perlin2003
    /// </summary>
    public static class Noise2d
    {
        private static Random _random;
        private static int[] _permutation;

        private static vec2[] _gradients;

        static Noise2d()
        {
            CalculatePermutation(out _permutation);
            CalculateGradients(out _gradients);
        }

        private static void CalculatePermutation(out int[] p, int seed = -1)
        {
            if(seed != -1)
            {
                _random = new Random(seed);
            }
            else
            {
                _random = new Random();
            }

            p = new int[256];
            for(int i=0; i<256; ++i) { p[i] = i; }
            
            // System.Linq.Enumerable.Range(0, 256).ToArray();

            /// shuffle the array
            for (var i = 0; i < p.Length; i++)
            {
                var source = _random.Next(p.Length);

                var t = p[i];
                p[i] = p[source];
                p[source] = t;
            }
        }

        /// <summary>
        /// generate a new permutation.
        /// </summary>
        public static void Reseed(int seed = -1)
        {
            CalculatePermutation(out _permutation, seed);
        }

        private static void CalculateGradients(out vec2[] grad)
        {
            grad = new vec2[256];

            for (var i = 0; i < grad.Length; i++)
            {
                vec2 gradient;

                do
                {
                    gradient = new vec2((float)(_random.NextDouble() * 2 - 1), (float)(_random.NextDouble() * 2 - 1));
                }
                while (gradient.Len2() >= 1);

                gradient.Normalize();

                grad[i] = gradient;
            }

        }

        private static float Drop(float t)
        {
            t = Math.Abs(t);
            return 1f - t * t * t * (t * (t * 6 - 15) + 10);
        }

        private static float Q(float u, float v)
        {
            return Drop(u) * Drop(v);
        }

        public static float Noise(float x, float y)
        {
            var cell = new vec2((float)Math.Floor(x), (float)Math.Floor(y));

            var total = 0f;

            var corners = new[] { new vec2(0, 0), new vec2(0, 1), new vec2(1, 0), new vec2(1, 1) };

            foreach (var n in corners)
            {
                var ij = cell + n;
                var uv = new vec2(x - ij.x, y - ij.y);

                var index = _permutation[(int)ij.x % _permutation.Length];
                index = _permutation[(index + (int)ij.y) % _permutation.Length];

                var grad = _gradients[index % _gradients.Length];

                total += Q(uv.x, uv.y) * vec2.Dot(grad, uv);
            }

            return Math.Max(Math.Min(total, 1f), -1f);
        }
        public static float OctaveNoise(float x, float y, int octaves, float persistence)
        {
            float total = 0;
            float frequency = 1;
            float amplitude = 1;
            for (int i = 0; i < octaves; i++)
            {
                total += Noise(x * frequency, y * frequency) * amplitude;

                amplitude *= persistence;
                frequency *= 2;
            }

            return total;
        }


    }
}
