using Sandbox;

namespace EasyWeapons.Entities.Components;

public interface ISimulatedComponent : IComponent
{
    public void Simulate(IClient client);
}
