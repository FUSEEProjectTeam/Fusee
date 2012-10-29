using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Fusee.Engine;
using Fusee.Math;

class Weapon
{
    private int posX, posY, posZ;
    private int angXY, angXZ, angYZ;

    private Geometry weap;
    private Geometry shot;
    private Mesh mesh;
    private Mesh meshShot;

    private List<Shot> shots;

    private float rotationStatus;
    private float rotationSpeed;

    public Weapon()
    {
        weap = MeshReader.ReadWavefrontObj(new StreamReader(@"SampleObj/Mini.model"));       
        mesh = weap.ToMesh();
        shot = MeshReader.ReadWavefrontObj(new StreamReader(@"SampleObj/Sphere.obj.model"));
        meshShot = shot.ToMesh();
       

        shots = new List<Shot>();

        posX = -80;
        posY = -100;
        posZ = 200;
        angYZ = 90;
        rotationStatus = 0;
        rotationSpeed = 0.1f;

    }

    public void update(RenderContext Rcon, Input input)
    {
        if (input.IsKeyDown(KeyCodes.Space))
        {
            //shots.Add(new Shot(meshShot ,posX, posY, posZ, 0,0,0));
            rotationStatus += rotationSpeed;
        }


        ////// render weapon
        Rcon.ModelView = float4x4.CreateRotationY(rotationStatus);
        Rcon.ModelView *= float4x4.CreateRotationX((float)Math.PI / 2);
        Rcon.ModelView *= float4x4.CreateTranslation(posX, posY, posZ);
        Rcon.ModelView *= float4x4.LookAt(0, 0, 0, 0, 0, 1, 0, 1, 0);
        Rcon.Render(mesh);



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
    public int getaAngleYZ()
    {
        return angYZ;
    }

    public List<Shot> getShots()
    {
        return shots;
    }
}
