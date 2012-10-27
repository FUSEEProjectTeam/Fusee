using System;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.MyTestGame
{
    public class Level
    {
        private int _cnt;
        public Feld[] LevelFeld;
        private byte[,] LevelTpl;
        public float4x4 CamPosition = float4x4.LookAt(0, 0, 4000, 0, 0, 0, 0, 1, 0);
        public float4x4 CamTranslation;
        public float4x4 ObjectOrientation = float4x4.CreateRotationX((float)Math.PI/2);

        public Level(byte sizex, byte sizey, RenderContext rc, ShaderProgram sp)
        {
            LevelFeld = new Feld[sizex * sizey];
            LevelTpl = new byte[sizex,sizey];
            CamTranslation = float4x4.CreateTranslation((float) (sizex-1) * -100, (float) (sizey-1) * -100, 0);
            SetLevel(1, rc, sp);
        }
        public Level(byte sizex, byte sizey, RenderContext rc, ShaderProgram sp, byte id)
        {
            LevelFeld = new Feld[sizex * sizey];
            LevelTpl = new byte[sizex,sizey];
            SetLevel(id, rc, sp);
        }

        private void SetLevel(int id, RenderContext rc, ShaderProgram sp)
        {
            //Dummy for a level file - level files need a check on read not to be bigger than the board 
            LevelTpl[0, 0] = 1;
            LevelTpl[0, 1] = 2;
            LevelTpl[0, 2] = 2;
            LevelTpl[0, 3] = 2;
            LevelTpl[1, 3] = 2;
            LevelTpl[2, 3] = 2;
            LevelTpl[3, 3] = 2;
            LevelTpl[3, 2] = 2;
            LevelTpl[3, 1] = 3;
            //Dummy end

            for (byte x = 0; x < LevelTpl.GetLength(0); x++)
            {
                for (byte y = 0; y < LevelTpl.GetLength(1); y++)
                {
                    LevelFeld[_cnt] = new Feld(rc, sp) {X = x, Y = y, Type = LevelTpl[x, y]};
                    _cnt++;
                }
            }
        }

        public void Render(float4x4 mtxRot)
        {
            foreach (Feld feld in LevelFeld)
            {
                feld.Render(CamPosition, CamTranslation, ObjectOrientation, mtxRot);
            }
        }
    }
}
