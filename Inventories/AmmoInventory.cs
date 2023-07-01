
using Sandbox;
using System;
using System.Collections.Generic;

namespace EasyWeapons.Inventories;

public abstract class AmmoInventory : BaseNetworkable, IAmmoInventory
{
    protected abstract void Set(string ammoId, int count);
    public abstract int GetAmmoCount(string ammoId);
    public abstract int GetAmmoCount();

    public abstract List<OneTypeAmmoInventory> TakeSomeAmmo(int maxCount);


    public virtual bool HasAmmo() => GetAmmoCount() > 0;

    public virtual bool HasAmmo(string ammoId) => GetAmmoCount(ammoId) > 0;


    protected virtual void Subtract(string ammoId, int count)
    {
        var oldCount = GetAmmoCount(ammoId);
        Set(ammoId, oldCount - count);
    }

    public virtual void Add(string ammoId, int count)
    {
        if(CanAdd(ammoId, count) == false)
            throw new InvalidOperationException($"Can't add. Test {nameof(CanAdd)} first.");

        int currentAmount = GetAmmoCount(ammoId);
        Set(ammoId, currentAmount + count);
    }

    public virtual bool CanAdd(string ammoId, int count)
    {
        if(count < 0)
            throw new ArgumentOutOfRangeException(nameof(count));

        int currentAmount = GetAmmoCount(ammoId);
        return int.MaxValue - currentAmount > count;
    }

    public virtual int GetMaxAmountCanAdd(string ammoId)
    {
        int currentAmount = GetAmmoCount(ammoId);
        return int.MaxValue - currentAmount;
    }

    public virtual OneTypeAmmoInventory TakeExactAmmo(string ammoId, int count)
    {
        int currentCount = GetAmmoCount(ammoId);
        if(currentCount < count)
            throw new InvalidOperationException($"Can't take. Check {nameof(GetAmmoCount)} first.");

        return TakeSomeAmmo(ammoId, count);
    }

    public virtual List<OneTypeAmmoInventory> TakeExactAmmo(int count)
    {
        int currentCount = GetAmmoCount();
        if(currentCount < count)
            throw new InvalidOperationException($"Can't take. Check {nameof(GetAmmoCount)} first.");

        return TakeSomeAmmo(count);
    }

    public virtual OneTypeAmmoInventory TakeSomeAmmo(string ammoId, int maxCount)
    {
        int currentCount = GetAmmoCount(ammoId);
        int maxCanTake = Math.Min(currentCount, maxCount);
        Set(ammoId, currentCount - maxCanTake);
        return OneTypeAmmoInventory.Full(ammoId, maxCanTake);

    }
}
