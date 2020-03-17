namespace Fusee.Engine.Core.Effects
{
    public enum ChangedEnum
    {
        DISPOSE = 0,
        UNIFORM_VAR_UPDATED = 1,

        //Not needed at the moment, because a ShaderEffect must declare all it's parameter declarations at creation.
        //UNIFORM_VAR_ADDED = 2,

        UNCHANGED = 3
    }
}
