using EasyWeapons.Weapons;
using Sandbox;
using System.Threading;

namespace EasyWeapons.Effects;

public abstract partial class WeaponEffect: BaseNetworkable
{
    public abstract void Play(Weapon weapon, CancellationToken? cancellationToken = null);
}
