using Sandbox;

namespace EasyWeapons.Networking;

public enum NetworkSide
{
    Server,
    Client,
    Both
}

public static class NetworkSideMethods
{
    public static bool FitsToCurrent(this NetworkSide networkSide)
    {
        if(Game.IsServer)
            return networkSide == NetworkSide.Server || networkSide == NetworkSide.Both;

        if(Game.IsClient)
            return networkSide == NetworkSide.Client || networkSide == NetworkSide.Both;

        return false;
    }
}
