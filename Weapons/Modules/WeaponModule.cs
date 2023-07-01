using Sandbox;

namespace EasyWeapons.Weapons.Modules;

public abstract class WeaponModule : EntityComponent<Weapon>
{
    public Weapon Weapon => Entity;

    public abstract SimulationResult Simulate();
}
