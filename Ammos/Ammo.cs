using Sandbox;
using System;

namespace EasyWeapons.Ammos;

public partial class Ammo : BaseNetworkable
{
    [Net]
    public string Id { get; private set; }

    [Net]
    public string Name { get; private set; }

    public Ammo()
    {
        Game.AssertClient();
        Id = null!;
        Name = null!;
    }

    public Ammo(string id, string name)
    {
        if(name == null)
            throw new ArgumentNullException(nameof(name));

        Id = id;
        Name = name;
    }

    public static bool operator ==(Ammo left, Ammo right)
    {
        if(ReferenceEquals(left, right))
            return true;

        return left.Id == right.Id;
    }

    public static bool operator !=(Ammo left, Ammo right)
    {
        return !(left == right);
    }

    public override bool Equals(object? obj)
    {
        if(obj is Ammo ammoType)
            return this == ammoType;
        return false;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public override string ToString()
    {
        return $"{GetType().Name}({nameof(Id)}={Id}, {nameof(Name)}={Name})";
    }
}
