using Fusee.Math;
using ProtoBuf;

namespace Examples.CubeAndTiles
{
    public enum Directions
    {
        None,
        Left,
        Right,
        Forward,
        Backward
    };

    public enum LevelStates
    {
        LsLoadFields,
        LsLoadCube,
        LsPlaying,
        LsWinning,
        LsDying
    };

    public enum CubeStates
    {
        CsLoading,
        CsAlive,
        CsWinning,
        CsWon,
        CsDying,
        CsDead
    };

    public enum FieldStates
    {
        FsLoading,
        FsAlive,
        FsDead
    };

    [ProtoContract]
    internal struct GameState
    {
        internal float4x4 GsCubeModelView;
        internal Field[,] GsLevelField;

        internal Directions Direction;
        internal LevelStates LvlState;
        internal CubeStates CubeState;
    };
}
