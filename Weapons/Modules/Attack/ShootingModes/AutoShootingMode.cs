using Sandbox;

namespace EasyWeapons.Weapons.Modules.Attack.ShootingModes;

public partial class AutoShootingMode : ShootingMode
{
    [Net, Predicted, Local]
    public bool ShouldShoot { get; set; }

    public override bool ShouldAttack()
    {
        return ShouldShoot;
    }

    public override void OnSimulateBegin(SimulationType simulationType, string inputAction)
    {
        if(simulationType != SimulationType.Simulating)
        {
            ShouldShoot = false;
            return;
        }

        if(ShouldShoot)
        {
            if(Input.Down(inputAction) == false)
                ShouldShoot = false;
        }
        else
        {
            if(Input.Pressed(inputAction))
                ShouldShoot = true;
        }
    }

    public override void OnShot()
    {
        base.OnShot();
    }

    public override void OnFail()
    {
        base.OnFail();
        ShouldShoot = false;
    }

}