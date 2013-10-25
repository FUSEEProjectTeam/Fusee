
namespace Fusee.Engine
{
    class Font : IFont
    {
        internal int Width;
        internal int Height;

        internal ITexture TexAtlas;
        
        internal struct CharInfoStruct
        {
            internal float AdvanceX;
            internal float AdvanceY;

            internal float BitmapW;
            internal float BitmapH;
            internal float BitmapL;
            internal float BitmapT;

            internal float TexOffX;
            internal float TexOffY;
        }

        internal CharInfoStruct[] CharInfo;
    }
}
