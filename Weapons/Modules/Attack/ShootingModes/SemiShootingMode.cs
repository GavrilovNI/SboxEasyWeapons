using Sandbox;

namespace EasyWeapons.Weapons.Modules.Attack.ShootingModes;

public class SemiShootingMode : ShootingMode
{
    public override bool ShouldAttack()
    {
        return false;
    }

    public override bool ShouldAttack(string inputAction)
    {
        return Input.Pressed(inputAction);
    }

    public override void OnShot()
    {
        base.OnShot();
        IsShooting = false;
    }
}
