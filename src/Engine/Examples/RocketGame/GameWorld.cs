using System;
using System.Collections.Generic;
using System.Globalization;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.RocketGame
{
    public class GameWorld
    {
        private readonly List<GameEntity> _furniture = new List<GameEntity>();
        private readonly List<Goal> _goals = new List<Goal>();

        private readonly Player _player;
        private readonly GameEntity _room;

        private readonly GUI _gui;

        private readonly RenderContext _rc;

        private readonly RenderStateSet _defaultRenderStateSet = new RenderStateSet{AlphaBlendEnable = false,ZEnable = true};

        internal enum GameState
        {
            StartScreen,
            Running,
            GameOver
        }

        internal int CurrentGameState;

        public GameWorld(RenderContext rc)
        {
            _rc = rc;

            CurrentGameState = (int) GameState.StartScreen;

            _player = new Player("Assets/rocket2.obj.model", rc);
            _player.SetScale(0.5f);
            _player.SetShader("Assets/rocket2.jpg");
            _player.SetCorrectionMatrix(float4x4.CreateRotationX((float) -Math.PI/2) * float4x4.CreateTranslation(-30,0,0));

            _room = new GameEntity("Assets/spacebox.obj.model", rc);
            //_room.SetShader(new float4(1,0,0,1));
            _room.SetShader(new float4(1, 0, 0, 1), "Assets/toon_generic_5_tex.png", new float4(0, 0, 0, 1), new float2(5, 5));
            _room.SetScale(8);

            _gui = new GUI(rc, this);

            _goals.Add(new Goal("Assets/cube.obj.model", rc, 0, 0, -800, 0.5f, 0.5f, 0.5f));
            _goals[0].SetScale(0.5f);

            _furniture.Add(new GameEntity("Assets/rocket2.obj.model", rc, 0, 250));
            _furniture[0].SetShader("Assets/rocket2.jpg", "Assets/toon_generic_5_tex.png", new float4(0, 0, 0, 1), new float2(5, 5));
        }

        public void Render()
        {
            foreach (var goal in _goals)
            {
                var distanceVector = _player.GetPositionVector() - goal.GetPositionVector();
                if (distanceVector.Length < 500)
                {
                    goal.SetActive();
                }
                else
                {
                    goal.SetInactive();
                }
                
                _gui.SetDebugMsg(distanceVector.Length.ToString(CultureInfo.InvariantCulture));
            }

            _rc.SetRenderState(_defaultRenderStateSet);

            _player.Move();

            var camMatrix = _player.GetCamMatrix();
            
            _room.Render(camMatrix);


            foreach (var gameEntity in _furniture)
            {
                gameEntity.Render(camMatrix);
            }

            foreach (var gameEntity in _goals)
            {
                gameEntity.Render(camMatrix);
            }

            _player.Render(camMatrix);
            _gui.Render();
        }

        public void StartGame()
        {
            _gui.SetDebugMsg("Game Started");
            CurrentGameState = (int) GameState.Running;
        }
    }
}
