using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;

//TODO: Testen ob GarbageCollector Arbeitsspeicher freigibt

namespace SceneManagement
{
    public class Transformation : Component
    {
        private float4x4 _transformMatrix;
        private float3 _localPosition;
        private Quaternion _quaternion;
        private bool _quaternionDirty;
        private float3 _localScale;
        private float3 _eulerAngles;
        private SceneEntity _entity;
        private bool _matrixDirty;

       public Transformation()
       {
           _transformMatrix = float4x4.Identity;
           _localScale = new float3(1,1,1);
           _matrixDirty = false;
       }

       public Transformation(SceneEntity entity)
       {
           _transformMatrix = float4x4.Identity;
           _localScale = new float3(1, 1, 1);
           _entity = entity;
           _matrixDirty = false;

       }

       override public void Traverse(ITraversalState _traversalState)
       {          
           _traversalState.AddTransform(Matrix);
       }



       public float4x4 Matrix
       {
           get
           {
               if (_matrixDirty)
               {
                   _transformMatrix = float4x4.Scale(_localScale) * float4x4.CreateRotationY(_eulerAngles.y) * float4x4.CreateRotationX(_eulerAngles.x) * float4x4.CreateRotationZ(_eulerAngles.z) * float4x4.CreateTranslation(_localPosition);
                   _matrixDirty = false;
               }
               return _transformMatrix;
           }
           set { _transformMatrix = value; } // TODO: Extract Position, Rotation, Scale after assignment.
       }


       public float3 LocalPosition
       {
           set
           {
               _localPosition = value;
               _matrixDirty = true;
           }
           get { return _localPosition; }
       }
        
        

       public float3 LocalScale
       {
           set
           {
               _localScale = value;
               _matrixDirty = true;
           }
           get
           {
               float3 scale = _localScale;
               return scale;
           }
       }

       public Quaternion LocalQuaternion
       {
           get
           {
               if (_quaternionDirty)
               {
                   // TODO: Add Euler to quaternion conversion and vice versa
                   _quaternionDirty = false;
               }
               
               return _quaternion;
           }
           set
           {
               _quaternion = value;
               // TODO: Update eulerangles value from quaternion value
           }
       }


        // TODO: Add eulerdirty functionality
       public float3 LocalEulerAngles
       {
           set
           {
               _eulerAngles = value;
               _matrixDirty = true;
               _quaternionDirty = true;
           }

           get { return _eulerAngles; }
       }
      
    }

}
