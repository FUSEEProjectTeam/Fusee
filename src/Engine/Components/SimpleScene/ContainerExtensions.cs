using System;
using System.Collections.Generic;
using Fusee.Serialization;

namespace Fusee.Engine.SimpleScene
{
    static public class ContainerExtensions
    {
        public static SceneComponentContainer GetComponent(this SceneNodeContainer snc, Type type, int inx = 0)
        {
            if (snc == null || snc.Components == null || type == null)
                return null;

            foreach (var cont in snc.Components)
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

        public static SceneComponentContainer GetComponent<TComp>(this SceneNodeContainer snc, int inx = 0)
            where TComp : SceneComponentContainer
        {
            return GetComponent(snc, typeof (TComp), inx);
        }

        public static MeshComponent GetMesh(this SceneNodeContainer snc, int inx = 0)
        {
            return (MeshComponent) GetComponent<MeshComponent>(snc, inx);
        }

        public static MaterialComponent GetMaterial(this SceneNodeContainer snc, int inx = 0)
        {
            return (MaterialComponent)GetComponent<MaterialComponent>(snc, inx);
        }

        public static LightComponent GetLight(this SceneNodeContainer snc, int inx = 0)
        {
            return (LightComponent)GetComponent<LightComponent>(snc, inx);
        }
        public static WeightComponent GetWeights(this SceneNodeContainer snc, int inx = 0)
        {
            return (WeightComponent)GetComponent<WeightComponent>(snc, inx);
        }

        public static void AddComponent(this SceneNodeContainer snc, SceneComponentContainer scc)
        {
            if (scc == null || snc == null)
                return;

            if (snc.Components == null)
            {
                snc.Components = new List<SceneComponentContainer>();
            }
            snc.Components.Add(scc);
        }
    }
}
