using Sandbox;

namespace EasyWeapons.Weapons.Modules.Aiming.Effects;

public abstract partial class AimingEffect : BaseNetworkable
{
    [Net, Predicted, Local]
    public Weapon Weapon { get; set; } = null!;

    public AimingEffect()
    {
        Game.AssertClient();
    }

    public AimingEffect(Weapon weapon)
    {
        Weapon = weapon;
    }

    public abstract SimulationResult Simulate();

    public abstract void EnableAiming();
    public abstract void DisableAiming();

    public abstract void Cancel();
}
