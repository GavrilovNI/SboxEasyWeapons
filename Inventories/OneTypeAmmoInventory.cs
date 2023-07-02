using Sandbox;
using System;
using System.Collections.Generic;

namespace EasyWeapons.Inventories;

public partial class OneTypeAmmoInventory : AmmoInventory
{
    [Net, Predicted, Local]
    public string AmmoId { get; set; } = null!;

    [Net, Predicted, Local]
    public int AmmoCount { get; set; }

    [Net, Predicted, Local]
    public int MaxAmmoCount { get; set; }


    public OneTypeAmmoInventory()
    {
        Game.AssertClient();
    }

    private OneTypeAmmoInventory(string ammoId)
    {
        AmmoId = ammoId;
    }


    public static OneTypeAmmoInventory Of(string ammoId, int count, int maxAmmoCount)
    {
        if(maxAmmoCount < 0)
            maxAmmoCount = 0;

        count = Math.Clamp(count, 0, maxAmmoCount);

        return new OneTypeAmmoInventory(ammoId) { AmmoId = ammoId, AmmoCount = count, MaxAmmoCount = maxAmmoCount };
    }

    public static OneTypeAmmoInventory Full(string ammoId, int maxAmmoCount)
    {
        return Of(ammoId, maxAmmoCount, maxAmmoCount);
    }

    public static OneTypeAmmoInventory Empty(string ammoId, int maxAmmoCount)
    {
        return Of(ammoId, 0, maxAmmoCount);
    }

    protected override void Set(string ammoId, int count)
    {
        if(AmmoId == ammoId)
            AmmoCount = count;
    }

    public override int GetCount(string ammoId) => AmmoId == ammoId ? AmmoCount : 0;

    public override int GetCount() => AmmoCount;


    public override List<OneTypeAmmoInventory> TakeSome(int maxCount)
    {
        var countToTake = Math.Min(maxCount, AmmoCount);
        AmmoCount -= countToTake;
        return new List<OneTypeAmmoInventory>() { OneTypeAmmoInventory.Full(AmmoId, countToTake) };
    }
}
