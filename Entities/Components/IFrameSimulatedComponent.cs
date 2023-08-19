using Sandbox;

namespace EasyWeapons.Entities.Components;

public interface IFrameSimulatedComponent : IComponent
{
    void FrameSimulate(IClient client);
}
