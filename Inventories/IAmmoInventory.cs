using System.Collections.Generic;

namespace EasyWeapons.Inventories;

public interface IAmmoInventory
{
    bool CanAdd(string ammoId, int count);
    int GetMaxAmountCanAdd(string ammoId);
    void Add(string ammoId, int count);

    bool HasAmmo();
    bool HasAmmo(string ammoId);

    int GetAmmoCount(string ammoId);
    int GetAmmoCount();

    OneTypeAmmoInventory TakeExactAmmo(string ammoId, int count);
    OneTypeAmmoInventory TakeSomeAmmo(string ammoId, int maxCount);

    List<OneTypeAmmoInventory> TakeExactAmmo(int count);
    List<OneTypeAmmoInventory> TakeSomeAmmo(int maxCount);
}
