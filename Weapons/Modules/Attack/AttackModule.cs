using EasyWeapons.Inventories;
using Sandbox;
using System;

namespace EasyWeapons.Weapons.Modules.Attack;

public abstract partial class AttackModule : WeaponModule
{
    [Net, Predicted, Local]
    public AmmoInventory WeaponClip { get; private set; }

    [Net, Predicted, Local]
    public TimeSince TimeSinceLastAttackTry { get; protected set; } = float.MaxValue;

    [Net, Predicted, Local]
    public TimeSince TimeSinceLastAttack { get; protected set; } = float.MaxValue;

    [Net, Predicted, Local]
    public TimeSince TimeSinceLastFail { get; protected set; } = float.MaxValue;

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
        var result = SimulationResult.Finished;
        if(CanStartAttack())
        {
            Attack();
            result = SimulationResult.Continuing;
        }
        else if(CanStartFail())
        {
            Fail();
        }

        TimeSinceLastAttackTry = 0;
        return result;
    }

    public virtual bool CanStartAttack()
    {
        return TimeSinceLastAttack >= MinTimeBetweenAttacks && WeaponClip.HasAmmo();
    }

    protected virtual bool CanStartFail()
    {
        return TimeSinceLastAttackTry >= MinTimeBetweenAttacks;
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

    protected virtual void Fail()
    {
        TimeSinceLastFail = 0;
    }

    protected abstract void Shoot();
}
