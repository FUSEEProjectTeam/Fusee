using System;
using System.Diagnostics;
using Fusee.Engine;
namespace Fusee.SceneManagement
{
    public class Material
    {
        private bool test;
        protected ImageData imgData;
        protected ImageData imgData2;
        protected ITexture iTex;
        protected ITexture iTex2;
        protected IShaderParam _vColorParam;
        protected IShaderParam _texture1Param;
        public ShaderProgram sp;
        public string _vs = @"
            //#ifndef GL_ES
              // #version 120
            //#endif

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
        
            uniform mat4 FUSEE_MVP;
            uniform mat4 FUSEE_ITMV;

            void main()
            {
                gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
                // vColor = vec4(fuNormal * 0.5 + 0.5, 1.0);
                // vec4 norm4 = FUSEE_MVP * vec4(fuNormal, 0.0);
                // vNormal = norm4.xyz;
                vNormal = mat3(FUSEE_ITMV) * fuNormal;
                vUV = fuUV;
            }";

        public string _ps = @"
           //#ifndef GL_ES
             //  #version 120
            //#endif

            /* Copies incoming fragment color without change. */
            #ifdef GL_ES
                precision highp float;
            #endif

         
            uniform sampler2D texture1;
            uniform vec4 vColor;
            varying vec3 vNormal;
            varying vec2 vUV;

            void main()
            {             
                gl_FragColor = texture2D(texture1, vUV);
            }";

        public void InitValues(RenderContext rc)
        {
            _vColorParam = sp.GetShaderParam("vColor");
            _texture1Param = sp.GetShaderParam("texture1");
            imgData = rc.LoadImage("Assets/earth_texture512x256.png");
            iTex = rc.CreateTexture(imgData);
            //imgData2 = rc.LoadImage("Assets/planet.jpg");
            //iTex2 = rc.CreateTexture(imgData2);
            rc.SetShaderParamTexture(_texture1Param, iTex);
            Console.WriteLine("Initvalues got called");
        }

        /*public ImageData GetTexture(string texturename)
        {
            
        }*/


        /*public void UpdateValues(RenderContext rc)
        {
            //if(rc.GetShaderParam(sp,"texture1") != _texture1Param)
            if(!test)
            {
                rc.SetShaderParamTexture(_texture1Param, iTex);  
            }else
            {
                rc.SetShaderParamTexture(_texture1Param, iTex2);
            }
            
        }*/

        public void SwitchTexture()
        {
            test = true;

        }






    }
}
