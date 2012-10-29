using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Fusee.Engine;
using Fusee.Math;



class World
{
    private RenderContext RC;
    private Input In;
    private List<Object> objects;
    private Weapon weapon;

    private float globalPosX;
    private float globalPosY;
    private float globalPosZ;
    private float globalAngleXZ;
    private float speed;
    private float rotationSpeed;

    private float weaponRotation;
    private float weaponRotationSpeed;


    public World(RenderContext renderCon, Input input)
    {
        RC = renderCon;
        In = input;
        objects = new List<Object>();
        weapon = new Weapon();

        globalPosX = 0;
        globalPosY = 0;
        globalPosZ = 0;
        globalAngleXZ = 0;
        speed = 7;
        rotationSpeed = 0.015f;

        weaponRotation = 0;
        weaponRotationSpeed = 0.1f;
    }





    public void addObject(Geometry geo, int posX, int posY, int posZ)
    {
        objects.Add(new Object(geo, posX, posY, posZ));
    }


    public void RenderWorld(float drehungHorz)
    {

        if (In.IsKeyDown(KeyCodes.W))
        {
            globalPosX += speed * (float)Math.Sin(globalAngleXZ);
            globalPosZ += speed * (float)Math.Cos(globalAngleXZ);
        }
        if (In.IsKeyDown(KeyCodes.S))
        {
            globalPosX -= speed * (float)Math.Sin(globalAngleXZ);
            globalPosZ -= speed * (float)Math.Cos(globalAngleXZ);
        }
        if (In.IsKeyDown(KeyCodes.A))
        {
            globalPosX += speed * (float)Math.Cos(globalAngleXZ);
            globalPosZ -= speed * (float)Math.Sin(globalAngleXZ);
        }
        if (In.IsKeyDown(KeyCodes.D))
        {
            globalPosX -= speed * (float)Math.Cos(globalAngleXZ);
            globalPosZ += speed * (float)Math.Sin(globalAngleXZ);
        }
        if (In.IsKeyDown(KeyCodes.Left))
        {
            globalAngleXZ += rotationSpeed;
        }
        if (In.IsKeyDown(KeyCodes.Right))
        {
            globalAngleXZ -= rotationSpeed;
        }

        ////// render all objects
        for (int i = 0; i < objects.Count; i++)
        {
            RC.ModelView = float4x4.CreateTranslation(objects[i].getPosX() - globalPosX, objects[i].getPosY() + globalPosY, objects[i].getPosZ() - globalPosZ) ;
            RC.ModelView *= float4x4.LookAt(0, 0, 0, (float) Math.Sin(globalAngleXZ), 0, (float) Math.Cos(globalAngleXZ), 0, 1, 0);
            RC.Render(objects[i].getMesh());
        }

        ////// render Shots
        for (int i = 0; i < weapon.getShots().Count; i++)
        {
            weapon.getShots()[i].update();
            RC.ModelView = float4x4.CreateTranslation(weapon.getShots()[i].getPosX() - globalPosX, weapon.getShots()[i].getPosY() + globalPosY, weapon.getShots()[i].getPosZ() - globalPosZ);
            RC.ModelView *= float4x4.LookAt(0, 0, 0, (float)Math.Sin(globalAngleXZ), 0, (float)Math.Cos(globalAngleXZ), 0, 1, 0);
            RC.Render(weapon.getShots()[i].getMesh());
        }


        weapon.update(RC, In);

    }


}

