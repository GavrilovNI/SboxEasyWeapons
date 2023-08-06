using EasyWeapons.Bullets;
using EasyWeapons.Bullets.Datas;
using EasyWeapons.Bullets.Spawners;
using EasyWeapons.Effects;
using EasyWeapons.Inventories;
using EasyWeapons.Recoiles;
using EasyWeapons.Recoiles.Modules;
using EasyWeapons.Weapons.Modules.Attack.ShootingModes;
using Sandbox;
using System.Collections.Generic;

namespace EasyWeapons.Weapons.Modules.Attack;

public partial class SimpleAttackModule : AttackModule
{
    public virtual Ray AimRay => Weapon.AimRay;

    [Net, Local]
    public string AttackAction { get; protected set; } = "attack1";

    [Net, Local]
    public BulletSpawner BulletSpawner { get; protected set; }

    [Net, Local]
    public IList<WeaponEffect> AttackEffects { get; set; } = null!;

    [Net, Local]
    public IList<WeaponEffect> DryfireEffects { get; set; } = null!;

    [Net, Local]
    public Recoil? Recoil { get; set; }

    [Net, Local]
    public float NoOwnerRecoilForce { get; set; } = 50000;

    [Net, Predicted, Local]
    public ShootingMode ShootingMode { get; protected set; }


    public SimpleAttackModule()
    {
        Game.AssertClient();
        BulletSpawner = null!;
        ShootingMode = null!;
    }

    public SimpleAttackModule(AmmoInventory weaponClip, BulletSpawner bulletSpawner, ShootingMode shootingMode, string attackAction = "attack1") : base(weaponClip)
    {
        BulletSpawner = bulletSpawner;
        AttackAction = attackAction;
        ShootingMode = shootingMode;
    }

    public override SimulationResult Simulate(SimulationType simulationType)
    {
        ShootingMode.OnSimulateBegin(simulationType, AttackAction);

        if(ShouldAttack())
            base.Simulate(simulationType);

        return ShootingMode.ShouldAttack() ? SimulationResult.Continuing : SimulationResult.Finished;
    }

    public override void Attack()
    {
        base.Attack();
        ShootingMode.OnShot();
    }

    protected override void Fail()
    {
        base.Fail();
        ShootingMode.OnFail();
        DryfireEffects.Play(Weapon);
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        ShootingMode.OnFail();
    }

    protected virtual bool ShouldAttack()
    {
        bool timePassed = TimeSinceLastAttackTry >= MinTimeBetweenAttacks &&
                            TimeSinceLastAttack >= MinTimeBetweenAttacks &&
                            TimeSinceLastFail >= MinTimeBetweenAttacks;

        if(timePassed == false)
            return false;

        return ShootingMode.ShouldAttack();
    }

    protected override void Shoot(string ammoId)
    {
        BulletSpawner.Spawn(AimRay, BulletsRegister.Instanse, ammoId);
        ApplyRecoil();
        AttackEffects.Play(Weapon);
    }

    protected virtual void ApplyRecoil()
    {
        if(Recoil is null)
            return;

        var owner = Entity.Owner;
        if(owner is null)
        {
            Weapon.PhysicsBody.ApplyForceAt(AimRay.Position, -AimRay.Forward.Normal * NoOwnerRecoilForce);
            return;
        }

        var recoilApplier = owner.Components.Get<IRecoilApplier>();
        if(recoilApplier is null)
            return;

        Recoil.ApplyRecoil(recoilApplier);
    }
}
