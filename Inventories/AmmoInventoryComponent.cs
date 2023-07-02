using Sandbox;

namespace EasyWeapons.Inventories;

public partial class AmmoInventoryComponent : EntityComponent, IAmmoInventoryComponent
{
    [Net, Local]
    public AmmoInventory AmmoInventory { get; set; } = new MultiTypeAmmoInventory();

    IAmmoInventory IAmmoInventoryOwner.AmmoInventory => AmmoInventory;
}
