using Microsoft.Extensions.Logging;
using OpenMod.API.Eventing;
using OpenMod.Unturned.Players.Stats.Events;
using System;
using System.Threading.Tasks;

namespace WeaponFeatures
{
    public class StatsEventListener : IEventListener<UnturnedPlayerStatUpdatedEvent>
    {
        protected ILogger<StatsEventListener> m_Logger;

        public StatsEventListener(ILogger<StatsEventListener> logger)
        {
            m_Logger = logger;
        }

        public Task HandleEventAsync(object sender, UnturnedPlayerStatUpdatedEvent @event)
        {
            m_Logger.LogInformation(@event.GetType().Name + " " + @event.Player.Player.channel.owner.playerID.playerName);

            foreach (var property in @event.GetType().GetProperties())
            {
                if (property.GetType().IsPublic && property.CanRead && !property.Name.Contains("Player"))
                {
                    m_Logger.LogInformation("\t" + property.Name + " - " + property.GetValue(@event));
                }
            }

            return Task.CompletedTask;
        }
    }
}
