using System;
using System.Collections.Generic;
using Fusee.Math;

namespace Fusee.Engine
{
    internal sealed class ParticleData
    {
        public float3 Position;
        public float3 Velocity; 
        public float3 Gravity;
        public int Life;
        public float MaxSize;
        public float MinSize;
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
               
                //Offset rotieren um fuNormal.x
                vec2 offset = fuUV;
                offset.x  = fuUV.x*cos(fuNormal.x) - fuUV.y*sin(fuNormal.x);
                offset.y =  fuUV.y*cos(fuNormal.x) + fuUV.x*sin(fuNormal.x);
                vPos = vPos + vec4(100.0*offset, 0, 1.0);   //Offset  aus Partikelzentrum in Partikel-Eckpunkt          
                gl_Position = FUSEE_P * vPos; //Perspektive-Projektion
                vNormal = mat3(FUSEE_ITMV[0].xyz, FUSEE_ITMV[1].xyz, FUSEE_ITMV[2].xyz) * fuNormal;
                vNormal = vec3(0, 0, 1);
                vUV.x = (fuUV.x <= 0.0) ? 0.0 : 1.0;
                vUV.y = (fuUV.y <= 0.0) ? 0.0 : 1.0;

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
             vec4 AlphaColor = vec4(1.0, 1.0, 1.0, vTransparency);
             gl_FragColor = texture2D(texture1, vUV)*AlphaColor;        
            }";

        #endregion
        /// <summary>
        /// The entire particle system consists of one mesh.
        /// </summary>
        public Mesh ParticleMesh = new Mesh();

        public Mesh PMesh
        {
            get { return ParticleMesh; }
        }

        private List<ParticleData> _particleList = new List<ParticleData>();
        private ParticleData _particle;
        private Random _rnd;
        private double _randVelX;
        private double _randVelY;
        private double _randVelZ;
        private double _randPosX;
        private double _randPosY;
        private double _randPosZ;
        private double _randRot;
        private int _randLife;

        private int _customCount;
        private int _customLifeMin;
        private int _customLifeMax;
        private float _customMinSize;
        private float _customMaxSize;
        private double _customRandPosX;
        private double _customRandPosY;
        private double _customRandPosZ;
        private double _customRandVelX;
        private double _customRandVelY;
        private double _customRandVelZ;
        private float _customGravityX;
        private float _customGravityY;
        private float _customGravityZ;
        private float _customTransparency;
        private float _customRotation;
        

/// <summary>
/// Initializes a new instance of the ParticleEmittter class.
/// </summary>
/// <param name="myCount"></param>
/// <param name="myLifeMin"></param>
/// <param name="myLifeMax"></param>
/// <param name="myMinSize"></param>
/// <param name="myMaxSize"></param>
/// <param name="myRotation"></param>
/// <param name="myTransparency"></param>
/// <param name="myRandPosX"></param>
/// <param name="myRandPosY"></param>
/// <param name="myRandPosZ"></param>
/// <param name="myRandVelX"></param>
/// <param name="myRandVelY"></param>
/// <param name="myRandVelZ"></param>
/// <param name="myGravityX"></param>
/// <param name="myGravityY"></param>
/// <param name="myGravityZ"></param>
        public ParticleEmitter(int myCount, int myLifeMin, int myLifeMax, float myMinSize, float myMaxSize, float myRotation, float myTransparency, float myRandPosX, float myRandPosY, float myRandPosZ, double myRandVelX, double myRandVelY, double myRandVelZ, float myGravityX, float myGravityY, float myGravityZ)
        {
            
            _customCount = myCount;
            _customLifeMin = myLifeMin;
            _customLifeMax = myLifeMax;
            _customMinSize = myMinSize;
            _customMaxSize = myMaxSize;
            _customRotation = myRotation;
            _customTransparency = myTransparency;
            _customRandPosX = myRandPosX;
            _customRandPosY = myRandPosY;
            _customRandPosZ = myRandPosZ;
            _customRandVelX = myRandVelX;
            _customRandVelY = myRandVelY;
            _customRandVelZ = myRandVelZ;
            _customGravityX = myGravityX;
            _customGravityY = myGravityY;
            _customGravityZ = myGravityZ;
            
        }
       /*
        /// <summary>
        ///  Gets and sets the count of the particles.
        /// </summary>
        public int Count
        {
            get
            {
                return _customCount;
            }
            set
            {
                _customCount = value;
            }
        }
*/
        /// <summary>
        /// Gets and sets the minimum life of the particles.
        /// </summary>
        public int LifeMin
        {
            get
            {
                return _customLifeMin;
            }
            set
            {
                _customLifeMin = value;
            }
        }
        /// <summary>
        /// Gets and sets the maximum life of the particles.
        /// </summary>
        public int LifeMax
        {
            get
            {
                return _customLifeMax;
            }
            set
            {
                _customLifeMax = value;
            }
        }
        /// <summary>
        /// Gets and sets the minimum size of the particles.
        /// </summary>
        public float MinSize
        {
            get
            {
                return _customMinSize;
            }
            set
            {
                _customMinSize = value;
            }
        }
        /// <summary>
        /// Gets and sets the maximum size of the particles.
        /// </summary>
        public float MaxSize
        {
            get
            {
                return _customMaxSize;
            }
            set
            {
                _customMaxSize = value;
            }
        }
        /// <summary>
        /// Gets and sets the transparency of particles.
        /// </summary>
        public float Transparency
        {
            get
            {
                return _customTransparency;
            }
            set
            {
                _customTransparency = value;
            }
        }
        /// <summary>
        /// Gets and sets the random position on the x-axis.
        /// </summary>
        public double RandPosX 
        {
            get
            {
                return _customRandPosX;
            }
            set
            {
                _customRandPosX = value;
            }
        }
        /// <summary>
        /// Gets and sets the random position on the y-axis.
        /// </summary>
        public double RandPosY 
        {
            get
            {
                return _customRandPosY;
            }
            set
            {
                _customRandPosY = value;
            }
        }
        /// <summary>
        /// Gets and sets the random position on the z-axis.
        /// </summary>
        public double RandPosZ 
        {
            get
            {
                return _customRandPosZ;
            }
            set
            {
                _customRandPosZ = value;
            }
        }
        /// <summary>
        /// Gets and sets the random velocity towards the x-axis.
        /// </summary>
        public double RandVelX
        {
            get
            {
                return _customRandVelX;
            }
            set
            {
                _customRandVelX = value;
            }
        }
        /// <summary>
        /// Gets and sets the random velocity towards the y-axis.
        /// </summary>
        public double RandVelY
        {
            get
            {
                return _customRandVelY;
            }
            set
            {
                _customRandVelY = value;
            }
        }
        /// <summary>
        /// Gets and sets the random velocity towards the z-axis.
        /// </summary>
        public double RandVelZ
        {
            get
            {
                return _customRandVelZ;
            }
            set
            {
                _customRandVelZ = value;
            }
        }
        /// <summary>
        /// Gets and sets the gravity towards the x-axis.
        /// </summary>
        public float GravityX
        {
            get
            {
                return _customGravityX;
            }
            set
            {
                _customGravityX = value;
            }
        }
        /// <summary>
        /// Gets and sets the gravity towards the y-axis.
        /// </summary>
        public float GravityY
        {
            get
            {
                return _customGravityY;
            }
            set
            {
                _customGravityY = value;
            }
        }
        /// <summary>
        /// Gets and sets the gravity towards the z-axis.
        /// </summary>
        public float GravityZ
        {
            get
            {
                return _customGravityZ;
            }
            set
            {
                _customGravityZ = value;
            }
        }
        /// <summary>
        /// This method fills the particleList with data, creates the particle mesh and manipulates the information of particleList.
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Tick(double deltaTime)
        {
             var vertices = new float3[_customCount * 4];
             var triangles = new ushort[_customCount * 6];
             var normals = new float3[_customCount * 4];
             var uVs = new float2[_customCount * 4];
             _rnd = new Random();

             //Initializes particle Data
             while (_particleList.Count < _customCount)
             {

                 _randVelX = GenRand(-_customRandVelX, _customRandVelX);
                 _randVelY = GenRand(-_customRandVelY, _customRandVelY);
                 _randVelZ = GenRand(-_customRandVelZ, _customRandVelZ);
                 _randPosX = GenRand(-_customRandPosX, _customRandPosX);
                 _randPosY = GenRand(-_customRandPosY, _customRandPosY);
                 _randPosZ = GenRand(-_customRandPosZ, _customRandPosZ);
                 _randRot = GenRand(0.0, 1.3);
                 _randLife = GenIntRand(_customLifeMin, _customLifeMax);
                 _particle = new ParticleData();
                 _particle.Position = new float3((float)_randPosX, (float)_randPosY, (float)_randPosZ);
                 _particle.Velocity = new float3((float)_randVelX, (float)_randVelY, (float)_randVelZ);
                 _particle.Gravity = new float3(_customGravityX, _customGravityY, _customGravityZ);
                 _particle.Life = _randLife;
                 _particle.MaxSize = _customMaxSize;
                 _particle.MinSize = _customMinSize;
                 _particle.Rotation = (float)_randRot;
                 _particle.Transparency = _customTransparency;
                 _particleList.Add(_particle);
             }

             // creates particle mesh
             for (int k = 0; k < _particleList.Count; k++)
             {
                 ParticleData t = _particleList[k];
                 float3 currentPos = t.Position;
                 float currentSize = t.MinSize;

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

             ParticleMesh.Vertices = vertices;
             ParticleMesh.Triangles = triangles;
             ParticleMesh.Normals = normals;
             ParticleMesh.UVs = uVs;

             //Updates information
             for (int i = 0; i < _particleList.Count; i++)
             {
                 ParticleData changeParticle = _particleList[i];
                 changeParticle.Position += (float)deltaTime * changeParticle.Velocity;
                 changeParticle.Velocity -= changeParticle.Gravity;

                 //Rotation
                 changeParticle.Rotation += _customRotation;

                 //Transparency
                 changeParticle.Transparency -= (changeParticle.Transparency/changeParticle.Life);

                 //Scale
                 if (changeParticle.MinSize <= changeParticle.MaxSize)
                 {
                     changeParticle.MinSize += changeParticle.MaxSize / changeParticle.Life;
                 }
                 //Life
                 if (changeParticle.Life != 0)
                 {
                     changeParticle.Life -= 1;
                 }
                 _particleList[i] = changeParticle;
                 //Death 
                 if (changeParticle.Life == 0 || changeParticle.Life <= 0)
                 {
                     _particleList.Remove(changeParticle);
                 }
             }
        }

        private  double GenRand(double one, double two)
        {
            Random rand = _rnd;
            return one + rand.NextDouble()*(two - one);
        }

        private  int GenIntRand(int one, int two)
        {
            Random rand = _rnd;
            if (one < two)
            {
                return rand.Next(one, two);
            }
            else
            {
                return rand.Next(two, one);
            }

        }
    }
}