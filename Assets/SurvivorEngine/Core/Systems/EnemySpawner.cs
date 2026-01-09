using System;
using System.Collections.Generic;
using SurvivorEngine.Core.Interfaces;
using SurvivorEngine.Core.Types;

namespace SurvivorEngine.Core.Systems
{
    /// <summary>
    /// 敌人生成系统
    /// </summary>
    public class EnemySpawner
    {
        private Random _random;
        private float _spawnTimer;
        private float _spawnInterval;
        private int _enemiesPerWave;
        private int _currentWaveEnemies;
        private Vector2D _spawnAreaMin;
        private Vector2D _spawnAreaMax;
        private Vector2D _playerPosition;

        public EnemySpawner(Vector2D spawnAreaMin, Vector2D spawnAreaMax)
        {
            _random = new Random();
            _spawnAreaMin = spawnAreaMin;
            _spawnAreaMax = spawnAreaMax;
            _spawnInterval = 2f;
            _spawnTimer = 0f;
            _enemiesPerWave = 10;
            _currentWaveEnemies = 0;
        }

        public void Update(float deltaTime, Vector2D playerPosition, float gameTime, int waveNumber)
        {
            _playerPosition = playerPosition;
            _spawnTimer += deltaTime;
            
            // 根据波次调整生成参数
            _enemiesPerWave = 10 + waveNumber * 5;
            _spawnInterval = Math.Max(0.5f, 2f - waveNumber * 0.1f);
            
            if (_spawnTimer >= _spawnInterval && _currentWaveEnemies < _enemiesPerWave)
            {
                _spawnTimer = 0f;
                _currentWaveEnemies++;
            }
        }

        public bool ShouldSpawn()
        {
            if (_spawnTimer >= _spawnInterval && _currentWaveEnemies < _enemiesPerWave)
            {
                _spawnTimer = 0f;
                return true;
            }
            return false;
        }

        public Vector2D GetSpawnPosition()
        {
            // 在玩家视野外生成敌人
            Vector2D spawnPos;
            int attempts = 0;
            const float minDistanceFromPlayer = 8f;
            
            do
            {
                // 随机选择边界
                int side = _random.Next(4);
                float x, y;
                
                switch (side)
                {
                    case 0: // 上
                        x = _random.Next((int)_spawnAreaMin.X, (int)_spawnAreaMax.X);
                        y = _spawnAreaMax.Y;
                        break;
                    case 1: // 下
                        x = _random.Next((int)_spawnAreaMin.X, (int)_spawnAreaMax.X);
                        y = _spawnAreaMin.Y;
                        break;
                    case 2: // 左
                        x = _spawnAreaMin.X;
                        y = _random.Next((int)_spawnAreaMin.Y, (int)_spawnAreaMax.Y);
                        break;
                    default: // 右
                        x = _spawnAreaMax.X;
                        y = _random.Next((int)_spawnAreaMin.Y, (int)_spawnAreaMax.Y);
                        break;
                }
                
                spawnPos = new Vector2D(x, y);
                attempts++;
            } while (Vector2D.Distance(spawnPos, _playerPosition) < minDistanceFromPlayer && attempts < 10);
            
            return spawnPos;
        }

        public EnemyType GetRandomEnemyType(int waveNumber)
        {
            // 根据波次决定敌人类型
            if (waveNumber < 3)
                return EnemyType.Bat;
            else if (waveNumber < 6)
                return (EnemyType)_random.Next((int)EnemyType.Bat, (int)EnemyType.Zombie + 1);
            else
                return (EnemyType)_random.Next((int)EnemyType.Bat, (int)EnemyType.Ghost + 1);
        }

        public void ResetWave()
        {
            _currentWaveEnemies = 0;
            _spawnTimer = 0f;
        }

        public bool IsWaveComplete(int currentEnemyCount)
        {
            return _currentWaveEnemies >= _enemiesPerWave && currentEnemyCount == 0;
        }
    }
}
