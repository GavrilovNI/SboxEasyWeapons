using Sandbox;
using System;
using System.Collections.Generic;

namespace EasyWeapons.Inventories;

public partial class MultiTypeAmmoInventory : AmmoInventory
{
    [Net, Predicted, Local]
    public int AmmoCount { get; private set; }

    [Net, Predicted, Local]
    private IDictionary<string, int> Ammos { get; set; } = new Dictionary<string, int>();


    protected override void Set(string ammoId, int count)
    {
        var oldCount = GetAmmoCount(ammoId);
        if(count == 0)
            Ammos.Remove(ammoId);
        else
            Ammos[ammoId] = count;
        AmmoCount += count - oldCount;
    }

    public override int GetMaxAmountCanAdd(string ammoId)
    {
        if(Ammos.TryGetValue(ammoId, out int currentAmount))
            return int.MaxValue - currentAmount;

        return int.MaxValue;
    }

    public override bool HasAmmo()
    {
        return Ammos.Keys.Count > 0;
    }

    public override int GetAmmoCount(string ammoId)
    {
        if(Ammos.TryGetValue(ammoId, out int currentAmount))
            return currentAmount;
        return 0;
    }

    public override int GetAmmoCount()
    {
        return AmmoCount;
    }

    public override List<OneTypeAmmoInventory> TakeSomeAmmo(int maxCount)
    {
        List<OneTypeAmmoInventory> result = new();

        foreach(var ammo in Ammos)
        {
            int maxCanGetFromCurrent = Math.Min(maxCount, ammo.Value);

            result.Add(OneTypeAmmoInventory.Full(ammo.Key!, maxCanGetFromCurrent));
            maxCount -= maxCanGetFromCurrent;
            Subtract(ammo.Key!, maxCanGetFromCurrent);
        }

        return result;
    }
}