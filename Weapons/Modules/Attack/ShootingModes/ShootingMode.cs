using Sandbox;
using System;

namespace EasyWeapons.Weapons.Modules.Attack.ShootingModes;

public abstract partial class ShootingMode : BaseNetworkable
{
    [Net, Predicted, Local]
    public bool IsShooting { get; protected set; } = false;

    public abstract bool ShouldAttack(string inputAction);
    public abstract bool ShouldAttack();

    public virtual void OnShot()
    {
        IsShooting = true;
    }

    public virtual void OnFail()
    {
        IsShooting = false;
    }
}
