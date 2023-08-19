using Sandbox;

namespace EasyWeapons.ViewModels;

public partial class WeaponViewModel : BaseViewModel
{
    [Net, Predicted, Local]
    public Vector3 PositionOffset { get; set; } = Vector3.Zero;

    [Net, Predicted, Local]
    public Rotation RotationOffset { get; set; } = Rotation.Identity;

    [Net, Predicted, Local]
    public float FieldOfView { get; set; } = 60;

    public sealed override void PlaceViewmodel()
    {

    }

    [GameEvent.Client.Frame]
    public virtual void PlaceViewmodelOnFrame()
    {
        if(Game.IsRunningInVR)
            return;

        Position = Camera.Position + PositionOffset * Camera.Rotation;
        Rotation = Camera.Rotation * RotationOffset;

        Camera.Main.SetViewModelCamera(Screen.CreateVerticalFieldOfView(FieldOfView), 1, 1000.0f);
    }
}
