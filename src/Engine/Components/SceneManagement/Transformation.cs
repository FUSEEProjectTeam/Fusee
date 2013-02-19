using System;
using System.Collections.Generic;
using System.Text;
using Fusee.Math;



namespace Fusee.SceneManagement
{
    public class Transformation : Component
    {
        private float4x4 _transformMatrix;
        private float3 _localPosition;
        private Quaternion _quaternion;
        private float3 _localScale;
        private float3 _eulerAngles;
        private SceneEntity _entity;
        private bool _matrixDirty;
        private bool _quaternionDirty;
        private bool _eulerDirty;
       public Transformation()
       {
           _transformMatrix = float4x4.Identity;
           _localScale = new float3(1,1,1);
           _matrixDirty = false;
           _quaternionDirty = false;
           _eulerDirty = false;
       }

       public Transformation(SceneEntity entity)
       {
           _transformMatrix = float4x4.Identity;
           _localScale = new float3(1, 1, 1);
           _entity = entity;
           _matrixDirty = false;
           _quaternionDirty = false;
           _eulerDirty = false;

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
                   _quaternion = Quaternion.EulerToQuaternion(_eulerAngles);
                   // TODO: Add Euler to quaternion conversion and vice versa
                   _quaternionDirty = false;
               }
               
               return _quaternion;
           }
           set
           {
               _matrixDirty = true;
               _eulerDirty = true;
               _quaternion = value;
               _eulerAngles = LocalEulerAngles; // Hack ??
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

           get
           {
               if(_eulerDirty)
               {
                   _eulerAngles = Quaternion.QuaternionToEuler(_quaternion);
                   _eulerDirty = false;
               }
               return _eulerAngles;
           }
       }
      private void UpdateMembers() // Extract members from transformmatrix
      {
          _quaternion = Quaternion.Identity;
          _eulerAngles = float3.One;
          _localScale = float3.One;
          _localPosition = float3.One;
      }
    }

}
