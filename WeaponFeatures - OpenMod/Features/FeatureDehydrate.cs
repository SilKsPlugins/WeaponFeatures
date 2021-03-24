using SDG.Unturned;

namespace WeaponFeatures.Features
{
    public class FeatureDehydrate : IWeaponFeatureDamage
    {
        public float Amount { get; set; }

        public double? Chance { get; set; }

        public void OnDamage(Player victim, Player killer)
        {
            if (Amount == 0) return;

            victim.life.serverModifyWater(-Amount);
        }
    }
}