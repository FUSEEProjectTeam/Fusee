using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Fusee.Math;



namespace Fusee.SceneManagement
{
    /// <summary>
    /// Transformation is derived from Component and stores all positions, angles and movement of all SceneEntities. 
    /// </summary>
    public class Transformation : Component
    {
        #region Private Fields
        private float4x4 _transformMatrix;
        private float3 _localPosition;
        private Quaternion _quaternion;
        private float3 _localScale;
        private float3 _eulerAngles;

        private float4x4 _globalMatrix;
        private float3 _globalPosition;
        private Quaternion _globalQuaternion;
        private float3 _globalScale;
        private float3 _globalEulerAngles;


        private bool _matrixDirty;
        private bool _quaternionDirty;
        private bool _eulerDirty;

        private bool _hasParent;

        private bool _globalMatrixDirty;
        private bool _globalQuaternionDirty;
        private bool _globalEulerDirty;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Transformation"/> class. No SceneEntity will be set.
        /// </summary>
       public Transformation()
       {
           _hasParent = false;
           _transformMatrix = float4x4.Identity;
            _globalMatrix = _transformMatrix;
            _quaternion = Quaternion.Identity;
            _globalQuaternion = Quaternion.Identity;
           _globalScale = new float3(1,1,1);
           _localScale = new float3(1,1,1);
           _matrixDirty = false;
           _quaternionDirty = false;
           _eulerDirty = false;
       }

       /// <summary>
       /// Initializes a new instance of the <see cref="Transformation" /> class. Sets a SceneEntity for Transform.
       /// This is the recommended Constructor.
       /// </summary>
       /// <param name="entity">The SceneEntity that will be set in Transform.</param>
       public Transformation(SceneEntity entity)
       {
           if (entity.parent == null)
           {
               _hasParent = false;
           }
           else
           {
               _hasParent = true;
           }
           SceneEntity = entity;
           _transformMatrix = float4x4.Identity;
           _globalMatrix = _transformMatrix;
           _quaternion = Quaternion.Identity;
           _globalQuaternion = Quaternion.Identity;
           _globalScale = new float3(1,1,1);
           _localScale = new float3(1, 1, 1);
           // _entity = entity;
           _matrixDirty = false;
           _quaternionDirty = false;
           _eulerDirty = false;
       }
        #endregion

        #region Public Fields
       /// <summary>
       /// Gets the forward direction as float3 of the Transformation instance.
       /// </summary>
       /// <value>
       /// The forward.
       /// </value>
       public float3 Forward
       {
           get { return GlobalMatrix.Row2.xyz; }
       }

       /// <summary>
       /// Sets the parent <see cref="SceneEntity"/>.
       /// </summary>
       /// <value>
       /// The parent.
       /// </value>
       public SceneEntity Parent
       {
           set
           {
               if (value == null)
               {
                   _hasParent = false;
               }
               else
               {
                   _hasParent = true;
               }
           }
       }



       /// <summary>
       /// Gets or sets the float4x4 local transformMatrix.
       /// </summary>
       public float4x4 Matrix
       {
           get
           {
               if (_matrixDirty)
               {
                   _transformMatrix = float4x4.CreateTranslation(_localPosition) * Quaternion.QuaternionToMatrix(LocalQuaternion) * float4x4.Scale(_localScale);
                   _matrixDirty = false;
               }
               return _transformMatrix;
           }
           set
           {
               _globalMatrixDirty = false;
               _transformMatrix = value;
               UpdateLocalMembers();
           } 
       }

       /// <summary>
       /// Gets or sets the float4x4 global transformMatrix.
       /// </summary>
       public float4x4 GlobalMatrix
       {
           get
           {
               if (_globalMatrixDirty)
               {
                   _globalMatrix = float4x4.CreateTranslation(_globalPosition) * Quaternion.QuaternionToMatrix(GlobalQuaternion) * float4x4.Scale(_globalScale);
               }
               return _globalMatrix;
           }
           set
           {
              /* if(_globalMatrix != value)
               {
                   _globalMatrix = value;
                   _globalMatrixDirty = true;
                   UpdateGlobalMembers();
                   return;
               }*/
               _globalMatrix = value;
               UpdateGlobalMembers();
           } 
       }

       /// <summary>
       /// Gets a value indicating whether [global matrix dirty].
       /// Do not use this method. It is intended to be used inside of <see cref="SceneVisitor"/> for covering the special cases of overwriting local and global matrices on this transformation instance. 
       /// </summary>
       /// <value>
       ///   <c>true</c> if [global matrix dirty]; otherwise, <c>false</c>.
       /// </value>
        public bool GlobalMatrixDirty
        {
            get { return _globalMatrixDirty; }
        }

        /// <summary>
        /// Sets the global Matrix of this Transformation instance. This method is used by <see cref="SceneVisitor"/>. 
        /// Do not use this method. use the GlobalMatrix property instead.
        /// </summary>
        /// <param name="mat">The mat.</param>
        public void SetGlobalMat(float4x4 mat)
        {
            _globalMatrix = mat;
            UpdateGlobalMembers();
            _globalMatrixDirty = false;
        }



        /// <summary>
        /// Gets the scale as float3 from a matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns></returns>
        private float3 GetScaleFromMatrix(float4x4 matrix)
        {
            return new float3(GetLengthOfVector(matrix.Column0.xyz), GetLengthOfVector(matrix.Column1.xyz), GetLengthOfVector(matrix.Column2.xyz));
        }


        private float3 GetPositionFromMatrix(float4x4 matrix)
        {
            return matrix.Column3.xyz;
        }


        private float GetLengthOfVector(float3 vector)
        {
            double sum = (vector.x*vector.x + vector.y*vector.y + vector.z*vector.z);
            return (float)System.Math.Sqrt(sum);
        }

        /// <summary>
        /// Gets or sets the float3 LocalPosition of this Transformation instance.
        /// </summary>
       public float3 LocalPosition
       {
           set
           {
               if (!_hasParent)
               {
                   GlobalPosition = value;
               }
               _localPosition = value;
               _matrixDirty = true;
           }
           get
           {
               return _localPosition;
           }
       }


       /// <summary>
       /// Gets or sets the global position of this Transformation instance.
       /// </summary>
       /// <value>
       /// The global position as float3.
       /// </value>
        public float3 GlobalPosition
        {
            set
            {
                _globalPosition = value;
                _globalMatrixDirty = true;
            }
            get
            {
                return _globalPosition;
            }
        }



        /// <summary>
        /// Gets or sets the float3 LocalScale of this Transformation instance.
        /// </summary>
        /// <value>
        /// The local scale as float3.
        /// </value>
       public float3 LocalScale
       {
           set
           {
               if (!_hasParent)
               {
                   GlobalScale = value;
               }
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
       /// Gets or sets the global scale of this Transformation instance.
       /// </summary>
       /// <value>
       /// The global scale as float3.
       /// </value>
        public float3 GlobalScale
        {
            set
            {
                _globalScale = value;
                _globalMatrixDirty = true;
            }
            get
            {
                float3 scale = _globalScale;
                return scale;
            }
        }

        /// <summary>
        /// Gets or sets the Quaternion LocalQuaternion.
        /// </summary>
        /// <value>
        /// The local quaternion.
        /// </value>
       public Quaternion LocalQuaternion
       {
           get
           {
               if (_quaternionDirty)
               {
                   _quaternion = Quaternion.EulerToQuaternion(_eulerAngles, true);
                   _quaternionDirty = false;
               }
               
               return _quaternion;
           }
           set
           {
               if (!_hasParent)
               {
                   GlobalQuaternion = value;
               }
               _matrixDirty = true;
               _eulerDirty = true;
               _quaternion = value;
           }
       }

       /// <summary>
       /// Gets or sets the Quaternion GlobalQuaternion.
       /// </summary>
       /// <value>
       /// The global quaternion.
       /// </value>
       public Quaternion GlobalQuaternion
       {
           get
           {
               if (_globalQuaternionDirty)
               {
                   _globalQuaternion = Quaternion.EulerToQuaternion(_globalEulerAngles, true);
                   _globalQuaternionDirty = false;
               }

               return _globalQuaternion;
           }
           set
           {
               _globalMatrixDirty = true;
               _globalEulerDirty = true;
               _globalQuaternion = value;
           }
       }


       /// <summary>
       /// Gets or sets the float3 LocalEulerAngles.
       /// </summary>
       /// <value>
       /// The local euler angles as float3.
       /// </value>
       public float3 LocalEulerAngles
       {
           set
           {
               if(!_hasParent)
               {
                   GlobalEulerAngles = value;
               }
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

       /// <summary>
       /// Gets or sets the float3 GlobalEulerAngles.
       /// </summary>
       /// <value>
       /// The global euler angles as float3.
       /// </value>
       public float3 GlobalEulerAngles
       {
           set
           {
               _globalEulerAngles = value;
               _globalMatrixDirty = true;
               _globalQuaternionDirty = true;
           }

           get
           {
               if (_globalEulerDirty)
               {
                   _globalEulerAngles = Quaternion.QuaternionToEuler(_globalQuaternion);
                   _globalEulerDirty = false;
               }
               return _globalEulerAngles;
           }
       }
       #endregion

        #region Members
       private void UpdateLocalMembers() // Extract members from transformMatrix
      {
          
          _quaternion = Quaternion.LookRotation(_transformMatrix.Column2.xyz, _transformMatrix.Column2.xyz);
          //_quaternion = Quaternion.Identity;
          //Debug.WriteLine("LocalQuaternion: "+_quaternion);
          _eulerAngles = Quaternion.QuaternionToEuler(_quaternion);
          //Debug.WriteLine("LocalEuler: " + _eulerAngles);
          _localScale = GetScaleFromMatrix(_transformMatrix);
          //Debug.WriteLine("LocalScale: " + _localScale);
          _localPosition = GetPositionFromMatrix(_transformMatrix);
          //Debug.WriteLine("LocalPosition: " + _localPosition);
          _eulerDirty = false;
          _quaternionDirty = false;
      }

      private void UpdateGlobalMembers() // Extract members from globalMatrix
      {
          //_globalQuaternion = Quaternion.Identity;
          _globalQuaternion = Quaternion.LookRotation(_globalMatrix.Column2.xyz, _globalMatrix.Column2.xyz);
          _globalEulerAngles = Quaternion.QuaternionToEuler(_globalQuaternion);
          _globalScale = GetScaleFromMatrix(_globalMatrix);
          _globalPosition = GetPositionFromMatrix(_globalMatrix);

          _globalEulerDirty = false;
          _globalQuaternionDirty = false;
      }
       #endregion

        #region Overrides
      /// <summary>
      /// Passes this Component to the <see cref="SceneVisitor"/> which decides what to do with that Component.
      /// </summary>
      /// <param name="sv">The SceneVisitor.</param>
      public override void Accept(SceneVisitor sv)
      {
          sv.Visit(this);
      }
      #endregion
    }
}
