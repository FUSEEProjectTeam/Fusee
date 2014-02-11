using System;
using System.Collections.Generic;
using Fusee.Math;

namespace Fusee.Engine
{
    internal struct ParticleData
    {
        public float3 Position;
        public float3 Velocity; // Contains the direction of movement (the vector's length ist the speed)
        public float3 Gravity;
        public int Life;
        public float maxSize;
        public float minSize;
        public float Rotation;
        public float Transparency;
    }

    public class ParticleEmitter
    {
        #region Shader

        // At first we have to define the shader.
        public string VsSimpleTexture = @"
            /* Copies incoming vertex color without change.
             * Applies the transformation matrix to vertex position.
             */

            attribute vec4 fuColor;
            attribute vec3 fuVertex;
            attribute vec3 fuNormal;
            attribute vec2 fuUV;

            varying vec4 vColor;
            varying vec3 vNormal;
            varying vec2 vUV;
            varying float vTransparency;
        
            uniform mat4 FUSEE_MVP;
            uniform mat4 FUSEE_MV;
            uniform mat4 FUSEE_P;
            uniform mat4 FUSEE_ITMV;
            
            uniform float timer;
            attribute vec4 position;
            varying vec2 texcoord;
            varying float fade_factor;

            void main()
            {
     
                vec4 vPos = FUSEE_MV * vec4(fuVertex, 1.0);//umwandlung in Kamerakoordinaten
               
                // Offset rotieren um fuNormal.x
                vec2 offset = fuUV;
                offset.x  = fuUV.x*cos(fuNormal.x) - fuUV.y*sin(fuNormal.x);
                offset.y =  fuUV.y*cos(fuNormal.x) + fuUV.x*sin(fuNormal.x);
                vPos = vPos + vec4(100*offset, 0, 1.0);   //Offset  aus Partikelzentrum in Partikel-Eckpunkt          
                gl_Position = FUSEE_P * vPos; //Perspektive-Projektion
                vNormal = mat3(FUSEE_ITMV[0].xyz, FUSEE_ITMV[1].xyz, FUSEE_ITMV[2].xyz) * fuNormal;
                vNormal = vec3(0, 0, 1);
                vUV.x = (fuUV.x <= 0) ? 0 : 1;
                vUV.y = (fuUV.y <= 0) ? 0 : 1;

                vTransparency = fuNormal.y;
            }";

        public string PsSimpleTexture = @"
            /* Copies incoming fragment color without change. */
            #ifdef GL_ES
                precision highp float;
            #endif

            // The parameter required for the texturing process
            uniform sampler2D texture1;
            uniform vec4 vColor;
            varying vec3 vNormal;
            varying float vTransparency;

            // The parameter holding the UV-Coordinates of the texture
            varying vec2 vUV;

            void main()
            {    
              // The most basic texturing function, expecting the above mentioned parameters  
              // max(dot(vec3(0,0,1),normalize(vNormal)), 0.1) 
             vec4 AlphaColor = vec4(1.0, 1.0, 1.0, vTransparency);
             gl_FragColor = texture2D(texture1, vUV)*AlphaColor;        
            }";

        #endregion
        public Mesh _particleMesh = new Mesh();

        public Mesh ParticleMesh
        {
            get { return _particleMesh; }
        }

        private List<ParticleData> _particleList = new List<ParticleData>();
        private ParticleData _particle;
        private static Random _rnd;
        private static double _randVelX;
        private static double _randVelY;
        private static double _randVelZ;
        private static double _randPosX;
        private static double _randPosY;
        private static double _randPosZ;
        private static double _randRot;
        private int customCount;
        private int customLifeMin;
        private int customLifeMax;
        private float customMinSize;
        private float customMaxSize;
        private double customRandPosX;
        private double customRandPosY;
        private double customRandPosZ;
        private double customRandVelX;
        private double customRandVelY;
        private double customRandVelZ;
        private float customGravityX;
        private float customGravityY;
        private float customGravityZ;

        //particelcount,minLife, maxLife,minSize, maxSize,randPosX,randPosY,randPosY,randVelX,randVelY,randVelZ,gravityX, gravityY, gravityZ,
        public ParticleEmitter(int myCount, int myLifeMin, int myLifeMax, float myMinSize, float myMaxSize, float myRandPosX, float myRandPosY, float myRandPosZ, double myRandVelX, double myRandVelY, double myRandVelZ, float myGravityX, float myGravityY, float myGravityZ)
        {
            _rnd = new Random();
            customCount = myCount;
            customLifeMin = myLifeMin;
            customLifeMax = myLifeMax;
            customMinSize = myMinSize;
            customMaxSize = myMaxSize;
            customRandPosX = myRandPosX;
            customRandPosY = myRandPosY;
            customRandPosZ = myRandPosZ;
            customRandVelX = myRandVelX;
            customRandVelY = myRandVelY;
            customRandVelZ = myRandVelZ;
            customGravityX = myGravityX;
            customGravityY = myGravityY;
            customGravityZ = myGravityZ;
        }

        public void Tick(double deltaTime)
        {
            var vertices = new float3[customCount*4];
            var triangles = new ushort[customCount*6];
            var normals = new float3[customCount*4];
            var uVs = new float2[customCount*4];

            //Partikel-Daten initialisieren
            while (_particleList.Count < customCount)
            {
                _randVelX = GenRand(-customRandVelX, customRandVelX);
                _randVelY = GenRand(-customRandVelY, customRandVelY);
                _randVelZ = GenRand(-customRandVelZ, customRandVelZ);
                _randPosX = GenRand(-customRandPosX, customRandPosX);
                _randPosY = GenRand(-customRandPosY, customRandPosY);
                _randPosZ = GenRand(-customRandPosZ, customRandPosZ);
                _randRot = GenRand(0.0, 1.3);
                _particle = new ParticleData();
                _particle.Position = new float3((float)_randPosX, (float)_randPosY, (float)_randPosZ);
                _particle.Velocity = new float3((float)_randVelX, (float)_randVelY, (float)_randVelZ);
                _particle.Gravity = new float3(customGravityX, customGravityY, customGravityZ);
                _particle.Life = GenIntRand(customLifeMin, customLifeMax);
                _particle.maxSize = customMaxSize;
                _particle.minSize = customMinSize;
                _particle.Rotation = (float)_randRot;
                _particle.Transparency = 0.4f;
                _particleList.Add(_particle);
            }

            // Partikel erstellen und Wert zuweisen
            for (int k = 0; k < _particleList.Count; k++)
            {
                ParticleData t = _particleList[k];
                float3 currentPos = t.Position;
                float currentSize = t.minSize;


                if (t.Life > 0)
                {
                    vertices[k * 4 + 0] = currentPos;
                    vertices[k * 4 + 1] = currentPos;
                    vertices[k * 4 + 2] = currentPos;
                    vertices[k * 4 + 3] = currentPos;

                    uVs[k * 4 + 0] = new float2(currentSize / 2, -currentSize / 2);
                    uVs[k * 4 + 1] = new float2(currentSize / 2, currentSize / 2);
                    uVs[k * 4 + 2] = new float2(-currentSize / 2, currentSize / 2);
                    uVs[k * 4 + 3] = new float2(-currentSize / 2, -currentSize / 2);

                    normals[k * 4 + 0] = new float3(t.Rotation, t.Transparency, 1);
                    normals[k * 4 + 1] = new float3(t.Rotation, t.Transparency, 1);
                    normals[k * 4 + 2] = new float3(t.Rotation, t.Transparency, 1);
                    normals[k * 4 + 3] = new float3(t.Rotation, t.Transparency, 1);

                    triangles[k*6 + 0] = (ushort) (k*4 + 0);
                    triangles[k*6 + 1] = (ushort) (k*4 + 1);
                    triangles[k*6 + 2] = (ushort) (k*4 + 2);
                    triangles[k*6 + 3] = (ushort) (k*4 + 0);
                    triangles[k*6 + 4] = (ushort) (k*4 + 2);
                    triangles[k*6 + 5] = (ushort) (k*4 + 3);
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

                //Rotation
                _changeParticle.Rotation += 0.008f;

                //Transparency
                _changeParticle.Transparency -= _changeParticle.Transparency/_changeParticle.Life;

                //Skalierung
                if (_changeParticle.minSize <= _changeParticle.maxSize)
                {
                    _changeParticle.minSize += _changeParticle.maxSize / _changeParticle.Life;
                }

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


        private static double GenRand(double one, double two)
        {
            Random rand = _rnd;
            return one + rand.NextDouble()*(two - one);
        }

        private static int GenIntRand(int one, int two)
        {
            Random rand = _rnd;
            return rand.Next(one, two);
        }
    }
}