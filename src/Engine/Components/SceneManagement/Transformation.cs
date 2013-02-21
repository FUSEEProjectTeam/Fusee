using System;
using System.Collections.Generic;
using System.Text;
using Fusee.Math;



namespace Fusee.SceneManagement
{
    /// <summary>
    /// Transformation is derived from Component and stores all positions, angles and movement of all SceneEntitys. 
    /// </summary>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="Transformation"/> class. No SceneEntity will be set.
        /// </summary>
       public Transformation()
       {
           _transformMatrix = float4x4.Identity;
           _localScale = new float3(1,1,1);
           _matrixDirty = false;
           _quaternionDirty = false;
           _eulerDirty = false;
       }

       /// <summary>
       /// Initializes a new instance of the <see cref="Transformation"/> class. Sets a SceneEntity for Transform.
       /// </summary>
       /// <param name="entity">The SceneEntity that will be set to in Transform.</param>
       public Transformation(SceneEntity entity)
       {
           _transformMatrix = float4x4.Identity;
           _localScale = new float3(1, 1, 1);
           _entity = entity;
           _matrixDirty = false;
           _quaternionDirty = false;
           _eulerDirty = false;
       }







       /// <summary>
       /// Gets or sets the float4x4 transformMatrix.
       /// </summary>
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
           set
           {

               _transformMatrix = value;
               UpdateMembers();
           } // TODO: Extract Position, Rotation, Scale after assignment.
       }


        private float3 GetScaleFromMatrix(float4x4 matrix)
        {
            float xs, ys, zs;
            xs = GetLengthOfVector(new float3(matrix.M11, matrix.M12, matrix.M13));
            ys = GetLengthOfVector(new float3(matrix.M21, matrix.M22, matrix.M23));
            zs = GetLengthOfVector(new float3(matrix.M31, matrix.M32, matrix.M33));
            return new float3(xs,ys,zs);
        }


        private float3 GetPositionFromMatrix(float4x4 matrix)
        {
            return new float3(matrix.M41,matrix.M42,matrix.M43);
        }


        private float GetLengthOfVector(float3 vector)
        {
            double sum = (vector.x*vector.x + vector.y*vector.y + vector.z*vector.z);
            return (float)System.Math.Sqrt(sum);
        }

        /// <summary>
        /// Gets or sets the float3 LocalPosition.
        /// </summary>
       public float3 LocalPosition
       {
           set
           {
               _localPosition = value;
               _matrixDirty = true;
           }
           get
           {
               return _localPosition;
           }
       }



       /// <summary>
       /// Gets or sets the float3 LocalScale.
       /// </summary>
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

       /// <summary>
       /// Gets or sets the Quaternion LocalQuaternion.
       /// </summary>
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
       /// <summary>
       /// Gets or sets the float3 LocalEulerAngles.
       /// </summary>
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
          //_quaternion = Quaternion.Identity;
          //_eulerAngles = float3.;
          _localScale = GetScaleFromMatrix(_transformMatrix);
          _localPosition = GetPositionFromMatrix(_transformMatrix);
      }
      public override void Accept(SceneVisitor sv)
      {
          sv.Visit((Transformation)this);
      }
    }
}
