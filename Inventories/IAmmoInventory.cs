using System.Collections.Generic;

namespace EasyWeapons.Inventories;

public interface IAmmoInventory
{
    bool CanAdd(string ammoId, int count);
    void Add(string ammoId, int count);

    int GetLimit(string ammoId);
    int GetLimit();
    int GetCount(string ammoId);
    int GetCount();

    OneTypeAmmoInventory TakeExact(string ammoId, int count);
    OneTypeAmmoInventory TakeSome(string ammoId, int maxCount);

    List<OneTypeAmmoInventory> TakeExact(int count);
    List<OneTypeAmmoInventory> TakeSome(int maxCount);
}
