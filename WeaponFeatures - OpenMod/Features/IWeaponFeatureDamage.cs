using SDG.Unturned;

namespace WeaponFeatures.Features
{
    public interface IWeaponFeatureDamage : IWeaponFeature
    {
        void OnDamage(Player victim, Player killer);
    }
}
