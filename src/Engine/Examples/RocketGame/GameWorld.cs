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


        public GameWorld(RenderContext rc)
        {
            _player = new Player("Assets/rocket.obj.model", rc);
            _player.SetShader("Assets/rocket.png");

            _furniture.Add(new GameEntity("Assets/cube.obj.model", rc, 250, 0, 0, 0.5f, 0.5f, 0.5f));
            _furniture[0].SetShader(new float4(0, 1, 0, 1));
            _furniture.Add(new GameEntity("Assets/cube.obj.model", rc, 0, 250));
            _furniture[1].SetShader("Assets/tex_cube.jpg");
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
