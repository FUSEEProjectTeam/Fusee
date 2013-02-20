using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace Examples.XNASoundTest
{
    public class XNASoundTest : Game
    {
        GraphicsDeviceManager graphics;

        protected override void LoadContent()
        {
            var mysong = Content.Load<Song>("Assets/tetris");
            MediaPlayer.Play(mysong);
            graphics = new GraphicsDeviceManager(this);
        }

        private static void Main(string[] args)
        {
            using (var game = new XNASoundTest())
            {
                game.Run();
            }
        }
    }
}