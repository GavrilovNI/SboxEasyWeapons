

namespace EasyWeapons.Inventories;

public static class AmmoInventoryExtensions
{
    public static bool IsEmpty(this IAmmoInventory inventory) => inventory.GetCount() <= 0;

    public static bool ContainsAny(this IAmmoInventory inventory) => inventory.GetCount() > 0;

    public static bool Contains(this IAmmoInventory inventory, string ammoId) => inventory.GetCount(ammoId) > 0;

    public static int GetMaxAmountCanAdd(this IAmmoInventory inventory) => inventory.GetLimit() - inventory.GetCount();
    public static int GetMaxAmountCanAdd(this IAmmoInventory inventory, string ammoId) => inventory.GetLimit(ammoId) - inventory.GetCount(ammoId);

    public static bool CanAdd(this IAmmoInventory inventory, OneTypeAmmoInventory ammoSet) => inventory.CanAdd(ammoSet.AmmoId, ammoSet.AmmoCount);
    public static void Add(this IAmmoInventory inventory, OneTypeAmmoInventory ammoSet) => inventory.Add(ammoSet.AmmoId, ammoSet.AmmoCount);
    public static OneTypeAmmoInventory TakeExact(this IAmmoInventory inventory, OneTypeAmmoInventory ammoSet) => inventory.TakeExact(ammoSet.AmmoId, ammoSet.AmmoCount);
    public static OneTypeAmmoInventory TakeSome(this IAmmoInventory inventory, OneTypeAmmoInventory ammoSet) => inventory.TakeSome(ammoSet.AmmoId, ammoSet.AmmoCount);

    //returns not added ammo
    public static OneTypeAmmoInventory AddAsMaxAsCan(this IAmmoInventory inventory, string ammoId, int count)
    {
        int amountCanAdd = inventory.GetMaxAmountCanAdd(ammoId);

        if(amountCanAdd > count)
            amountCanAdd = count;

        inventory.Add(ammoId, amountCanAdd);

        return OneTypeAmmoInventory.Full(ammoId, count - amountCanAdd);
    }
    public static OneTypeAmmoInventory AddAsMaxAsCan(this IAmmoInventory inventory, OneTypeAmmoInventory ammoSet) => AddAsMaxAsCan(inventory, ammoSet.AmmoId, ammoSet.AmmoCount);
}
