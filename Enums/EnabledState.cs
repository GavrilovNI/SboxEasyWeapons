

namespace EasyWeapons.Enums;

public enum EnabledState
{
    Disabled,
    Enabling,
    Enabled,
    Disabling
}

public static class EnabledStateMethods
{
    public static bool IsChanging(this EnabledState state)
    {
        return state == EnabledState.Enabling || state == EnabledState.Disabling;
    }

    public static EnabledState ToFinishedState(this EnabledState state)
    {
        return (state == EnabledState.Enabling || state == EnabledState.Enabled) ? EnabledState.Enabled : EnabledState.Disabled;
    }
}
