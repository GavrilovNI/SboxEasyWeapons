using Sandbox;
using System.Collections.Generic;

namespace EasyWeapons.Inventories;

public partial class InfinityAmmoInventory : AmmoInventory
{
    [Net, Predicted, Local]
    public string AmmoId { get; set; }

    public InfinityAmmoInventory()
    {
        Game.AssertClient();
        AmmoId = null!;
    }

    public InfinityAmmoInventory(string ammoId)
    {
        AmmoId = ammoId;
    }

    protected override void Set(string ammoId, int count) { }
    public override int GetAmmoCount(string ammoId) => int.MaxValue;
    public override int GetAmmoCount() => int.MaxValue;
    public override List<OneTypeAmmoInventory> TakeSomeAmmo(int maxCount) => new() { OneTypeAmmoInventory.Full(AmmoId, maxCount) };
}
