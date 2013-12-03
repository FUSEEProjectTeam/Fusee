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

        //public bool Active;
        public float3 Position;
        public float3 Velocity; // Contains the direction of movement (the vector's length ist the speed)
        public int Life;
        public float Size;
        public ShaderProgram ParticleTexture;
        public IShaderParam ParticleTextureParam;
        public ITexture ParticleTex;
        //public int Id;

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

        public float3 _gravity = new float3(0.0f, 0.05f, 0.0f);
        private static int maxParticles = 100; // Number of particles to create
        
        private List<ParticleData> _particleList = new List<ParticleData>();
        private ParticleData _particle = new ParticleData();
        
        private static Random _rnd;
        private static double _randVelX;
        private static double _randVelY;
        private static double _randVelZ;

   

        public ParticleEmitter(int nStartParticles)
        {
            _rnd = new Random();
     
        }

        public void Tick(double deltaTime)
        {
            float3[] vertices = new float3[maxParticles*4];
            short[] triangles = new short[maxParticles * 6];
            float3[] normals = new float3[maxParticles * 4];
            float2[] uVs = new float2[maxParticles * 4];
            //Partikel-Daten initialisieren
           while (_particleList.Count < maxParticles)
            {
                    _randVelX = GenRand(-10.0, 10.0);
                    _randVelY = GenRand(-10.0, 10.0);
                    _randVelZ = GenRand(-10.0, 10.0); 
                    _particle = new ParticleData();
                    _particle.Position = new float3(1, 1, 0);
                    _particle.Velocity = new float3((float)_randVelX, (float)_randVelY, (float)_randVelZ);
                    _particle.Life = GenIntRand(280, 620);
                    _particle.Size = 1.6f;
                    _particleList.Add(_particle);
            }


            // Partikel erstellen und Wert zuweisen
            for (int k = 0; k < _particleList.Count; k++)
            {

                ParticleData t = _particleList[k];
                float3 currentPos = t.Position;
                float currentSize = t.Size;
                
               // Console.WriteLine("HalloWeltNeu " + t.Life);
                if (t.Life > 0)
                {
                    //Console.WriteLine("HalloWeltNeu " + k);
                    vertices[k * 4 + 0] = currentPos + new float3(currentSize, -currentSize, 0);
                    vertices[k * 4 + 1] = currentPos + new float3(currentSize, currentSize, 0);
                    vertices[k * 4 + 2] = currentPos + new float3(-currentSize, currentSize, 0);
                    vertices[k * 4 + 3] = currentPos + new float3(-currentSize, -currentSize, 0);

                    triangles[k*6 + 0] = (short) (k*4 + 0);
                    triangles[k * 6 + 1] = (short)(k * 4 + 1);
                    triangles[k * 6 + 2] = (short)(k * 4 + 2);
                    triangles[k * 6 + 3] = (short)(k * 4 + 0);
                    triangles[k * 6 + 4] = (short)(k * 4 + 2);
                    triangles[k * 6 + 5] = (short)(k * 4 + 3);

                    normals[k * 4 + 0] = new float3(0, 0, 1);
                    normals[k * 4 + 1] = new float3(0, 0, 1);
                    normals[k * 4 + 2] = new float3(0, 0, 1);
                    normals[k * 4 + 3] = new float3(0, 0, 1);

                    uVs[k * 4 + 0] = new float2(1, 0);
                    uVs[k * 4 + 1] = new float2(1, 1);
                    uVs[k * 4 + 2] = new float2(0, 1);
                    uVs[k * 4 + 3] = new float2(0, 0);

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
               //
                _changeParticle.Position += (float)deltaTime * _changeParticle.Velocity;
                _changeParticle.Velocity -= _gravity;
                //Skalierung
                if (_changeParticle.Size >= 0.0f)
                {
                    _changeParticle.Size -= 0.001f;
                }

                if (_changeParticle.Life != 0)
                {
                    _changeParticle.Life -= 1;
                    //Console.WriteLine(_changeParticle.Life);
                    
                }
                _particleList[i] = _changeParticle;
                if (_changeParticle.Life == 0)
                {
                    //Array.Clear(ParticleMesh.Vertices, i, 0);
                    //Array.Clear(ParticleMesh.Triangles, i, 0);
                    _particleList.Remove(_changeParticle);
                    Console.WriteLine(_particleList.Count);

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
