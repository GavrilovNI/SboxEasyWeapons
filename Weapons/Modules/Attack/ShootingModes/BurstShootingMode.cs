using Sandbox;

namespace EasyWeapons.Weapons.Modules.Attack.ShootingModes;

public partial class BurstShootingMode : ShootingMode
{
    [Net]
    public int ShotsCount { get; set; } = 3;

    [Net, Predicted, Local]
    public int MadeShots { get; private set; } = 0;

    [Net, Predicted, Local]
    public bool AttackPressed { get; set; }

    [Net, Predicted, Local]
    public bool NeedToBePressed { get; set; } = true;

    public override bool ShouldAttack()
    {
        return AttackPressed || (NeedToBePressed == false && MadeShots < ShotsCount);
    }

    public override void OnSimulateBegin(SimulationType simulationType, string inputAction)
    {
        base.OnSimulateBegin(simulationType, inputAction);
        AttackPressed = simulationType == SimulationType.Simulating && Input.Pressed(inputAction);
    }

    public override void OnShot()
    {
        base.OnShot();
        MadeShots++;
        AttackPressed = false;

        if(MadeShots >= ShotsCount)
        {
            MadeShots = 0;
            NeedToBePressed = true;
        }
        else
        {
            NeedToBePressed = false;
        }
    }

    public override void OnFail()
    {
        base.OnFail();
        MadeShots = 0;
        AttackPressed = false;
        NeedToBePressed = true;
    }
}