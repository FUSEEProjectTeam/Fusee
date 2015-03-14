namespace Fusee.Engine
{
    /// <summary>
    ///     Contains all pixel and vertex shaders and a method to create a ShaderProgram in Rendercontext.
    /// </summary>
    public static class MoreShaders
    {
        /// <summary>
        /// Creates a simple unlit texture shader in RenderContext.
        /// </summary>
        /// <param name="rc">RenderContext.</param>
        /// <returns>An instance of <see cref="ShaderProgram"/> to render a Texture without any lighting.</returns>
        public static ShaderProgram GetTextureShader(RenderContext rc)
        {
            var spSimple = rc.CreateShader(VsSimpleTexture, PsSimpleTexture);
            return spSimple;
        }

        /// <summary>
        /// Creates a simple diffuse texture shader in RenderContext.
        /// </summary>
        /// <param name="rc">RenderContext.</param>
        /// <returns>An instance of <see cref="ShaderProgram"/> to render a Texture with diffuse lighting.</returns>
        public static ShaderProgram GetDiffuseTextureShader(RenderContext rc)
        {
            var spSimple = rc.CreateShader(_vsDiffuse, _psDiffuse);
            return spSimple;
        }

        /// <summary>
        /// Creates a diffuse color shader in RenderContext.
        /// </summary>
        /// <param name="rc">RenderContext.</param>
        /// <returns>An instance of <see cref="ShaderProgram"/> to render a color with diffuse lighting.</returns>
        public static ShaderProgram GetDiffuseColorShader(RenderContext rc)
        {
            var spSimple = rc.CreateShader(_vsSimpleColor, _psSimpleColor);
            return spSimple;
        }


        /// <summary>
        /// Creates a specular texture shader in RenderContext.
        /// </summary>
        /// <param name="rc">RenderContext.</param>
        /// <returns>An instance of <see cref="ShaderProgram"/> to render a Texture with specular lighting.</returns>
        public static ShaderProgram GetSpecularShader(RenderContext rc)
        {
            var spSimple = rc.CreateShader(_vsSpecular, _psSpecular);
            return spSimple;
        }

        /// <summary>
        /// Creates a bumpmap and diffuse texture shader in RenderContext.
        /// </summary>
        /// <param name="rc">RenderContext.</param>
        /// <returns>An instance of <see cref="ShaderProgram"/> to render an object with diffuse lighting and a texture for the surface and another for the bump effect.</returns>
        public static ShaderProgram GetBumpDiffuseShader(RenderContext rc)
        {
            var spSimple = rc.CreateShader(_vsBump, _psBump);
            return spSimple;
        }

        private const string VsSimpleTexture = @"
            #ifdef GL_ES
                precision mediump float;
            #endif

            attribute vec3 fuVertex;
            attribute vec3 fuNormal;
            attribute vec2 fuUV;

            varying vec3 vNormal;
            varying vec2 vUV;

            uniform mat4 FUSEE_MVP;
            uniform mat4 FUSEE_ITMV;

            void main(){
                vUV = fuUV;
                gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
                vNormal = mat3(FUSEE_ITMV[0].xyz, FUSEE_ITMV[1].xyz, FUSEE_ITMV[2].xyz) * fuNormal;
            }";

        private const string PsSimpleTexture = @"
            #ifdef GL_ES
                precision mediump float;
            #endif

            uniform sampler2D texture1;
            varying vec3 vNormal;
            varying vec2 vUV;

            void main(){
                gl_FragColor = max(dot(vec3(0,0,-1),normalize(vNormal)), 0.2) * texture2D(texture1, vUV);
            }";

        private const string _vsDiffuse = @"
            attribute vec4 fuColor;
            attribute vec3 fuVertex;
            attribute vec3 fuNormal;
            attribute vec2 fuUV;
       
            uniform mat4 FUSEE_MVP;
            uniform mat4 FUSEE_MV;

            varying vec2 vUV;
            varying vec3 vNormal;
            varying vec3 vViewPos;

            vec3 vPos;
 
            void main()
            {
                vUV = fuUV;

                vec4 vViewPosTemp = FUSEE_MV * vec4(fuVertex, 1);
                vViewPos = vec3(vViewPosTemp)/vViewPosTemp.w;      

                vNormal = normalize(mat3(FUSEE_MV[0].xyz, FUSEE_MV[1].xyz, FUSEE_MV[2].xyz) * fuNormal);

                gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
            }";

        private const string _psDiffuse = @"
            #ifdef GL_ES
                precision highp float;
            #endif

            #define LIGHT_COUNT 8

            struct FUSEE_LIGHT
            {
                float active;
                vec4 diffuse;
                vec4 ambient;
                vec3 position;
                vec3 direction;
                float spotAngle;
            } uniform FUSEE_LIGHTS[LIGHT_COUNT];

            uniform mat4 FUSEE_V;
            uniform sampler2D texture1;

            varying vec3 vNormal;
            varying vec2 vUV;
            varying vec3 vViewPos;

            void CalcDirectLight(vec4 difColor, vec4 ambColor, vec3 direction, inout vec4 intensity) {
                intensity += ambColor;
                intensity += max(dot(-normalize(direction), normalize(vNormal)), 0.0) * difColor;
            }

            void CalcPointLight(vec4 difColor, vec4 ambColor, vec3 position, inout vec4 intensity) {
                intensity += ambColor;

                vec3 pos = position - vViewPos;
                intensity += max(dot(normalize(pos), normalize(vNormal)), 0.0) * difColor;   
            }

            void CalcSpotLight(vec4 difColor, vec4 ambColor, vec3 position, vec3 direction, float angle, inout vec4 intensity) {
                intensity += ambColor;

                vec3 pos = position - vViewPos;
                float alpha = acos(dot(normalize(pos), normalize(-direction)));

                if (alpha < angle)
                    intensity += max(dot(normalize(pos), normalize(vNormal)), 0.0) * difColor;   
            }
 
            void main()
            {
                vec4 endInt = vec4(0, 0, 0, 0);

                for (int i = 0; i < LIGHT_COUNT; i++) {
                    vec4 diffuse = FUSEE_LIGHTS[i].diffuse;
                    vec4 ambient = FUSEE_LIGHTS[i].ambient;
                    vec3 position = FUSEE_LIGHTS[i].position;
                    vec3 direction = FUSEE_LIGHTS[i].direction;
                    float spotAngle = FUSEE_LIGHTS[i].spotAngle;

                    if (FUSEE_LIGHTS[i].active == 1.0)
                        CalcDirectLight(diffuse, ambient, direction, endInt);

                    if (FUSEE_LIGHTS[i].active == 2.0)
                        CalcPointLight(diffuse, ambient, position, endInt);

                    if (FUSEE_LIGHTS[i].active == 3.0)
                        CalcSpotLight(diffuse, ambient, position, direction, spotAngle, endInt);
                }

                gl_FragColor = texture2D(texture1, vUV) * endInt; 
            }";

        private const string _vsSpecular = @"
            attribute vec3 fuVertex;
            attribute vec3 fuNormal;
            attribute vec2 fuUV;
                  
            varying vec3 vNormal;
            varying vec2 vUV;
            varying vec3 vViewPos;

            uniform mat4 FUSEE_MV; 
            uniform mat4 FUSEE_MVP;

            void main()
            {
                vUV = fuUV;

                vec4 vViewTemp = FUSEE_MV * vec4(fuVertex, 1);
                vViewPos = vec3(vViewTemp)/vViewTemp.w;

                vNormal = normalize(mat3(FUSEE_MV[0].xyz, FUSEE_MV[1].xyz, FUSEE_MV[2].xyz) * fuNormal);

                gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
            }";

        private const string _psSpecular = @"
            #ifdef GL_ES
                precision highp float;
            #endif

            #define LIGHT_COUNT 8

            struct FUSEE_LIGHT
            {
                float active;
                vec4 diffuse;
                vec4 ambient;
                vec4 specular;
                vec3 position;
                vec3 direction;
                float spotAngle;
            } uniform FUSEE_LIGHTS[LIGHT_COUNT];

            uniform mat4 FUSEE_V;
            uniform sampler2D texture1;
            uniform float specularLevel;
            uniform float shininess;

            varying vec3 vNormal;
            varying vec2 vUV;
            varying vec3 vViewPos;

            void CalcDirectLight(vec4 difColor, vec4 ambColor, vec4 specColor, vec3 direction, inout vec4 intensity) {
                intensity += ambColor;
                intensity += max(dot(-normalize(direction), normalize(vNormal)), 0.0) * difColor;

                if (specularLevel != 0.0){
                    vec3 lightVector = normalize(direction);
                    vec3 r = normalize(reflect(lightVector, normalize(vNormal)));
                    float s = pow(max(dot(r, vec3(0, 0, 1.0)), 0.0), specularLevel) * shininess;

                    intensity += specColor * s;
                }
            }

            void CalcPointLight(vec4 difColor, vec4 ambColor, vec4 specColor, vec3 position, inout vec4 intensity) {
                intensity += ambColor;

                vec3 pos = position - vViewPos;
                intensity += max(dot(normalize(pos), normalize(vNormal)), 0.0) * difColor;

                if (specularLevel != 0.0){
                    vec3 lightVector = normalize(-pos);
                    vec3 r = normalize(reflect(lightVector, normalize(vNormal)));
                    float s = pow(max(dot(r, vec3(0,0,1.0)), 0.0), specularLevel) * shininess;

                    intensity += specColor * s;
                }
            }

            void CalcSpotLight(vec4 difColor, vec4 ambColor, vec4 specColor, vec3 position, vec3 direction, float angle, inout vec4 intensity){
                intensity += ambColor;

                vec3 pos = position - vViewPos;
                float alpha = dot(normalize(pos), normalize(-direction));

                if (alpha > angle){
                    intensity += max(dot(normalize(pos), normalize(vNormal)), 0.0) * difColor;

                    if (specularLevel != 0.0){
                        vec3 lightVector = normalize(-pos);  
                        vec3 r = normalize(reflect(lightVector, normalize(vNormal)));

                        float s = pow(max(dot(r, vec3(0,0,1.0)), 0.0), specularLevel) * shininess;
                        intensity += specColor * s; 
                    }
                }
            }

            void main()
            {              
                vec4 endInt = vec4(0, 0, 0, 0);

                for (int i = 0; i < LIGHT_COUNT; i++) {
                    vec4 diffuse = FUSEE_LIGHTS[i].diffuse;
                    vec4 ambient = FUSEE_LIGHTS[i].ambient;
                    vec4 specular = FUSEE_LIGHTS[i].specular;
                    vec3 position = FUSEE_LIGHTS[i].position;
                    vec3 direction = FUSEE_LIGHTS[i].direction;
                    float spotAngle = FUSEE_LIGHTS[i].spotAngle;

                    if (FUSEE_LIGHTS[i].active == 1.0)
                        CalcDirectLight(diffuse, ambient, specular, direction, endInt);

                    if (FUSEE_LIGHTS[i].active == 2.0)
                        CalcPointLight(diffuse, ambient, specular, position, endInt);

                    if (FUSEE_LIGHTS[i].active == 3.0)
                        CalcSpotLight(diffuse, ambient, specular, position, direction, spotAngle, endInt);
                }

                gl_FragColor = texture2D(texture1, vUV) * endInt;
            }";

        private const string _vsBump = @"
            attribute vec3 fuVertex;
            attribute vec3 fuNormal;
            attribute vec2 fuUV;
                  
            varying vec3 vNormal;
            varying vec2 vUV;
            varying vec3 vViewPos;
    
            uniform mat4 FUSEE_MV; 
            uniform mat4 FUSEE_MVP;

            void main()
            {
                vUV = fuUV;

                vec4 vViewTemp = FUSEE_MV * vec4(fuVertex, 1);
                vViewPos = vec3(vViewTemp) / vViewTemp.w;

                vNormal = normalize(mat3(FUSEE_MV[0].xyz, FUSEE_MV[1].xyz, FUSEE_MV[2].xyz) * fuNormal);

                gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
            }";

        private const string _psBump = @"
            /* Copies incoming fragment color without change. */
            #ifdef GL_ES
                precision highp float;
            #endif

            #define LIGHT_COUNT 8

            struct FUSEE_LIGHT
            {
                float active;
                vec4 diffuse;
                vec4 ambient;
                vec4 specular;
                vec3 position;
                vec3 direction;
                float spotAngle;
            } uniform FUSEE_LIGHTS[LIGHT_COUNT];
    
            uniform mat4 FUSEE_V;
            uniform mat4 FUSEE_MV;
         
            uniform sampler2D texture1;
            uniform sampler2D normalTex;
            uniform float shininess;
            uniform float specularLevel;

            varying vec3 vNormal;
            varying vec2 vUV;
            varying vec3 vViewPos;

            void CalcDirectLight(vec4 difColor, vec4 ambColor, vec4 specColor, vec3 direction, inout vec4 intensity) {
                float maxVariance = 100.0; // Parameter for Bump Intensity
                float minVariance = maxVariance / 2.0;

                vec3 bumpNormal = vNormal + normalize(texture2D(normalTex, vUV).rgb * maxVariance - minVariance);

                intensity += ambColor;
                intensity += max(dot(-normalize(direction), normalize(bumpNormal)), 0.0) * difColor;

                if (specularLevel != 0.0){
                    vec3 lightVector = normalize(direction);
                    vec3 r = normalize(reflect(lightVector, normalize(bumpNormal)));
                    float s = pow(max(dot(r, vec3(0, 0, 1.0)), 0.0), specularLevel) * shininess;

                    intensity += specColor * s;
                }
            }

            void CalcPointLight(vec4 difColor, vec4 ambColor, vec4 specColor, vec3 position, inout vec4 intensity) {
                float maxVariance = 100.0; // Parameter for Bump Intensity
                float minVariance = maxVariance / 2.0;

                vec3 bumpNormal = vNormal + normalize(texture2D(normalTex, vUV).rgb * maxVariance - minVariance);

                intensity += ambColor;

                vec3 pos = position - vViewPos;
                intensity += max(dot(normalize(pos), normalize(bumpNormal)), 0.0) * difColor;

                if (specularLevel != 0.0){
                    vec3 lightVector = normalize(-pos);  
                    vec3 r = normalize(reflect(lightVector, normalize(bumpNormal)));
                    float s = pow(max(dot(r, vec3(0, 0, 1.0)), 0.0), specularLevel) * shininess;

                    intensity += specColor * s;
                }
            }

            void CalcSpotLight(vec4 difColor, vec4 ambColor, vec4 specColor, vec3 position, vec3 direction, float angle, inout vec4 intensity){
                float maxVariance = 100.0; // Parameter for Bump Intensity
                float minVariance = maxVariance / 2.0;

                vec3 bumpNormal = vNormal + normalize(texture2D(normalTex, vUV).rgb * maxVariance - minVariance);

                intensity += ambColor;

                vec3 pos = position - vViewPos;
                float alpha = acos(dot(normalize(pos), normalize(-direction)));

                if (alpha < angle){
                    intensity += max(dot(normalize(pos), normalize(bumpNormal)), 0.0) * difColor;

                    if (specularLevel != 0.0){
                        vec3 lightVector = normalize(-pos);  
                        vec3 r = normalize(reflect(lightVector, normalize(bumpNormal)));
                        float s = pow(max(dot(r, vec3(0, 0, 1.0)), 0.0), specularLevel) * shininess;

                        intensity += specColor * s;
                    }
                }
            }

            void main()
            {
                vec4 endInt = vec4(0, 0, 0, 0);

                 for (int i = 0; i < LIGHT_COUNT; i++) {
                    vec4 diffuse = FUSEE_LIGHTS[i].diffuse;
                    vec4 ambient = FUSEE_LIGHTS[i].ambient;
                    vec4 specular = FUSEE_LIGHTS[i].specular;
                    vec3 position = FUSEE_LIGHTS[i].position;
                    vec3 direction = FUSEE_LIGHTS[i].direction;
                    float spotAngle = FUSEE_LIGHTS[i].spotAngle;

                    if (FUSEE_LIGHTS[i].active == 1.0)
                        CalcDirectLight(diffuse, ambient, specular, direction, endInt);

                    if (FUSEE_LIGHTS[i].active == 2.0)
                        CalcPointLight(diffuse, ambient, specular, position, endInt);

                    if (FUSEE_LIGHTS[i].active == 3.0)
                        CalcSpotLight(diffuse, ambient, specular, position, direction, spotAngle, endInt);
                }

                gl_FragColor = texture2D(texture1, vUV) * endInt; 
            }";

        private const string _vsSimpleColor = @"
            attribute vec3 fuVertex;
            attribute vec3 fuNormal;       
        
            varying vec3 vNormal;
        
            uniform mat4 FUSEE_MVP;
            uniform mat4 FUSEE_ITMV;

            void main()
            {
                gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
                vNormal = mat3(FUSEE_ITMV[0].xyz, FUSEE_ITMV[1].xyz, FUSEE_ITMV[2].xyz) * fuNormal;
            }";

        private const string _psSimpleColor = @"
            #ifdef GL_ES
                precision highp float;
            #endif    
  
            uniform vec4 color;
            varying vec3 vNormal;

            void main()
            {             
                gl_FragColor = max(dot(vec3(0,0,-1),normalize(vNormal)), 0.1) * color;
            }";
    }
}