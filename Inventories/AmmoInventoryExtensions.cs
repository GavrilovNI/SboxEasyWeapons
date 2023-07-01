

namespace EasyWeapons.Inventories;

public static class AmmoInventoryExtensions
{
    public static bool CanAdd(this IAmmoInventory inventory, OneTypeAmmoInventory ammoSet) => inventory.CanAdd(ammoSet.AmmoId, ammoSet.AmmoCount);
    public static void Add(this IAmmoInventory inventory, OneTypeAmmoInventory ammoSet) => inventory.Add(ammoSet.AmmoId, ammoSet.AmmoCount);
    public static OneTypeAmmoInventory TakeExactAmmo(this IAmmoInventory inventory, OneTypeAmmoInventory ammoSet) => inventory.TakeExactAmmo(ammoSet.AmmoId, ammoSet.AmmoCount);
    public static OneTypeAmmoInventory TakeSomeAmmo(this IAmmoInventory inventory, OneTypeAmmoInventory ammoSet) => inventory.TakeSomeAmmo(ammoSet.AmmoId, ammoSet.AmmoCount);

    //returns left ammo set
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
