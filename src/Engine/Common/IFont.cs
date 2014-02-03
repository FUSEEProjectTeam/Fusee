namespace Fusee.Engine
{
    // char info
    public struct CharInfoStruct
    {
        public float AdvanceX;
        public float AdvanceY;

        public float BitmapW;
        public float BitmapH;
        public float BitmapL;
        public float BitmapT;

        public float TexOffX;
        public float TexOffY;
    }

    public interface IFont
    {
        // texture atlas
        ITexture TexAtlas { get; }

        int Width { get; }
        int Height { get; }

        // font settings
        uint FontSize { get; }
        bool UseKerning { get; set; }

        // char info
        CharInfoStruct[] CharInfo { get; }
    }
}