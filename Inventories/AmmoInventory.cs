
using Sandbox;
using System;
using System.Collections.Generic;

namespace EasyWeapons.Inventories;

public abstract class AmmoInventory : BaseNetworkable, IAmmoInventory
{
    protected abstract void Set(string ammoId, int count);
    public abstract int GetCount(string ammoId);
    public abstract int GetCount();

    public abstract List<OneTypeAmmoInventory> TakeSome(int maxCount);

    public virtual int GetLimit(string ammoId) => int.MaxValue;
    public virtual int GetLimit() => int.MaxValue;


    protected virtual void Subtract(string ammoId, int count)
    {
        var oldCount = GetCount(ammoId);
        Set(ammoId, oldCount - count);
    }

    public virtual void Add(string ammoId, int count)
    {
        if(CanAdd(ammoId, count) == false)
            throw new InvalidOperationException($"Can't add. Test {nameof(CanAdd)} first.");

        int currentAmount = GetCount(ammoId);
        Set(ammoId, currentAmount + count);
    }

    public virtual bool CanAdd(string ammoId, int count)
    {
        if(count < 0)
            throw new ArgumentOutOfRangeException(nameof(count));

        int currentAmount = GetCount(ammoId);
        return int.MaxValue - currentAmount > count;
    }

    public virtual OneTypeAmmoInventory TakeExact(string ammoId, int count)
    {
        int currentCount = GetCount(ammoId);
        if(currentCount < count)
            throw new InvalidOperationException($"Can't take. Check {nameof(GetCount)} first.");

        return TakeSome(ammoId, count);
    }

    public virtual List<OneTypeAmmoInventory> TakeExact(int count)
    {
        int currentCount = GetCount();
        if(currentCount < count)
            throw new InvalidOperationException($"Can't take. Check {nameof(GetCount)} first.");

        return TakeSome(count);
    }

    public virtual OneTypeAmmoInventory TakeSome(string ammoId, int maxCount)
    {
        int currentCount = GetCount(ammoId);
        int maxCanTake = Math.Min(currentCount, maxCount);
        Set(ammoId, currentCount - maxCanTake);
        return OneTypeAmmoInventory.Full(ammoId, maxCanTake);

    }
}
