using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;

namespace Fusee.Tools.FusViewer.ViewModel
{
    internal class OpenFusFileCmd : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        private readonly MainWindowViewModel _caller;

        public OpenFusFileCmd(MainWindowViewModel caller)
        {
            _caller = caller;
        }
        public void Execute(object parameter)
        {
            var openFileDialog = new OpenFileDialog()
            {
                Filter = "Fusee Scene|*.fus"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                _caller.PathToFile = openFileDialog.FileName;
                using var stream = File.OpenRead(openFileDialog.FileName);
                _caller.CurrentContainer = FusSceneConverter.ConvertFrom(ProtoBuf.Serializer.Deserialize<FusFile>(stream));
            }
        }

    }

    internal class SaveAsJSONCmd : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        private readonly MainWindowViewModel _caller;

        public SaveAsJSONCmd(MainWindowViewModel caller)
        {
            _caller = caller;
        }
        public async void Execute(object parameter)
        {
            var saveFileDialog = new SaveFileDialog()
            {
                AddExtension = true,
                Filter = "Json|*.json"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                var converted = new SceneToJSONConv().Convert(_caller.CurrentContainer);
                var conv = JsonSerializer.Serialize(converted);
                await File.WriteAllTextAsync(saveFileDialog.FileName, conv).ConfigureAwait(false);
                MessageBox.Show("Saved json");
            }
        }
    }

    internal class ViewInPlayerCmd : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        private readonly MainWindowViewModel _caller;

        public ViewInPlayerCmd(MainWindowViewModel caller)
        {
            _caller = caller;
        }
        public void Execute(object parameter)
        {
            // TODO: Spawn fusee window   
        }
    }

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bool FusLoaded => PathToFile != null;

        public string PathToFile
        {
            get => _pathToFile;
            set
            {
                _pathToFile = value;
                OnPropertyChanged(nameof(PathToFile));
                OnPropertyChanged(nameof(FusLoaded));
            }
        }

        public SceneContainer CurrentContainer
        {
            get => _scene;
            set
            {
                _scene = value;
                _sceneAsTreeItem = new SceneToTreeConv().Convert(value);
                OnPropertyChanged(nameof(CurrentContainer));
                OnPropertyChanged(nameof(SceneAsTreeView));
            }
        }

        public List<TreeItem> SceneAsTreeView => new List<TreeItem> { _sceneAsTreeItem };

        private string _pathToFile;
        private SceneContainer _scene;
        private TreeItem _sceneAsTreeItem;

        public MainWindowViewModel()
        {
            _sceneAsTreeItem = new TreeItem { Title = "Please select *.fus scene" };
        }

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ICommand OpenFusFile => new OpenFusFileCmd(this);

        public ICommand SaveAsJSON => new SaveAsJSONCmd(this);

        public ICommand ViewInPlayer => new ViewInPlayerCmd(this);
    }

    public class SceneToTreeConv : Visitor<SceneNode, SceneComponent>
    {
        private TreeItem _convertedScene;
        private Stack<TreeItem> _predecessors;
        private TreeItem _currentNode;

        protected override void PopState()
        {
            _predecessors.Pop();
        }

        /// <summary>
        /// Traverses the given SceneContainer and creates new high level graph by converting and/or splitting its components into the high level equivalents.
        /// </summary>
        /// <param name="sc">The SceneContainer to convert.</param>
        /// <returns></returns>
        public TreeItem Convert(SceneContainer sc)
        {
            _predecessors = new Stack<TreeItem>();
            _convertedScene = new TreeItem
            {
                Title = $"Scene created {(sc.Header.CreationDate == string.Empty ? "unknown" : sc.Header.CreationDate)} by {sc.Header.CreatedBy}" +
                        $", generated via {sc.Header.Generator}"
            };

            Traverse(sc.Children);

            return _convertedScene;
        }

        #region Visitors
        /// <summary>
        /// Converts the scene node container.
        /// </summary>
        /// <param name="snc"></param>
        [VisitMethod]
        public void ConvSceneNodeContainer(SceneNode snc)
        {
            if (_predecessors.Count != 0)
            {
                var parent = _predecessors.Peek();

                if (parent.Children == null)
                    parent.Children = new ObservableCollection<TreeItem>();

                _currentNode = new TreeItem { Title = (snc.Name == string.Empty ? "[SceneContainer]: Unnamed" : $"[SceneContainer]: {snc.Name}") };
                parent.Children.Add(_currentNode);
                _predecessors.Push(_currentNode);
            }
            else //Add first node to SceneContainer
            {
                _predecessors.Push(new TreeItem { Title = (CurrentNode.Name == null ? "[SceneContainer]: Unnamed" : $"[SceneContainer]: {CurrentNode.Name}") });
                _currentNode = _predecessors.Peek();
                if (_convertedScene.Children != null)
                    _convertedScene.Children.Add(_currentNode);
                else
                    _convertedScene.Children = new ObservableCollection<TreeItem> { _currentNode };
            }
        }

        ///<summary>
        ///Converts the transform component.
        ///</summary>
        [VisitMethod]
        public void ConvTransform(Transform transform)
        {
            if (_currentNode.Components == null)
                _currentNode.Components = new ObservableCollection<TreeComponentItem>();

            _currentNode.Components.Add(new TreeComponentItem
            {
                Name = "Transform Component",
                Desc = $"{transform.Name}, Scale: {transform.Scale}, Rotation: {transform.Rotation}, Translation: {transform.Translation}"
            });
        }

        /// <summary>
        /// Converts the physically based rendering component
        /// </summary>
        /// <param name="matComp"></param>
        [VisitMethod]
        public void ConvMaterial(ShaderEffect sfx)
        {
            _currentNode.Components.Add(new TreeComponentItem
            {
                Name = "Material PBR Component",
                Desc = $"{sfx.Name}, Diffuse: {sfx.GetFxParam<float4>(UniformNameDeclarations.Albedo)}, Specular: {sfx.GetFxParam<float4>(UniformNameDeclarations.Albedo)}"
            });
        }

        /// <summary>
        /// Converts the shader.
        /// </summary>
        [VisitMethod]
        public void ConvCamComp(Camera camComp)
        {
            _currentNode.Components.Add(new TreeComponentItem
            {
                Name = "Camera Component",
                Desc = $"{camComp}, Mode: {camComp.ProjectionMethod}, FOV: {camComp.Fov}, Near/Far: {camComp.ClippingPlanes.x}/{camComp.ClippingPlanes.y}"
            });
        }

        /// <summary>
        /// Converts the mesh.
        /// </summary>
        /// <param name="mesh">The mesh to convert.</param>
        [VisitMethod]
        public void ConvMesh(Mesh mesh)
        {
            var comp = new TreeComponentItem
            {
                Name = "Mesh Component",
                Desc = $"{mesh.Name}, Type: {mesh.MeshType} "
            };

            if (mesh.NormalsSet)
                comp.Desc += $"Normals Cnt: {mesh.Normals.Length} ";
            if (mesh.VerticesSet)
                comp.Desc += $"Vertices Cnt: {mesh.Vertices.Length} ";
            if (mesh.TrianglesSet)
                comp.Desc += $"Triangles Cnt: {mesh.Triangles.Length} ";
            if (mesh.UVsSet)
                comp.Desc += $"UVs Cnt: {mesh.UVs.Length} ";


            _currentNode.Components.Add(comp);
        }

        /// <summary>
        /// Adds the light component.
        /// </summary>
        /// <param name="lightComponent"></param>
        [VisitMethod]
        public void ConvLight(Light lightComponent)
        {
            _currentNode.Components.Add(new TreeComponentItem
            {
                Name = "Light Component",
                Desc = $"{lightComponent.Name}, Color: {lightComponent.Color}"
            });
        }

        /// <summary>
        /// Adds the bone component.
        /// </summary>
        /// <param name="bone"></param>
        [VisitMethod]
        public void ConvBone(Bone bone)
        {
            _currentNode.Components.Add(new TreeComponentItem
            {
                Name = "Bone Component",
                Desc = $"{bone.Name}"
            });
        }

        /// <summary>
        /// Converts the weight component.
        /// </summary>
        /// <param name="weight"></param>
        [VisitMethod]
        public void ConVWeight(Weight weight)
        {
            _currentNode.Components.Add(new TreeComponentItem
            {
                Name = "Weight Component",
                Desc = $"{weight.Name}, Matrices: {weight.BindingMatrices}"
            });
        }
        #endregion

    }


    public class SceneToJSONConv : Visitor<SceneNode, SceneComponent>
    {
        private JSONItem _convertedScene;
        private Stack<JSONItem> _predecessors;
        private JSONItem _currentNode;

        protected override void PopState()
        {
            _predecessors.Pop();
        }

        /// <summary>
        /// Traverses the given SceneContainer and creates new high level graph by converting and/or splitting its components into the high level equivalents.
        /// </summary>
        /// <param name="sc">The SceneContainer to convert.</param>
        /// <returns></returns>
        public JSONItem Convert(SceneContainer sc)
        {
            _predecessors = new Stack<JSONItem>();
            _convertedScene = new JSONItem
            {
                Title = $"Scene created {(sc.Header.CreationDate == string.Empty ? "unknown" : sc.Header.CreationDate)} by {sc.Header.CreatedBy}" +
                        $", generated via {sc.Header.Generator}"
            };

            Traverse(sc.Children);

            return _convertedScene;
        }

        #region Visitors
        /// <summary>
        /// Converts the scene node container.
        /// </summary>
        /// <param name="snc"></param>
        [VisitMethod]
        public void ConvSceneNodeContainer(SceneNode snc)
        {
            if (_predecessors.Count != 0)
            {
                var parent = _predecessors.Peek();

                if (parent.Children == null)
                    parent.Children = new List<JSONItem>();

                _currentNode = new JSONItem { Title = (snc.Name == string.Empty ? "[SceneContainer]: Unnamed" : $"[SceneContainer]: {snc.Name}") };
                parent.Children.Add(_currentNode);
                _predecessors.Push(_currentNode);
            }
            else //Add first node to SceneContainer
            {
                _predecessors.Push(new JSONItem { Title = (CurrentNode.Name == null ? "[SceneContainer]: Unnamed" : $"[SceneContainer]: {CurrentNode.Name}") });
                _currentNode = _predecessors.Peek();
                if (_convertedScene.Children != null)
                    _convertedScene.Children.Add(_currentNode);
                else
                    _convertedScene.Children = new List<JSONItem> { _currentNode };
            }
        }

        ///<summary>
        ///Converts the transform component.
        ///</summary>
        [VisitMethod]
        public void ConvTransform(Transform transform)
        {
            if (_currentNode.Components == null)
                _currentNode.Components = new List<JSONComponentItem>();

            _currentNode.Components.Add(new JSONComponentItem
            {
                Name = "Transform Component",
                Desc = $"{transform.Name}, Scale: {transform.Scale}, Rotation: {transform.Rotation}, Translation: {transform.Translation}"
            });
        }

        /// <summary>
        /// Converts the material.
        /// </summary>
        /// <param name="matComp"></param>
        [VisitMethod]
        public void ConvMaterial(ShaderEffect sfx)
        {
            _currentNode.Components.Add(new JSONComponentItem
            {
                Name = "Material Component",
                Desc = $"{sfx.Name}, Diffuse: {sfx.GetFxParam<float4>(UniformNameDeclarations.Albedo)}"
            });
        }

        /// <summary>
        /// Converts the shader.
        /// </summary>
        [VisitMethod]
        public void ConvCameraComp(Camera camComp)
        {
            _currentNode.Components.Add(new JSONComponentItem
            {
                Name = "Camera Component",
                Desc = $"{camComp}, Mode: {camComp.ProjectionMethod.ToString()}, FOV: {camComp.Fov}, Near/Far: {camComp.ClippingPlanes.x}/{camComp.ClippingPlanes.y}"
            });
        }

        /// <summary>
        /// Converts the mesh.
        /// </summary>
        /// <param name="mesh">The mesh to convert.</param>
        [VisitMethod]
        public void ConvMesh(Mesh mesh)
        {
            var comp = new JSONComponentItem
            {
                Name = "Mesh Component",
                Desc = $"{mesh.Name}, Type: {mesh.MeshType} "
            };

            if (mesh.NormalsSet)
                comp.Desc += $"Normals Cnt: {mesh.Normals.Length} ";
            if (mesh.VerticesSet)
                comp.Desc += $"Vertices Cnt: {mesh.Vertices.Length} ";
            if (mesh.TrianglesSet)
                comp.Desc += $"Triangles Cnt: {mesh.Triangles.Length} ";
            if (mesh.UVsSet)
                comp.Desc += $"UVs Cnt: {mesh.UVs.Length} ";


            _currentNode.Components.Add(comp);
        }

        /// <summary>
        /// Adds the light component.
        /// </summary>
        /// <param name="lightComponent"></param>
        [VisitMethod]
        public void ConvLight(Light lightComponent)
        {
            _currentNode.Components.Add(new JSONComponentItem
            {
                Name = "Light Component",
                Desc = $"{lightComponent.Name}, Color: {lightComponent.Color}"
            });
        }

        /// <summary>
        /// Adds the bone component.
        /// </summary>
        /// <param name="bone"></param>
        [VisitMethod]
        public void ConvBone(Bone bone)
        {
            _currentNode.Components.Add(new JSONComponentItem
            {
                Name = "Bone Component",
                Desc = $"{bone.Name}"
            });
        }

        /// <summary>
        /// Converts the weight component.
        /// </summary>
        /// <param name="weight"></param>
        [VisitMethod]
        public void ConVWeight(Weight weight)
        {
            _currentNode.Components.Add(new JSONComponentItem
            {
                Name = "Weight Component",
                Desc = $"{weight.Name}, Matrices: {weight.BindingMatrices}"
            });
        }
        #endregion

    }

    /// <summary>
    ///     Helper structure for tree view representation
    /// </summary>
    public class TreeItem
    {
        public string Title { get; set; }

        public ObservableCollection<TreeComponentItem> Components { get; set; }

        public ObservableCollection<TreeItem> Children { get; set; }
    }

    /// <summary>
    ///     Helper structure for tree view representation
    /// </summary>
    public class TreeComponentItem
    {
        public string Name { get; set; }
        public string Desc { get; set; }

        public string AsString => $"[{Name}], {Desc}";
    }


    /// <summary>
    ///     Helper structure for json representation
    /// </summary>
    [Serializable]
    public class JSONItem
    {
        public string Title { get; set; }

        public List<JSONComponentItem> Components { get; set; }

        public List<JSONItem> Children { get; set; }
    }

    /// <summary>
    ///     Helper structure for tree view representation
    /// </summary>
    [Serializable]
    public class JSONComponentItem
    {
        public string Name { get; set; }
        public string Desc { get; set; }
    }


}

