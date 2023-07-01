using Sandbox;

namespace EasyWeapons.Weapons.Modules.Attack.ShootingModes;

public class AutoShootingMode : ShootingMode
{
    public override bool ShouldAttack()
    {
        return false;
    }

    public override bool ShouldAttack(string inputAction)
    {
        bool shouldAttack = Input.Pressed(inputAction) || (IsShooting && Input.Down(inputAction));
        if(shouldAttack == false)
            IsShooting = false;
        return shouldAttack;
    }
}