using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Fusee.Engine;
using Fusee.Math;


class Shot
{
    float posX, posY, posZ;
    float _directionX, _directionY, _directionZ;
    private Mesh mesh;


    public Shot(Mesh m, float x, float y, float z, float dirX, float dirY, float dirZ)
    {
        mesh = m;
        posX = x;
        posY = y;
        posZ = z;
        _directionX = dirX;
        _directionY = dirY;
        _directionZ = dirZ;
    }

    public void update()
    {
        posX += _directionX;
        posY += _directionY;
        posZ += _directionZ;
    }

    public float getPosX()
    {
        return posX;
    }
    public float getPosY()
    {
        return posY;
    }
    public float getPosZ()
    {
        return posZ;
    }
    public Mesh getMesh()
    {
        return mesh;
    }
}
