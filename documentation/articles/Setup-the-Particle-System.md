  > ❌ **Outdated**

# Introduction
The FUSEE particle system enables to create particle simulations like fire, smoke or explosions. For each particle system there will be created a mesh which consists a lot triangles which represents particles. You can manipulate several properties to create your own particle effects.

## Description of properties
There are 16 components of the particle system. You can define them, when create a new object. 
… = new ParticleEmitter(count, minLife, maxLife, minSize, maxSize, rotationSpeed, transparency, randPosX, randPosY, randPosZ, randVelX, randVelY, randVelZ, gravityX, gravityY, gravityZ);

Below the components will be described.

### Count	
= Number of particles of the particle system 

### MinLife	
= Minimum Lifetime of particles

### MaxLife	
= Maximum Lifetime of particles

Note: The particle system will create random lifetime between “MinLife” and “MaxLife”. If you want to create a fluent effect make sure to use numbers which are far away from eachother.

### MinSize	
= Minimum Size of each particles.

### MaxSize	
= Maximum Size of each particles.

Note: The particle Size interpolate from the minimum to the maximum.

### RotationSpeed
= Controls speed of the rotation of each particle. Use small values (for example 0.012f).

### Transparency	
= Controls the visibility of each particle. At 1.0 each particle is full visible and at 0.0 each particle is invisible. Each particle gets invisible over time. 

### RandPosX,RandPosY,RandPosZ
= The random position where particles emits.

### RandVelX,RandVelY,RandVelZ
= The velocity of each particle. The velocity will be positive and negative. Particles will move in both directions. 

### GravityX, GravityY, GravityZ
= The gravity changes the velocity to create for example real-life behaviour. It is also helpful, to steer particles in one direction(in combination with velocity or alone).

### Hint 
The usage of getter/setter-method to change a property is also available(not for count). But its not possible to change properties dynamically yet. You have to wait until new particles are created during the simulation to see the changes. I do not recommend to use it yet.

## Example: Star explosion
```C#
//declaration

private ParticleEmitter _starEmitter;     
private ShaderProgram _starTexture; 
private IShaderParam _starParam;
private ITexture _iStar;

//Init()
_starEmitter = new ParticleEmitter(0, 300, 300, 0.1f, 0.1f, 1.0f, 0.5f, 0.0f, 0.5f, 4.1f, 4.0f, 4.1f, 0.0f, 0.016f, 0.0f);
_starTexture      = RC.CreateShader(_starEmitter.VsSimpleTexture, _starEmitter.PsSimpleTexture);
_starParam         = _starTexture.GetShaderParam("texture1");
var imgStarData    = RC.LoadImage("Assets/star.png");
_iStar             = RC.CreateTexture(imgStarData);

//Remderaframe()
RC.SetShader(_starTexture);
RC.SetShaderParamTexture(_starParam, _iStar);
_starEmitter.Tick(Time.Instance.DeltaTime);//Method which do all the work

RC.Render(_starEmitter.ParticleMesh);
```
