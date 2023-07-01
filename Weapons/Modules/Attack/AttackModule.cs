using EasyWeapons.Inventories;
using Sandbox;
using System;

namespace EasyWeapons.Weapons.Modules.Attack;

public abstract partial class AttackModule : WeaponModule
{
    [Net, Predicted, Local]
    public AmmoInventory WeaponClip { get; private set; }
    [Net, Predicted, Local]
    public TimeSince TimeSinceLastAttack { get; protected set; } = float.MaxValue;

    [Net, Local]
    public float FireRate { get; set; } = 10f;

    public float MinTimeBetweenAttacks => FireRate < 0.0001f ? float.MaxValue : 1f / FireRate;


    public AttackModule()
    {
        Game.AssertClient();
        WeaponClip = null!;
    }

    public AttackModule(AmmoInventory weaponClip)
    {
        WeaponClip = weaponClip;
    }


    public override SimulationResult Simulate()
    {
        if(CanStartAttack())
        {
            Attack();
            return SimulationResult.Continuing;
        }

        return SimulationResult.Finished;
    }

    public virtual bool CanStartAttack()
    {
        return TimeSinceLastAttack >= MinTimeBetweenAttacks && WeaponClip.HasAmmo();
    }


    public virtual void Attack()
    {
        if(CanStartAttack() == false)
            throw new InvalidOperationException($"Can't attack. Test {CanStartAttack} first.");

        Enabled = true;

        WeaponClip.TakeExactAmmo(1);
        using(Sandbox.Entity.LagCompensation())
        {
            Shoot();
        }
        TimeSinceLastAttack = 0;
    }

    protected abstract void Shoot();
}
