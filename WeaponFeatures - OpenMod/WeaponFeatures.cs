using Cysharp.Threading.Tasks;
using HarmonyLib;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OpenMod.API.Plugins;
using OpenMod.Unturned.Plugins;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Reflection;
using WeaponFeatures.Configuration;
using WeaponFeatures.Features;

[assembly: PluginMetadata("WeaponFeatures", DisplayName = "Weapon Features")]
namespace WeaponFeatures
{
    public class WeaponFeatures : OpenModUnturnedPlugin
    {
        private readonly IConfiguration m_Configuration;
        private readonly IStringLocalizer m_StringLocalizer;
        private readonly ILogger<WeaponFeatures> m_Logger;

        public List<WeaponConfig> WeaponsConfigs;

        public WeaponFeatures(
            IConfiguration configuration, 
            IStringLocalizer stringLocalizer,
            ILogger<WeaponFeatures> logger, 
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
            m_Configuration = configuration;
            m_StringLocalizer = stringLocalizer;
            m_Logger = logger;

            WeaponsConfigs = new List<WeaponConfig>();
        }

        protected override UniTask OnLoadAsync()
        {
            Level.onPostLevelLoaded += OnPostLevelLoaded;
            if (Level.isLoaded)
            {
                OnPostLevelLoaded(0);
            }

            return UniTask.CompletedTask;
        }

        private void OnPostLevelLoaded(int level)
        {
            m_Logger.LogInformation(m_StringLocalizer["config:loading"]);

            WeaponsConfigs.Clear();
            var configs = m_Configuration.GetSection("Weapons").Get<WeaponConfig[]>();

            int errors = 0;

            if (configs?.Length > 0)
            {
                foreach (WeaponConfig config in configs)
                {
                    var features = config.GetFeatures();

                    if (features.Count == 0)
                    {
                        errors++;
                        m_Logger.LogWarning(m_StringLocalizer["config:warnings:no_features", new { config.Id }]);
                        continue;
                    }

                    double sum = 0;

                    foreach (IWeaponFeature feature in features)
                    {
                        if (feature.Chance.HasValue)
                        {
                            sum += feature.Chance.Value;

                            if (feature.Chance.Value == 0)
                            {
                                errors++;
                                m_Logger.LogWarning(m_StringLocalizer["config:warnings:chance_zero",
                                    new { config.Id, Feature = feature.GetType().Name }]);
                            }
                        }
                    }

                    if (sum > 1)
                    {
                        errors++;
                        m_Logger.LogWarning(m_StringLocalizer["config:warnings:sum_above_one", new { config.Id }]);
                    }

                    WeaponsConfigs.Add(config);
                }
            }
            else
            {
                errors++;
                m_Logger.LogWarning(m_StringLocalizer["config:warnings:no_weapons"]);
            }

            if (errors > 0)
            {
                m_Logger.LogWarning(m_StringLocalizer["config:warnings:loaded_errors"], new { Errors = errors });
            }
            else
            {
                m_Logger.LogInformation(m_StringLocalizer["config:loaded_success"]);
            }

            UseableGun.onBulletHit += OnBulletHit;
        }

        protected override UniTask OnUnloadAsync()
        {
            // ReSharper disable once DelegateSubtraction
            Level.onPostLevelLoaded -= OnPostLevelLoaded;
            UseableGun.onBulletHit -= OnBulletHit;

            foreach (WeaponConfig config in WeaponsConfigs)
            {
                foreach (IWeaponFeature feature in config.GetFeatures())
                {
                    if (feature is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
            }

            return UniTask.CompletedTask;
        }

        public void TriggerOnDamage(ushort itemId, Player victim, Player killer)
        {
            foreach (WeaponConfig weapon in WeaponsConfigs)
            {
                if (weapon.GetAsset().id != itemId) continue;

                foreach (var feature in weapon.GetDamageFeatures().FilterChances())
                {
                    feature.OnDamage(victim, killer);
                }
            }
        }

        public void TriggerOnKill(ushort itemId, Player victim, Player killer)
        {
            foreach (WeaponConfig weapon in WeaponsConfigs)
            {
                if (weapon.GetAsset().id != itemId) continue;

                foreach (var feature in weapon.GetKillFeatures().FilterChances())
                {
                    feature.OnKill(victim, killer);
                }
            }
        }

        private static MethodInfo _getBulletDamageMultiplierMethodInfo;

        private static float GetBulletDamageMultiplier(UseableGun gun, BulletInfo bullet)
        {
            _getBulletDamageMultiplierMethodInfo ??=
                AccessTools.Method(typeof(UseableGun), "getBulletDamageMultiplier");

            return (float) _getBulletDamageMultiplierMethodInfo.Invoke(gun, new object[] {bullet});
        }

        private static byte CalculateDamage(Player victim, UseableGun gun, BulletInfo bullet, ELimb limb)
        {
            IDamageMultiplier damageMultiplier = gun.equippedGunAsset.playerDamageMultiplier;

            bool respectArmor = limb == ELimb.SKULL && gun.equippedGunAsset.instakillHeadshots && Provider.modeConfigData.Players.Allow_Instakill_Headshots;

            float times = GetBulletDamageMultiplier(gun, bullet);

            times *= Provider.modeConfigData.Players.Armor_Multiplier;

            if (respectArmor)
            {
                times *= DamageTool.getPlayerArmor(limb, victim);
            }

            float damage = damageMultiplier.multiply(limb);

            int totalDamage = UnityEngine.Mathf.FloorToInt(damage * times);

            return (byte)UnityEngine.Mathf.Min(255, totalDamage);
        }

        private void OnBulletHit(UseableGun gun, BulletInfo bullet, InputInfo hit, ref bool shouldAllow)
        {
            if (hit.type != ERaycastInfoType.PLAYER) return;

            Player killer = gun.player;

            Player victim = hit.player;

            if (killer == null || victim == null) return;

            if (!DamageTool.isPlayerAllowedToDamagePlayer(killer, victim)) return;

            if (victim.movement.isSafe && victim.movement.isSafeInfo.noWeapons)
            {
                return;
            }

            byte damage = CalculateDamage(victim, gun, bullet, hit.limb);

            if (damage < victim.life.health)
            {
                TriggerOnDamage(gun.equippedGunAsset.id, victim, killer);
            }
            else
            {
                TriggerOnKill(gun.equippedGunAsset.id, victim, killer);
            }
        }
    }
}
