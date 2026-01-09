using System;
using System.Collections.Generic;
using System.Linq;
using SurvivorEngine.Core.Interfaces;
using SurvivorEngine.Core.Types;

namespace SurvivorEngine.Core.Systems
{
    /// <summary>
    /// 升级系统
    /// </summary>
    public class UpgradeSystem
    {
        private Random _random;
        private Dictionary<UpgradeType, int> _upgradeLevels;
        private List<WeaponType> _availableWeapons;

        public UpgradeSystem()
        {
            _random = new Random();
            _upgradeLevels = new Dictionary<UpgradeType, int>();
            _availableWeapons = new List<WeaponType>();
        }

        public List<UpgradeOption> GenerateUpgradeOptions(int count, IPlayer player, List<IWeapon> weapons)
        {
            List<UpgradeOption> options = new List<UpgradeOption>();
            List<UpgradeOption> allOptions = GetAllPossibleUpgrades(player, weapons);
            
            // 随机选择不重复的升级选项
            var shuffled = allOptions.OrderBy(x => _random.Next()).ToList();
            
            for (int i = 0; i < Math.Min(count, shuffled.Count); i++)
            {
                options.Add(shuffled[i]);
            }
            
            return options;
        }

        private List<UpgradeOption> GetAllPossibleUpgrades(IPlayer player, List<IWeapon> weapons)
        {
            List<UpgradeOption> options = new List<UpgradeOption>();
            
            // 属性升级
            options.Add(new UpgradeOption(UpgradeType.MaxHealth, "最大生命值", "增加最大生命值 20", 20f));
            options.Add(new UpgradeOption(UpgradeType.MoveSpeed, "移动速度", "增加移动速度 10%", 0.1f));
            options.Add(new UpgradeOption(UpgradeType.Damage, "伤害", "增加所有武器伤害 15%", 0.15f));
            options.Add(new UpgradeOption(UpgradeType.AttackSpeed, "攻击速度", "增加所有武器攻击速度 10%", 0.1f));
            options.Add(new UpgradeOption(UpgradeType.AttackRange, "攻击范围", "增加所有武器攻击范围 20%", 0.2f));
            options.Add(new UpgradeOption(UpgradeType.ProjectileSpeed, "投射物速度", "增加投射物速度 15%", 0.15f));
            options.Add(new UpgradeOption(UpgradeType.ExperienceGain, "经验获得", "增加经验获得 20%", 0.2f));
            options.Add(new UpgradeOption(UpgradeType.PickupRange, "拾取范围", "增加拾取范围 30%", 0.3f));
            
            // 武器升级
            foreach (var weapon in weapons)
            {
                options.Add(new UpgradeOption(UpgradeType.WeaponLevel, $"{weapon.Type} 等级", $"提升 {weapon.Type} 等级", 1f, weapon.Type));
            }
            
            // 新武器（如果还没有）
            if (!weapons.Any(w => w.Type == WeaponType.Whip))
                options.Add(new UpgradeOption(UpgradeType.NewWeapon, "鞭子", "获得新武器：鞭子", 0f, WeaponType.Whip));
            if (!weapons.Any(w => w.Type == WeaponType.Knife))
                options.Add(new UpgradeOption(UpgradeType.NewWeapon, "飞刀", "获得新武器：飞刀", 0f, WeaponType.Knife));
            if (!weapons.Any(w => w.Type == WeaponType.Fireball))
                options.Add(new UpgradeOption(UpgradeType.NewWeapon, "火球", "获得新武器：火球", 0f, WeaponType.Fireball));
            if (!weapons.Any(w => w.Type == WeaponType.MagicWand))
                options.Add(new UpgradeOption(UpgradeType.NewWeapon, "魔法棒", "获得新武器：魔法棒", 0f, WeaponType.MagicWand));
            
            return options;
        }

        public void ApplyUpgrade(UpgradeOption upgrade, IPlayer player, List<IWeapon> weapons)
        {
            switch (upgrade.Type)
            {
                case UpgradeType.MaxHealth:
                    player.MaxHealth += upgrade.Value;
                    player.Heal(upgrade.Value);
                    break;
                
                case UpgradeType.MoveSpeed:
                    player.MoveSpeed *= (1f + upgrade.Value);
                    break;
                
                case UpgradeType.Damage:
                    foreach (var weapon in weapons)
                    {
                        weapon.Damage *= (1f + upgrade.Value);
                    }
                    break;
                
                case UpgradeType.AttackSpeed:
                    foreach (var weapon in weapons)
                    {
                        weapon.AttackSpeed *= (1f + upgrade.Value);
                        weapon.Cooldown = 1f / weapon.AttackSpeed;
                    }
                    break;
                
                case UpgradeType.AttackRange:
                    foreach (var weapon in weapons)
                    {
                        weapon.Range *= (1f + upgrade.Value);
                    }
                    break;
                
                case UpgradeType.ProjectileSpeed:
                    foreach (var weapon in weapons)
                    {
                        weapon.ProjectileSpeed *= (1f + upgrade.Value);
                    }
                    break;
                
                case UpgradeType.ExperienceGain:
                    // 这个在引擎中处理
                    break;
                
                case UpgradeType.PickupRange:
                    player.PickupRange *= (1f + upgrade.Value);
                    break;
                
                case UpgradeType.WeaponLevel:
                    var weaponToUpgrade = weapons.FirstOrDefault(w => w.Type == upgrade.WeaponType);
                    if (weaponToUpgrade != null)
                    {
                        weaponToUpgrade.Upgrade();
                    }
                    break;
                
                case UpgradeType.NewWeapon:
                    // 这个在引擎中处理
                    break;
            }
        }
    }
}
