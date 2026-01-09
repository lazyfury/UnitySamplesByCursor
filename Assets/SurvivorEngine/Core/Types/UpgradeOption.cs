namespace SurvivorEngine.Core.Types
{
    /// <summary>
    /// 升级选项
    /// </summary>
    public class UpgradeOption
    {
        public UpgradeType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Value { get; set; }
        public WeaponType? WeaponType { get; set; }  // 如果是武器相关升级

        public UpgradeOption(UpgradeType type, string name, string description, float value, WeaponType? weaponType = null)
        {
            Type = type;
            Name = name;
            Description = description;
            Value = value;
            WeaponType = weaponType;
        }
    }
}
