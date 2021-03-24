using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WeaponFeatures.Features;

namespace WeaponFeatures.Configuration
{
    public class WeaponConfig
    {
        public string Id { get; set; }

        private static int LevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];
            if (n == 0)
            {
                return m;
            }
            if (m == 0)
            {
                return n;
            }
            for (int i = 0; i <= n; d[i, 0] = i++)
                ;
            for (int j = 0; j <= m; d[0, j] = j++)
                ;
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            return d[n, m];
        }

        private ItemAsset _cachedAsset;

        public ItemAsset GetAsset()
        {
            if (_cachedAsset != null) return _cachedAsset;

            if (string.IsNullOrWhiteSpace(Id)) return null;

            if (ushort.TryParse(Id, out ushort parsed))
            {
                _cachedAsset = Assets.find(EAssetType.ITEM, parsed) as ItemAsset;

                if (_cachedAsset != null) return _cachedAsset;
            }

            List<ItemAsset> possibilities = new List<ItemAsset>();

            string lowered = Id.ToLower();

            foreach (ItemAsset asset in Assets.find(EAssetType.ITEM).OfType<ItemAsset>())
            {
                if (string.IsNullOrWhiteSpace(asset.itemName)) continue;

                if (asset.itemName.ToLower().Contains(lowered))
                {
                    possibilities.Add(asset);
                }
            }

            _cachedAsset = possibilities
                .OrderBy(x => LevenshteinDistance(x.itemName.ToLower(), lowered))
                .FirstOrDefault();

            return _cachedAsset;
        }

        private List<IWeaponFeature> _cachedFeatures;

        private List<IWeaponFeatureDamage> _cachedDamageFeatures;

        private List<IWeaponFeatureKill> _cachedKillFeatures;

        public IReadOnlyCollection<IWeaponFeature> GetFeatures()
        {
            if (_cachedFeatures == null)
            {
                _cachedFeatures = new List<IWeaponFeature>();

                foreach (PropertyInfo property in GetType().GetProperties())
                {
                    if (!typeof(IWeaponFeature).IsAssignableFrom(property.PropertyType)) continue;

                    IWeaponFeature value = property.GetValue(this) as IWeaponFeature;

                    if (value == null) continue;

                    _cachedFeatures.Add(value);
                }
            }

            return _cachedFeatures;
        }

        public IReadOnlyCollection<IWeaponFeatureDamage> GetDamageFeatures()
        {
            if (_cachedDamageFeatures == null)
            {
                _cachedDamageFeatures = GetFeatures().OfType<IWeaponFeatureDamage>().ToList();
            }

            return _cachedDamageFeatures;
        }

        public IReadOnlyCollection<IWeaponFeatureKill> GetKillFeatures()
        {
            if (_cachedKillFeatures == null)
            {
                _cachedKillFeatures = GetFeatures().OfType<IWeaponFeatureKill>().ToList();
            }

            return _cachedKillFeatures;
        }

        public FeatureBreakLegs BreakLegs { get; set; }

        public FeatureBurnEffect BurnEffect { get; set; }

        public FeatureDehydrate Dehydrate { get; set; }

        public FeatureDrugEffect DrugEffect { get; set; }

        public FeatureHealHolder HealHolder { get; set; }

        public FeatureStaminaDrain StaminaDrain { get; set; }
        
        public FeatureStarve Starve { get; set; }
    }
}
