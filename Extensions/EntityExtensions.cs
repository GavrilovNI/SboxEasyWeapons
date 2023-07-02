using EasyWeapons.Inventories;
using Sandbox;
using System;

namespace EasyWeapons.Extensions;

public static class EntityExtensions
{
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
