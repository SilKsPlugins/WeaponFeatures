using HarmonyLib;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace WeaponFeatures.Features
{
    public class FeatureBurnEffect : IWeaponFeatureDamage, IDisposable
    {
        public FeatureBurnEffect()
        {
            _burningPlayers = new Dictionary<CSteamID, float>();
            _lastEffect = new Dictionary<CSteamID, float>();

            OnLifeSimulating += Events_OnLifeSimulating;
            OnLifeSimulated += Events_OnLifeSimulated;
        }

        public void Dispose()
        {
            OnLifeSimulating -= Events_OnLifeSimulating;
            OnLifeSimulated -= Events_OnLifeSimulated;
        }

        public float Duration { get; set; }

        public byte Damage { get; set; } = 10;

        public double? Chance { get; set; }

        private readonly Dictionary<CSteamID, float> _burningPlayers;
        private readonly Dictionary<CSteamID, float> _lastEffect;

        public void OnDamage(Player victim, Player killer)
        {
            if (Duration == 0) return;

            CSteamID id = victim.channel.owner.playerID.steamID;

            if (_burningPlayers.ContainsKey(id))
            {
                _burningPlayers[id] = Time.time;
            }
            else
            {
                _burningPlayers.Add(id, Time.time);
            }
        }

        private static FieldInfo _lastBurnFieldInfo;
        private static FieldInfo _lastWarmFieldInfo;
        private static FieldInfo _wasWarmFieldInfo;

        private void Events_OnLifeSimulating(Player player, uint simulation, ref bool applied)
        {
            _lastBurnFieldInfo ??= AccessTools.Field(typeof(PlayerLife), "lastBurn");
            _lastWarmFieldInfo ??= AccessTools.Field(typeof(PlayerLife), "lastWarm");
            _wasWarmFieldInfo ??= AccessTools.Field(typeof(PlayerLife), "wasWarm");

            if (_lastBurnFieldInfo == null || _lastWarmFieldInfo == null || _wasWarmFieldInfo == null)
            {
                throw new Exception("A private field accessor for PlayerLife is null");
            }

            CSteamID id = player.channel.owner.playerID.steamID;

            if (_burningPlayers.TryGetValue(id, out float startTime))
            {
                if (Time.time - startTime <= Duration)
                {
                    if (player.life.isDead)
                    {
                        _burningPlayers[id] = 0;
                        return;
                    }

                    applied = true;

                    if (simulation - (uint)_lastBurnFieldInfo.GetValue(player.life) > 10U)
                    {
                        _lastBurnFieldInfo.SetValue(player.life, simulation);
                        player.life.askDamage(Damage, Vector3.up, EDeathCause.BURNING, ELimb.SPINE, Provider.server, out _);
                    }

                    _lastWarmFieldInfo.SetValue(player.life, simulation);
                    _wasWarmFieldInfo.SetValue(player.life, true);

                    if (!_lastEffect.TryGetValue(id, out float lastEffectTime))
                    {
                        lastEffectTime = 0;
                        _lastEffect.Add(id, Time.time);
                    }

                    if (Time.time - lastEffectTime >= 1)
                    {
                        _lastEffect[id] = Time.time;

                        EffectManager.sendEffectReliable(139, EffectManager.MEDIUM, player.look.getEyesPosition());
                    }
                }
            }
        }

        private void Events_OnLifeSimulated(Player player, bool applied)
        {
            if (applied)
            {
                player.life.onTemperatureUpdated?.Invoke(EPlayerTemperature.BURNING);
            }
        }

        private delegate void LifeSimulating(Player player, uint simulation, ref bool applied);
        private static event LifeSimulating OnLifeSimulating;

        private delegate void LifeSimulated(Player player, bool applied);
        private static event LifeSimulated OnLifeSimulated;

        [Obfuscation(Exclude = true)]
        [HarmonyPatch(typeof(PlayerLife), "simulate")]
        private class PlayerLifePatch
        {
            [HarmonyPrefix]
            static void Prefix(PlayerLife __instance, uint simulation, ref uint ___lastBurn, ref uint ___lastWarm, ref bool ___wasWarm, ref bool __state)
            {
                __state = false;

                OnLifeSimulating?.Invoke(__instance.player, simulation, ref __state);
            }

            [HarmonyPostfix]
            static void Postfix(PlayerLife __instance, bool __state)
            {
                OnLifeSimulated?.Invoke(__instance.player, __state);
            }
        }
    }
}