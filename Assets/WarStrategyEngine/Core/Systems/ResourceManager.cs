using System;
using System.Collections.Generic;
using System.Linq;

namespace WarStrategyEngine.Core.Systems
{
    /// <summary>
    /// 资源管理器
    /// </summary>
    public class ResourceManager
    {
        private Dictionary<ResourceType, Resource> _resources;
        
        public ResourceManager()
        {
            _resources = new Dictionary<ResourceType, Resource>();
            InitializeDefaultResources();
        }
        
        private void InitializeDefaultResources()
        {
            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            {
                _resources[type] = new Resource(type, 0);
            }
        }
        
        /// <summary>
        /// 获取资源数量
        /// </summary>
        public int GetResource(ResourceType type)
        {
            return _resources.TryGetValue(type, out var resource) ? resource.Amount : 0;
        }
        
        /// <summary>
        /// 设置资源数量
        /// </summary>
        public void SetResource(ResourceType type, int amount)
        {
            if (_resources.TryGetValue(type, out var resource))
            {
                resource.Amount = Math.Max(0, amount);
            }
            else
            {
                _resources[type] = new Resource(type, Math.Max(0, amount));
            }
        }
        
        /// <summary>
        /// 增加资源
        /// </summary>
        public void AddResource(ResourceType type, int amount)
        {
            SetResource(type, GetResource(type) + amount);
        }
        
        /// <summary>
        /// 消耗资源
        /// </summary>
        public bool ConsumeResource(ResourceType type, int amount)
        {
            int current = GetResource(type);
            if (current >= amount)
            {
                SetResource(type, current - amount);
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// 检查是否有足够的资源
        /// </summary>
        public bool HasEnoughResource(ResourceType type, int amount)
        {
            return GetResource(type) >= amount;
        }
        
        /// <summary>
        /// 检查是否有足够的多种资源
        /// </summary>
        public bool HasEnoughResources(Dictionary<ResourceType, int> costs)
        {
            return costs.All(cost => HasEnoughResource(cost.Key, cost.Value));
        }
        
        /// <summary>
        /// 消耗多种资源
        /// </summary>
        public bool ConsumeResources(Dictionary<ResourceType, int> costs)
        {
            if (!HasEnoughResources(costs))
                return false;
            
            foreach (var cost in costs)
            {
                ConsumeResource(cost.Key, cost.Value);
            }
            return true;
        }
        
        /// <summary>
        /// 获取所有资源
        /// </summary>
        public Dictionary<ResourceType, int> GetAllResources()
        {
            return _resources.ToDictionary(r => r.Key, r => r.Value.Amount);
        }
    }
}
