using Sandbox;

namespace EasyWeapons.Entities.Components;

public interface ICancelableComponent : IComponent
{
    public void Cancel();
}
