using System;
using System.Collections.Generic;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.RocketGame
{
    public class GameWorld
    {
        private readonly List<GameEntity> _furniture = new List<GameEntity>();
        private readonly List<GameEntity> _goals = new List<GameEntity>();

        private readonly Player _player;

        private readonly GUI _gui;

        private readonly RenderContext _rc;

        private readonly RenderStateSet _defaultRenderStateSet = new RenderStateSet{AlphaBlendEnable = false,ZEnable = true};

        internal enum GameStates
        {
            StartScreen,
            Running,
            GameOver
        }

        internal int GameState;

        public GameWorld(RenderContext rc)
        {
            _rc = rc;

            GameState = (int) GameStates.StartScreen;

            _player = new Player("Assets/rocket2.obj.model", rc);
            _player.SetShader("Assets/rocket2.jpg");
            _player.SetCorrectionMatrix(float4x4.CreateRotationX((float) -Math.PI/2));

            _gui = new GUI(rc, this);

            _furniture.Add(new GameEntity("Assets/cube.obj.model", rc, 250, 0, 0, 0.5f, 0.5f, 0.5f));
            _furniture[0].SetShader(new float4(0, 1, 0, 1));
            _furniture[0].SetScale(0.5f);
            _furniture.Add(new GameEntity("Assets/cube.obj.model", rc, 0, 250));
            _furniture[1].SetShader("Assets/tex_cube.jpg");
        }

        public void Render()
        {
            _rc.SetRenderState(_defaultRenderStateSet);

            _player.Move();

            var camMatrix = _player.GetCamMatrix();

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
            GameState = (int) GameStates.Running;
        }
    }
}
