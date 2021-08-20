using Fusee.Math.Core;
using Fusee.Xene;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fusee.Engine.Core.Scene
{
    /// <summary>
    /// Static extension methods on <see cref="SceneNode"/> and <see cref="SceneComponent"/> instances (or enumerations)
    /// for various purposes.
    /// </summary>
    public static class SceneExtensions
    {
        /// <summary>
        /// Finds all components matching a given search predicate within the given node.
        /// </summary>
        /// <remarks>
        /// Narrows the generic parameters of <see cref="Fusee.Xene.SceneFinderExtensions.FindComponents{TNode, TComponent}(TNode, Predicate{TComponent})"/> 
        /// to the concrete Types <see cref="SceneNode"/> and <see cref="SceneComponent"/>.
        /// </remarks>
        /// <param name="root">The root node where to start the traversal.</param>
        /// <param name="match">The matching predicate. Enumeration will yield on every matching node.</param>
        /// <returns>An enumerable that can be used in foreach statements.</returns>
        public static IEnumerable<SceneComponent> FindComponents(this SceneNode root, Predicate<SceneComponent> match)
        {
            return root.FindComponents<SceneNode, SceneComponent>(match);
        }

        /// <summary>
        /// Finds all components matching a given search predicate within the given list of nodes.
        /// </summary>
        /// <param name="roots"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static IEnumerable<SceneComponent> FindComponents(this IEnumerable<SceneNode> roots, Predicate<SceneComponent> match)
        {
            return roots.FindComponents<SceneNode, SceneComponent>(match);
        }

        /// <summary>
        /// Finds all components of a certain type matching a given search predicate within the given node.
        /// </summary>
        /// <remarks>
        /// Narrows the generic parameters of <see cref="Fusee.Xene.SceneFinderExtensions.FindComponents{TNode, TComponent}(TNode, Predicate{TComponent})"/> 
        /// to the concrete Types <see cref="SceneNode"/> and <see cref="SceneComponent"/>.
        /// </remarks>
        /// <param name="root">The root node where to start the traversal.</param>
        /// <param name="match">The matching predicate. Enumeration will yield on every matching node.</param>
        /// <returns>An enumerable that can be used in foreach statements.</returns>
        public static IEnumerable<TComponentToFind> FindComponents<TComponentToFind>(this SceneNode root, Predicate<TComponentToFind> match)
            where TComponentToFind : SceneComponent
        {
            return root.FindComponents<TComponentToFind, SceneNode, SceneComponent>(match);
        }

        /// <summary>
        /// Finds all components of a certain type matching a given search predicate within the given list of nodes.
        /// </summary>
        /// <param name="roots"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static IEnumerable<TComponentToFind> FindComponents<TComponentToFind>(this IEnumerable<SceneNode> roots, Predicate<TComponentToFind> match)
            where TComponentToFind : SceneComponent
        {
            return roots.FindComponents<TComponentToFind, SceneNode, SceneComponent>(match);
        }

        /// <summary>
        /// Finds all components matching a given search predicate within the given node.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static IEnumerable<SceneNode> FindNodesWhereComponent(this SceneNode root, Predicate<SceneComponent> match)
        {
            return root.FindNodesWhereComponent<SceneNode, SceneComponent>(match);
        }

        /// <summary>
        /// Finds all components matching a given search predicate within the given node.
        /// </summary>
        /// <param name="roots"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static IEnumerable<SceneNode> FindNodesWhereComponent(this IEnumerable<SceneNode> roots, Predicate<SceneComponent> match)
        {
            return roots.FindNodesWhereComponent<SceneNode, SceneComponent>(match);
        }

        /// <summary>
        /// Returns the global transformation matrix as the product of all transformations along the scene graph branch this SceneNode is a part of. 
        /// </summary>
        public static float4x4 GetGlobalTransformation(this SceneNode snThis)
        {
            var res = GetLocalTransformation(snThis.GetComponent<Transform>());
            if (snThis.Parent == null)
                return snThis.GetComponent<Transform>().Matrix;

            snThis.AccumulateGlobalTransform(ref res);
            return res;
        }

        /// <summary>
        /// Returns the global rotation matrix as the product of all rotations along the scene graph branch this SceneNode is a part of. 
        /// </summary>
        public static float4x4 GetGlobalRotation(this SceneNode snThis)
        {
            var transform = GetGlobalTransformation(snThis);
            return transform.RotationComponent();
        }

        /// <summary>
        /// Returns the global translation as the product of all translations along the scene graph branch this SceneNode is a part of. 
        /// </summary>
        public static float3 GetGlobalTranslation(this SceneNode snThis)
        {
            var transform = GetGlobalTransformation(snThis);
            return transform.Translation();
        }

        /// <summary>
        /// Returns the global scale as the product of all scaling along the scene graph branch this SceneNode is a part of. 
        /// </summary>
        public static float3 GetGlobalScale(this SceneNode snThis)
        {
            var transform = GetGlobalTransformation(snThis);
            return transform.Scale();
        }

        private static void AccumulateGlobalTransform(this SceneNode snThis, ref float4x4 res)
        {
            while (true)
            {
                if (snThis.Parent == null)
                {
                    return;
                }

                var tcp = snThis.Parent.GetComponent<Transform>();

                if (tcp == null)
                {
                    snThis = snThis.Parent;
                    continue;
                }

                res = GetLocalTransformation(tcp) * res;
                snThis = snThis.Parent;
            }
        }

        /// <summary>
        /// Get the local transformation matrix from this TransformationComponent. 
        /// </summary>
        public static float4x4 GetLocalTransformation(this Transform tansThis)
        {
            return tansThis == null ? float4x4.Identity : tansThis.TranslationMatrix;
        }

        /// <summary>
        /// Removes the components with the specified type and the sub-types in the children of this scene node container.
        /// </summary>
        /// <param name="snThisThis">This scene node container.</param>
        /// <param name="type">The type of the components to look for.</param>
        /// <returns>A List of components of the specified type, if contained within the given container.</returns>
        public static void RemoveComponentsInChildren(this SceneNode snThisThis, Type type)
        {
            if (snThisThis == null || type == null)
                throw new ArgumentException("SceneNode or type is null!");

            foreach (var child in snThisThis.Children)
            {
                for (var i = 0; i < child.Components.Count; i++)
                {
                    var comp = child.Components[i];
                    if (comp.GetType().IsAssignableFrom(type) || comp.GetType().IsSubclassOf(type))
                        child.Components.RemoveAt(i);
                }

                RemoveComponentsInChildren(child, type);
            }
        }

        /// <summary>
        /// Removes components with the specified type in the children of this scene node container.
        /// </summary>
        /// <typeparam name="TComp">The type of the components to look for.</typeparam>
        /// <param name="snThisThis">This scene node container.</param>
        /// <returns>A List of components of the specified type, if contained within the given container.</returns>
        public static void RemoveComponentsInChildren<TComp>(this SceneNode snThisThis)
            where TComp : SceneComponent
        {
            RemoveComponentsInChildren(snThisThis, typeof(TComp));
        }

        /// <summary>
        /// Finds the components with the specified type and the sub-types in the children of this scene node container.
        /// </summary>
        /// <param name="snThisThis">This scene node container.</param>
        /// <param name="type">The type of the components to look for.</param>
        /// <returns>A List of components of the specified type, if contained within the given container.</returns>
        public static IEnumerable<SceneComponent> GetComponentsInChildren(this SceneNode snThisThis, Type type)
        {
            if (snThisThis == null || type == null)
                throw new ArgumentException("SceneNode or type is null!");

            foreach (var child in snThisThis.Children)
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
        /// <param name="snThisThis">This scene node container.</param>
        /// <returns>A List of components of the specified type, if contained within the given container.</returns>
        public static IEnumerable<TComp> GetComponentsInChildren<TComp>(this SceneNode snThisThis)
            where TComp : SceneComponent
        {
            return GetComponentsInChildren(snThisThis, typeof(TComp)).Cast<TComp>();
        }

        /// <summary>
        /// Finds the component with the specified type in this scene node container.
        /// </summary>
        /// <param name="snThisThis">This scene node container.</param>
        /// <param name="type">The type of the component to look for.</param>
        /// <param name="inx">specifies the n'th component if more than component of the given type exists.</param>
        /// <returns>A component of the specified type, if contained within the given container, null otherwise.</returns>
        public static SceneComponent GetComponent(this SceneNode snThisThis, Type type, int inx = 0)
        {
            if (snThisThis == null || snThisThis.Components == null || type == null)
                return null;

            foreach (var comp in snThisThis.Components)
            {
                var inxC = 0;
                if (type.IsAssignableFrom(comp.GetType()))
                {
                    if (inxC == inx)
                        return comp;
                }
            }
            return null;
        }

        /// <summary>
        /// Removes the component with the specified type in this scene node container.
        /// </summary>
        /// <param name="snThisThis">This scene node container.</param>
        /// <param name="type">The type of the component to look for.</param>
        /// <param name="inx">specifies the n'th component if more than component of the given type exists.</param>
        /// <returns>A component of the specified type, if contained within the given container, null otherwise.</returns>
        public static void RemoveComponent(this SceneNode snThisThis, Type type, int inx = 0)
        {
            if (snThisThis == null || snThisThis.Components == null || type == null)
                throw new ArgumentException("SceneNode or type is null!");

            var inxC = 0;
            for (var i = 0; i < snThisThis.Components.Count; i++)
            {
                var cont = snThisThis.Components[i];

                if (cont.GetType().IsAssignableFrom(type))
                {
                    if (inxC == inx)
                        snThisThis.Components.RemoveAt(i);
                    inxC++;
                }
            }
        }

        /// <summary>
        /// Finds the components with the specified type in this scene node container.
        /// </summary>
        /// <param name="snThisThis">This scene node container.</param>       
        /// <returns>A component of the specified type, if contained within the given container, null otherwise.</returns>
        public static IEnumerable<TComp> GetComponents<TComp>(this SceneNode snThisThis) where TComp : SceneComponent
        {
            return GetComponents(snThisThis, typeof(TComp)).Cast<TComp>();
        }

        /// <summary>
        /// Finds the components with the specified type in this scene node container.
        /// </summary>
        /// <param name="snThisThis">This scene node container.</param>
        /// <param name="type">The type of the component to look for.</param>        
        public static IEnumerable<SceneComponent> GetComponents(this SceneNode snThisThis, Type type)
        {
            foreach (var cont in snThisThis.Components)
            {
                if (cont.GetType().IsAssignableFrom(type))
                {
                    yield return cont;
                }
            }
        }

        /// <summary>
        /// Finds the component with the specified type in this scene node container.
        /// </summary>
        /// <typeparam name="TComp">The type of the component to look for.</typeparam>
        /// <param name="snThisThis">This scene node container.</param>
        /// <param name="inx">specifies the n'th component if more than component of the given type exists.</param>
        /// <returns>A component of the specified type, if contained within this container, null otherwise.</returns>
        public static TComp GetComponent<TComp>(this SceneNode snThisThis, int inx = 0)
            where TComp : SceneComponent
        {
            return (TComp)GetComponent(snThisThis, typeof(TComp), inx);
        }

        /// <summary>
        /// Removes the component with the specified type in this scene node container.
        /// </summary>
        /// <typeparam name="TComp">The type of the component to look for.</typeparam>
        /// <param name="snThisThis">This scene node container.</param>
        /// <param name="inx">specifies the n'th component if more than component of the given type exists.</param>
        /// <returns>A component of the specified type, if contained within this container, null otherwise.</returns>
        public static void RemoveComponent<TComp>(this SceneNode snThisThis, int inx = 0)
            where TComp : SceneComponent
        {
            RemoveComponent(snThisThis, typeof(TComp), inx);
        }

        /// <summary>
        /// Shortcut for <code>GetComponent&lt;Mesh&gt;(snThisThis, inx);</code>. See <see cref="GetComponent{TComp}"/>.
        /// </summary>
        /// <param name="snThisThis">This scene node container.</param>
        /// <param name="inx">specifies the n'th component if more than component of the given type exists.</param>
        /// <returns>A mesh if contained within this container.</returns>
        public static Mesh GetMesh(this SceneNode snThisThis, int inx = 0)
        {
            return GetComponent<Mesh>(snThisThis, inx);
        }

        /// <summary>
        /// Shortcut for <code>GetComponent&lt;Light&gt;(snThisThis, inx);</code>. See <see cref="GetComponent{TComp}"/>.
        /// </summary>
        /// <param name="snThisThis">This scene node container.</param>
        /// <param name="inx">specifies the n'th component if more than component of the given type exists.</param>
        /// <returns>A light if contained within this container.</returns>
        public static Light GetLight(this SceneNode snThisThis, int inx = 0)
        {
            return GetComponent<Light>(snThisThis, inx);
        }

        /// <summary>
        /// Shortcut for <code>GetComponent&lt;Weight&gt;(snThisThis, inx);</code>. See <see cref="GetComponent{TComp}"/>.
        /// </summary>
        /// <param name="snThisThis">This scene node container.</param>
        /// <param name="inx">specifies the n'th component if more than component of the given type exists.</param>
        /// <returns>A weight component if contained within this container.</returns>
        public static Weight GetWeights(this SceneNode snThisThis, int inx = 0)
        {
            return GetComponent<Weight>(snThisThis, inx);
        }

        /// <summary>
        /// Shortcut for <code>GetComponent&lt;Transform&gt;(snThisThis, inx);</code>. See <see cref="GetComponent{TComp}"/>.
        /// </summary>
        /// <param name="snThisThis">This scene node container.</param>
        /// <param name="inx">specifies the n'th component if more than component of the given type exists.</param>
        /// <returns>A transform if contained within this container.</returns>
        public static Transform GetTransform(this SceneNode snThisThis, int inx = 0)
        {
            return GetComponent<Transform>(snThisThis, inx);
        }

        /// <summary>
        /// Adds the given component into this container's list of components.
        /// </summary>
        /// <param name="snThisThis">This node.</param>
        /// <param name="scc">The component to add.</param>
        public static void AddComponent(this SceneNode snThisThis, SceneComponent scc)
        {
            if (scc == null || snThisThis == null)
                return;
            (snThisThis.Components ??= new List<SceneComponent>()).Add(scc);
        }

        /// <summary>
        /// Converts the Scene to a SceneNode with a separate Transform
        /// </summary>
        /// <param name="sc">this node.</param>
        public static SceneNode ToSceneNode(this SceneContainer sc)
        {
            var snThis = new SceneNode();
            snThis.AddComponent(new Transform());

            foreach (var scc in sc.Children)
            {
                snThis.Children.Add(scc);
            }

            return snThis;
        }

        /// <summary>
        /// Converts the SceneNode to a Scene
        /// </summary>
        /// <param name="snThis">this node.</param>
        public static SceneContainer ToScene(this SceneNode snThis)
        {
            var sc = new SceneContainer();

            foreach (var snThisc in snThis.Children)
            {
                sc.Children.Add(snThisc);
            }

            return sc;
        }

        /// <summary>
        /// Translate this node.
        /// </summary>
        /// <param name="tc"></param>
        /// <param name="xyz">Translation amount as float3.</param>
        public static void Translate(this Transform tc, float3 xyz)
        {
            tc.Translation += xyz;
        }

        /// <summary>
        /// Translates a transform component with a specified float4x4 translation matrix
        /// </summary>
        /// <param name="tc"></param>
        /// <param name="translationMtx">Translation amount as represented in float4x4.</param>
        public static void Translate(this Transform tc, float4x4 translationMtx)
        {
            tc.TranslationMatrix *= translationMtx;
        }

        /// <summary>
        /// Rotates this node.
        /// </summary>
        /// <param name="tc"></param>
        /// <param name="xyz">Rotation amount as float3.</param>
        /// <param name="space">Rotation in reference to model or world space.</param>
        public static void Rotate(this Transform tc, float3 xyz, Space space = Space.Model)
        {
            Rotate(tc, float4x4.CreateRotationZXY(xyz), space);
        }

        /// <summary>
        /// Rotates this node.
        /// </summary>
        /// <param name="tc"></param>
        /// <param name="quaternion">Rotation amount in Quaternion.</param>
        /// <param name="space">Rotation in reference to model or world space.</param>
        public static void Rotate(this Transform tc, Quaternion quaternion, Space space = Space.Model)
        {
            Rotate(tc, Quaternion.QuaternionToMatrix(quaternion), space);
        }

        /// <summary>
        /// Roates around a given center and angle
        /// </summary>
        /// <param name="tc"></param>
        /// <param name="center"></param>
        /// <param name="angles"></param>
        public static void RotateAround(this Transform tc, float3 center, float3 angles)
        {
            var pos = tc.Translation;
            var addRotationMtx = float4x4.CreateRotationZXY(angles); // get the desired rotation
            var dir = pos - center; // find current direction relative to center
            dir = addRotationMtx * dir; // rotate the direction
            tc.Translation = center + dir; // define new position

            // rotate object to keep looking at the center:
            var currentRotationMtx = tc.RotationMatrix;
            var euler = float4x4.RotMatToEuler(currentRotationMtx);
            tc.RotationMatrix = addRotationMtx * float4x4.CreateFromAxisAngle(float4x4.Invert(currentRotationMtx) * float3.UnitY, euler.y) * float4x4.CreateFromAxisAngle(float4x4.Invert(currentRotationMtx) * float3.UnitX, euler.x) * float4x4.CreateFromAxisAngle(float4x4.Invert(currentRotationMtx) * float3.UnitZ, euler.z);
        }

        /// <summary>
        /// Rotates this node.
        /// </summary>
        /// <param name="tc"></param>
        /// <param name="rotationMtx">Rotation amount as represented in float4x4.</param>
        /// <param name="space">Rotation in reference to model or world space.</param>
        public static void Rotate(this Transform tc, float4x4 rotationMtx, Space space = Space.Model)
        {
            var addRotationMtx = rotationMtx.RotationComponent();

            if (space == Space.Model)
            {
                tc.RotationMatrix *= addRotationMtx;
            }
            else
            {
                var euler = float4x4.RotMatToEuler(tc.RotationMatrix);
                tc.RotationMatrix = addRotationMtx * float4x4.CreateFromAxisAngle(float4x4.Invert(tc.RotationMatrix) * float3.UnitY, euler.y) * float4x4.CreateFromAxisAngle(float4x4.Invert(tc.RotationMatrix) * float3.UnitX, euler.x) * float4x4.CreateFromAxisAngle(float4x4.Invert(tc.RotationMatrix) * float3.UnitZ, euler.z);
            }
        }

        /// <summary>
        /// Use this if the TransformComponent is part of a camera and you want to achieve a first person behavior.
        /// </summary>
        /// <param name="tc">This TransformComponent</param>
        /// <param name="angleHorz">The horizontal rotation angle in rad. Should probably come from Mouse input.</param>
        /// <param name="angleVert">The vertical rotation angle in rad. Should probably come from Mouse input.</param>
        /// <param name="inputWSAxis">The value we want to translate the camera when pressing the W or S key.</param>
        /// <param name="inputADAxis">The value we want to translate the camera when pressing the A or D key.</param>
        /// <param name="speed">Changes the speed of the camera movement.</param>
        public static void FpsView(this Transform tc, float angleHorz, float angleVert, float inputWSAxis, float inputADAxis, float speed)
        {
            if ((angleHorz >= M.TwoPi && angleHorz > 0f) || angleHorz <= -M.TwoPi)
                angleHorz %= M.TwoPi;
            if ((angleVert >= M.TwoPi && angleVert > 0f) || angleVert <= -M.TwoPi)
                angleVert %= M.TwoPi;

            var camForward = float4x4.CreateRotationXY(new float2(angleVert, angleHorz)) * float3.UnitZ;
            var camRight = float4x4.CreateRotationXY(new float2(angleVert, angleHorz)) * float3.UnitX;

            tc.Translation += camForward * inputWSAxis * speed;
            tc.Translation += camRight * inputADAxis * speed;

            tc.Rotation = new float3(angleVert, angleHorz, 0);
        }

        /// <summary>
        /// Reference space for rotation.
        /// </summary>
        public enum Space
        {
            /// <summary>
            /// World space
            /// </summary>
            World,
            /// <summary>
            /// Model space
            /// </summary>
            Model
        }
    }
}