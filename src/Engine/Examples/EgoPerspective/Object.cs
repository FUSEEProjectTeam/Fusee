using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Fusee.Engine;
using Fusee.Math;


class Object
{
    private int posX, posY, posZ;
    private int angXY, angXZ, angYZ;
    private float4x4 position;

    private Mesh mesh;

    public Object(Geometry geo, int x, int y, int z)
    {
        mesh = geo.ToMesh();
        posX = x;
        posY = y;
        posZ = z;
    }

    public Mesh getMesh()
    {
        return mesh;
    }

    public int getPosX()
    {
        return posX;
    }
    public int getPosY()
    {
        return posY;
    }
    public int getPosZ()
    {
        return posZ;
    }
    public int angleXY()
    {
        return angXY;
    }

}

