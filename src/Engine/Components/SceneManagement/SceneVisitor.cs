using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;


namespace Fusee.SceneManagement
{

    /// <summary>
    /// The SceneVisitor is the base class for Scene traversing functions
    /// </summary>
    public class SceneVisitor
    {
        // private delegate void VisitorMethod(Component c);

        // Implementation notice: Our visitor scheme relys on the DoubleDispatch mechanism.
        // <DoubleDispatch>


        // An adapter object builds the bridge between 
        /*protected class Adapter<TVisitor, TComponent>
            where TVisitor : SceneVisitor
            where TComponent : Component
        {
            public readonly ComponentVisitor<TVisitor, TComponent> Method;

            public Adapter(ComponentVisitor<TVisitor, TComponent> method)
            {
                Method = method;
            }

            public void Visit(SceneVisitor @this, Component component)
            {
                Method((TVisitor) @this, (TComponent) component);
            }
        }

        public delegate void ComponentVisitor(SceneVisitor @this, Component component);

        public delegate void ComponentVisitor<TVisitor, TComponent>(TVisitor @this, TComponent component)
            where TVisitor : SceneVisitor
            where TComponent : Component;

        private static Dictionary<Type, Dictionary<Type, ComponentVisitor>> _ddVisitorCaches;
        private Dictionary<Type, ComponentVisitor> _ddMethods;


        // </DoubleDispatch>
        protected void PrepareDoubleDispatch()
        {
            if (_ddMethods == null)
            {
                if (_ddVisitorCaches == null)
                    _ddVisitorCaches = new Dictionary<Type, Dictionary<Type, ComponentVisitor>>();

                if (!_ddVisitorCaches.TryGetValue(this.GetType(), out _ddMethods))
                {
                    _ddMethods = new Dictionary<Type, ComponentVisitor>();
                    foreach (var m in GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                    {
                        if (m.Name != "VisitComponent")
                            continue;

                        var parameters = m.GetParameters();
                        if (parameters.Length != 1)
                            continue;

                        var componentType = parameters[0].ParameterType;

                        _ddMethods.Add(componentType, MakeVisitorAdapter(m, GetType(), componentType));
                    }
                    _ddVisitorCaches.Add(this.GetType(), _ddMethods);
                }
            }
        }

        protected static ComponentVisitor MakeVisitorAdapter(MethodInfo method, Type visitorType, Type nodeType)
        {
            var tAdapterUnbound = typeof (Adapter<,>);
            var tAdapter = tAdapterUnbound.MakeGenericType(visitorType, nodeType);
            var tVisitorMethodUnbound = typeof (ComponentVisitor<,>);
            var tVisitorMethod = tVisitorMethodUnbound.MakeGenericType(visitorType, nodeType);
            var tAdapterMethod = typeof (ComponentVisitor);

            var visitorMethod = Delegate.CreateDelegate(tVisitorMethod, method, true);

            var adapter = tAdapter.GetConstructor(new[]
                                                      {
                                                          tVisitorMethod
                                                      }).Invoke(new object[] {visitorMethod});

            var adapterMethod = adapter.GetType().GetMethod("Visit",
                                                            BindingFlags.Public | BindingFlags.Instance);
            var result = Delegate.CreateDelegate(tAdapterMethod, adapter, adapterMethod);

            return (ComponentVisitor) result;
        }*/

        /*public void Visit(Component c)
        {
            if (_ddMethods == null)
            {
                PrepareDoubleDispatch();
                
            }
            ComponentVisitor cv;
            if(c is ActionCode)
            {
                if (_ddMethods.TryGetValue(typeof(ActionCode), out cv))
                {
                    cv(this, c);
                    return;  
                }
                    
            }
            if (_ddMethods.TryGetValue(c.GetType(), out cv))
                cv(this, c);
            
        }*/



        


       


        // Polymorphic Component Visits
        /* virtual public void Visit(Component component)
         {
             Debug.WriteLine("component has been visitted");
         }*/
        #region Members
        /// <summary>
        /// Visits the specified cEntity to collect data if required by the current Visitor derivate.
        /// </summary>
        /// <param name="cEntity">The cEntity.</param>
        virtual public void Visit(SceneEntity cEntity)
        {

        }
        /// <summary>
        /// Visits the specified action code to collect data if required by the current Visitor derivate.
        /// </summary>
        /// <param name="actionCode">The action code.</param>
        public virtual void Visit(ActionCode actionCode)
        {
            
        }

        /// <summary>
        /// Visits the specified directional light to collect data if required by the current Visitor derivate.
        /// </summary>
        /// <param name="directionalLight">The directional light.</param>
        public virtual void Visit(DirectionalLight directionalLight)
        {
            
        }

        /// <summary>
        /// Visits the specified point light to collect data if required by the current Visitor derivate.
        /// </summary>
        /// <param name="pointLight">The point light.</param>
        public virtual void Visit(PointLight pointLight)
        {
           
        }

        /// <summary>
        /// Visits the specified renderer to collect data if required by the current Visitor derivate.
        /// </summary>
        /// <param name="renderer">The renderer.</param>
        public virtual void Visit(Renderer renderer)
        {
            
        }


        /// <summary>
        /// Visits the specified spot light to collect data if required by the current Visitor derivate.
        /// </summary>
        /// <param name="spotLight">The spot light.</param>
        public virtual void Visit(SpotLight spotLight)
        {
           
        }

        /// <summary>
        /// Visits the specified transformation to collect data if required by the current Visitor derivate.
        /// </summary>
        /// <param name="transformation">The transformation.</param>
        public virtual void Visit(Transformation transformation)
        {
            
        }

        /// <summary>
        /// Visits the specified camera to collect data if required by the current Visitor derivate.
        /// </summary>
        /// <param name="camera">The camera.</param>
        public virtual void Visit(Camera camera)
        {

        }
        #endregion
    }

}




