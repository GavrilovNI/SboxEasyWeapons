using Sandbox;

namespace EasyWeapons.Recoiles.Modules;

public partial class RandomRecoil : Recoil
{
    [Net, Local]
    public RangedFloat XRecoil { get; set; } = 0;

    [Net, Local]
    public RangedFloat YRecoil { get; set; } = 0;

    public override void ApplyRecoil(IRecoilApplier recoilApplier)
    {
        Game.SetRandomSeed(Time.Tick);
        var angles = new Angles(-YRecoil.GetValue(), XRecoil.GetValue(), 0);
        recoilApplier.ApplyRecoil(angles);
    }
}
