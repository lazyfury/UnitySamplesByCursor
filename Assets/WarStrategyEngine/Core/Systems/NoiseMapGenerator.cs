using System;

namespace WarStrategyEngine.Core.Systems
{
    /// <summary>
    /// 噪声图生成器，用于生成Perlin噪声
    /// </summary>
    public static class NoiseMapGenerator
    {
        /// <summary>
        /// 生成Perlin噪声图
        /// </summary>
        /// <param name="width">地图宽度</param>
        /// <param name="height">地图高度</param>
        /// <param name="scale">噪声缩放（值越小，噪声越平滑）</param>
        /// <param name="octaves">八度数量（控制细节层次）</param>
        /// <param name="persistence">持久度（控制每个八度的影响）</param>
        /// <param name="lacunarity">间隙度（控制每个八度的频率）</param>
        /// <param name="offsetX">X偏移（用于随机种子）</param>
        /// <param name="offsetY">Y偏移（用于随机种子）</param>
        /// <returns>噪声值数组（0-1范围）</returns>
        public static float[,] GenerateNoiseMap(
            int width, 
            int height, 
            float scale = 10f,
            int octaves = 4,
            float persistence = 0.5f,
            float lacunarity = 2f,
            float offsetX = 0f,
            float offsetY = 0f)
        {
            if (scale <= 0)
                scale = 0.0001f;
            
            float[,] noiseMap = new float[width, height];
            
            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;
            
            // 计算每个点的噪声值
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float amplitude = 1f;
                    float frequency = 1f;
                    float noiseHeight = 0f;
                    
                    // 多八度噪声叠加
                    for (int i = 0; i < octaves; i++)
                    {
                        float sampleX = (x + offsetX) / scale * frequency;
                        float sampleY = (y + offsetY) / scale * frequency;
                        
                        float perlinValue = PerlinNoise(sampleX, sampleY) * 2 - 1; // 转换为-1到1
                        noiseHeight += perlinValue * amplitude;
                        
                        amplitude *= persistence;
                        frequency *= lacunarity;
                    }
                    
                    noiseMap[x, y] = noiseHeight;
                    
                    if (noiseHeight > maxNoiseHeight)
                        maxNoiseHeight = noiseHeight;
                    if (noiseHeight < minNoiseHeight)
                        minNoiseHeight = noiseHeight;
                }
            }
            
            // 归一化到0-1范围
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    noiseMap[x, y] = (noiseMap[x, y] - minNoiseHeight) / (maxNoiseHeight - minNoiseHeight);
                }
            }
            
            return noiseMap;
        }
        
        /// <summary>
        /// 简化的Perlin噪声实现
        /// </summary>
        private static float PerlinNoise(float x, float y)
        {
            // 使用Unity的Mathf.PerlinNoise或实现简化版本
            // 这里使用一个简化的伪随机噪声函数
            int X = (int)Math.Floor(x) & 255;
            int Y = (int)Math.Floor(y) & 255;
            
            float fx = x - (float)Math.Floor(x);
            float fy = y - (float)Math.Floor(y);
            
            float u = Fade(fx);
            float v = Fade(fy);
            
            int A = (Permutation[X] + Y) & 255;
            int B = (Permutation[X + 1] + Y) & 255;
            
            return Lerp(v,
                Lerp(u, Grad(Permutation[A], fx, fy),
                         Grad(Permutation[B], fx - 1, fy)),
                Lerp(u, Grad(Permutation[A + 1], fx, fy - 1),
                         Grad(Permutation[B + 1], fx - 1, fy - 1)));
        }
        
        private static float Fade(float t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }
        
        private static float Lerp(float t, float a, float b)
        {
            return a + t * (b - a);
        }
        
        private static float Grad(int hash, float x, float y)
        {
            int h = hash & 3;
            float u = h < 2 ? x : y;
            float v = h < 2 ? y : x;
            return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
        }
        
        // Perlin噪声的排列表
        private static readonly int[] Permutation = {
            151,160,137,91,90,15,131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
            190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,88,237,149,56,87,174,20,
            125,136,171,168, 68,175,74,165,71,134,139,48,27,166,77,146,158,231,83,111,229,122,60,211,133,230,220,
            105,92,41,55,46,245,40,244,102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
            135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,5,202,38,147,118,126,255,82,85,
            212,207,206,59,227,47,16,58,17,182,189,28,42,223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,
            167, 43,172,9,129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,251,34,242,193,238,
            210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,
            50,45,127, 4,150,254,138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180,
            // 重复一次以简化索引计算
            151,160,137,91,90,15,131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
            190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,88,237,149,56,87,174,20,
            125,136,171,168, 68,175,74,165,71,134,139,48,27,166,77,146,158,231,83,111,229,122,60,211,133,230,220,
            105,92,41,55,46,245,40,244,102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
            135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,5,202,38,147,118,126,255,82,85,
            212,207,206,59,227,47,16,58,17,182,189,28,42,223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,
            167, 43,172,9,129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,251,34,242,193,238,
            210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,
            50,45,127, 4,150,254,138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180
        };
    }
}
