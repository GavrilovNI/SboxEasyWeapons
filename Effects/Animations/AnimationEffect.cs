using EasyWeapons.Effects.Animations.Parameters;
using EasyWeapons.Weapons;
using Sandbox;
using System.Threading;

namespace EasyWeapons.Effects.Animations;

public partial class AnimationEffect : WeaponEffect
{
    [Net, Predicted, Local]
    public AnimationParameter AnimationParameter { get; set; } = null!;


    public virtual AnimatedEntity? GetAnimatedEntity(Weapon weapon)
    {
        return weapon;
    }

    public override void Play(Weapon weapon, CancellationToken? cancellationToken = null)
    {
        var animatedEntity = GetAnimatedEntity(weapon);

        if(animatedEntity.IsValid() == false)
            return;

        AnimationParameter.Apply(animatedEntity);
    }
}
