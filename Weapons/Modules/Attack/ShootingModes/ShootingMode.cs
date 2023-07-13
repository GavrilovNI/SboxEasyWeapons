using Sandbox;
using System;

namespace EasyWeapons.Weapons.Modules.Attack.ShootingModes;

public abstract partial class ShootingMode : BaseNetworkable
{
    public abstract bool ShouldAttack();

    public virtual void OnSimulateBegin(SimulationType simulationType, string inputAction)
    {
    }

    public virtual void OnShot()
    {
    }

    public virtual void OnFail()
    {
    }
}
