using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace SceneManagement
{
    public class Transformation : Component
    {
        private readonly int _id = 2;
        private float4x4 _transformMatrix;
        private float4x4 _worldMatrix;
        private float3 _localPosition;
        private float3 _worldPosition;
        private Quaternion _localRotation;
        private float3 _localScale;
        private float3 _eulerAngles;
        private SceneEntity _entity;



       public Transformation()
       {
           _transformMatrix = float4x4.Identity;
           _localPosition = new float3(_transformMatrix.Row3);
           _localScale = new float3(1,1,1);
       }

       public Transformation(SceneEntity entity)
       {
           _transformMatrix = float4x4.Identity;
           _localPosition = new float3(_transformMatrix.Row3);
           _localScale = new float3(1, 1, 1);
           _entity = entity;
      

       }

        override public void Traverse(ITraversalState _traversalState)
       {
          
           // _worldMatrix =_traversalState.Matrix*Matrix;
       }

        public float4x4 WorldMatrix
        {
            get { return _worldMatrix; }
        }


       public float4x4 Matrix
       {
           get
           {
               return _transformMatrix;
           }
           set { _transformMatrix = value; }
       }


       public float3 LocalPosition
       {
           set
           {
               _localPosition = value;
               _transformMatrix.Row3.x = _localPosition.x;
               _transformMatrix.Row3.y = _localPosition.y;
               _transformMatrix.Row3.z = _localPosition.z;
           }
           get { return _localPosition; }
       }
        
        
        /*public float3 Position
        {
            set {  Will cause some headache
                _worldPosition = value;
                _worldMatrix.Row3.x = _worldPosition.x;
                _worldMatrix.Row3.y = _worldPosition.y;
                _worldMatrix.Row3.z = _worldPosition.z;
            }
            get
            {
                // recurse over parent chain up to the root object;
                return 
            }
        }*/

       public float3 LocalScale
       {
           set
           {
               _localScale = value;
               _transformMatrix *= float4x4.Scale(this.LocalScale);
           }
           get
           {
               float3 scale = _localScale;
               return scale;
           }
       }

       public Quaternion LocalRotation
       {
           get
           {
               Quaternion locrot = _localRotation;
               return locrot;
           }
           set
           {
               _localRotation = value;
               _transformMatrix *= float4x4.Rotate(_localRotation);
           }
       }

       public float3 EulerAngles
       {
          set
          {
              _eulerAngles = value;
              _worldMatrix *= (float4x4.CreateRotationY(_eulerAngles.y)*float4x4.CreateRotationX(_eulerAngles.x)*
                             float4x4.CreateRotationZ(_eulerAngles.z));
          }
           
           get { return _eulerAngles; }
       }
      
    }

}
