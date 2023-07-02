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
    public override int GetCount(string ammoId) => int.MaxValue;
    public override int GetCount() => int.MaxValue;
    public override List<OneTypeAmmoInventory> TakeSome(int maxCount) => new() { OneTypeAmmoInventory.Full(AmmoId, maxCount) };
}
