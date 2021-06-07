using System;

public class Class1
{
    //private GUIButton _btnStart;
    //private float2 _btnStartPosition;

    //private GUIButton _btnRetry;
    //private float2 _btnRetryPosition;
    public Class1()
    {
        //UI für Start mit Button
        //private SceneContainer CreateUIStart() 
        //{
        //    var vsTex = AssetStorage.Get<string>("texture.vert");
        //    var psTex = AssetStorage.Get<string>("texture.frag");
        //    var vsNineSlice = AssetStorage.Get<string>("nineSlice.vert");
        //    var psNineSlice = AssetStorage.Get<string>("nineSliceTile.frag");

        //    var canvasWidth = Width / 100f;
        //    var canvasHeight = Height / 100f;

        //    _btnStart = new GUIButton
        //    {
        //        Name = "Start_Button"
        //    };
        //    _btnStart.OnMouseDown += StartGame;
        //    _btnStartPosition = new float2(canvasWidth / 2 - 1f, canvasHeight / 3);

        //    var startNode = new TextureNode(
        //    "StartButtonLogo",
        //    vsTex,
        //    psTex,
        //    new Texture(AssetStorage.Get<ImageData>("StartButton.png")),
        //    UIElementPosition.GetAnchors(AnchorPos.DownDownRight),
        //    UIElementPosition.CalcOffsets(AnchorPos.DownDownRight, _btnStartPosition, canvasHeight, canvasWidth, new float2(1.6f, 0.6f)),
        //    float2.One
        //    );
        //    //var startNode = new TextureNode(
        //    //    "StartButtonLogo",
        //    //    vsNineSlice,
        //    //    psNineSlice,
        //    //    new Texture(AssetStorage.Get<ImageData>("StartButton.png")),
        //    //    UIElementPosition.GetAnchors(AnchorPos.DownDownRight),
        //    //    UIElementPosition.CalcOffsets(AnchorPos.DownDownRight, _btnStartPosition, canvasHeight, canvasWidth, new float2(1.6f, 0.6f)),
        //    //    float2.One,
        //    //    new float4(1, 1, 1, 1),
        //    //    0, 0, 0, 0
        //    //    );
        //    startNode.Components.Add(_btnStart);


        //    var canvas = new CanvasNode(
        //        "Canvas",
        //        _canvasRenderMode,
        //        new MinMaxRect
        //        {
        //            Min = new float2(-canvasWidth / 2, -canvasHeight / 2f),
        //            Max = new float2(canvasWidth / 2, canvasHeight / 2f)
        //        })
        //    {
        //        Children = new ChildList()
        //        {
        //            //Simple Texture Node, contains the fusee logo. Lüge
        //            startNode

        //        }
        //    };


        //    return new SceneContainer
        //    {
        //        Children = new List<SceneNode>
        //        {
        //            //Add canvas.
        //            canvas
        //        }
        //    };

        //}


        //flüssige Steuerung

        //if (Keyboard.LeftRightAxis != 0)
        //{
        //    if (Keyboard.LeftRightAxis > 0) //rechts
        //    {
        //        _starshipTrans.Translation.x += 10 * DeltaTime * Keyboard.LeftRightAxis;

        //        //leichter tilt nach links
        //        _starshipTrans.Rotation.x = Keyboard.LeftRightAxis * 0.167f;


        //        //Bewegungsgrenze rechts
        //        if (_starshipTrans.Translation.x >= 3.8f)
        //        {
        //            _starshipTrans.Translation.x -= 10 * DeltaTime * Keyboard.LeftRightAxis;
        //        }

        //    }
        //    else if (Keyboard.LeftRightAxis < 0)  //links
        //    {
        //        _starshipTrans.Translation.x += 10 * DeltaTime * Keyboard.LeftRightAxis;

        //        //leichter tilt nach rechts
        //        _starshipTrans.Rotation.x = Keyboard.LeftRightAxis * 0.167f;


        //        //Bewegungsgrenze links
        //        if (_starshipTrans.Translation.x <= -3.8f)
        //        {
        //            _starshipTrans.Translation.x -= 10 * DeltaTime * Keyboard.LeftRightAxis;
        //        }
        //    }
        //    //Console.WriteLine(_starshipTrans.Translation.x);
        //}

        ////Steuerung oben/ unten

        //if (Keyboard.UpDownAxis != 0)
        //{
        //    if (Keyboard.UpDownAxis > 0) //oben
        //    {
        //        _starshipTrans.Translation.y += 7 * DeltaTime * Keyboard.UpDownAxis;

        //        //leichter tilt nach oben
        //        _starshipTrans.Rotation.z = -Keyboard.UpDownAxis * 0.083f;


        //        //Bewegungsgrenze oben
        //        if (_starshipTrans.Translation.y >= 4.5f)
        //        {
        //            _starshipTrans.Translation.y -= 7 * DeltaTime * Keyboard.UpDownAxis;
        //        }
        //    }
        //    else if (Keyboard.UpDownAxis < 0) //unten
        //    {
        //        _starshipTrans.Translation.y += 7 * DeltaTime * Keyboard.UpDownAxis;

        //        //leichter tilt nach unten
        //        _starshipTrans.Rotation.z = -Keyboard.UpDownAxis * 0.083f;


        //        //Bewegungsgrenze unten
        //        if (_starshipTrans.Translation.y <= -0.3f)
        //        {
        //            _starshipTrans.Translation.y -= 7 * DeltaTime * Keyboard.UpDownAxis;
        //        }
        //    }
        //}
    }
}