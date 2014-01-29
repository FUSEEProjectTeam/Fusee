using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Fusee.Math;


namespace Fusee.Engine
{
    struct ParticleData
    {
        public float3 Position;
        public float3 Velocity; // Contains the direction of movement (the vector's length ist the speed)
        public float3 Gravity;
        public int Life;
        public float Size;
        public ShaderProgram ParticleTexture;
        public IShaderParam ParticleTextureParam;
        public ITexture ParticleTex;
    }
    
    public class ParticleEmitter
    {

        public Mesh _particleMesh = new Mesh();

        public Mesh ParticleMesh
        {
            get { return _particleMesh; }
        }

        private int _nStartParticles;
        // ... Rules for the particles system, e.g.
        // Direction/Speed vector of emission
        // Speed damping / Direction variation of particels over time
        // Initial rotation of particles
        // rotation damping over time

        
        
        private List<ParticleData> _particleList = new List<ParticleData>();
        private ParticleData _particle;
        private static Random _rnd;
        private static double _randVelX;
        private static double _randVelY;
        private static double _randVelZ;
        private static double _randPosX;
        private static double _randPosY;
        private static double _randPosZ;

        private int customCount;
        private int customLifeMin;
        private int customLifeMax;
        private float customSize;
        private double customRandPosX;
        private double customRandPosY;
        private double customRandPosZ;
        private double customRandVelX;
        private double customRandVelY;
        private double customRandVelZ;
        private  float customGravityX;
        private  float customGravityY;
        private  float customGravityZ;
        public float Al = 0.2f;
        
        
        

        //particelcount,Size,Life,randVelX,randVelY,randVelZ,gravityX, gravityY, gravityZ,
        public ParticleEmitter(int myCount, int myLifeMin, int myLifeMax, float mySize, float myRandPosX, float myRandPosY, float myRandPosZ, double myRandVelX, double myRandVelY, double myRandVelZ, float myGravityX, float myGravityY, float myGravityZ)
        {
            _rnd            = new Random();
            customCount     = myCount;
            customLifeMin   = myLifeMin;
            customLifeMax   = myLifeMax;
            customSize      = mySize;
            customRandPosX  = myRandPosX;
            customRandPosY  = myRandPosY;
            customRandPosZ  = myRandPosZ;
            customRandVelX  = myRandVelX;
            customRandVelY  = myRandVelY;
            customRandVelZ  = myRandVelZ;
            customGravityX  = myGravityX;
            customGravityY  = myGravityY;
            customGravityZ  = myGravityZ;

        }

        public void Tick(double deltaTime)
        {
            float3[] vertices = new float3[customCount * 4];
            short[] triangles = new short[customCount * 6];
            float3[] normals = new float3[customCount * 4];
            float2[] uVs = new float2[customCount * 4];

            //var mtxNew = float4x4.CreateRotationY(5) * float4x4.CreateRotationX(5);
            //var myRot = float4x4.CreateRotationZ(1.0f);
            //Partikel-Daten initialisieren
            while (_particleList.Count < customCount)
            {
                    _randVelX = GenRand(-customRandVelX, customRandVelX);
                    _randVelY = GenRand(-customRandVelY, customRandVelY);
                    _randVelZ = GenRand(-customRandVelZ, customRandVelZ);
                    _randPosX = GenRand(-customRandPosX, customRandPosX);
                    _randPosY = GenRand(-customRandPosY, customRandPosY);
                    _randPosZ = GenRand(-customRandPosZ, customRandPosZ);
                    _particle = new ParticleData();
                    _particle.Position = new float3((float)_randPosX, (float)_randPosY, (float)_randPosZ);
                    _particle.Velocity = new float3((float)_randVelX, (float)_randVelY, (float)_randVelZ);
                    _particle.Gravity = new float3(customGravityX, customGravityY, customGravityZ);
                    _particle.Life = GenIntRand(customLifeMin, customLifeMax);
                    _particle.Size = customSize;
                    _particleList.Add(_particle);
            }


            // Partikel erstellen und Wert zuweisen
            for (int k = 0; k < _particleList.Count; k++)
            {

                ParticleData t = _particleList[k];
                float3 currentPos = t.Position;
                float currentSize = t.Size;

                
                if (t.Life > 0)
                {
                    vertices[k * 4 + 0] = currentPos; 
                    vertices[k * 4 + 1] = currentPos;
                    vertices[k * 4 + 2] = currentPos;
                    vertices[k * 4 + 3] = currentPos;

                    uVs[k * 4 + 0] = new float2(currentSize, 0);
                    uVs[k * 4 + 1] = new float2(currentSize, currentSize);
                    uVs[k * 4 + 2] = new float2(0, currentSize);
                    uVs[k * 4 + 3] = new float2(0, 0);

                    normals[k * 4 + 0] = new float3(0, 0, 1);
                    normals[k * 4 + 1] = new float3(0, 0, 1);
                    normals[k * 4 + 2] = new float3(0, 0, 1);
                    normals[k * 4 + 3] = new float3(0, 0, 1);

                    triangles[k * 6 + 0] = (short)(k * 4 + 0);
                    triangles[k * 6 + 1] = (short)(k * 4 + 1);
                    triangles[k * 6 + 2] = (short)(k * 4 + 2);
                    triangles[k * 6 + 3] = (short)(k * 4 + 0);
                    triangles[k * 6 + 4] = (short)(k * 4 + 2);
                    triangles[k * 6 + 5] = (short)(k * 4 + 3);
                }
                
 
            }
            _particleMesh.Vertices = vertices;
            _particleMesh.Triangles = triangles;
            _particleMesh.Normals = normals;
            _particleMesh.UVs = uVs;

            //Update für Änderungen
            for (int i = 0; i < _particleList.Count; i++)
            {
                ParticleData _changeParticle = _particleList[i];
                _changeParticle.Position += (float)deltaTime * _changeParticle.Velocity;
                _changeParticle.Velocity -= _changeParticle.Gravity;
               // _changeParticle.Position *= float4x4.CreateRotationZ(0.01f);
                //_changeParticle.Velocity *= float4x4.CreateRotationZ(360.0f);
                
                //Skalierung
                /*if (_changeParticle.Size >= 0.0f)
                {
                    _changeParticle.Size -= 0.001f;
                }*/

                //Rotation
                //_changeParticle.Size *= 0.001f;

                if (_changeParticle.Life != 0)
                {
                    _changeParticle.Life -= 1;                    
                }
                _particleList[i] = _changeParticle;
                if (_changeParticle.Life == 0)
                {
                    _particleList.Remove(_changeParticle);
                }

            }
            


        }


        static double GenRand(double one, double two)
        {
            Random rand = _rnd;
            return one + rand.NextDouble() * (two - one);
        }
        static int GenIntRand(int one, int two)
        {
            Random rand = _rnd;
            return rand.Next(one, two);
        }



    }
}
