using EasyWeapons.Weapons;
using Sandbox;

namespace EasyWeapons.Effects.Animations;

public partial class OwnerAnimationEffect : AnimationEffect
{
    public override AnimatedEntity? GetAnimatedEntity(Weapon weapon)
    {
        return weapon.Owner as AnimatedEntity;
    }
}
