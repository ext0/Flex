using Flex.Development.Instances.Properties;
using Flex.Development.Rendering;
using Flex.Development.Rendering.Manual;
using Flex.Development.Rendering.Modules;
using Flex.Misc.Tracker;
using Microsoft.ClearScript;
using Mogre;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace Flex.Development.Instances
{
    [Serializable]
    public abstract class SizedInstance : PositionedInstance
    {
        [NonSerialized()]
        private static readonly ColourValue HOVER_LIGHT_COLOR = new ColourValue(0, 159f / 255f, 1, 1);

        [NonSerialized()]
        private static readonly ColourValue HOVER_DARK_COLOR = new ColourValue(0, 96f / 255f, 1, 1);

        protected Properties.Vector3 _size;

        [NonSerialized()]
        protected SceneNode _boundingBoxSceneNode;

        [NonSerialized()]
        protected MaterialPtr _boundingBoxMaterialPtr;

        [NonSerialized()]
        protected bool _showingBoundingBox;

        [NonSerialized()]
        private bool _isSelected;

        [NonSerialized()]
        private float _boundingBoxThickness = 0.5f;

        [NonSerialized()]
        private List<ManualObject> _boundLines;

        protected SizedInstance() : base()
        {
            _boundLines = new List<ManualObject>();

            Engine.QueueForRenderDispatcher(() =>
            {
                _boundingBoxSceneNode = Engine.Renderer.Scene.CreateSceneNode();
                _boundingBoxSceneNode.SetVisible(false);

                _boundingBoxMaterialPtr = ((MaterialPtr)MaterialManager.Singleton.GetByName("BoundingBox/Blue")).Clone("BoundingBoxMaterial/" + Guid.NewGuid().ToString());

                BuildBoundingBox();

                _showingBoundingBox = false;
                _isSelected = false;
            });
        }

        protected void UpdateBoundingBox()
        {
            if (_boundingBoxSceneNode.ParentSceneNode == null)
            {
                if (_sceneNode != null)
                {
                    _sceneNode.AddChild(_boundingBoxSceneNode);
                }
            }
            if (_sceneNode == null)
            {
                return;
            }

            float ax = (size.x / 2);
            float ay = (size.y / 2);
            float az = (size.z / 2);

            float bx = -(size.x / 2);
            float by = -(size.y / 2);
            float bz = -(size.z / 2);

            MeshGenerator.UpdateCube(
                _boundLines[0],
                _boundingBoxThickness + size.x,
                _boundingBoxThickness,
                _boundingBoxThickness,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                Face.BACK | Face.BOTTOM | Face.FRONT | Face.LEFT | Face.RIGHT | Face.TOP,
                new Mogre.Vector3(0, ay, az));

            MeshGenerator.UpdateCube(
                _boundLines[1],
                _boundingBoxThickness + size.x,
                _boundingBoxThickness,
                _boundingBoxThickness,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                Face.BACK | Face.BOTTOM | Face.FRONT | Face.LEFT | Face.RIGHT | Face.TOP,
                new Mogre.Vector3(0, by, az));

            MeshGenerator.UpdateCube(
                _boundLines[2],
                _boundingBoxThickness + size.x,
                _boundingBoxThickness,
                _boundingBoxThickness,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                Face.BACK | Face.BOTTOM | Face.FRONT | Face.LEFT | Face.RIGHT | Face.TOP,
                new Mogre.Vector3(0, ay, bz));

            MeshGenerator.UpdateCube(
                _boundLines[3],
                _boundingBoxThickness + size.x,
                _boundingBoxThickness,
                _boundingBoxThickness,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                Face.BACK | Face.BOTTOM | Face.FRONT | Face.LEFT | Face.RIGHT | Face.TOP,
                new Mogre.Vector3(0, by, bz));

            MeshGenerator.UpdateCube(
                _boundLines[4],
                _boundingBoxThickness,
                _boundingBoxThickness,
                _boundingBoxThickness + size.z,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                Face.BACK | Face.BOTTOM | Face.FRONT | Face.LEFT | Face.RIGHT | Face.TOP,
                new Mogre.Vector3(ax, ay, 0));

            MeshGenerator.UpdateCube(
                _boundLines[5],
                _boundingBoxThickness,
                _boundingBoxThickness,
                _boundingBoxThickness + size.z,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                Face.BACK | Face.BOTTOM | Face.FRONT | Face.LEFT | Face.RIGHT | Face.TOP,
                new Mogre.Vector3(ax, by, 0));

            MeshGenerator.UpdateCube(
                _boundLines[6],
                _boundingBoxThickness,
                _boundingBoxThickness,
                _boundingBoxThickness + size.z,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                Face.BACK | Face.BOTTOM | Face.FRONT | Face.LEFT | Face.RIGHT | Face.TOP,
                new Mogre.Vector3(bx, ay, 0));

            MeshGenerator.UpdateCube(
                _boundLines[7],
                _boundingBoxThickness,
                _boundingBoxThickness,
                _boundingBoxThickness + size.z,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                Face.BACK | Face.BOTTOM | Face.FRONT | Face.LEFT | Face.RIGHT | Face.TOP,
                new Mogre.Vector3(bx, by, 0));

            MeshGenerator.UpdateCube(
                _boundLines[8],
                _boundingBoxThickness,
                _boundingBoxThickness + size.y,
                _boundingBoxThickness,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                Face.BACK | Face.BOTTOM | Face.FRONT | Face.LEFT | Face.RIGHT | Face.TOP,
                new Mogre.Vector3(ax, 0, az));

            MeshGenerator.UpdateCube(
                _boundLines[9],
                _boundingBoxThickness,
                _boundingBoxThickness + size.y,
                _boundingBoxThickness,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                Face.BACK | Face.BOTTOM | Face.FRONT | Face.LEFT | Face.RIGHT | Face.TOP,
                new Mogre.Vector3(bx, 0, az));

            MeshGenerator.UpdateCube(
                _boundLines[10],
                _boundingBoxThickness,
                _boundingBoxThickness + size.y,
                _boundingBoxThickness,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                Face.BACK | Face.BOTTOM | Face.FRONT | Face.LEFT | Face.RIGHT | Face.TOP,
                new Mogre.Vector3(ax, 0, bz));


            MeshGenerator.UpdateCube(
                _boundLines[11],
                _boundingBoxThickness,
                _boundingBoxThickness + size.y,
                _boundingBoxThickness,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                _boundingBoxMaterialPtr,
                Face.BACK | Face.BOTTOM | Face.FRONT | Face.LEFT | Face.RIGHT | Face.TOP,
                new Mogre.Vector3(bx, 0, bz));
        }

        private void BuildBoundingBox()
        {
            if (_boundingBoxSceneNode.ParentSceneNode == null)
            {
                if (_sceneNode != null)
                {
                    _sceneNode.AddChild(_boundingBoxSceneNode);
                }
            }
            if (_sceneNode == null)
            {
                return;
            }
            if (_boundLines.Count != 0)
            {
                throw new InvalidOperationException("Cannot build bounding box - bounding box already exists! Use UpdateBoundingBox() instead.");
            }

            float ax = (size.x / 2);
            float ay = (size.y / 2);
            float az = (size.z / 2);

            float bx = -(size.x / 2);
            float by = -(size.y / 2);
            float bz = -(size.z / 2);

            ManualObject meshPtr;

            meshPtr = MeshGenerator.GenerateCube(
                _boundingBoxThickness + size.x,
                _boundingBoxThickness,
                _boundingBoxThickness,
                Guid.NewGuid().ToString(),
                _boundingBoxMaterialPtr,
                new Mogre.Vector3(0, ay, az));

            _boundLines.Add(meshPtr);

            meshPtr = MeshGenerator.GenerateCube(
                _boundingBoxThickness + size.x,
                _boundingBoxThickness,
                _boundingBoxThickness,
                Guid.NewGuid().ToString(),
                _boundingBoxMaterialPtr,
                new Mogre.Vector3(0, by, az));

            _boundLines.Add(meshPtr);

            meshPtr = MeshGenerator.GenerateCube(
                _boundingBoxThickness + size.x,
                _boundingBoxThickness,
                _boundingBoxThickness,
                Guid.NewGuid().ToString(),
                _boundingBoxMaterialPtr,
                new Mogre.Vector3(0, ay, bz));

            _boundLines.Add(meshPtr);

            meshPtr = MeshGenerator.GenerateCube(
                _boundingBoxThickness + size.x,
                _boundingBoxThickness,
                _boundingBoxThickness,
                Guid.NewGuid().ToString(),
                _boundingBoxMaterialPtr,
                new Mogre.Vector3(0, by, bz));

            _boundLines.Add(meshPtr);

            meshPtr = MeshGenerator.GenerateCube(
                _boundingBoxThickness,
                _boundingBoxThickness,
                _boundingBoxThickness + size.z,
                Guid.NewGuid().ToString(),
                _boundingBoxMaterialPtr,
                new Mogre.Vector3(ax, ay, 0));

            _boundLines.Add(meshPtr);

            meshPtr = MeshGenerator.GenerateCube(
                _boundingBoxThickness,
                _boundingBoxThickness,
                _boundingBoxThickness + size.z,
                Guid.NewGuid().ToString(),
                _boundingBoxMaterialPtr,
                new Mogre.Vector3(ax, by, 0));

            _boundLines.Add(meshPtr);

            meshPtr = MeshGenerator.GenerateCube(
                _boundingBoxThickness,
                _boundingBoxThickness,
                _boundingBoxThickness + size.z,
                Guid.NewGuid().ToString(),
                _boundingBoxMaterialPtr,
                new Mogre.Vector3(bx, ay, 0));

            _boundLines.Add(meshPtr);

            meshPtr = MeshGenerator.GenerateCube(
                _boundingBoxThickness,
                _boundingBoxThickness,
                _boundingBoxThickness + size.z,
                Guid.NewGuid().ToString(),
                _boundingBoxMaterialPtr,
                new Mogre.Vector3(bx, by, 0));

            _boundLines.Add(meshPtr);

            meshPtr = MeshGenerator.GenerateCube(
                _boundingBoxThickness,
                _boundingBoxThickness + size.y,
                _boundingBoxThickness,
                Guid.NewGuid().ToString(),
                _boundingBoxMaterialPtr,
                new Mogre.Vector3(ax, 0, az));

            _boundLines.Add(meshPtr);

            meshPtr = MeshGenerator.GenerateCube(
                _boundingBoxThickness,
                _boundingBoxThickness + size.y,
                _boundingBoxThickness,
                Guid.NewGuid().ToString(),
                _boundingBoxMaterialPtr,
                new Mogre.Vector3(bx, 0, az));

            _boundLines.Add(meshPtr);

            meshPtr = MeshGenerator.GenerateCube(
                _boundingBoxThickness,
                _boundingBoxThickness + size.y,
                _boundingBoxThickness,
                Guid.NewGuid().ToString(),
                _boundingBoxMaterialPtr,
                new Mogre.Vector3(ax, 0, bz));

            _boundLines.Add(meshPtr);

            meshPtr = MeshGenerator.GenerateCube(
                _boundingBoxThickness,
                _boundingBoxThickness + size.y,
                _boundingBoxThickness,
                Guid.NewGuid().ToString(),
                _boundingBoxMaterialPtr,
                new Mogre.Vector3(bx, 0, bz));

            _boundLines.Add(meshPtr);

            foreach (ManualObject manualObject in _boundLines)
            {
                manualObject.QueryFlags = (uint)QueryFlags.IGNORE_ALL;
                _boundingBoxSceneNode.AttachObject(manualObject);
            }
        }

        private void SetSelected()
        {
            if (!_showingBoundingBox)
            {
                return;
            }
            _boundingBoxMaterialPtr.GetTechnique(0).GetPass(0).SetAmbient(HOVER_DARK_COLOR.r, HOVER_DARK_COLOR.g, HOVER_DARK_COLOR.b);
            _boundingBoxMaterialPtr.GetTechnique(0).GetPass(0).SetSpecular(HOVER_DARK_COLOR.r, HOVER_DARK_COLOR.g, HOVER_DARK_COLOR.b, HOVER_DARK_COLOR.a);
            _boundingBoxMaterialPtr.GetTechnique(0).GetPass(0).SetDiffuse(HOVER_DARK_COLOR.r, HOVER_DARK_COLOR.g, HOVER_DARK_COLOR.b, HOVER_DARK_COLOR.a);
        }

        private void SetHover()
        {
            if (!_showingBoundingBox)
            {
                return;
            }
            _boundingBoxMaterialPtr.GetTechnique(0).GetPass(0).SetAmbient(HOVER_LIGHT_COLOR.r, HOVER_LIGHT_COLOR.g, HOVER_LIGHT_COLOR.b);
            _boundingBoxMaterialPtr.GetTechnique(0).GetPass(0).SetSpecular(HOVER_LIGHT_COLOR.r, HOVER_LIGHT_COLOR.g, HOVER_LIGHT_COLOR.b, HOVER_LIGHT_COLOR.a);
            _boundingBoxMaterialPtr.GetTechnique(0).GetPass(0).SetDiffuse(HOVER_LIGHT_COLOR.r, HOVER_LIGHT_COLOR.g, HOVER_LIGHT_COLOR.b, HOVER_LIGHT_COLOR.a);
        }

        [Browsable(false)]
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (value == _isSelected) return;

                _isSelected = value;

                if (value)
                {
                    SetSelected();
                }
                else
                {
                    SetHover();
                }
            }
        }

        [Browsable(false)]
        public bool IsBoundingBoxEnabled
        {
            get
            {
                return _showingBoundingBox;
            }
            set
            {
                if (value == _showingBoundingBox) return;

                if (value)
                {
                    ShowBoundingBox();
                }
                else
                {
                    HideBoundingBox();
                }
                _showingBoundingBox = value;

            }
        }

        private void ShowBoundingBox()
        {
            if (_boundLines.Count == 0)
            {
                BuildBoundingBox();
            }
            if (_showingBoundingBox || _movableObject == null || _boundLines.Count == 0)
            {
                return;
            }
            _boundingBoxSceneNode.SetVisible(true);
            _showingBoundingBox = true;
            SetHover();
        }

        private void HideBoundingBox()
        {
            if (!_showingBoundingBox || _movableObject == null || _boundLines.Count == 0)
            {
                return;
            }
            _boundingBoxSceneNode.SetVisible(false);
            _showingBoundingBox = false;
        }

        [Category("3D")]
        [DisplayName("Size")]
        [Description("The size of this instance")]
        [ExpandableObject]
        [ScriptMember(ScriptAccess.Full)]
        public abstract Properties.Vector3 size
        {
            get;
            set;
        }
    }
}
