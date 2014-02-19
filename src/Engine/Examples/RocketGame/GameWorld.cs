using System;
using System.Collections.Generic;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.RocketGame
{
    public class GameWorld
    {
        private readonly RenderContext _rc;

        private readonly List<GameEntity> _furniture = new List<GameEntity>();
        private readonly List<Target> _targets = new List<Target>();

        private readonly Player _player;

        private readonly GameEntity _room;

        private readonly GUI _gui;

        private float4x4 _camMatrix;

        private int _currentGameState;
        private int _lastGameState;

        private int _curScore;
        private int _maxScore;
        private int _oldScore;

        public GameWorld(RenderContext rc)
        {

            _rc = rc;

            _currentGameState = (int)GameState.StartScreen;

            _camMatrix = float4x4.CreateTranslation(0, -400, 0);

            _gui = new GUI(rc, this);

            _player = new Player("Assets/rocket3.protobuf.model", rc);
            _player.SetShader("Assets/rocket3.png", "Assets/toon_generic_4_tex.png", new float4(0, 0, 0, 1), new float2(5, 5));
            _player.SetCorrectionMatrix(float4x4.CreateRotationX((float)-Math.PI / 2) * float4x4.CreateTranslation(-30, 0, 0) * float4x4.Scale(0.5f));

            _room = new GameEntity("Assets/spacebox.obj.model", rc, 150, 1600, -1700);
            _room.SetShader(new float4(1, 1, 1, 1), "Assets/toon_room_7_tex.png", new float4(0, 0, 0, 1), new float2(15, 15));
            _room.SetCorrectionMatrix(float4x4.Scale(13, 8, 19));

            _furniture.Add(new GameEntity("Assets/rocket3.protobuf.model", rc, -1900, 0, -5000));
            _furniture[0].SetShader("Assets/rocket3.png", "Assets/toon_generic_4_tex.png", new float4(0, 0, 0, 1), new float2(15, 15));
            _furniture[0].SetCorrectionMatrix(float4x4.Scale(5));

            _furniture.Add(new GameEntity("Assets/chair.protobuf.model", rc, 1600, 0, -4100, 0, 2.6f));
            _furniture[1].SetShader("Assets/chair.png", "Assets/toon_generic_4_tex.png", new float4(0, 0, 0, 1), new float2(15, 15));
            _furniture[1].SetCorrectionMatrix(float4x4.Scale(29));

            _furniture.Add(new GameEntity("Assets/desk.protobuf.model", rc, 1500, 0, -4950));
            _furniture[2].SetShader("Assets/desk.png", "Assets/toon_generic_4_tex.png", new float4(0, 0, 0, 1), new float2(15, 15));
            _furniture[2].SetCorrectionMatrix(float4x4.Scale(5));

            _furniture.Add(new GameEntity("Assets/bed.protobuf.model", rc, 1800, 0, 800));
            _furniture[3].SetShader("Assets/bed.png", "Assets/toon_generic_4_tex.png", new float4(0, 0, 0, 1), new float2(15, 15));
            _furniture[3].SetCorrectionMatrix(float4x4.Scale(13));

            _furniture.Add(new GameEntity("Assets/drawer.protobuf.model", rc, -700, 0, 1850, 0, (float)Math.PI));
            _furniture[4].SetShader("Assets/drawer.png", "Assets/toon_generic_4_tex.png", new float4(0, 0, 0, 1), new float2(15, 15));
            _furniture[4].SetCorrectionMatrix(float4x4.Scale(14));

            _furniture.Add(new GameEntity("Assets/football.obj.model", rc, -1200, 0, 1000));
            _furniture[5].SetShader("Assets/football.png", "Assets/toon_generic_4_tex.png", new float4(0, 0, 0, 1), new float2(15, 15));
            _furniture[5].SetCorrectionMatrix(float4x4.Scale(4));

            _furniture.Add(new GameEntity("Assets/book.protobuf.model", rc, 500, 1030, -4950, 0, 1));
            _furniture[6].SetShader("Assets/book.png", "Assets/toon_generic_4_tex.png", new float4(0, 0, 0, 1), new float2(15, 15));
            _furniture[6].SetCorrectionMatrix(float4x4.Scale(35));

            _targets.Add(new Target("Assets/cube.obj.model", rc, 1500, 1500, -4950, (float)Math.PI / 4, (float)Math.PI / 4, (float)Math.PI / 4));
            _targets[0].SetCorrectionMatrix(float4x4.Scale(0.5f));
            _targets.Add(new Target("Assets/cube.obj.model", rc, -1200, 800, 1000, (float)Math.PI / 4, (float)Math.PI / 4, (float)Math.PI / 4));
            _targets[1].SetCorrectionMatrix(float4x4.Scale(0.5f));

            _maxScore = _targets.Count;
        }

        public void RenderAFrame()
        {
            if (_currentGameState != _lastGameState)
            {
                switch (_currentGameState)
                {
                    case (int)GameState.StartScreen:
                        _camMatrix = float4x4.CreateTranslation(0, -400, 0);
                        _gui.ShowStartGUI();
                        break;

                    case (int)GameState.Running:
                        _gui.ShowPlayGUI();
                        _player.SetPosition(float4x4.Identity * float4x4.CreateTranslation(0, 400, 0));
                        break;

                    case (int)GameState.GameOver:
                        foreach (var target in _targets)
                        {
                            target.SetInactive();
                        }
                        _gui.ShowStartGUI();
                        _gui.ShowOverGUI();
                        _camMatrix = float4x4.CreateTranslation(0, -400, 0);
                        _player.Speed = 0;
                        break;
                }
                _lastGameState = _currentGameState;
            }

            if (_currentGameState == (int)GameState.Running)
            {
                _player.Move();
                _camMatrix = _player.GetCamMatrix();
                _player.Render(_camMatrix);

                int activeGoalCount = 0;
                foreach (var target in _targets)
                {
                    var distanceVector = _player.GetPositionVector() - target.GetPositionVector();
                    if (distanceVector.Length < 500)
                    {
                        target.SetActive();
                    }
                    if (target.GetStatus())
                    {
                        activeGoalCount++;
                    }
                    target.Render(_camMatrix);
                }
                _curScore = activeGoalCount;

                if (_curScore != _oldScore)
                {
                    _gui.UpdateScore();
                }

                _oldScore = _curScore;

                if (_curScore == _maxScore)
                {
                    _currentGameState = (int)GameState.GameOver;
                }

            }
            else
            {
                _camMatrix *= float4x4.CreateRotationY(0.005f);
            }

            _room.Render(_camMatrix);

            foreach (var gameEntity in _furniture)
            {
                gameEntity.Render(_camMatrix);
            }

            _gui.Render();
        }

        public void Resize()
        {
            _gui.Resize();
        }

        public void SetGamestate(int gameState)
        {
            _currentGameState = gameState;
        }

        public int GetScore()
        {
            return _curScore;
        }

        public int GetMaxScore()
        {
            return _maxScore;
        }
    }

    public enum GameState
    {
        StartScreen,
        Running,
        GameOver
    }
}
