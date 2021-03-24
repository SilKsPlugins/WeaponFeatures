using OpenMod.API.Eventing;
using OpenMod.Unturned.Players;
using OpenMod.Unturned.Players.Life.Events;
using SDG.Unturned;
using System.Threading.Tasks;

namespace WeaponFeatures
{
    public class MeleeEventListener : IEventListener<UnturnedPlayerDamagedEvent>
    {
        private readonly WeaponFeatures m_Plugin;

        public MeleeEventListener(WeaponFeatures plugin)
        {
            m_Plugin = plugin;
        }

        public Task HandleEventAsync(object sender, UnturnedPlayerDamagedEvent @event)
        {
            if (@event.Cause == EDeathCause.MELEE)
            {
                UnturnedPlayer killer = @event.DamageSource as UnturnedPlayer;

                var asset = killer?.Player.equipment.asset;

                if (asset != null)
                {
                    if (@event.Player.IsAlive)
                    {
                        m_Plugin.TriggerOnDamage(asset.id, @event.Player.Player, killer.Player);
                    }
                    else
                    {
                        m_Plugin.TriggerOnKill(asset.id, @event.Player.Player, killer.Player);
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
