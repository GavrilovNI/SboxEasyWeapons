using Sandbox;

namespace EasyWeapons.Weapons.Modules.Attack.ShootingModes;

public partial class SemiShootingMode : ShootingMode
{
    [Net, Predicted, Local]
    public bool AttackPressed { get; set; }

    public override bool ShouldAttack()
    {
        return AttackPressed;
    }

    public override void OnSimulateBegin(SimulationType simulationType, string inputAction)
    {
        base.OnSimulateBegin(simulationType, inputAction);
        AttackPressed = simulationType == SimulationType.Simulating && Input.Pressed(inputAction);
    }

    public override void OnShot()
    {
        base.OnShot();
        AttackPressed = false;
    }

    public override void OnFail()
    {
        base.OnFail();
        AttackPressed = false;
    }
}
