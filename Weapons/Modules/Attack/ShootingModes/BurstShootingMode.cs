using Sandbox;

namespace EasyWeapons.Weapons.Modules.Attack.ShootingModes;

public partial class BurstShootingMode : ShootingMode
{
    [Net]
    public int ShotsCount { get; set; } = 3;

    [Net, Predicted]
    public int MadeShots { get; private set; } = 0;

    public override bool ShouldAttack()
    {
        return IsShooting;
    }

    public override bool ShouldAttack(string inputAction)
    {
        bool inputPressed = Input.Pressed(inputAction);
        if(inputPressed && MadeShots >= ShotsCount)
            MadeShots = 0;

        return inputPressed || IsShooting;
    }

    public override void OnShot()
    {
        base.OnShot();
        MadeShots++;
        IsShooting = MadeShots < ShotsCount;
    }

    public override void OnFail()
    {
        base.OnFail();
        MadeShots = 0;
    }
}