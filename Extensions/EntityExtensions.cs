using EasyWeapons.Inventories;
using Sandbox;
using System;

namespace EasyWeapons.Extensions;

public static class EntityExtensions
{
    public static IAmmoInventory? FindAmmoInventory(this IEntity entity, bool includeDisabledComponents = false)
    {
        if(entity is IAmmoInventoryOwner owner)
            return owner.AmmoInventory;

        if(entity is IAmmoInventory ammoInventory)
            return ammoInventory;

        return entity.Components.Get<IAmmoInventoryComponent>(includeDisabledComponents)?.AmmoInventory;
    }

    public static IAmmoInventoryOwner? GetOrCreateAmmoInventoryComponent(this IEntity entity, bool includeDisabled = false)
    {
        var components = entity.Components;

        var foundComponent = components.Get<IAmmoInventoryComponent>(includeDisabled);
        if (foundComponent != null)
            return foundComponent;

        if(Game.IsServer == false)
            return null;

        var newComponent = new AmmoInventoryComponent();
        bool added = components.Add(newComponent);

        if(added == false)
            return null;

        return newComponent;
    }
}
