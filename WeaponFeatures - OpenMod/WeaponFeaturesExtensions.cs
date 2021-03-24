using System;
using System.Collections.Generic;
using WeaponFeatures.Features;

namespace WeaponFeatures
{
    public static class WeaponFeaturesExtensions
    {
        private static Random _rng = new Random();

        public static IEnumerable<T> FilterChances<T>(this IEnumerable<T> features) where T : IWeaponFeature
        {
            List<T> filtered = new List<T>();

            double random = _rng.NextDouble();

            foreach (T feature in features)
            {
                if (feature.Chance.HasValue)
                {
                    // If random < 0, chance-based feature is already found
                    if (random < 0) continue;

                    random -= feature.Chance.Value;

                    if (random >= 0) continue;

                    filtered.Add(feature);
                }
                else
                {
                    filtered.Add(feature);
                }
            }

            return filtered;
        }
    }
}
