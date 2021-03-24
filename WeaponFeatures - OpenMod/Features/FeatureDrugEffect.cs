using Cysharp.Threading.Tasks;
using SDG.Unturned;

namespace WeaponFeatures.Features
{
    public class FeatureDrugEffect : IWeaponFeatureDamage
    {
        public float Duration { get; set; }

        public double? Chance { get; set; }

        public void OnDamage(Player victim, Player killer)
        {
            if (Duration == 0) return;

            victim.life.serverModifyHallucination(Duration);
        }
    }
}