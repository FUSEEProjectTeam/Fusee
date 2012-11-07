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
        private TraversalState _traversalState; 
        private float4x4 _transformMatrix;
        private float4x4 _worldMatrix;
        private float3 _localPosition;
        private Quaternion _localRotation;
        private float3 _localScale;
        private float3 _eulerAngles;
       public Transformation()
       {
           _transformMatrix = float4x4.Identity;
           _localPosition = new float3(_transformMatrix.Row3);
           _localScale = new float3(1,1,1);
       }
       override public void Traverse(TraversalState _traversalState)
       {
           
           _worldMatrix = _traversalState.Matrix*Matrix;
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

     /*  public float3 EulerAngles
       {
           get
           {
               
           }
       }
       */

       public override int GETID()
       {
           return _id;
       }

       
    }


}
