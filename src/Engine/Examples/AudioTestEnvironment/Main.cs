using System.Diagnostics;
using System.Windows.Forms;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.AudioTestEnvironment
{
    public enum AudioFileFormat
    {
        mp3,
        wav,
        ogg
    };

    public enum LoadMethod
    {
        stream,
        load
    };

    [FuseeApplication(Name = "Audio Test Environment", Description = "This is an audio test environment to test for different bugs and options.")]
    public class AudioTest : RenderCanvas
    {
        private AudioFileFormat _fileFormat;
        private LoadMethod _streamOrLoad;

        private IAudioStream _sound1;
        private IAudioStream _sound2;
        private IAudioStream _sound3;
        private IAudioStream _sound4;

        private bool _longFilePlaying = false;

        #region Irrelevant for Audio
        // Vars.
        /*
        private int _screenWidth;
        private int _screenHeight;
        */

        private GUIHandler _guiHandler;
        private GUIImage _guiImage;
        #endregion

        public override void Init()
        {
            #region For some planned features.
            /*
            _screenWidth = Screen.PrimaryScreen.Bounds.Width;
            _screenHeight = Screen.PrimaryScreen.Bounds.Height;
            SetWindowSize(_screenWidth, _screenHeight / 9 * 2, true);
             */
            #endregion

            #region GUI
            // GUIHandler
            _guiHandler = new GUIHandler();
            _guiHandler.AttachToContext(RC);

            _guiImage = new GUIImage("Assets/img/AudioTestWallpaper.png", 0, 0, 0, 1280, 720);
            _guiHandler.Add(_guiImage);
            #endregion

            // Set this to control the file formats and loading methods.
            _fileFormat = AudioFileFormat.mp3;
            _streamOrLoad = LoadMethod.load;

            // Now load the correct files.
            LoadFiles();

            #region Audio settings
            _sound1.Volume = 3.0f;
            _sound2.Volume = 3.0f;
            _sound3.Volume = 3.0f;
            _sound4.Volume = 2.0f;

            _sound1.Loop = false;
            _sound2.Loop = false;
            _sound3.Loop = false;
            _sound4.Loop = false;
            #endregion
        }

        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            PullUserInput();

            _guiHandler.RenderGUI();

            Present();
        }

        public override void Resize()
        {
            RC.Viewport(0, 0, Width, Height);

            var aspectRatio = Width / (float)Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 10000);

            // refresh all elements
            _guiHandler.Refresh();
        }

        public static void Main()
        {
            var app = new AudioTest();
            app.Run();
        }


        #region Non Fusee Methods

        /// <summary>
        /// Pulls the users input.
        /// </summary>
        private void PullUserInput()
        {

            if (Input.Instance.IsKeyDown(KeyCodes.D1))
            {
                _sound1.Stop();
                _sound1.Play();
            }
            if (Input.Instance.IsKeyDown(KeyCodes.D2))
            {
                _sound2.Stop();
                _sound2.Play();
            }
            if (Input.Instance.IsKeyDown(KeyCodes.D3))
            {
                _sound3.Stop();
                _sound3.Play();
            }
            if (Input.Instance.IsKeyDown(KeyCodes.D4))
            {
                if (_longFilePlaying)
                {
                    _sound4.Stop();
                    _longFilePlaying = false;
                    return;
                }
                _sound4.Play();
                _longFilePlaying = true;
            }
        }

        /// <summary>
        /// This method loads the files with different settings.
        /// </summary>
        private void LoadFiles()
        {
            string fileType;
            switch (_fileFormat)
            {
                case AudioFileFormat.mp3:
                    fileType = "mp3";
                    break;
                case AudioFileFormat.wav:
                    fileType = "wav";
                    break;
                case AudioFileFormat.ogg:
                    fileType = "ogg";
                    break;
                default:
                    fileType = "mp3";
                    break;
            }
            Debug.WriteLine("Type: " + fileType);

            bool loadType;
            switch (_streamOrLoad)
            {
                case LoadMethod.load:
                    loadType = false;
                    break;
                case LoadMethod.stream:
                    loadType = true;
                    break;
                default:
                    loadType = false;
                    break;
            }
            Debug.WriteLine("Streaming: " + loadType);

            _sound1 = Audio.Instance.LoadFile("Assets/audio/Bounce1." + fileType, loadType);
            _sound2 = Audio.Instance.LoadFile("Assets/audio/Bounce2." + fileType, loadType);
            _sound3 = Audio.Instance.LoadFile("Assets/audio/Bounce3." + fileType, loadType);
            _sound4 = Audio.Instance.LoadFile("Assets/audio/LaserBallMusic." + fileType, loadType);
        }
        #endregion
    }
}