using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Engine;
using Fusee.Math;

namespace Fusee.SceneManagement
{
    class TraversalStateSearch : ITraversalState
    {
        private string _name;
        private SceneEntity _searchresult;
        private Input _input;
        private double _deltaTime;

        public void StoreMesh(Mesh mesh){}
        public void AddTransform(float4x4 mtx){}
        public void StoreRenderer(Renderer renderer){}
        public void Push(){}
        public void Pop(){}
        public void AddLightDirectional(float3 direction, float4 color, Light.LightType type){}
        public void AddLightPoint(float3 position, float4 color, Light.LightType type){}
        public void AddLightSpot(float3 position, float3 direction, float4 color, Light.LightType type){}
        public void SetDeltaTime(double delta){}       
        public void GetDeltaTime(out double deltaTime)
        {
            deltaTime = _deltaTime;
        }
        public Input Input
        {
            set { _input = value; }
            get
            {
                if (_input != null)
                {
                    return _input;
                }
                else
                {
                    return null;
                }
            }
        }
        // Polymorphic Visits
        public void Visit(ActionCode actionCode){}
        public void Visit(DirectionalLight directionalLight){}
        public void Visit(PointLight pointLight){}
        public void Visit(Renderer renderer){}
        public void Visit(SceneEntity sceneEntity)
        {
            if (sceneEntity.name == _name)
            {
                _searchresult = sceneEntity;
            }
        }
        public void Visit(SpotLight spotLight){}
        public void Visit(Transformation transformation){}
        public SceneEntity FindSceneEntity(List<SceneEntity> sceneEntities, string name)
        {
            _name = name;
                foreach (var sceneMember in sceneEntities)
                {
                    sceneMember.Traverse(this);
                    foreach (var child in sceneMember.GetChildren())
                    {
                        child.Traverse(this);
                    }
                }
                 return _searchresult;
          }

           
    }
}

        
    

