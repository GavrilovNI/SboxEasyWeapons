using EasyWeapons.Weapons;
using Sandbox;

namespace EasyWeapons.Effects.Animations;

public partial class ViewModelAnimationEffect : AnimationEffect
{
    public override AnimatedEntity? GetAnimatedEntity(Weapon weapon)
    {
        return weapon.ViewModelEntity as AnimatedEntity;
    }
}
