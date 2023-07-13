using EasyWeapons.Extensions;
using EasyWeapons.Inventories;
using EasyWeapons.Sounds;
using Sandbox;
using System.Threading;

namespace EasyWeapons.Weapons.Modules.Reload;

public partial class SimpleReloadModule : ReloadModule
{
    [Net]
    public AmmoInventory WeaponClip { get; set; }

    public IAmmoInventory? AmmoInventory
    {
        get
        {
            if(Weapon.Owner == null)
                return null;

            return Weapon.Owner.GetOrCreateAmmoInventoryComponent()?.AmmoInventory;
        }
    }

    [Net]
    public string AmmoId { get; set; }

    [Net]
    public string ReloadAction { get; set; }

    [Net]
    public PlayableDelayedSound? ReloadSound { get; set; }

    [Net]
    public PlayableDelayedSound? ReloadFailSound { get; set; }

    [Net]
    public string? ReloadAnimation { get; set; } = "reload";

    [Net]
    public string? WorldReloadAnimation { get; set; } = "b_reload";

    [Net]
    private OneTypeAmmoInventory? ReloadingSet { get; set; }

    private CancellationTokenSource? SoundCancellationTokenSource { get; set; }

    public SimpleReloadModule()
    {
        Game.AssertClient();
        WeaponClip = null!;
        AmmoId = null!;
        ReloadAction = null!;
    }

    public SimpleReloadModule(OneTypeAmmoInventory weaponClip, string reloadAction = "reload") :
        this(weaponClip, weaponClip.AmmoId, reloadAction)
    {
    }

    public SimpleReloadModule(AmmoInventory weaponClip, string ammoTypeId, string reloadAction = "reload")
    {
        WeaponClip = weaponClip;
        AmmoId = ammoTypeId;
        ReloadAction = reloadAction;
    }

    protected override void OnDeactivate()
    {
        if(IsReloading)
            Cancel();

        base.OnDeactivate();
    }

    public override SimulationResult Simulate(SimulationType simulationType)
    {
        if(Input.Pressed(ReloadAction))
        {
            if(CanStartReload())
                Reload();
            else if(WeaponClip.GetMaxAmountCanAdd(AmmoId) > 0)
                DoFailEffects();
        }

        if(IsReloading)
        {
            HandleReload();
            return SimulationResult.Continuing;
        }

        return SimulationResult.Finished;
    }

    public override bool CanStartReload()
    {
        if(base.CanStartReload() == false)
            return false;

        var ammoInventory = AmmoInventory;
        if(ammoInventory == null)
            return false;

        var maxAmmoNeeded = WeaponClip.GetMaxAmountCanAdd(AmmoId);
        if(maxAmmoNeeded == 0)
            return false;

        return ammoInventory.Contains(AmmoId);
    }

    public override void Reload()
    {
        base.Reload();
        var maxAmmoNeeded = WeaponClip.GetMaxAmountCanAdd(AmmoId);
        ReloadingSet = AmmoInventory!.TakeSome(AmmoId, maxAmmoNeeded);
        DoReloadEffects();
    }

    public override void Cancel()
    {
        base.Cancel();

        SoundCancellationTokenSource?.Cancel();
        SoundCancellationTokenSource = null;

        ReturnReloadingAmmo();
        if(ReloadingSet!.AmmoCount > 0)
            DropReloadingAmmo();

        ReloadingSet = null;
    }

    protected virtual void ReturnReloadingAmmo()
    {
        ReloadingSet = AmmoInventory!.AddAsMaxAsCan(ReloadingSet!);
    }

    protected virtual void DropReloadingAmmo()
    {
        //TODO: drop ammo
    }

    protected override void OnFinishedReload()
    {
        ReloadingSet = WeaponClip.AddAsMaxAsCan(ReloadingSet!);
        ReturnReloadingAmmo();
        if(ReloadingSet.AmmoCount > 0)
            DropReloadingAmmo();

        SoundCancellationTokenSource = null;
    }

    protected virtual void DoReloadEffects()
    {
        if(WorldReloadAnimation is not null)
            Weapon.SetWorldModelAnimParameter(WorldReloadAnimation, true);

        if(Game.IsServer)
        {
            SoundCancellationTokenSource = new CancellationTokenSource();
            ReloadSound?.PlayOnEntity(Weapon, SoundCancellationTokenSource.Token);
            if(ReloadAnimation is not null)
                Weapon.SetViewModelAnimParameter(ReloadAnimation, true);
        }
    }

    protected virtual void DoFailEffects()
    {
        if(Game.IsClient)
            ReloadFailSound?.PlayOnEntity(Weapon);
    }
}