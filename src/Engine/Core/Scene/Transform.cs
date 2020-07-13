using Fusee.Math.Core;

namespace Fusee.Engine.Core.Scene
{
    /// <summary>
    /// Transformation (position, orientation and size) of the node.
    /// </summary>
    /// <seealso cref="Fusee.Engine.Common.SceneComponent" />
    public class Transform : SceneComponent
    {
        public Transform()
        {
            _translation = float4x4.Identity;
            _rotation = float4x4.Identity;
            _scale = float4x4.Identity;

            _matrix = float4x4.Identity;
            _matrixDirty = false;
        }

        private bool _matrixDirty;
        private float4x4 _matrix;

        private float4x4 _translation;
        private float4x4 _rotation;
        private float4x4 _scale;

        public float4x4 Matrix
        {
            get
            {
                if (_matrixDirty)
                {
                    _matrix = _translation * _rotation * _scale;
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

                    _translation = float4x4.TranslationDecomposition(_matrix);
                    _rotation = float4x4.RotationDecomposition(_matrix);
                    _scale = float4x4.ScaleDecomposition(_matrix);
                }
            }
        }

        /// <summary>
        /// The translation (position) of the node.
        /// </summary>
        public float3 Translation
        {
            get => float4x4.GetTranslation(_translation);
            set
            {
                if (Translation != value)
                {
                    _translation = float4x4.CreateTranslation(value);
                    _matrixDirty = true;
                }
            }
        }

        /// <summary>
        /// The translation (position) of the node.
        /// </summary>
        public float4x4 TranslationMatrix
        {
            get => _translation;
            set
            {
                if (TranslationMatrix != value)
                {
                    _translation = float4x4.TranslationDecomposition(value);
                    _matrixDirty = true;
                }
            }
        }

        /// <summary>
        /// The rotation (orientation) of the node.
        /// </summary>
        public float3 Rotation
        {
            get => float4x4.RotMatToEuler(_rotation);
            set
            {
                if (Rotation != value)
                {
                    _rotation = float4x4.CreateRotationYXZ(value);
                    _matrixDirty = true;
                }
            }
        }

        /// <summary>
        /// The rotation (orientation) of the node.
        /// </summary>
        public float4x4 RotationMatrix
        {
            get => _rotation;
            set
            {
                if (RotationMatrix != value)
                {
                    _rotation = float4x4.RotationDecomposition(value);
                    _matrixDirty = true;
                }
            }
        }

        /// <summary>
        /// The scale (size) of the node.
        /// </summary>
        public float3 Scale
        {
            get => float4x4.GetScale(_scale);
            set
            {
                if (Scale != value)
                {
                    _scale = float4x4.CreateScale(value);
                    _matrixDirty = true;
                }
            }
        }

        /// <summary>
        /// The scale (size) of the node.
        /// </summary>
        public float4x4 ScaleMatrix
        {
            get => _scale;
            set
            {
                if (ScaleMatrix != value)
                {
                    _scale = float4x4.ScaleDecomposition(value);
                    _matrixDirty = true;
                }
            }
        }
    }
}