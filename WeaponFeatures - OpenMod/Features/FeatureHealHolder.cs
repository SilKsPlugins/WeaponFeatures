using SDG.Unturned;

namespace WeaponFeatures.Features
{
    public class FeatureHealHolder : IWeaponFeatureKill
    {
        public double? Chance { get; set; }

        public byte Amount { get; set; }

        public bool HealBleeding { get; set; }

        public bool HealBroken { get; set; }

        public void OnKill(Player victim, Player killer)
        {
            if (Amount == 0 && !HealBleeding && !HealBroken) return;

            killer.life.askHeal(Amount, HealBleeding, HealBroken);
        }
    }
}
