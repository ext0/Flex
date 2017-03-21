using Caliburn.Micro;
using Flex.Development.Execution.Data;
using Flex.Development.Instances;
using Flex.Development.Physics;
using Flex.Development.Rendering.Modules.Enum;
using Flex.Modules.Explorer;
using Flex.Modules.Scene.ViewModels;
using Gemini.Framework.Services;
using Mogre;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace Flex.Development.Rendering.Modules
{
    public class MouseHandler
    {
        private static readonly int MAX_TRANSLATE_DRAG_DISTANCE = 256;
        private static readonly int TRANSLATE_DRAG_THRESHOLD = 1;

        private SceneNode _gizmoNode;

        private SceneNode _pointerNode;

        private SceneNode _translateNode;

        private Vector3 _translateDragDifference;

        private SceneNode _xArrowNodeA;
        private SceneNode _yArrowNodeA;
        private SceneNode _zArrowNodeA;

        private SceneNode _xArrowNodeB;
        private SceneNode _yArrowNodeB;
        private SceneNode _zArrowNodeB;

        private TransformDragging _translateDragging;

        private static readonly Vector3 SCALE_HANDLE_SCALE = new Vector3(2, 2, 2);

        private SceneNode _scaleNode;

        private Vector3 _scaleDragDifference;

        private SceneNode _xScaleNodeA;
        private SceneNode _yScaleNodeA;
        private SceneNode _zScaleNodeA;

        private SceneNode _xScaleNodeB;
        private SceneNode _yScaleNodeB;
        private SceneNode _zScaleNodeB;

        private DirectionalTransformDragging _scaleDragging;

        private SceneNode _rotateNode;

        private System.Windows.Forms.Integration.WindowsFormsHost _host;
        private Panel _panel;

        private Point _oldMousePosition;

        private Dictionary<SelectionType, List<SceneNode>> _boundingBoxes;

        private bool _translateFreeDragging;

        public MouseHandler(System.Windows.Forms.Integration.WindowsFormsHost host, Panel panel)
        {
            _boundingBoxes = new Dictionary<SelectionType, List<SceneNode>>();
            _translateFreeDragging = false;
            _panel = panel;
            _host = host;
        }

        public void Initialize()
        {
            _panel.MouseMove += _panel_MouseMove;
            _panel.MouseClick += _panel_MouseClick;
            _panel.MouseWheel += _panel_MouseWheel;
            _panel.MouseDown += _panel_MouseDown;
            _panel.MouseUp += _panel_MouseUp;

            CreatePointerEntity();
            CreateTranslateEntity();
            CreateScaleEntity();
            CreateRotateEntity();

            ActiveScene.GizmoChanged += ActiveScene_GizmoChanged;

            ActiveScene_GizmoChanged(null, null);
        }

        private void ActiveScene_GizmoChanged(object sender, EventArgs e)
        {
            SceneNode node;
            if (ExistsSelectedNode(out node))
            {
                PositionedInstance instance = Engine.SceneNodeStore.GetInstance(node);
                if (instance != null)
                {
                    instance.RemoveGizmoVisual();
                }
            }

            switch (ActiveScene.ActiveGizmoType)
            {
                case Misc.Runtime.GizmoType.POINTER:
                    _gizmoNode = _pointerNode;
                    break;
                case Misc.Runtime.GizmoType.TRANSLATE:
                    _gizmoNode = _translateNode;
                    break;
                case Misc.Runtime.GizmoType.SCALE:
                    _gizmoNode = _scaleNode;
                    break;
                case Misc.Runtime.GizmoType.ROTATE:
                    _gizmoNode = _rotateNode;
                    break;
            }

            if (ExistsSelectedNode(out node))
            {
                PositionedInstance instance = Engine.SceneNodeStore.GetInstance(node);
                if (instance != null)
                {
                    instance.SetGizmoVisual(new GizmoVisual(_gizmoNode));
                }
            }
        }

        private void CreatePointerEntity()
        {
            _pointerNode = Engine.Renderer.Scene.CreateSceneNode();
        }

        private void CreateTranslateEntity()
        {
            _translateNode = Engine.Renderer.Scene.CreateSceneNode();

            Entity xArrowEntityA;
            _xArrowNodeA = Engine.Renderer.CreateEntity(out xArrowEntityA, "arrow.mesh", "XA");
            _xArrowNodeA.Rotate(Vector3.UNIT_Y, new Degree(90f), Node.TransformSpace.TS_WORLD);
            xArrowEntityA.SetMaterialName("Translate/X");
            _xArrowNodeA.InheritScale = false;
            _xArrowNodeA.InheritOrientation = false;

            Entity yArrowEntityA;
            _yArrowNodeA = Engine.Renderer.CreateEntity(out yArrowEntityA, "arrow.mesh", "YA");
            _yArrowNodeA.Rotate(Vector3.UNIT_X, new Degree(-90f), Node.TransformSpace.TS_WORLD);
            yArrowEntityA.SetMaterialName("Translate/Y");
            _yArrowNodeA.InheritScale = false;
            _yArrowNodeA.InheritOrientation = false;

            Entity zArrowEntityA;
            _zArrowNodeA = Engine.Renderer.CreateEntity(out zArrowEntityA, "arrow.mesh", "ZA");
            zArrowEntityA.SetMaterialName("Translate/Z");
            _zArrowNodeA.InheritScale = false;
            _zArrowNodeA.InheritOrientation = false;

            Entity xArrowEntityB;
            _xArrowNodeB = Engine.Renderer.CreateEntity(out xArrowEntityB, "arrow.mesh", "XB");
            _xArrowNodeB.Rotate(Vector3.UNIT_Y, new Degree(-90f), Node.TransformSpace.TS_WORLD);
            xArrowEntityB.SetMaterialName("Translate/X");
            _xArrowNodeB.InheritScale = false;
            _xArrowNodeB.InheritOrientation = false;

            Entity yArrowEntityB;
            _yArrowNodeB = Engine.Renderer.CreateEntity(out yArrowEntityB, "arrow.mesh", "YB");
            _yArrowNodeB.Rotate(Vector3.UNIT_X, new Degree(90f), Node.TransformSpace.TS_WORLD);
            yArrowEntityB.SetMaterialName("Translate/Y");
            _yArrowNodeB.InheritScale = false;
            _yArrowNodeB.InheritOrientation = false;

            Entity zArrowEntityB;
            _zArrowNodeB = Engine.Renderer.CreateEntity(out zArrowEntityB, "arrow.mesh", "ZB");
            zArrowEntityB.SetMaterialName("Translate/Z");
            _zArrowNodeB.Rotate(Vector3.UNIT_Y, new Degree(180f), Node.TransformSpace.TS_WORLD);
            _zArrowNodeB.InheritScale = false;
            _zArrowNodeB.InheritOrientation = false;

            _translateNode.AddChild(_xArrowNodeA);
            _translateNode.AddChild(_yArrowNodeA);
            _translateNode.AddChild(_zArrowNodeA);

            _translateNode.AddChild(_xArrowNodeB);
            _translateNode.AddChild(_yArrowNodeB);
            _translateNode.AddChild(_zArrowNodeB);
        }

        private void CreateScaleEntity()
        {
            _scaleNode = Engine.Renderer.Scene.CreateSceneNode();

            Entity xScaleEntityA;
            _xScaleNodeA = Engine.Renderer.CreateEntity(out xScaleEntityA, "scaleSphereXA.mesh", "XA");
            xScaleEntityA.SetMaterialName("Scale/X");
            _xScaleNodeA.InheritScale = false;
            _xScaleNodeA.SetScale(SCALE_HANDLE_SCALE);

            Entity yScaleEntityA;
            _yScaleNodeA = Engine.Renderer.CreateEntity(out yScaleEntityA, "scaleSphereYA.mesh", "YA");
            yScaleEntityA.SetMaterialName("Scale/Y");
            _yScaleNodeA.InheritScale = false;
            _yScaleNodeA.SetScale(SCALE_HANDLE_SCALE);

            Entity zScaleEntityA;
            _zScaleNodeA = Engine.Renderer.CreateEntity(out zScaleEntityA, "scaleSphereZA.mesh", "ZA");
            zScaleEntityA.SetMaterialName("Scale/Z");
            _zScaleNodeA.InheritScale = false;
            _zScaleNodeA.SetScale(SCALE_HANDLE_SCALE);

            /*
            Entity xScaleEntityB;
            _xScaleNodeB = Engine.Renderer.CreateEntity(out xScaleEntityB, "scaleSphereXB.mesh", "XB");
            xScaleEntityB.SetMaterialName("Scale/X");
            _xScaleNodeB.InheritScale = false;
            _xScaleNodeB.SetScale(SCALE_HANDLE_SCALE);

            Entity yScaleEntityB;
            _yScaleNodeB = Engine.Renderer.CreateEntity(out yScaleEntityB, "scaleSphereYB.mesh", "YB");
            yScaleEntityB.SetMaterialName("Scale/Y");
            _yScaleNodeB.InheritScale = false;
            _yScaleNodeB.SetScale(SCALE_HANDLE_SCALE);

            Entity zScaleEntityB;
            _zScaleNodeB = Engine.Renderer.CreateEntity(out zScaleEntityB, "scaleSphereZB.mesh", "ZB");
            zScaleEntityB.SetMaterialName("Scale/Z");
            _zScaleNodeB.InheritScale = false;
            _zScaleNodeB.SetScale(SCALE_HANDLE_SCALE);
            */
            _scaleNode.AddChild(_xScaleNodeA);
            _scaleNode.AddChild(_yScaleNodeA);
            _scaleNode.AddChild(_zScaleNodeA);

            /*
            _scaleNode.AddChild(_xScaleNodeB);
            _scaleNode.AddChild(_yScaleNodeB);
            _scaleNode.AddChild(_zScaleNodeB);

            */

            /*
            _xScaleNodeA._setDerivedPosition(new Vector3(0.5f, 0, 0));
            _yScaleNodeA._setDerivedPosition(new Vector3(0, 0.5f, 0));
            _zScaleNodeA._setDerivedPosition(new Vector3(0, 0, 0.5f));

            _xScaleNodeB._setDerivedPosition(new Vector3(-0.5f, 0, 0));
            _yScaleNodeB._setDerivedPosition(new Vector3(0, -0.5f, 0));
            _zScaleNodeB._setDerivedPosition(new Vector3(0, 0, -0.5f));
            */
        }

        private void CreateRotateEntity()
        {
            _rotateNode = Engine.Renderer.Scene.CreateSceneNode();
        }

        private bool IsTranslateClick(SceneNode checking, out TransformDragging translateDragging)
        {
            if (checking.Equals(_xArrowNodeA) || checking.Equals(_xArrowNodeB))
            {
                translateDragging = TransformDragging.X;
                return true;
            }
            else if (checking.Equals(_yArrowNodeA) || checking.Equals(_yArrowNodeB))
            {
                translateDragging = TransformDragging.Y;
                return true;
            }
            else if (checking.Equals(_zArrowNodeA) || checking.Equals(_zArrowNodeB))
            {
                translateDragging = TransformDragging.Z;
                return true;
            }
            translateDragging = TransformDragging.NONE;
            return false;
        }

        private bool IsScaleClick(SceneNode checking, out DirectionalTransformDragging scaleDragging)
        {
            if (checking.Equals(_xScaleNodeA))
            {
                scaleDragging = DirectionalTransformDragging.XA;
                return true;
            }
            else if (checking.Equals(_xScaleNodeB))
            {
                scaleDragging = DirectionalTransformDragging.XB;
                return true;
            }
            else if (checking.Equals(_yScaleNodeA))
            {
                scaleDragging = DirectionalTransformDragging.YA;
                return true;
            }
            else if (checking.Equals(_yScaleNodeB))
            {
                scaleDragging = DirectionalTransformDragging.YB;
                return true;
            }
            else if (checking.Equals(_zScaleNodeA))
            {
                scaleDragging = DirectionalTransformDragging.ZA;
                return true;
            }
            else if (checking.Equals(_zScaleNodeB))
            {
                scaleDragging = DirectionalTransformDragging.ZB;
                return true;
            }
            scaleDragging = DirectionalTransformDragging.NONE;
            return false;
        }

        private IEnumerable<SceneNode> GetHoveredNodes()
        {
            return _boundingBoxes.ContainsKey(SelectionType.HOVER) ? _boundingBoxes[SelectionType.HOVER] : null;
        }

        public bool IsAlreadyHovered(SceneNode node)
        {
            IEnumerable<SceneNode> selected = GetHoveredNodes();
            return (selected != null) ? selected.Contains(node) : false;
        }

        private void MarkAsHovered(SceneNode input)
        {
            if (!_boundingBoxes.ContainsKey(SelectionType.HOVER))
            {
                _boundingBoxes[SelectionType.HOVER] = new List<SceneNode>();
            }
            _boundingBoxes[SelectionType.HOVER].Add(input);
        }

        public void ClearHovered()
        {
            if (_boundingBoxes.ContainsKey(SelectionType.HOVER))
            {
                _boundingBoxes[SelectionType.HOVER].Clear();
            }
        }

        private IEnumerable<SceneNode> GetSelectedNode()
        {
            return _boundingBoxes.ContainsKey(SelectionType.SELECT) ? _boundingBoxes[SelectionType.SELECT] : null;
        }

        private bool ExistsSelectedNode(out SceneNode node)
        {
            IEnumerable<SceneNode> selected = GetSelectedNode();
            if (selected == null)
            {
                node = null;
                return false;
            }
            else
            {
                bool ret = selected.Count() != 0;
                if (ret)
                {
                    node = selected.First();
                }
                else
                {
                    node = null;
                }
                return ret;
            }
        }

        public bool IsSelectedNode(SceneNode node)
        {
            IEnumerable<SceneNode> selected = GetSelectedNode();
            if (selected == null)
            {
                return false;
            }
            else
            {
                return node.Equals(selected.FirstOrDefault());
            }
        }

        private void SetSelectedNode(SceneNode node)
        {
            if (!_boundingBoxes.ContainsKey(SelectionType.SELECT))
            {
                _boundingBoxes[SelectionType.SELECT] = new List<SceneNode>();
            }
            if (_boundingBoxes[SelectionType.SELECT].Count != 0)
            {
                _boundingBoxes[SelectionType.SELECT].Clear();
            }
            _boundingBoxes[SelectionType.SELECT].Add(node);
        }

        public void ClearSelectedNode()
        {
            SceneNode node = null;
            if (_boundingBoxes.ContainsKey(SelectionType.SELECT))
            {
                node = _boundingBoxes[SelectionType.SELECT].FirstOrDefault();
                _boundingBoxes[SelectionType.SELECT].Clear();
            }
            if (_boundingBoxes.ContainsKey(SelectionType.HOVER) && node != null)
            {
                if (_boundingBoxes[SelectionType.HOVER].Contains(node))
                {
                    _boundingBoxes[SelectionType.HOVER].Remove(node);
                }
            }
        }

        public void SetActiveSelectedNode(SceneNode newNode)
        {
            PositionedInstance instance = Engine.SceneNodeStore.GetInstance(newNode);
            if (instance == null)
            {
                return;
            }
            SceneNode node;
            if (ExistsSelectedNode(out node))
            {
                if (!newNode.Equals(node))
                {
                    PositionedInstance oldInstance = Engine.SceneNodeStore.GetInstance(node);
                    if (oldInstance != null)
                    {
                        oldInstance.IsSelected = false;
                        oldInstance.IsBoundingBoxEnabled = false;
                        oldInstance.RemoveGizmoVisual();
                        ClearSelectedNode();
                    }
                    instance.IsBoundingBoxEnabled = true;
                    instance.IsSelected = true;
                    SetSelectedNode(newNode);

                    instance.SetGizmoVisual(new GizmoVisual(_gizmoNode));
                }
            }
            else
            {
                instance.IsBoundingBoxEnabled = true;
                instance.IsSelected = true;
                SetSelectedNode(newNode);

                instance.SetGizmoVisual(new GizmoVisual(_gizmoNode));
            }
        }

        public void AddToHoverNode(SceneNode newNode)
        {
            if (!IsAlreadyHovered(newNode))
            {
                if (!IsSelectedNode(newNode))
                {
                    PositionedInstance instance = Engine.SceneNodeStore.GetInstance(newNode);
                    if (instance != null)
                    {
                        instance.IsBoundingBoxEnabled = true;
                        MarkAsHovered(newNode);
                    }
                }
            }
        }

        private void _panel_MouseUp(object sender, MouseEventArgs e)
        {
            _translateDragging = TransformDragging.NONE;
            _scaleDragging = DirectionalTransformDragging.NONE;
            if (_translateFreeDragging)
            {
                _translateFreeDragging = false;
            }
        }

        private void _panel_MouseDown(object sender, MouseEventArgs e)
        {
            Point point = new Point(e.X, e.Y);
            Engine.QueueForRenderDispatcher(() =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    Ray ray;
                    RaySceneQueryResult.Enumerator itr = GetRaySceneQuery(point.X, point.Y, out ray);

                    if (itr != null)
                    {
                        if (itr.MoveNext())
                        {
                            RaySceneQueryResultEntry entry = itr.Current;
                            SceneNode parentNode = entry.movable.ParentSceneNode;

                            TransformDragging dragging;
                            DirectionalTransformDragging directionalDragging;
                            if (IsTranslateClick(parentNode, out dragging))
                            {
                                _translateDragging = dragging;
                                IEnumerable<SceneNode> selected = GetSelectedNode();
                                if (selected != null)
                                {
                                    SceneNode node = selected.FirstOrDefault();
                                    if (node != null)
                                    {
                                        _translateDragDifference = ray.GetPoint(entry.distance) - node.Position;
                                    }
                                }
                                return;
                            }
                            else if (IsScaleClick(parentNode, out directionalDragging))
                            {
                                _scaleDragging = directionalDragging;
                                IEnumerable<SceneNode> selected = GetSelectedNode();
                                if (selected != null)
                                {
                                    SceneNode node = selected.FirstOrDefault();
                                    if (node != null)
                                    {
                                        _scaleDragDifference = ray.GetPoint(entry.distance) - node.Position;
                                    }
                                }
                                return;
                            }
                            else
                            {
                                _translateDragging = TransformDragging.NONE;
                            }
                        }
                    }
                }
            });
        }

        private void _panel_MouseClick(object sender, MouseEventArgs e)
        {
            Point point = new Point(e.X, e.Y);
            Engine.QueueForRenderDispatcher(() =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    Ray ray;
                    RaySceneQueryResult.Enumerator itr = GetRaySceneQuery(point.X, point.Y, out ray);

                    if (itr != null)
                    {
                        if (itr.MoveNext())
                        {
                            RaySceneQueryResultEntry entry = itr.Current;
                            SceneNode parentNode = entry.movable.ParentSceneNode;

                            PositionedInstance instance = Engine.SceneNodeStore.GetInstance(parentNode);

                            if (instance != null)
                            {
                                Engine.RunOnUIThread(() =>
                                {
                                    IoC.Get<IExplorer>().SelectInstance(instance);
                                });

                                ActiveScene.SelectedInstance = instance;

                                SetActiveSelectedNode(parentNode);
                            }
                        }
                        else
                        {
                            SceneNode node;
                            if (ExistsSelectedNode(out node))
                            {
                                PositionedInstance oldInstance = Engine.SceneNodeStore.GetInstance(node);
                                if (oldInstance != null)
                                {
                                    oldInstance.IsSelected = false;
                                    oldInstance.IsBoundingBoxEnabled = false;
                                }

                                ActiveScene.SelectedInstance = null;

                                oldInstance.RemoveGizmoVisual();

                                ClearSelectedNode();
                            }
                        }
                    }
                }
            });
        }

        private static RaySceneQueryResult.Enumerator GetRaySceneQuery(double x, double y, out Ray ray)
        {
            float width = Engine.Renderer.Camera.Viewport.ActualWidth;
            float height = Engine.Renderer.Camera.Viewport.ActualHeight;

            Ray mouseRay = Engine.Renderer.Camera.GetCameraToViewportRay((float)((x - 1) / width), (float)((y - 1) / height));

            RaySceneQuery mRaySceneQuery = Engine.Renderer.Scene.CreateRayQuery(mouseRay);
            mRaySceneQuery.SetSortByDistance(true, 16);
            mRaySceneQuery.QueryTypeMask = SceneManager.ENTITY_TYPE_MASK;

            RaySceneQueryResult result = mRaySceneQuery.Execute();
            RaySceneQueryResult.Enumerator itr = (RaySceneQueryResult.Enumerator)(result.GetEnumerator());

            ray = mouseRay;

            return itr;
        }

        private static bool GetRaySceneLocationWithPlane(double x, double y, Plane plane, out Vector3 vector, SceneNode ignore)
        {
            float width = Engine.Renderer.Camera.Viewport.ActualWidth;
            float height = Engine.Renderer.Camera.Viewport.ActualHeight;

            Ray mouseRay = Engine.Renderer.Camera.GetCameraToViewportRay((float)((x - 1) / width), (float)((y - 1) / height));

            Plane inverse = new Plane(-plane.normal, -plane.d);

            Pair<bool, float> d0 = mouseRay.Intersects(plane);
            Pair<bool, float> d1 = mouseRay.Intersects(inverse);

            RaySceneQuery mRaySceneQuery = Engine.Renderer.Scene.CreateRayQuery(mouseRay);

            mRaySceneQuery.SetSortByDistance(true, 16);
            mRaySceneQuery.QueryMask = (uint)QueryFlags.INSTANCE_ENTITY;
            //mRaySceneQuery.QueryTypeMask = SceneManager.ENTITY_TYPE_MASK;


            RaySceneQueryResult result = mRaySceneQuery.Execute();

            RaySceneQueryResult.Enumerator itr = (RaySceneQueryResult.Enumerator)(result.GetEnumerator());

            if (itr != null)
            {
                RaySceneQueryResultEntry entry = null;
                while (itr.MoveNext())
                {
                    entry = itr.Current;
                    if (entry != null && !entry.movable.ParentSceneNode.Equals(ignore))
                    {
                        vector = mouseRay.GetPoint(entry.distance);
                        return true;
                    }
                }
                if (d0.first)
                {
                    Vector3 planePoint = mouseRay.GetPoint(d0.second);
                    if ((Engine.Renderer.Camera.Position - planePoint).Length < MAX_TRANSLATE_DRAG_DISTANCE)
                    {
                        vector = planePoint;
                        return true;
                    }
                }
                if (d1.first)
                {
                    Vector3 planePoint = mouseRay.GetPoint(d1.second);
                    if ((Engine.Renderer.Camera.Position - planePoint).Length < MAX_TRANSLATE_DRAG_DISTANCE)
                    {
                        vector = planePoint;
                        return true;
                    }
                    else
                    {
                        vector = Vector3.ZERO;
                        return false;
                    }
                }
                else
                {
                    vector = Vector3.ZERO;
                    return false;
                }
            }
            else
            {
                vector = Vector3.ZERO;
                return false;
            }
        }

        private void _panel_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta != 0)
            {
                ActiveScene.Context.ActiveWorld.Camera.move(Engine.Renderer.Camera.Direction * (e.Delta / 10f));
            }
        }

        private void _panel_MouseMove(object sender, MouseEventArgs e)
        {
            if (_oldMousePosition == null)
            {
                _oldMousePosition = new Point(e.X, e.Y);
                return;
            }
            Point point = new Point(e.X, e.Y);
            Engine.QueueForRenderDispatcher(() =>
            {
                double deltaDirectionX = point.X - _oldMousePosition.X;
                double deltaDirectionY = point.Y - _oldMousePosition.Y;

                if (e.Button.HasFlag(MouseButtons.Right))
                {
                    ActiveScene.Context.ActiveWorld.Camera.pitch(new Radian((float)-deltaDirectionY / 200f).ValueDegrees);
                    ActiveScene.Context.ActiveWorld.Camera.yaw(new Radian((float)-deltaDirectionX / 200f).ValueDegrees);
                }

                IEnumerable<SceneNode> selected = GetSelectedNode();
                SceneNode selectedNode = null;
                if (selected != null)
                {
                    selectedNode = selected.FirstOrDefault();
                }

                if (_translateDragging != TransformDragging.NONE && ActiveScene.ActiveGizmoType == Misc.Runtime.GizmoType.TRANSLATE)
                {
                    if (selectedNode != null)
                    {
                        Plane plane = new Plane();
                        if (_translateDragging == TransformDragging.X)
                        {
                            Vector3 normal = Vector3.UNIT_Y;
                            plane = new Plane(normal, selectedNode.Position);
                        }
                        else if (_translateDragging == TransformDragging.Y)
                        {
                            Vector3 normal = (Engine.Renderer.Camera.Position - selectedNode.Position).NormalisedCopy;
                            normal.y = 0;
                            plane = new Plane(normal, selectedNode.Position);
                        }
                        else if (_translateDragging == TransformDragging.Z)
                        {
                            Vector3 normal = Vector3.UNIT_Y;
                            plane = new Plane(normal, selectedNode.Position);
                        }

                        Vector3 translateVector;
                        if (GetRaySceneLocationWithPlane(point.X, point.Y, plane, out translateVector, selectedNode))
                        {
                            PositionedInstance instance = Engine.SceneNodeStore.GetInstance(selectedNode);

                            SizedInstance sized = instance as SizedInstance;

                            if (sized != null)
                            {
                                translateVector -= _translateDragDifference;

                                if (_translateDragging == TransformDragging.X)
                                {
                                    instance.position.x = (float)System.Math.Round(translateVector.x - (sized.size.x / 2));
                                    //PhysicsEngine.GetCollisionVectorResolvement(instance as PhysicsInstance);
                                }
                                else if (_translateDragging == TransformDragging.Y)
                                {
                                    instance.position.y = (float)System.Math.Round(translateVector.y - (sized.size.y / 2));
                                    //PhysicsEngine.GetCollisionVectorResolvement(instance as PhysicsInstance);
                                }
                                else if (_translateDragging == TransformDragging.Z)
                                {
                                    instance.position.z = (float)System.Math.Round(translateVector.z - (sized.size.z / 2));
                                    //PhysicsEngine.GetCollisionVectorResolvement(instance as PhysicsInstance);
                                }
                            }
                        }
                    }
                }
                else if (_scaleDragging != DirectionalTransformDragging.NONE && ActiveScene.ActiveGizmoType == Misc.Runtime.GizmoType.SCALE)
                {
                    if (selectedNode != null)
                    {
                        Plane plane = new Plane();
                        if (_scaleDragging == DirectionalTransformDragging.XA || _scaleDragging == DirectionalTransformDragging.XB)
                        {
                            Vector3 normal = Vector3.UNIT_Y;
                            plane = new Plane(normal, selectedNode.Position);
                        }
                        else if (_scaleDragging == DirectionalTransformDragging.YA || _scaleDragging == DirectionalTransformDragging.YB)
                        {
                            Vector3 normal = (Engine.Renderer.Camera.Position - selectedNode.Position).NormalisedCopy;
                            normal.y = 0;
                            plane = new Plane(normal, selectedNode.Position);
                        }
                        else if (_scaleDragging == DirectionalTransformDragging.ZA || _scaleDragging == DirectionalTransformDragging.ZB)
                        {
                            Vector3 normal = Vector3.UNIT_Y;
                            plane = new Plane(normal, selectedNode.Position);
                        }

                        Vector3 scaleVector;
                        if (GetRaySceneLocationWithPlane(point.X, point.Y, plane, out scaleVector, selectedNode))
                        {
                            PositionedInstance instance = Engine.SceneNodeStore.GetInstance(selectedNode);

                            SizedInstance sized = instance as SizedInstance;

                            if (sized != null)
                            {
                                int magicOffsetA = 2;

                                if (_scaleDragging == DirectionalTransformDragging.XA)
                                {
                                    sized.size.x = System.Math.Max(1, (float)System.Math.Round(scaleVector.x - (sized.position.x) - magicOffsetA));
                                }
                                else if (_scaleDragging == DirectionalTransformDragging.YA)
                                {
                                    sized.size.y = System.Math.Max(1, (float)System.Math.Round(scaleVector.y - (sized.position.y) - magicOffsetA));
                                }
                                else if (_scaleDragging == DirectionalTransformDragging.ZA)
                                {
                                    sized.size.z = System.Math.Max(1, (float)System.Math.Round(scaleVector.z - (sized.position.z) - magicOffsetA));
                                }
                                else if (_scaleDragging == DirectionalTransformDragging.XB)
                                {
                                    //TODO: fix
                                    float newX = scaleVector.x + (sized.size.x / 2) + _scaleDragDifference.x; //wtf?
                                    float delta = (sized.position.x - newX);

                                    float rounded = (float)System.Math.Round(delta);

                                    sized.position.x -= rounded;
                                    sized.size.x += rounded;
                                }
                                else if (_scaleDragging == DirectionalTransformDragging.YB)
                                {

                                }
                                else if (_scaleDragging == DirectionalTransformDragging.ZB)
                                {

                                }
                            }
                        }
                    }
                }

                Cursor cursor = null;
                if (_translateFreeDragging && _scaleDragging == DirectionalTransformDragging.NONE && _translateDragging == TransformDragging.NONE)
                {
                    if (selectedNode != null)
                    {
                        Plane plane = new Plane(Vector3.UNIT_Y, Engine.Renderer.Camera.Position.y - 16);

                        Vector3 vector;
                        if (GetRaySceneLocationWithPlane(point.X, point.Y, plane, out vector, selectedNode))
                        {
                            PositionedInstance instance = Engine.SceneNodeStore.GetInstance(selectedNode);

                            SizedInstance sized = instance as SizedInstance;

                            if (sized != null)
                            {
                                instance.position.x = (float)System.Math.Round(vector.x - (sized.size.x / 2));
                                instance.position.y = (float)System.Math.Round(vector.y - (sized.size.y / 2));
                                instance.position.z = (float)System.Math.Round(vector.z - (sized.size.z / 2));
                            }
                        }

                        cursor = Cursors.NoMove2D;
                    }
                    else
                    {
                        _translateFreeDragging = false;
                    }
                }

                Ray ray;
                RaySceneQueryResult.Enumerator itr = GetRaySceneQuery(point.X, point.Y, out ray);

                if (itr != null)
                {
                    bool nothing = true;
                    while (itr.MoveNext())
                    {
                        RaySceneQueryResultEntry entry = itr.Current;
                        if (entry.movable.QueryFlags == (uint)QueryFlags.INSTANCE_ENTITY)
                        {
                            SceneNode parentNode = entry.movable.ParentSceneNode;
                            if (selectedNode != null && selectedNode.Equals(parentNode) && e.Button.HasFlag(MouseButtons.Left) && _translateDragging == TransformDragging.NONE)
                            {
                                if (System.Math.Abs(deltaDirectionX) >= TRANSLATE_DRAG_THRESHOLD || System.Math.Abs(deltaDirectionY) >= TRANSLATE_DRAG_THRESHOLD)
                                {
                                    _translateFreeDragging = true;
                                }
                            }
                            if (!IsAlreadyHovered(parentNode))
                            {
                                IEnumerable<SceneNode> hovered = GetHoveredNodes();
                                if (hovered != null)
                                {
                                    foreach (SceneNode node in hovered)
                                    {
                                        if (!IsSelectedNode(node))
                                        {
                                            PositionedInstance oldInstance = Engine.SceneNodeStore.GetInstance(node);
                                            if (oldInstance != null)
                                            {
                                                oldInstance.IsBoundingBoxEnabled = false;
                                            }
                                        }
                                    }
                                    ClearHovered();
                                }
                                AddToHoverNode(parentNode);
                            }
                        }
                        nothing = false;
                    }
                    if (nothing)
                    {
                        if (cursor == null)
                        {
                            cursor = Cursors.Default;
                        }
                        IEnumerable<SceneNode> hovered = GetHoveredNodes();
                        if (hovered != null)
                        {
                            foreach (SceneNode node in hovered)
                            {
                                if (!IsSelectedNode(node))
                                {
                                    PositionedInstance oldInstance = Engine.SceneNodeStore.GetInstance(node);
                                    if (oldInstance != null)
                                    {
                                        oldInstance.IsBoundingBoxEnabled = false;
                                    }
                                }
                            }
                            ClearHovered();
                        }
                    }
                    else
                    {
                        if (cursor == null)
                        {
                            cursor = Cursors.Hand;
                        }
                    }
                }
                if (cursor != null)
                {
                    System.Windows.Application.Current.Dispatcher.InvokeAsync(() => _panel.Cursor = cursor, DispatcherPriority.Background);
                }
                _oldMousePosition = point;
            });
        }
    }
}
