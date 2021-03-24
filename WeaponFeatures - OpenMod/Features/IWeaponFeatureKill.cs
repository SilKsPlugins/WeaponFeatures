using SDG.Unturned;

namespace WeaponFeatures.Features
{
    public interface IWeaponFeatureKill : IWeaponFeature
    {
        void OnKill(Player victim, Player killer);
    }
}
