using System.Collections.Generic;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Serialization;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class ShaderEffectManager
    {
        private readonly IRenderContextImp _rci;

        private Stack<ShaderEffect> _shaderEffectsToDelete = new Stack<ShaderEffect>();

        private Dictionary<Suid, ShaderEffect> _allShaderEffects = new Dictionary<Suid, ShaderEffect>();

        private void Remove(IMeshImp meshImp)
        {
            if (meshImp.VerticesSet)
                _rci.RemoveVertices(meshImp);

            if (meshImp.NormalsSet)
                _rci.RemoveNormals(meshImp);

            if (meshImp.ColorsSet)
                _rci.RemoveColors(meshImp);

            if (meshImp.UVsSet)
                _rci.RemoveUVs(meshImp);

            if (meshImp.TrianglesSet)
                _rci.RemoveTriangles(meshImp);

            if (meshImp.BoneWeightsSet)
                _rci.RemoveBoneWeights(meshImp);

            if (meshImp.BoneIndicesSet)
                _rci.RemoveBoneIndices(meshImp);
        }

        private void ShaderEffectChanged(object sender, ShaderEffectChangedArgs args)
        {
         
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ShaderEffect GetShaderEffect(Suid id)
        {
            return _allShaderEffects.TryGetValue(id, out var returnEffect) ? returnEffect : null;
        }

        public void RegisterNewShaderEffect(ShaderEffect ef)
        {
            // Register new ShaderEffect in RC

            // Add ShaderEffect to dictonary
            _allShaderEffects.Add(ef.SessionUniqueIdentifier, ef);

            // Setup handler to observe changes of the shadereffect and dispose event (deallocation)
            ef.ShaderEffectChanged += ShaderEffectChanged;
        }

        /// <summary>
        /// Creates a new Instance of MeshManager. Th instance is handling the memory allocation and deallocation on the GPU by observing Mesh.cs objects.
        /// </summary>
        /// <param name="rci">The RenderContextImp is used for GPU memory allocation and deallocation. See RegisterMesh.</param>
        public ShaderEffectManager(IRenderContextImp rci)
        {
            _rci = rci;
        }


        /// <summary>
        /// Call this method on the mainthread after RenderContext.Render in order to cleanup all not used Buffers from GPU memory.
        /// </summary>
        public void Cleanup()
        {

            Diagnostics.Log("Cleanup");

        }
    }
}