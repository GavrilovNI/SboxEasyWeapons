using Sandbox;

namespace EasyWeapons.Weapons.Modules.Attack.ShootingModes;

public class SemiShootingMode : ShootingMode
{
    public override bool ShouldAttack()
    {
        return false;
    }

    public override bool ShouldAttack(InputButton inputButton)
    {
        return Input.Pressed(inputButton);
    }

    public override void OnShot()
    {
        base.OnShot();
        IsShooting = false;
    }
}
