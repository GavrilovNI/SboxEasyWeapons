using EasyWeapons.Weapons;
using System.Collections.Generic;
using System.Threading;

namespace EasyWeapons.Effects;

public static class WeaponEffects
{
    public static void Play(this IList<WeaponEffect>? weaponEffects, Weapon weapon, CancellationToken? cancellationToken = null)
    {
        if(weaponEffects == null)
            return;

        foreach (var weaponEffect in weaponEffects)
            weaponEffect.Play(weapon, cancellationToken);
    }
}
