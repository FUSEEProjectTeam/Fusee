using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace SceneManagement
{
    public class TraversalState
    {
        private float4x4 _worldMatrix;
        private GameEntity _owner;
        public TraversalState(float4x4 ownerMatrix)
        {
            _worldMatrix = ownerMatrix;
        }        
        public TraversalState(float4x4 ownerMatrix, GameEntity owner)
        {
            _worldMatrix = ownerMatrix;
            _owner = owner;
        }

        public float4x4 Matrix
        {
            get { 
                float4x4 copy = _worldMatrix;
                return copy;
                }
            set { _worldMatrix = value*_worldMatrix; }
        }

        public GameEntity Owner
        {
            get { return _owner; }
        }
    }
}
