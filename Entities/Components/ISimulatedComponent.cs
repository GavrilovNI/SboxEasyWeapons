using Sandbox;

namespace EasyWeapons.Entities.Components;

public interface ISimulatedComponent : IComponent
{
    void Simulate(IClient client);
}
