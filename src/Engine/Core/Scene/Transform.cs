using Fusee.Math.Core;

namespace Fusee.Engine.Core.Scene
{
    /// <summary>
    /// Transformation (position, orientation and size) of the node.
    /// </summary>
    /// <seealso cref="Fusee.Engine.Core.Scene.SceneComponent" />
    public class Transform : SceneComponent
    {
        #region Fields

        private bool _matrixDirty = true;
        private float4x4 _matrix;

        private float4x4 _translationMtx;
        private float4x4 _rotationMtx;
        private float4x4 _scaleMtx;

        //cached Values
        private bool _translationVecDirty = true;
        private float3 _translationVec;
        private bool _rotationVecDirty = true;
        private float3 _rotationVec;
        private bool _scaleVecDirty = true;
        private float3 _scaleVec;

        private bool _rotationQuatDirty = true;
        private Quaternion _rotationQuat;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a Transform component
        /// </summary>
        public Transform() : this(float4x4.Identity, float4x4.Identity, float4x4.Identity)
        {
        }

        /// <summary>
        /// Creates a Transform component
        /// </summary>
        /// <param name="translation"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        public Transform(float3 translation, float3 rotation, float3 scale)
        {
            TranslationVector = translation;
            RotationEuler = rotation;
            ScaleVector = scale;
        }

        /// <summary>
        /// Creates a Transform component
        /// </summary>
        /// <param name="translation"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        public Transform(float3 translation, Quaternion rotation, float3 scale)
        {
            TranslationVector = translation;
            RotationQuaternion = rotation;
            ScaleVector = scale;
        }

        /// <summary>
        /// Creates a Transform component
        /// </summary>
        /// <param name="translation"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        public Transform(float4x4 translation, float4x4 rotation, float4x4 scale)
        {
            TranslationMatrix = translation;
            RotationMatrix = rotation;
            ScaleMatrix = scale;
        }

        #endregion Constructors

        #region Matrix Operations

        /// <summary>
        /// The matrix (position, rotation, scale) of the node.
        /// </summary>
        public float4x4 Matrix
        {
            get
            {
                if (_matrixDirty)
                {
                    _matrix = _translationMtx * _rotationMtx * _scaleMtx;
                    _matrixDirty = false;
                }

                return _matrix;
            }
            set
            {
                if (Matrix != value)
                {
                    _matrix = value;
                    _matrixDirty = false;

                    _translationMtx = float4x4.TranslationDecomposition(_matrix);
                    _rotationMtx = float4x4.RotationDecomposition(_matrix);
                    _scaleMtx = float4x4.ScaleDecomposition(_matrix);

                    _translationVecDirty = true;
                    _rotationVecDirty = true;
                    _rotationQuatDirty = true;
                    _scaleVecDirty = true;
                }
            }
        }

        /// <summary>
        /// The translation (position) of the node.
        /// </summary>
        public float4x4 TranslationMatrix
        {
            get => _translationMtx;
            set
            {
                if (TranslationMatrix != value)
                {
                    _translationMtx = float4x4.TranslationDecomposition(value);
                    
                    _matrixDirty = true;
                    _translationVecDirty = true;
                }
            }
        }

        /// <summary>
        /// The rotation (orientation) of the node.
        /// </summary>
        public float4x4 RotationMatrix
        {
            get => _rotationMtx;
            set
            {
                if (RotationMatrix != value)
                {
                    _rotationMtx = float4x4.RotationDecomposition(value);

                    _matrixDirty = true;
                    _rotationVecDirty = true;
                    _rotationQuatDirty = true;
                }
            }
        }

        /// <summary>
        /// The scale (size) of the node.
        /// </summary>
        public float4x4 ScaleMatrix
        {
            get => _scaleMtx;
            set
            {
                if (ScaleMatrix != value)
                {
                    _scaleMtx = float4x4.ScaleDecomposition(value);
                    
                    _matrixDirty = true;
                    _scaleVecDirty = true;
                }
            }
        }

        #endregion Matrix Operations

        #region Cached Vector Operations

        /// <summary>
        /// The translation (position) of the node.
        /// </summary>
        public float3 TranslationVector
        {
            get
            {
                if (_translationVecDirty)
                {
                    _translationVec = float4x4.GetTranslation(_translationMtx);
                    _translationVecDirty = false;
                }

                return _translationVec;
            }
            set
            {
                if (TranslationVector != value)
                {
                    _translationVec = value;
                    _translationMtx = float4x4.CreateTranslation(value);

                    _matrixDirty = true;
                    _translationVecDirty = false;
                }
            }
        }

        /// <summary>
        /// The rotation (orientation) of the node.
        /// </summary>
        public float3 RotationEuler
        {
            get
            {
                if (_rotationVecDirty)
                {
                    _rotationVec = float4x4.RotMatToEuler(_rotationMtx);
                    _rotationVecDirty = false;
                }

                return _rotationVec;
            }
            set
            {
                if (RotationEuler != value)
                {
                    _rotationVec = value;
                    _rotationMtx = float4x4.CreateRotationZXY(value);

                    _matrixDirty = true;
                    _rotationVecDirty = false;
                    _rotationQuatDirty = true;
                }
            }
        }

        /// <summary>
        /// The scale (size) of the node.
        /// </summary>
        public float3 ScaleVector
        {
            get
            {   
                if (_scaleVecDirty)
                {
                    _scaleVec = float4x4.GetScale(_scaleMtx);
                    _scaleVecDirty = false;
                }

                return _scaleVec;
            }
            set
            {
                if (ScaleVector != value)
                {
                    _scaleVec = value;
                    _scaleMtx = float4x4.CreateScale(value);

                    _matrixDirty = true;
                    _scaleVecDirty = false;
                }
            }
        }

        #endregion Cached Vector Operations

        #region Cached Quaternion Operations

        /// <summary>
        /// The rotation (orientation) of the node.
        /// </summary>
        public Quaternion RotationQuaternion
        {
            get
            {
                if (_rotationQuatDirty)
                {
                    _rotationQuat = Quaternion.FromRotationMatrix(_rotationMtx);
                    _rotationQuatDirty = false;
                }

                return _rotationQuat;
            }
            set
            {
                if (RotationQuaternion != value)
                {
                    _rotationQuat = value;
                    _rotationMtx = value.ToRotMat();

                    _matrixDirty = true;
                    _rotationQuatDirty = false;
                    _rotationVecDirty = true;
                }
            }

        }

        #endregion Cached Quaternion Operations

        #region Deprecated, remove with 0.12
        //TODO: Remove with 0.12

        /// <summary>
        /// The translation (position) of the node.
        /// </summary>
        [System.Obsolete("Translation is deprecated, please use TranslationVector instead. This poperty will be removed with 0.12.")]
        public float3 Translation
        {
            get => TranslationVector;
            set => TranslationVector = value;
        }

        /// <summary>
        /// The rotation (orientation) of the node.
        /// </summary>
        [System.Obsolete("Rotation is deprecated, please use RotationEuler instead. This property will be removed with 0.12.")]
        public float3 Rotation
        {
            get => RotationEuler;
            set => RotationEuler = value;
        }

        /// <summary>
        /// The scale (size) of the node.
        /// </summary>
        [System.Obsolete("Scale is deprecated, please use ScaleVector instead. This property will be removed with 0.12.")]
        public float3 Scale
        {
            get => ScaleVector;
            set => ScaleVector = value;
        }

        #endregion Deprecated, remove with 0.11
    }
}