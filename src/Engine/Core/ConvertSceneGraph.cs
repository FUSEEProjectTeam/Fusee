using System.Collections.Generic;
using Fusee.Jometri;
using Fusee.Serialization;
using Fusee.Xene;
using Fusee.Math.Core;
using System.Linq;
using System;
using Fusee.Engine.Core.ShaderShards;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Use ConVSceneToHighLevel to create new high level graph from a low level graph (made out of scene nodes and components) in order
    /// to have each visited element converted and/or split into its high level, render-ready components.
    /// </summary>
    public class ConvertSceneGraph : SceneVisitor
    {
        private SceneContainer _convertedScene;        
        private Stack<SceneNodeContainer> _predecessors;
        private SceneNodeContainer _currentNode;

        private Dictionary<MaterialComponent, ShaderEffect> _matMap;
        private Dictionary<MaterialPBRComponent, ShaderEffect> _pbrComponent;
        private Stack<SceneNodeContainer> _boneContainers;

        private List<Tuple<SceneNodeContainer, LightResult>> _lightViseratorResults;

        //private IEnumerable<System.Type> _codeComponentSubClasses;

        //public ConvertSceneGraph()
        //{
        //  _codeComponentSubClasses = 
        //    Assembly.GetExecutingAssembly()
        //    .GetTypes()
        //    .Where(t => t.IsSubclassOf(typeof(CodeComponent)));
        //}

        /// <summary>
        /// Method is called when going up one hierarchy level while traversing. Override this method to perform pop on any self-defined state.
        /// </summary>
        protected override void PopState()
        {
            _predecessors.Pop();
        }

        /// <summary>
        /// Traverses the given SceneContainer and creates new high level graph by converting and/or splitting its components into the high level equivalents.
        /// </summary>
        /// <param name="sc">The SceneContainer to convert.</param>
        /// <returns></returns>
        public SceneContainer Convert(SceneContainer sc)
        { 
            _predecessors = new Stack<SceneNodeContainer>();
            _convertedScene = new SceneContainer();
                        
            _matMap = new Dictionary<MaterialComponent, ShaderEffect>();            
            _pbrComponent = new Dictionary<MaterialPBRComponent, ShaderEffect>();
            _boneContainers = new Stack<SceneNodeContainer>();

            Traverse(sc.Children);            
            
            return _convertedScene;
        } 
        
        #region Visitors
        /// <summary>
        /// Converts the scene node container.
        /// </summary>
        /// <param name="snc"></param>
        [VisitMethod]
        public void ConvSceneNodeContainer(SceneNodeContainer snc)
        {
            if (_predecessors.Count != 0)
            {
                var parent = _predecessors.Peek();

                if (parent.Children == null)
                    parent.Children = new ChildList();

                _currentNode = new SceneNodeContainer { Name = snc.Name };
                parent.Children.Add(_currentNode);
                _predecessors.Push(_currentNode);
            }
            else //Add first node to SceneContainer
            {
                _predecessors.Push(new SceneNodeContainer { Name = CurrentNode.Name });
                _currentNode = _predecessors.Peek();
                if(_convertedScene.Children != null)
                    _convertedScene.Children.Add(_currentNode);
                else
                    _convertedScene.Children = new List<SceneNodeContainer> { _currentNode };
            }

            ////WIP!
            ////If the SceneNodeContainers' name contains the name of some CodeComponent subclass,
            ////create CodeComponent of this type and add it to the currentNode
            //foreach (var type in _codeComponentSubClasses)
            //{
            //    if (!snc.Name.Contains(type.ToString())) continue;
                
            //    var codeComp = Activator.CreateInstance(type);
            //    //_currentNode.AddComponent(codeComp);
            //}
        }

        ///<summary>
        ///Converts the transform component.
        ///</summary>
        [VisitMethod]        
        public void ConvTransform(TransformComponent transform)
        {
            if (_currentNode.Components == null)
                _currentNode.Components = new List<SceneComponentContainer>();

            _currentNode.Components.Add(transform);
        }

        /// <summary>
        /// Converts the material.
        /// </summary>
        /// <param name="matComp"></param>
        [VisitMethod]
        public void ConvMaterial(MaterialComponent matComp)
        {
            var effect = LookupMaterial(matComp);
            _currentNode.Components.Add(new ShaderEffectComponent{Effect = effect});
        }

        /// <summary>
        /// Converts the physically based rendering component
        /// </summary>
        /// <param name="matComp"></param>
        [VisitMethod]
        public void ConvMaterial(MaterialPBRComponent matComp)
        {
            var effect = LookupMaterial(matComp);
            _currentNode.Components.Add(new ShaderEffectComponent { Effect = effect });
        }        

        /// <summary>
        /// Converts the shader.
        /// </summary>
        [VisitMethod]
        public void ConvCamComp(CameraComponent cc)
        {
            _currentNode.Components.Add(cc);
        }

        /// <summary>
        /// Converts the mesh.
        /// </summary>
        /// <param name="mesh">The mesh to convert.</param>
        [VisitMethod]
        public void ConvMesh(Mesh mesh)
        {
            if (_currentNode.Components == null)
                _currentNode.Components = new List<SceneComponentContainer>();

            var currentNodeEffect = _currentNode.GetComponent<ShaderEffectComponent>();

            if (currentNodeEffect?.Effect.GetEffectParam(UniformNameDeclarations.BumpTextureName) != null)
            {
                mesh.Tangents = mesh.CalculateTangents();
                mesh.BiTangents = mesh.CalculateBiTangents();
            }

            _currentNode.Components.Add(mesh);
        }

        /// <summary>
        /// Adds the light component.
        /// </summary>
        /// <param name="lightComponent"></param>
        [VisitMethod]
        public void ConvLight(LightComponent lightComponent)
        {
            _currentNode.Components.Add(lightComponent);
        }

        /// <summary>
        /// Adds the bone component.
        /// </summary>
        /// <param name="bone"></param>
        [VisitMethod]
        public void ConvBone(BoneComponent bone)
        {
            if (_currentNode.Components == null)
                _currentNode.Components = new List<SceneComponentContainer>();

            _currentNode.Components.Add(bone);

            // Collect all bones, later, when a WeightComponent is found, we can set all Joints
            _boneContainers.Push(_currentNode);
        }

        /// <summary>
        /// Converts the weight component.
        /// </summary>
        /// <param name="weight"></param>
        [VisitMethod]
        public void ConVWeight(WeightComponent weight)
        {
            // check if we have bones
            if (_boneContainers.Count >= 1)
            {                
                if(weight.Joints == null) // initialize joint container
                    weight.Joints = new List<SceneNodeContainer>();

                // set all bones found until this WeightComponent
                while (_boneContainers.Count != 0)
                    weight.Joints.Add(_boneContainers.Pop());
            }
           
            _currentNode.Components.Add(weight);
        }
        #endregion

        #region Make ShaderEffect

        private ShaderEffect LookupMaterial(MaterialComponent mc)
        {
            if (_matMap.TryGetValue(mc, out var mat)) return mat;
            mat = ShaderCodeBuilder.MakeShaderEffectFromMatCompProto(mc, _currentNode.GetWeights()); // <- broken
            _matMap.Add(mc, mat);
            return mat;
        }

        private ShaderEffect LookupMaterial(MaterialPBRComponent mc)
        {
            if (_pbrComponent.TryGetValue(mc, out var mat)) return mat;
            mat = ShaderCodeBuilder.MakeShaderEffectFromMatCompProto(mc, _currentNode.GetWeights());
            _pbrComponent.Add(mc, mat);
            return mat;
        }

        #endregion
    }
}
