using EasyWeapons.Networking;
using EasyWeapons.Sounds;
using EasyWeapons.Weapons;
using Sandbox;
using System.Threading;

namespace EasyWeapons.Effects;

public partial class SoundEffect : WeaponEffect
{
    [Net, Predicted, Local]
    public NetworkSide Side { get; set; } = NetworkSide.Both;

    [Net, Predicted, Local]
    public PlayableDelayedSound Sound { get; set; } = null!;

    [Net, Predicted, Local]
    public float DelayRatio { get; set; } = 1f;

    [Net, Predicted, Local]
    public string? Attachment { get; set; } = null;

    public override void Play(Weapon weapon, CancellationToken? cancellationToken = null)
    {
        if(Side.FitsToCurrent())
            _ = Sound.PlayOnEntity(weapon, cancellationToken, DelayRatio, Attachment);
    }
}
