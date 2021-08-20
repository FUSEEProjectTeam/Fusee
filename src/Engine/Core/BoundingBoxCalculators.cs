using Fusee.Engine.Common;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using Fusee.Xene;
using System.Collections.Generic;


namespace Fusee.Engine.Core
{

    /// <summary>
    /// Object-Oriented Bounding Box Calculator. Use instances of this class to calculate axis-aligned bounding boxes
    /// on scenes, list of scene nodes or individual scene nodes. Calculations always include any child nodes.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class OBBCalculator : Visitor<SceneNode, SceneComponent>
    {
        /// <summary>
        /// Contains the model view state while traversing the scene to generate the OBB.
        /// </summary>
        public class OBBState : VisitorState
        {
            private readonly CollapsingStateStack<float4x4> _modelView = new();

            /// <summary>
            /// The model view matrix.
            /// </summary>
            public float4x4 ModelView
            {
                set { _modelView.Tos = value; }
                get { return _modelView.Tos; }
            }

            /// <summary>
            /// Creates a new instance of type OBBState.
            /// </summary>
            public OBBState()
            {
                RegisterState(_modelView);
            }
        }

        //private SceneContainer _sc;
        private readonly IEnumerable<SceneNode> _sncList;
        private readonly OBBState _state = new();
        private readonly List<float3> _allVerticesOfCurrentScene = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="AABBCalculator"/> class.
        /// </summary>
        /// <param name="sc">The scene container to calculate an axis-aligned bounding box for.</param>
        public OBBCalculator(SceneContainer sc)
        {
            _sncList = sc.Children;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OBBCalculator"/> class.
        /// </summary>
        /// <param name="sncList">The list of scene nodes to calculate an axis-aligned bounding box for.</param>
        public OBBCalculator(IEnumerable<SceneNode> sncList)
        {
            _sncList = sncList;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OBBCalculator"/> class.
        /// </summary>
        /// <param name="snc">A single scene node to calculate an axis-aligned bounding box for.</param>
        public OBBCalculator(SceneNode snc)
        {
            _sncList = VisitorHelpers.SingleRootEnumerable(snc);
        }

        /// <summary>
        /// Performs the calculation and returns the resulting box on the object(s) passed in the constructor. Any calculation
        /// always includes a full traversal over all child nodes.
        /// </summary>
        /// <returns>The resulting axis-aligned bounding box.</returns>
        public OBBf GetBox()
        {
            Traverse(_sncList);
            return new OBBf(_allVerticesOfCurrentScene.ToArray());
        }

        #region Visitors

        /// <summary>
        /// Do not call. Used for internal traversal purposes only
        /// </summary>
        /// <param name="transform">The transform component.</param>
        [VisitMethod]
        public void OnTransform(Transform transform)
        {
            _state.ModelView *= transform.Matrix;
        }

        /// <summary>
        /// Do not call. Used for internal traversal purposes only
        /// </summary>
        /// <param name="mesh">The mesh component.</param>
        [VisitMethod]
        public void OnMesh(Mesh mesh)
        {
            // modify mesh
            for (var i = 0; i < mesh.Vertices.Length; i++)
                _allVerticesOfCurrentScene.Add(_state.ModelView * mesh.Vertices[i]);
        }

        #endregion

        #region HierarchyLevel

        /// <summary>
        /// Method is called when traversal starts to initialize the traversal state.
        /// </summary>
        protected override void InitState()
        {
            _state.Clear();
            _state.ModelView = float4x4.Identity;
        }

        /// <summary>
        /// Method is called when going down one hierarchy level while traversing.
        /// </summary>
        protected override void PushState()
        {
            _state.Push();
        }

        /// <summary>
        /// Method is called when going up one hierarchy level while traversing.
        /// </summary>
        protected override void PopState()
        {
            _state.Pop();
        }

        #endregion
    }

    /// <summary>
    /// Axis-Aligned Bounding Box Calculator. Use instances of this class to calculate axis-aligned bounding boxes
    /// on scenes, list of scene nodes or individual scene nodes. Calculations always include any child nodes.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class AABBCalculator : Visitor<SceneNode, SceneComponent>
    {
        /// <summary>
        /// Contains the model view state while traversing the scene to generate the ABB.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public class AABBState : VisitorState
        {
            private readonly CollapsingStateStack<float4x4> _modelView = new();

            /// <summary>
            /// The model view matrix.
            /// </summary>
            public float4x4 ModelView
            {
                set { _modelView.Tos = value; }
                get { return _modelView.Tos; }
            }

            /// <summary>
            /// Creates a new instance of type AABBState.
            /// </summary>
            public AABBState()
            {
                RegisterState(_modelView);
            }
        }

        private readonly IEnumerable<SceneNode> _sncList;
        private readonly AABBState _state = new();
        private bool _boxValid;
        private AABBf _result;

        /// <summary>
        /// Initializes a new instance of the <see cref="AABBCalculator"/> class.
        /// </summary>
        /// <param name="sc">The scene container to calculate an axis-aligned bounding box for.</param>
        public AABBCalculator(SceneContainer sc)
        {
            _sncList = sc.Children;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AABBCalculator"/> class.
        /// </summary>
        /// <param name="sncList">The list of scene nodes to calculate an axis-aligned bounding box for.</param>
        public AABBCalculator(IEnumerable<SceneNode> sncList)
        {
            _sncList = sncList;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AABBCalculator"/> class.
        /// </summary>
        /// <param name="snc">A single scene node to calculate an axis-aligned bounding box for.</param>
        public AABBCalculator(SceneNode snc)
        {
            _sncList = VisitorHelpers.SingleRootEnumerable(snc);
        }

        /// <summary>
        /// Performs the calculation and returns the resulting box on the object(s) passed in the constructor. Any calculation
        /// always includes a full traversal over all child nodes.
        /// </summary>
        /// <returns>The resulting axis-aligned bounding box.</returns>
        public AABBf? GetBox()
        {
            Traverse(_sncList);
            if (_boxValid)
                return _result;
            return null;
        }

        #region Visitors

        /// <summary>
        /// Do not call. Used for internal traversal purposes only
        /// </summary>
        /// <param name="transform">The transform component.</param>
        [VisitMethod]
        public void OnTransform(Transform transform)
        {
            _state.ModelView *= transform.Matrix;
        }

        /// <summary>
        /// Do not call. Used for internal traversal purposes only
        /// </summary>
        /// <param name="mesh">The mesh component.</param>
        [VisitMethod]
        public void OnMesh(Mesh mesh)
        {
            var box = _state.ModelView * mesh.BoundingBox;
            if (!_boxValid)
            {
                _result = box;
                _boxValid = true;
            }
            else
            {
                _result = AABBf.Union(_result, box);
            }
        }

        #endregion

        #region HierarchyLevel

        /// <summary>
        /// Method is called when traversal starts to initialize the traversal state.
        /// </summary>
        protected override void InitState()
        {
            _boxValid = false;
            _state.Clear();
            _state.ModelView = float4x4.Identity;
        }

        /// <summary>
        /// Method is called when going down one hierarchy level while traversing.
        /// </summary>
        protected override void PushState()
        {
            _state.Push();
        }

        /// <summary>
        /// Method is called when going up one hierarchy level while traversing.
        /// </summary>
        protected override void PopState()
        {
            _state.Pop();
        }

        #endregion
    }
}