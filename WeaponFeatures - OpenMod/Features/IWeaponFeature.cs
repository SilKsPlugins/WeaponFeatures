using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponFeatures.Features
{
    public interface IWeaponFeature
    {
        double? Chance { get; set; }
    }
}
