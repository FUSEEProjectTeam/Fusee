using System;
using System.Collections.Generic;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.RocketGame
{
    public class GameWorld
    {
        private readonly RenderContext _rc;

        private List<GameEntity> _furniture = new List<GameEntity>();
        private List<GameEntity> _goals = new List<GameEntity>();

        private readonly Player _player;


        public GameWorld(RenderContext rc)
        {
            _rc = rc;

            var material = new ShaderMaterial(_rc.CreateShader(Shader.GetVsSimpleTextureShader(), Shader.GetPsSimpleTextureShader()));
            _player = new Player("Assets/cube.obj.model", material, _rc);

            _furniture.Add(new GameEntity("Assets/cube.obj.model", material, _rc, 250, 0, 0, 0.5f, 0.5f, 0.5f));
            _furniture.Add(new GameEntity("Assets/cube.obj.model", material, _rc, 0, 250, 0));

        }

        public void Render()
        {
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
        }
    }
}
