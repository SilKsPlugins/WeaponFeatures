using SDG.Unturned;

namespace WeaponFeatures.Features
{
    public class FeatureBreakLegs : IWeaponFeatureDamage
    {
        public double? Chance { get; set; }

        public void OnDamage(Player victim, Player killer)
        {
            victim.life.serverSetLegsBroken(true);
        }
    }
}