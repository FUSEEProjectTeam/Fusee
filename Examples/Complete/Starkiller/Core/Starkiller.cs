using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;
using Fusee.Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fusee.Engine.Core.Scene;
using Fusee.Xirkit;

namespace Fusee.Examples.Starkiller.Core
{
    [FuseeApplication(Name = "Starkiller", Description = "Yet another FUSEE App.")]
    public class Starkiller : RenderCanvas
    {
        private SceneContainer _starkillerScene;
        private SceneRendererForward _sceneRenderer;

        private int count = 0;

        private Transform _Projektil1;
        private Transform _Projektil2;
        private Transform _Projektil3;
        private Transform _Projektil4;
        private Transform _Projektil5;
        private Transform _Projektil6;
        private Transform _Projektil7;
        private Transform _Projektil8;
        private Transform _Projektil9;
        private Transform _Projektil10;
        private Transform _Projektil11;
        private Transform _Projektil12;
        private Transform _Projektil13;
        private Transform _Projektil14;
        private Transform _Projektil15;
                
        private Transform _meteorit1;
        private Transform _meteorit2;
        private Transform _meteorit3;
        private Transform _meteorit4;
        private Transform _meteorit5;
        private Transform _meteorit6;
        private Transform _meteorit7;
        private Transform _meteorit8;
        private Transform _meteorit9;
        private Transform _meteorit10;
        private Transform _meteorit11;
        private Transform _meteorit12;
        private Transform _meteorit13;
        private Transform _meteorit14;
        private Transform _meteorit15;
        private Transform _meteorit16;

        private Transform _starkillerTransform = new Transform();
        private SceneContainer _scene;
        private Transform _Schiff;

        Boolean Ap1abgefeuert = false;
        Boolean Ap2abgefeuert = false;
        Boolean Ap3abgefeuert = false;
        Boolean Ap4abgefeuert = false;
        Boolean Ap5abgefeuert = false;
        Boolean Ap6abgefeuert = false;
        Boolean Ap7abgefeuert = false;
        Boolean Ap8abgefeuert = false;
        Boolean Ap9abgefeuert = false;
        Boolean Ap10abgefeuert = false;
        Boolean Ap11abgefeuert = false;
        Boolean Ap12abgefeuert = false;
        Boolean Ap13abgefeuert = false;
        Boolean Ap14abgefeuert = false;
        Boolean Ap15abgefeuert = false;

        Boolean gamestart = true;
       


        private SceneContainer CreateScene()
        {
            //load the Nodes 
            SceneContainer _starkillerScene = AssetStorage.Get<SceneContainer>("StarkillerAssets.fus");
            SceneNode _schiff = _starkillerScene.Children.FindNodes(n => n.Name == "Schiff").First();
            SceneNode _projektil1 = _starkillerScene.Children.FindNodes(n => n.Name == "AP1").First();
            SceneNode _projektil2 = _starkillerScene.Children.FindNodes(n => n.Name == "AP2").First();
            SceneNode _projektil3 = _starkillerScene.Children.FindNodes(n => n.Name == "AP3").First();
            SceneNode _projektil4 = _starkillerScene.Children.FindNodes(n => n.Name == "AP4").First();
            SceneNode _projektil5 = _starkillerScene.Children.FindNodes(n => n.Name == "AP5").First();
            SceneNode _projektil6 = _starkillerScene.Children.FindNodes(n => n.Name == "AP6").First();
            SceneNode _projektil7 = _starkillerScene.Children.FindNodes(n => n.Name == "AP7").First();
            SceneNode _projektil8 = _starkillerScene.Children.FindNodes(n => n.Name == "AP8").First();
            SceneNode _projektil9 = _starkillerScene.Children.FindNodes(n => n.Name == "AP9").First();
            SceneNode _projektil10 = _starkillerScene.Children.FindNodes(n => n.Name == "AP10").First();
            SceneNode _projektil11 = _starkillerScene.Children.FindNodes(n => n.Name == "AP11").First();
            SceneNode _projektil12 = _starkillerScene.Children.FindNodes(n => n.Name == "AP12").First();
            SceneNode _projektil13 = _starkillerScene.Children.FindNodes(n => n.Name == "AP13").First();
            SceneNode _projektil14 = _starkillerScene.Children.FindNodes(n => n.Name == "AP14").First();
            SceneNode _projektil15 = _starkillerScene.Children.FindNodes(n => n.Name == "AP15").First();
            SceneNode _meteorit1 = _starkillerScene.Children.FindNodes(n => n.Name == "Meteorit1").First();
            SceneNode _meteorit2 = _starkillerScene.Children.FindNodes(n => n.Name == "Meteorit2").First();
            SceneNode _meteorit3 = _starkillerScene.Children.FindNodes(n => n.Name == "Meteorit3").First();
            SceneNode _meteorit4 = _starkillerScene.Children.FindNodes(n => n.Name == "Meteorit4").First();
            SceneNode _meteorit5 = _starkillerScene.Children.FindNodes(n => n.Name == "Meteorit5").First();
            SceneNode _meteorit6 = _starkillerScene.Children.FindNodes(n => n.Name == "Meteorit6").First();
            SceneNode _meteorit7 = _starkillerScene.Children.FindNodes(n => n.Name == "Meteorit7").First();
            SceneNode _meteorit8 = _starkillerScene.Children.FindNodes(n => n.Name == "Meteorit8").First();
            SceneNode _meteorit9 = _starkillerScene.Children.FindNodes(n => n.Name == "Meteorit9").First();
            SceneNode _meteorit10 = _starkillerScene.Children.FindNodes(n => n.Name == "Meteorit10").First();
            SceneNode _meteorit11 = _starkillerScene.Children.FindNodes(n => n.Name == "Meteorit11").First();
            SceneNode _meteorit12 = _starkillerScene.Children.FindNodes(n => n.Name == "Meteorit12").First();
            SceneNode _meteorit13 = _starkillerScene.Children.FindNodes(n => n.Name == "Meteorit13").First();
            SceneNode _meteorit14 = _starkillerScene.Children.FindNodes(n => n.Name == "Meteorit14").First();
            SceneNode _meteorit15 = _starkillerScene.Children.FindNodes(n => n.Name == "Meteorit15").First();
            SceneNode _meteorit16 = _starkillerScene.Children.FindNodes(n => n.Name == "Meteorit16").First();
            

            SceneNode starkiller = new SceneNode
            {
                Components = new List<SceneComponent>
                {
                    // TRANSFROM COMPONENT
                    _starkillerTransform,
                    // SHADER EFFECT COMPONENT
                    _schiff.GetComponent<ShaderEffect>(),
                },
                Children = new ChildList()
            };

            starkiller.Children.Add(_schiff);
            starkiller.Children.Add(_meteorit1);
            starkiller.Children.Add(_meteorit2);
            starkiller.Children.Add(_meteorit3);
            starkiller.Children.Add(_meteorit4);
            starkiller.Children.Add(_meteorit5);
            starkiller.Children.Add(_meteorit6);
            starkiller.Children.Add(_meteorit7);
            starkiller.Children.Add(_meteorit8);
            starkiller.Children.Add(_meteorit9);
            starkiller.Children.Add(_meteorit10);
            starkiller.Children.Add(_meteorit11);
            starkiller.Children.Add(_meteorit12);
            starkiller.Children.Add(_meteorit13);
            starkiller.Children.Add(_meteorit14);
            starkiller.Children.Add(_meteorit15);
            starkiller.Children.Add(_meteorit16);
            starkiller.Children.Add(_projektil1);
            starkiller.Children.Add(_projektil2);
            starkiller.Children.Add(_projektil3);
            starkiller.Children.Add(_projektil4);
            starkiller.Children.Add(_projektil5);
            starkiller.Children.Add(_projektil6);
            starkiller.Children.Add(_projektil7);
            starkiller.Children.Add(_projektil8);
            starkiller.Children.Add(_projektil9);
            starkiller.Children.Add(_projektil10);
            starkiller.Children.Add(_projektil11);
            starkiller.Children.Add(_projektil12);
            starkiller.Children.Add(_projektil13);
            starkiller.Children.Add(_projektil14);
            starkiller.Children.Add(_projektil15);
            

            return new SceneContainer
            {
                Children = new List<SceneNode>
                {
                    starkiller
                }
            };


        }
       
        public override async Task<IEnumerable<SceneNode>> Init()
        {
            //Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(0, 0, 0, 0);

          





            //Wrap a SceneRenderer around the model.
            _scene = CreateScene();
            _sceneRenderer = new SceneRendererForward(_scene);
            _Schiff = _scene.Children.FindNodes(Node => Node.Name == "Schiff")?.FirstOrDefault()?.GetTransform();
            _Projektil1 = _scene.Children.FindNodes(Node => Node.Name == "AP1")?.FirstOrDefault()?.GetTransform();
            _Projektil2 = _scene.Children.FindNodes(Node => Node.Name == "AP2")?.FirstOrDefault()?.GetTransform();
            _Projektil3 = _scene.Children.FindNodes(Node => Node.Name == "AP3")?.FirstOrDefault()?.GetTransform();
            _Projektil4 = _scene.Children.FindNodes(Node => Node.Name == "AP4")?.FirstOrDefault()?.GetTransform();
            _Projektil5 = _scene.Children.FindNodes(Node => Node.Name == "AP5")?.FirstOrDefault()?.GetTransform();
            _Projektil6 = _scene.Children.FindNodes(Node => Node.Name == "AP6")?.FirstOrDefault()?.GetTransform();
            _Projektil7 = _scene.Children.FindNodes(Node => Node.Name == "AP7")?.FirstOrDefault()?.GetTransform();
            _Projektil8 = _scene.Children.FindNodes(Node => Node.Name == "AP8")?.FirstOrDefault()?.GetTransform();
            _Projektil9 = _scene.Children.FindNodes(Node => Node.Name == "AP9")?.FirstOrDefault()?.GetTransform();
            _Projektil10 = _scene.Children.FindNodes(Node => Node.Name == "AP10")?.FirstOrDefault()?.GetTransform();
            _Projektil11 = _scene.Children.FindNodes(Node => Node.Name == "AP11")?.FirstOrDefault()?.GetTransform();
            _Projektil12 = _scene.Children.FindNodes(Node => Node.Name == "AP12")?.FirstOrDefault()?.GetTransform();
            _Projektil13 = _scene.Children.FindNodes(Node => Node.Name == "AP13")?.FirstOrDefault()?.GetTransform();
            _Projektil14 = _scene.Children.FindNodes(Node => Node.Name == "AP14")?.FirstOrDefault()?.GetTransform();
            _Projektil15 = _scene.Children.FindNodes(Node => Node.Name == "AP15")?.FirstOrDefault()?.GetTransform();
            _meteorit1 = _scene.Children.FindNodes(Node => Node.Name.Contains("Meteorit"))?.FirstOrDefault()?.GetTransform();

            IEnumerable<SceneNode>;

            foreach(SceneNode meteorit in _scene.Children.FindNodes(Node => Node.Name.Contains("Meteorit"))){

            } 

            _meteorit2 = _scene.Children.FindNodes(Node => Node.Name == "Meteorit2")?.FirstOrDefault()?.GetTransform();
            _meteorit3 = _scene.Children.FindNodes(Node => Node.Name == "Meteorit3")?.FirstOrDefault()?.GetTransform();
            _meteorit4 = _scene.Children.FindNodes(Node => Node.Name == "Meteorit4")?.FirstOrDefault()?.GetTransform();
            _meteorit5 = _scene.Children.FindNodes(Node => Node.Name == "Meteorit5")?.FirstOrDefault()?.GetTransform();
            _meteorit6 = _scene.Children.FindNodes(Node => Node.Name == "Meteorit6")?.FirstOrDefault()?.GetTransform();
            _meteorit7 = _scene.Children.FindNodes(Node => Node.Name == "Meteorit7")?.FirstOrDefault()?.GetTransform();
            _meteorit8 = _scene.Children.FindNodes(Node => Node.Name == "Meteorit8")?.FirstOrDefault()?.GetTransform();
            _meteorit9 = _scene.Children.FindNodes(Node => Node.Name == "Meteorit9")?.FirstOrDefault()?.GetTransform();
            _meteorit10 = _scene.Children.FindNodes(Node => Node.Name == "Meteorit10")?.FirstOrDefault()?.GetTransform();
            _meteorit11 = _scene.Children.FindNodes(Node => Node.Name == "Meteorit11")?.FirstOrDefault()?.GetTransform();
            _meteorit12 = _scene.Children.FindNodes(Node => Node.Name == "Meteorit12")?.FirstOrDefault()?.GetTransform();
            _meteorit13 = _scene.Children.FindNodes(Node => Node.Name == "Meteorit13")?.FirstOrDefault()?.GetTransform();
            _meteorit14 = _scene.Children.FindNodes(Node => Node.Name == "Meteorit14")?.FirstOrDefault()?.GetTransform();
            _meteorit15 = _scene.Children.FindNodes(Node => Node.Name == "Meteorit15")?.FirstOrDefault()?.GetTransform();
            _meteorit16 = _scene.Children.FindNodes(Node => Node.Name == "Meteorit16")?.FirstOrDefault()?.GetTransform();


            return true;
        }



        //RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            //Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            RC.Viewport(0, 0, Width, Height);


            RC.View = float4x4.CreateTranslation(0, -20, 50) * float4x4.CreateRotationX(-5 * M.Pi / 180);



            //Bewegung des Schiffs
            float bewegungHorizontal = _Schiff.Translation.x;
            bewegungHorizontal += 0.7f * Keyboard.ADAxis;
            float bewegungVertikal = _Schiff.Translation.y;
            bewegungVertikal += 0.7f * Keyboard.WSAxis;
            _Schiff.Translation = new float3(bewegungHorizontal, bewegungVertikal, 0);


            //Geschoss abfeuern
            
            if (Mouse.LeftButton && count ==0)
            {
                _Projektil1.Translation = new float3(_Schiff.Translation.x, _Schiff.Translation.y, _Schiff.Translation.z + 5);
                count++;
                Ap1abgefeuert = true;
            }
            if (Ap1abgefeuert)
            {
                _Projektil1.Translation = new float3(_Projektil1.Translation.x, _Projektil1.Translation.y, _Projektil1.Translation.z + DeltaTime * 200);
            }
            if(_Projektil1.Translation.z > 500)
            {
                _Projektil1.Translation = new float3(0, 0, -50);
               Ap1abgefeuert = false;
            }
            if (Mouse.LeftButton && count == 1)
            {
                _Projektil2.Translation = new float3(_Schiff.Translation.x, _Schiff.Translation.y, _Schiff.Translation.z + 5);
                count++;
                Ap2abgefeuert = true;
            }
            if (Ap2abgefeuert)
            {
                _Projektil2.Translation = new float3(_Projektil2.Translation.x, _Projektil2.Translation.y, _Projektil2.Translation.z + DeltaTime * 200);
            }
            if (_Projektil2.Translation.z > 500)
            {
                _Projektil2.Translation = new float3(0, 0, -50);
                Ap2abgefeuert = false;
            }
            if (Mouse.LeftButton && count == 2)
            {
                _Projektil3.Translation = new float3(_Schiff.Translation.x, _Schiff.Translation.y, _Schiff.Translation.z + 5);
                count++;
                Ap3abgefeuert = true;
            }
            if (Ap3abgefeuert)
            {
                _Projektil3.Translation = new float3(_Projektil3.Translation.x, _Projektil3.Translation.y, _Projektil3.Translation.z + DeltaTime * 200);
            }
            if (_Projektil3.Translation.z > 500)
            {
                _Projektil3.Translation = new float3(0, 0, -50);
                Ap3abgefeuert = false;
            }
            if (Mouse.LeftButton && count == 3)
            {
                _Projektil4.Translation = new float3(_Schiff.Translation.x, _Schiff.Translation.y, _Schiff.Translation.z + 5);
                count++;
                Ap4abgefeuert = true;
            }
            if (Ap4abgefeuert)
            {
                _Projektil4.Translation = new float3(_Projektil4.Translation.x, _Projektil4.Translation.y, _Projektil4.Translation.z + DeltaTime * 200);
            }
            if (_Projektil4.Translation.z > 500)
            {
                _Projektil4.Translation = new float3(0, 0, -50);
                Ap4abgefeuert = false;
            }
            if (Mouse.LeftButton && count == 4)
            {
                _Projektil5.Translation = new float3(_Schiff.Translation.x, _Schiff.Translation.y, _Schiff.Translation.z + 5);
                count++;
                Ap5abgefeuert = true;
            }
            if (Ap5abgefeuert)
            {
                _Projektil5.Translation = new float3(_Projektil5.Translation.x, _Projektil5.Translation.y, _Projektil5.Translation.z + DeltaTime * 200);
            }
            if (_Projektil5.Translation.z > 500)
            {
                _Projektil5.Translation = new float3(0, 0, -50);
                Ap5abgefeuert = false;
            }
            if (Mouse.LeftButton && count == 5)
            {
                _Projektil6.Translation = new float3(_Schiff.Translation.x, _Schiff.Translation.y, _Schiff.Translation.z + 5);
                count++;
                Ap6abgefeuert = true;
            }
            if (Ap6abgefeuert)
            {
                _Projektil6.Translation = new float3(_Projektil6.Translation.x, _Projektil6.Translation.y, _Projektil6.Translation.z + DeltaTime * 200);
            }
            if (_Projektil6.Translation.z > 500)
            {
                _Projektil6.Translation = new float3(0, 0, -50);
                Ap6abgefeuert = false;
            }
            if (Mouse.LeftButton && count == 6)
            {
                _Projektil7.Translation = new float3(_Schiff.Translation.x, _Schiff.Translation.y, _Schiff.Translation.z + 5);
                count++;
                Ap7abgefeuert = true;
            }
            if (Ap7abgefeuert)
            {
                _Projektil7.Translation = new float3(_Projektil7.Translation.x, _Projektil7.Translation.y, _Projektil7.Translation.z + DeltaTime * 200);
            }
            if (_Projektil2.Translation.z > 500)
            {
                _Projektil7.Translation = new float3(0, 0, -50);
                Ap7abgefeuert = false;
            }
            if (Mouse.LeftButton && count == 7)
            {
                _Projektil8.Translation = new float3(_Schiff.Translation.x, _Schiff.Translation.y, _Schiff.Translation.z + 5);
                count++;
                Ap8abgefeuert = true;
            }
            if (Ap8abgefeuert)
            {
                _Projektil8.Translation = new float3(_Projektil8.Translation.x, _Projektil8.Translation.y, _Projektil8.Translation.z + DeltaTime * 200);
            }
            if (_Projektil8.Translation.z > 500)
            {
                _Projektil8.Translation = new float3(0, 0, -50);
                Ap8abgefeuert = false;
            }
            if (Mouse.LeftButton && count == 8)
            {
                _Projektil9.Translation = new float3(_Schiff.Translation.x, _Schiff.Translation.y, _Schiff.Translation.z + 5);
                count++;
                Ap9abgefeuert = true;
            }
            if (Ap9abgefeuert)
            {
                _Projektil9.Translation = new float3(_Projektil9.Translation.x, _Projektil9.Translation.y, _Projektil9.Translation.z + DeltaTime * 200);
            }
            if (_Projektil9.Translation.z > 500)
            {
                _Projektil9.Translation = new float3(0, 0, -50);
                Ap9abgefeuert = false;
            }
            if (Mouse.LeftButton && count == 9)
            {
                _Projektil10.Translation = new float3(_Schiff.Translation.x, _Schiff.Translation.y, _Schiff.Translation.z + 5);
                count++;
                Ap10abgefeuert = true;
            }
            if (Ap10abgefeuert)
            {
                _Projektil10.Translation = new float3(_Projektil10.Translation.x, _Projektil10.Translation.y, _Projektil10.Translation.z + DeltaTime * 200);
            }
            if (_Projektil10.Translation.z > 500)
            {
                _Projektil10.Translation = new float3(0, 0, -50);
                Ap10abgefeuert = false;
            }
            if (Mouse.LeftButton && count == 10)
            {
                _Projektil11.Translation = new float3(_Schiff.Translation.x, _Schiff.Translation.y, _Schiff.Translation.z + 5);
                count++;
                Ap11abgefeuert = true;
            }
            if (Ap11abgefeuert)
            {
                _Projektil11.Translation = new float3(_Projektil11.Translation.x, _Projektil11.Translation.y, _Projektil11.Translation.z + DeltaTime * 200);
            }
            if (_Projektil11.Translation.z > 500)
            {
                _Projektil11.Translation = new float3(0, 0, -50);
                Ap11abgefeuert = false;
            }
            if (Mouse.LeftButton && count == 11)
            {
                _Projektil12.Translation = new float3(_Schiff.Translation.x, _Schiff.Translation.y, _Schiff.Translation.z + 5);
                count++;
                Ap12abgefeuert = true;
            }
            if (Ap12abgefeuert)
            {
                _Projektil12.Translation = new float3(_Projektil12.Translation.x, _Projektil12.Translation.y, _Projektil12.Translation.z + DeltaTime * 200);
            }
            if (_Projektil12.Translation.z > 500)
            {
                _Projektil12.Translation = new float3(0, 0, -50);
                Ap12abgefeuert = false;
            }
            if (Mouse.LeftButton && count == 12)
            {
                _Projektil13.Translation = new float3(_Schiff.Translation.x, _Schiff.Translation.y, _Schiff.Translation.z + 5);
                count++;
                Ap13abgefeuert = true;
               
            }
            if (Ap13abgefeuert)
            {
                _Projektil13.Translation = new float3(_Projektil13.Translation.x, _Projektil13.Translation.y, _Projektil13.Translation.z + DeltaTime * 200);
            }
            if (_Projektil13.Translation.z > 500)
            {
                _Projektil13.Translation = new float3(0, 0, -50);
                Ap13abgefeuert = false;
            }
            if (Mouse.LeftButton && count == 13)
            {
                _Projektil14.Translation = new float3(_Schiff.Translation.x, _Schiff.Translation.y, _Schiff.Translation.z + 5);
                count++;
                Ap14abgefeuert = true;
               
            }
            if (Ap14abgefeuert)
            {
                _Projektil14.Translation = new float3(_Projektil14.Translation.x, _Projektil14.Translation.y, _Projektil14.Translation.z + DeltaTime * 200);
            }
            if (_Projektil14.Translation.z > 500)
            {
                _Projektil14.Translation = new float3(0, 0, -50);
                Ap14abgefeuert = false;
            }
            if (Mouse.LeftButton && count == 14)
            {
                _Projektil15.Translation = new float3(_Schiff.Translation.x, _Schiff.Translation.y, _Schiff.Translation.z + 5);
                count=0;
                Ap15abgefeuert = true;
                
            }
            if (Ap15abgefeuert)
            {
                _Projektil15.Translation = new float3(_Projektil15.Translation.x, _Projektil15.Translation.y, _Projektil11.Translation.z + DeltaTime * 200);
            }
            if (_Projektil15.Translation.z > 500)
            {
                _Projektil15.Translation = new float3(0, 0, -50);
                Ap15abgefeuert = false;
            }

            //Bewegung der Meteoriten
            if (gamestart)
            {
                _meteorit1.Translation.z -= 500 * DeltaTime;
                _meteorit1.Rotation.y += 1 * DeltaTime;
                _meteorit1.Rotation.z += 1 * DeltaTime;
            }
            if(_meteorit1.Translation.z <= -200)
            {
               _meteorit1.Translation.z = 2000;
            }
            if (gamestart)
            {
                _meteorit2.Translation.z -= 450 * DeltaTime;
                _meteorit2.Rotation.y += 1 * DeltaTime;
                _meteorit2.Rotation.z += 1 * DeltaTime;
            }
            if (_meteorit2.Translation.z <= -200)
            {
                _meteorit2.Translation.z = 2000;
            }
            if (gamestart)
            {
                _meteorit3.Translation.z -= 430 * DeltaTime;
                _meteorit3.Rotation.y += 1 * DeltaTime;
                _meteorit3.Rotation.z += 1 * DeltaTime;
            }
            if (_meteorit3.Translation.z <= -200)
            {
                _meteorit3.Translation.z = 2000;
            }
            if (gamestart)
            {
                _meteorit4.Translation.z -= 520 * DeltaTime;
                _meteorit4.Rotation.y += 1 * DeltaTime;
                _meteorit4.Rotation.z += 1 * DeltaTime;
            }
            if (_meteorit4.Translation.z <= -200)
            {
                _meteorit4.Translation.z = 2000;
            }
            if (gamestart)
            {
                _meteorit5.Translation.z -= 470 * DeltaTime;
                _meteorit5.Rotation.y += 1 * DeltaTime;
                _meteorit5.Rotation.z += 1 * DeltaTime;
            }
            if (_meteorit5.Translation.z <= -200)
            {
                _meteorit5.Translation.z = 2000;
            }
            if (gamestart)
            {
                _meteorit6.Translation.z -= 510 * DeltaTime;
                _meteorit6.Rotation.y += 1 * DeltaTime;
                _meteorit6.Rotation.z += 1 * DeltaTime;
            }
            if (_meteorit6.Translation.z <= -200)
            {
                _meteorit6.Translation.z = 2000;
            }
            if (gamestart)
            {
                _meteorit7.Translation.z -= 400 * DeltaTime;
                _meteorit7.Rotation.y += 1 * DeltaTime;
                _meteorit7.Rotation.z += 1 * DeltaTime;
            }
            if (_meteorit7.Translation.z <= -200)
            {
                _meteorit7.Translation.z = 2000;
            }

            if (gamestart)
            {
                _meteorit8.Translation.z -= 420 * DeltaTime;
                _meteorit8.Rotation.y += 1 * DeltaTime;
                _meteorit8.Rotation.z += 1 * DeltaTime;
            }
            if (_meteorit8.Translation.z <= -200)
            {
                _meteorit8.Translation.z = 2000;
            }

            if (gamestart)
            {
                _meteorit9.Translation.z -= 440 * DeltaTime;
                _meteorit9.Rotation.y += 1 * DeltaTime;
                _meteorit9.Rotation.z += 1 * DeltaTime;
            }
            if (_meteorit9.Translation.z <= -200)
            {
                _meteorit9.Translation.z = 2000;
            }

            if (gamestart)
            {
                _meteorit10.Translation.z -= 460 * DeltaTime;
                _meteorit10.Rotation.y += 1 * DeltaTime;
                _meteorit10.Rotation.z += 1 * DeltaTime;
            }
            if (_meteorit10.Translation.z <= -200)
            {
                _meteorit10.Translation.z = 2000;
            }

            if (gamestart)
            {
                _meteorit11.Translation.z -= 530 * DeltaTime;
                _meteorit11.Rotation.y += 1 * DeltaTime;
                _meteorit11.Rotation.z += 1 * DeltaTime;
            }
            if (_meteorit11.Translation.z <= -200)
            {
                _meteorit11.Translation.z = 2000;
            }

            if (gamestart)
            {
                _meteorit12.Translation.z -= 470 * DeltaTime;
                _meteorit12.Rotation.y += 1 * DeltaTime;
                _meteorit12.Rotation.z += 1 * DeltaTime;
            }
            if (_meteorit12.Translation.z <= -200)
            {
                _meteorit12.Translation.z = 2000;
            }

            if (gamestart)
            {
                _meteorit13.Translation.z -= 480 * DeltaTime;
                _meteorit13.Rotation.y += 1 * DeltaTime;
                _meteorit13.Rotation.z += 1 * DeltaTime;
            }
            if (_meteorit13.Translation.z <= -200)
            {
                _meteorit13.Translation.z = 2000;
            }

            if (gamestart)
            {
                _meteorit14.Translation.z -= 450 * DeltaTime;
                _meteorit14.Rotation.y += 1 * DeltaTime;
                _meteorit14.Rotation.z += 1 * DeltaTime;
            }
            if (_meteorit14.Translation.z <= -300)
            {
                _meteorit14.Translation.z = 2000;
            }

            if (gamestart)
            {
                
                _meteorit15.Translation.z -= 540 * DeltaTime;
                _meteorit15.Rotation.y += 1 * DeltaTime;
                _meteorit15.Rotation.z += 1 * DeltaTime;
            }
            if (_meteorit15.Translation.z <= -200)
            {
                _meteorit15.Translation.z = 2000;
            }

            if (gamestart)
            {
                _meteorit16.Translation.z -= 410 * DeltaTime;
                _meteorit16.Rotation.y += 1 * DeltaTime;
                _meteorit16.Rotation.z += 1 * DeltaTime;
            }
            if (_meteorit16.Translation.z <= -200)
            {
                _meteorit16.Translation.z = 2000;
            }




            //Tick any animations and Render the scene loaded in Init()
            _sceneRenderer.Render(RC);

            //Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }
        public void SetProjectionAndViewport()

        {

            //Set the rendering area to the entire window size

            RC.Viewport(0, 0, Width, Height);



            //Create a new projection matrix generating undistorted images on the new aspect ratio.

            var aspectRatio = Width / (float)Height;



            //0.25*PI Rad -> 45° Opening angle along the vertical direction. Horizontal opening angle is calculated based on the aspect ratio

            //Front clipping happens at 1 (Objects nearer than 1 world unit get clipped)

            //Back clipping happens at 2000 (Anything further away from the camera than 2000 world units gets clipped, polygons will be cut)

            var projection = float4x4.CreatePerspectiveFieldOfView(M.PiOver4, aspectRatio, 1, 20000);

            RC.Projection = projection;

        }


    }
}
