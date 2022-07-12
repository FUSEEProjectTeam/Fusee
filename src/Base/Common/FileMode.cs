namespace Fusee.Base.Common
{
    /// <summary>
    /// Keep this binary compatible with System.IO.FileMode (unsupported on portable libraries).
    /// </summary>
    public enum FileMode
    {
#pragma warning disable 1591
        CreateNew = 1,
        Create = 2,
        Open = 3,
        OpenOrCreate = 4,
        Truncate = 5,
        Append = 6,
#pragma warning restore 1591
    }
}