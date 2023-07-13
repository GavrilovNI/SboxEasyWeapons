using EasyWeapons.Weapons;
using Sandbox;
using System.Threading;

namespace EasyWeapons.Effects;

public partial class ParticleEffect : WeaponEffect
{
    [Net, Predicted, Local]
    public string Name { get; set; } = null!;

    [Net, Predicted, Local]
    public string Attachment { get; set; } = null!;

    [Net, Predicted, Local]
    public bool ShouldFollow { get; set; } = true;


    public override void Play(Weapon weapon, CancellationToken? cancellationToken = null)
    {
        if(Game.IsServer)
            CreateParticle(Name, weapon, Attachment, ShouldFollow);
    }

    [ClientRpc]
    public static void CreateParticle(string name, Weapon weapon, string attachment, bool follow)
    {
        Game.AssertClient();
        Particles.Create(name, weapon.EffectEntity, attachment, follow);
    }
}
