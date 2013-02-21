using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;


namespace Fusee.SceneManagement
{
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
        virtual public void Visit(SceneEntity cEntity)
        {

        }
        public virtual void Visit(ActionCode actionCode)
        {
            
        }

        public virtual void Visit(DirectionalLight directionalLight)
        {
            
        }

        public virtual void Visit(PointLight pointLight)
        {
           
        }

        public virtual void Visit(Renderer renderer)
        {
            
        }


        public virtual void Visit(SpotLight spotLight)
        {
           
        }

        public virtual void Visit(Transformation transformation)
        {
            
        }
        public virtual void Visit(Camera camera)
        {

        }

    }

}




