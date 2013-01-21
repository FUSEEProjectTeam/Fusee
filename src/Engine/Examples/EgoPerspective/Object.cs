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
    private float posX, posY, posZ;
    private float angX, angY, angZ;
    private float4x4 _position;

    private Mesh mesh;
    private ShaderMaterial material;

    public Object(Geometry geo, ShaderMaterial m, int x, int y, int z)
    {
        mesh = geo.ToMesh();
        material = m;
        posX = x;
        posY = y;
        posZ = z;
        angX = .0f;
        angY = .0f;
        angY = .0f;
    }

    public Mesh GetMesh()
    {
        return mesh;
    }
    public ShaderProgram GetShader(RenderContext RC)
    {   
        material.UpdateMaterial(RC);
        return material.GetShader();
    }

    public float GetPosX()
    {
        return posX;
    }
    public float GetPosY()
    {
        return posY;
    }
    public float GetPosZ()
    {
        return posZ;
    }
    public float GetAngleX()
    {
        return angX;
    }
    public float GetAngleY()
    {
        return angY;
    }
    public float GetAngleZ()
    {
        return angZ;
    }

    public void SetX(float x)
    {
        posX = x;
    }
    public void setY(float y)
    {
        posY = y;
    }
    public void SetZ(float z)
    {
        posZ = z;
    }
    public void SetAngleX(float angle)
    {
        angX = angle;
    }
    public void SetAngleY(float angle)
    {
        angY = angle;
    }
    public void SetAngleZ(float angle)
    {
        angZ = angle;
    }

}

