using System;
using System.Collections.Generic;
using System.Linq;
using Fusee.Math.Core;
using Fusee.Serialization;


namespace Fusee.Xene
{
    /// <summary>
    /// Static quick-hack helpers to access components within nodes and get local and global transformation matrices.
    /// </summary>
    public static class ContainerExtensions
    {
        /// <summary>
        /// Calculates the appropriate projection matrix for the given projection method.
        /// </summary>
        /// <param name="pc">The projection component.</param>        
        public static float4x4 Matrix(this ProjectionComponent pc)
        {
            switch (pc.ProjectionMethod)
            {
                default:
                case ProjectionMethod.PERSPECTIVE:
                    var aspect = pc.Width / (float)pc.Height;
                    return float4x4.CreatePerspectiveFieldOfView(pc.Fov, aspect, pc.ZNear, pc.ZFar);
                case ProjectionMethod.ORTHOGRAPHIC:
                    return float4x4.CreateOrthographic(pc.Width, pc.Height, pc.ZNear, pc.ZFar);
            }
        }

        /// <summary>
        /// Calculates a transformation matrix from this transform component.
        /// </summary>
        /// <param name="tcThis">This transform component.</param>
        /// <returns>The transform component's translation, rotation and scale combined in a single matrix.</returns>
        public static float4x4 Matrix(this TransformComponent tcThis)
        {
            return float4x4.CreateTranslation(tcThis.Translation) * float4x4.CreateRotationY(tcThis.Rotation.y) *
                   float4x4.CreateRotationX(tcThis.Rotation.x) * float4x4.CreateRotationZ(tcThis.Rotation.z) *
                   float4x4.CreateScale(tcThis.Scale);
        }

        /// <summary>
        /// Returns the global transformation matrix as the product of all transformations along the scene graph branch this SceneNodeContainer is a part of. 
        /// </summary>
        public static float4x4 GetGlobalTransformation(this SceneNodeContainer snc)
        {
            var res = GetLocalTransformation(snc.GetComponent<TransformComponent>());
            if (snc.Parent == null)
                return snc.GetComponent<TransformComponent>().Matrix();

            snc.AccumulateGlobalTransform(ref res);
            return res;
        }

        /// <summary>
        /// Returns the global rotation matrix as the product of all rotations along the scene graph branch this SceneNodeContainer is a part of. 
        /// </summary>
        public static float4x4 GetGlobalRotation(this SceneNodeContainer snc)
        {
            var transform = GetGlobalTransformation(snc);
            return transform.RotationComponent();
        }

        /// <summary>
        /// Returns the global translation as the product of all translations along the scene graph branch this SceneNodeContainer is a part of. 
        /// </summary>
        public static float3 GetGlobalTranslation(this SceneNodeContainer snc)
        {
            var transform = GetGlobalTransformation(snc);
            return transform.Translation();
        }

        /// <summary>
        /// Returns the global scale as the product of all scaling along the scene graph branch this SceneNodeContainer is a part of. 
        /// </summary>
        public static float3 GetGlobalScale(this SceneNodeContainer snc)
        {
            var transform = GetGlobalTransformation(snc);
            return transform.Scale();
        }

        private static void AccumulateGlobalTransform(this SceneNodeContainer snc, ref float4x4 res)
        {
            while (true)
            {
                if (snc.Parent == null)
                {
                    return;
                }

                var tcp = snc.Parent.GetComponent<TransformComponent>();

                if (tcp == null)
                {
                    snc = snc.Parent;
                    continue;
                }

                res = GetLocalTransformation(tcp)* res;
                snc = snc.Parent;
            }
        }

        /// <summary>
        /// Get the local transformation matrix from this TransformationComponent. 
        /// </summary>
        public static float4x4 GetLocalTransformation(this TransformComponent tc)
        {
            return tc == null ? float4x4.Identity : tc.Matrix();
        }
        
        /// <summary>
        /// Returns the projection matrix of the next superordinate SceneNodeContainer that has a ProjectionComponent.
        /// </summary>
        public static float4x4 GetParentProjection(this SceneNodeContainer snc)
        {
            var res = float4x4.Identity;

            if (snc.Parent == null)
                return snc.GetComponent<ProjectionComponent>().Matrix();

            var parent = snc.Parent;
            while (true)
            {
                if (parent.Parent == null || res != float4x4.Identity)
                {
                    return res;
                }

                res = parent.GetComponent<ProjectionComponent>().Matrix();
                parent = parent.Parent;
            }
        }

        /// <summary>
        /// Finds the components with the specified type and the sub-types in the children of this scene node container.
        /// </summary>
        /// <param name="sncThis">This scene node container.</param>
        /// <param name="type">The type of the components to look for.</param>
        /// <returns>A List of components of the specified type, if contained within the given container.</returns>
        public static IEnumerable<SceneComponentContainer> GetComponentsInChildren(this SceneNodeContainer sncThis, Type type)
        {
            if (sncThis == null || type == null)
                throw new ArgumentException("SceneNodeContainer or type is null!");

            foreach (var child in sncThis.Children)
            {
                foreach (var comp in child.Components)
                {
                    if (comp.GetType().IsAssignableFrom(type) || comp.GetType().IsSubclassOf(type))
                        yield return comp;
                }

                foreach (var gChild in GetComponentsInChildren(child, type))
                    yield return gChild;
            }
        }

        /// <summary>
        /// Finds the components with the specified type in the children of this scene node container.
        /// </summary>
        /// <typeparam name="TComp">The type of the components to look for.</typeparam>
        /// <param name="sncThis">This scene node container.</param>
        /// <returns>A List of compontetns of the specified type, if contained within the given container.</returns>
        public static IEnumerable<TComp> GetComponentsInChildren<TComp>(this SceneNodeContainer sncThis)
            where TComp : SceneComponentContainer
        {
            return GetComponentsInChildren(sncThis, typeof(TComp)).Cast<TComp>();
        }

        /// <summary>
        /// Finds the component with the specified type in this scene node container.
        /// </summary>
        /// <param name="sncThis">This scene node container.</param>
        /// <param name="type">The type of the component to look for.</param>
        /// <param name="inx">specifies the n'th component if more than component of the given type exists.</param>
        /// <returns>A component of the specified type, if contained within the given container, null otherwise.</returns>
        public static SceneComponentContainer GetComponent(this SceneNodeContainer sncThis, Type type, int inx = 0)
        {
            if (sncThis == null || sncThis.Components == null || type == null)
                return null;

            foreach (var cont in sncThis.Components)
            {
                int inxC = 0;
                if (cont.GetType().IsAssignableFrom(type))
                {
                    if (inxC == inx)
                        return cont;
                    inxC++;
                }
            }
            return null;
        }

        /// <summary>
        /// Finds the component with the specified type in this scene node container.
        /// </summary>
        /// <typeparam name="TComp">The type of the component to look for.</typeparam>
        /// <param name="sncThis">This scene node container.</param>
        /// <param name="inx">specifies the n'th component if more than component of the given type exists.</param>
        /// <returns>A component of the specified type, if contained within this container, null otherwise.</returns>
        public static TComp GetComponent<TComp>(this SceneNodeContainer sncThis, int inx = 0)
            where TComp : SceneComponentContainer
        {
            return (TComp) GetComponent(sncThis, typeof (TComp), inx);
        }

        /// <summary>
        /// Shortcut for <code>GetComponent&lt;Mesh&gt;(sncThis, inx);</code>. See <see cref="GetComponent{TComp}"/>.
        /// </summary>
        /// <param name="sncThis">This scene node container.</param>
        /// <param name="inx">specifies the n'th component if more than component of the given type exists.</param>
        /// <returns>A mesh if contained within this container.</returns>
        public static Mesh GetMesh(this SceneNodeContainer sncThis, int inx = 0)
        {
            return GetComponent<Mesh>(sncThis, inx);
        }

        /// <summary>
        /// Shortcut for <code>GetComponent&lt;MaterialComponent&gt;(sncThis, inx);</code>. See <see cref="GetComponent{TComp}"/>.
        /// </summary>
        /// <param name="sncThis">This scene node container.</param>
        /// <param name="inx">specifies the n'th component if more than component of the given type exists.</param>
        /// <returns>A material if contained within this container.</returns>
        public static MaterialComponent GetMaterial(this SceneNodeContainer sncThis, int inx = 0)
        {
            return GetComponent<MaterialComponent>(sncThis, inx);
        }

        /// <summary>
        /// Shortcut for <code>GetComponent&lt;LightComponent&gt;(sncThis, inx);</code>. See <see cref="GetComponent{TComp}"/>.
        /// </summary>
        /// <param name="sncThis">This scene node container.</param>
        /// <param name="inx">specifies the n'th component if more than component of the given type exists.</param>
        /// <returns>A light if contained within this container.</returns>
        public static LightComponent GetLight(this SceneNodeContainer sncThis, int inx = 0)
        {
            return GetComponent<LightComponent>(sncThis, inx);
        }

        /// <summary>
        /// Shortcut for <code>GetComponent&lt;WeightComponent&gt;(sncThis, inx);</code>. See <see cref="GetComponent{TComp}"/>.
        /// </summary>
        /// <param name="sncThis">This scene node container.</param>
        /// <param name="inx">specifies the n'th component if more than component of the given type exists.</param>
        /// <returns>A weight component if contained within this container.</returns>
        public static WeightComponent GetWeights(this SceneNodeContainer sncThis, int inx = 0)
        {
            return GetComponent<WeightComponent>(sncThis, inx);
        }

        /// <summary>
        /// Shortcut for <code>GetComponent&lt;TransformComponent&gt;(sncThis, inx);</code>. See <see cref="GetComponent{TComp}"/>.
        /// </summary>
        /// <param name="sncThis">This scene node container.</param>
        /// <param name="inx">specifies the n'th component if more than component of the given type exists.</param>
        /// <returns>A transform if contained within this container.</returns>
        public static TransformComponent GetTransform(this SceneNodeContainer sncThis, int inx = 0)
        {
            return GetComponent<TransformComponent>(sncThis, inx);
        }

        /// <summary>
        /// Adds the given component into this container's list of components.
        /// </summary>
        /// <param name="sncThis">This node.</param>
        /// <param name="scc">The component to add.</param>
        public static void AddComponent(this SceneNodeContainer sncThis, SceneComponentContainer scc)
        {
            if (scc == null || sncThis == null)
                return;

            if (sncThis.Components == null)
            {
                sncThis.Components = new List<SceneComponentContainer>();
            }
            sncThis.Components.Add(scc);
        }

        /// <summary>
        /// Converts the SceneContainer to a SceneNodeContainer with a seperate TransformComponent
        /// </summary>
        /// <param name="sc">this node.</param>
        public static SceneNodeContainer ToSceneNodeContainer(this SceneContainer sc)
        {
            SceneNodeContainer snc = new SceneNodeContainer();
            snc.AddComponent(new TransformComponent());

            foreach (var scc in sc.Children)
            {
                snc.Children.Add(scc);
            }

            return snc;
        }

        /// <summary>
        /// Converts the SceneNodeContainer to a SceneContainer
        /// </summary>
        /// <param name="snc">this node.</param>
        public static SceneContainer ToSceneContainer(this SceneNodeContainer snc)
        {
            SceneContainer sc = new SceneContainer();

            foreach (var sncc in snc.Children)
            {
                sc.Children.Add(sncc);
            }

            return sc;
        }

        /// <summary>
        /// Rotates this node.
        /// </summary>
        /// <param name="tc"></param>
        /// <param name="xyz">Rotation amount as float3.</param>
        /// <param name="space"></param>
        public static void Rotate(this TransformComponent tc, float3 xyz, Space space = Space.Model)
        {
            Rotate(tc, float4x4.CreateRotationYXZ(xyz), space);
        }

        /// <summary>
        /// Rotates this node.
        /// </summary>
        /// <param name="tc"></param>
        /// <param name="quaternion">Rotation amount in Quaternion.</param>
        /// <param name="space"></param>
        public static void Rotate(this TransformComponent tc, Quaternion quaternion, Space space = Space.Model)
        {
            Rotate(tc, Quaternion.QuaternionToMatrix(quaternion), space);
        }

        /// <summary>
        /// Rotates this node.
        /// </summary>
        /// <param name="tc"></param>
        /// <param name="rotationMtx">Rotation amount represented as float4x4.</param>
        /// <param name="space">Rotation in reference of model or world space.</param>
        public static void Rotate(this TransformComponent tc, float4x4 rotationMtx, Space space = Space.Model)
        {
            var currentRotationMtx = float4x4.CreateRotationYXZ(tc.Rotation);
            var addRotationMtx = rotationMtx.RotationComponent();

            if (space == Space.Model)
            {
                tc.Rotation = float4x4.RotMatToEuler(currentRotationMtx * addRotationMtx);
            }
            else
            {
                var euler = float4x4.RotMatToEuler(currentRotationMtx);

                tc.Rotation = float4x4.RotMatToEuler(addRotationMtx * float4x4.CreateFromAxisAngle(float4x4.Invert(currentRotationMtx) * float3.UnitY, euler.y) * float4x4.CreateFromAxisAngle(float4x4.Invert(currentRotationMtx) * float3.UnitX, euler.x) * float4x4.CreateFromAxisAngle(float4x4.Invert(currentRotationMtx) * float3.UnitZ, euler.z));
            }
        }

        public enum Space
        {
            World,
            Model
        }
    }
}
