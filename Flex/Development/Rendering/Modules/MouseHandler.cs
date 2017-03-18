using Caliburn.Micro;
using Flex.Development.Execution.Data;
using Flex.Development.Instances;
using Flex.Development.Physics;
using Flex.Modules.Explorer;
using Jitter.Collision;
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
        private enum SelectionType
        {
            HOVER,
            DRAG,
            SELECT,
            DELETE
        }

        private enum TransformDragging
        {
            X,
            Y,
            Z,
            NONE
        }

        private static readonly int MAX_TRANSFORM_DRAG_DISTANCE = 256;
        private static readonly int TRANSFORM_DRAG_THRESHOLD = 1;

        private SceneNode _transformNode;

        private Vector3 _transformDragDifference;

        private SceneNode _xArrowNode;
        private SceneNode _yArrowNode;
        private SceneNode _zArrowNode;

        private TransformDragging _transformDragging;

        private System.Windows.Forms.Integration.WindowsFormsHost _host;
        private Panel _panel;

        private Point _oldMousePosition;

        private Dictionary<SelectionType, List<SceneNode>> _boundingBoxes;

        private bool _transformFreeDragging;

        public MouseHandler(System.Windows.Forms.Integration.WindowsFormsHost host, Panel panel)
        {
            _boundingBoxes = new Dictionary<SelectionType, List<SceneNode>>();
            _transformFreeDragging = false;
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

            CreateTransformEntity();
        }

        private void CreateTransformEntity()
        {
            _transformNode = Engine.Renderer.Scene.CreateSceneNode();

            Entity xArrowEntity;
            _xArrowNode = Engine.Renderer.CreateEntity(out xArrowEntity, "Arrow.mesh");
            _xArrowNode.Rotate(Vector3.UNIT_Y, new Degree(90f), Node.TransformSpace.TS_WORLD);
            xArrowEntity.SetMaterialName("Transform/X");
            _xArrowNode.InheritScale = false;
            _xArrowNode.InheritOrientation = false;

            Entity yArrowEntity;
            _yArrowNode = Engine.Renderer.CreateEntity(out yArrowEntity, "Arrow.mesh");
            _yArrowNode.Rotate(Vector3.UNIT_X, new Degree(-90f), Node.TransformSpace.TS_WORLD);
            yArrowEntity.SetMaterialName("Transform/Y");
            _yArrowNode.InheritScale = false;
            _yArrowNode.InheritOrientation = false;

            Entity zArrowEntity;
            _zArrowNode = Engine.Renderer.CreateEntity(out zArrowEntity, "Arrow.mesh");
            zArrowEntity.SetMaterialName("Transform/Z");
            _zArrowNode.InheritScale = false;
            _zArrowNode.InheritOrientation = false;

            _transformNode.AddChild(_xArrowNode);
            _transformNode.AddChild(_yArrowNode);
            _transformNode.AddChild(_zArrowNode);
        }

        private bool IsTransformClick(SceneNode checking, out TransformDragging transformDragging)
        {
            if (checking.Equals(_xArrowNode))
            {
                transformDragging = TransformDragging.X;
                return true;
            }
            else if (checking.Equals(_yArrowNode))
            {
                transformDragging = TransformDragging.Y;
                return true;
            }
            else if (checking.Equals(_zArrowNode))
            {
                transformDragging = TransformDragging.Z;
                return true;
            }
            transformDragging = TransformDragging.NONE;
            return false;
        }

        private IEnumerable<SceneNode> GetHoveredNodes()
        {
            return _boundingBoxes.ContainsKey(SelectionType.HOVER) ? _boundingBoxes[SelectionType.HOVER] : null;
        }

        private bool IsAlreadyHovered(SceneNode node)
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

        private void ClearHovered()
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

        private bool IsSelectedNode(SceneNode node)
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

        private void ClearSelectedNode()
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
                        node.RemoveChild(_transformNode);
                        ClearSelectedNode();
                    }
                    instance.IsBoundingBoxEnabled = true;
                    instance.IsSelected = true;
                    SetSelectedNode(newNode);
                    newNode.AddChild(_transformNode);
                }
            }
            else
            {
                instance.IsBoundingBoxEnabled = true;
                instance.IsSelected = true;
                SetSelectedNode(newNode);
                newNode.AddChild(_transformNode);
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
            _transformDragging = TransformDragging.NONE;
            if (_transformFreeDragging)
            {
                _transformFreeDragging = false;
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
                            if (IsTransformClick(parentNode, out dragging))
                            {
                                _transformDragging = dragging;
                                IEnumerable<SceneNode> selected = GetSelectedNode();
                                if (selected != null)
                                {
                                    SceneNode node = selected.FirstOrDefault();
                                    if (node != null)
                                    {
                                        _transformDragDifference = ray.GetPoint(entry.distance) - node.Position;
                                    }
                                }
                                return;
                            }
                            else
                            {
                                _transformDragging = TransformDragging.NONE;
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
                                node.RemoveChild(_transformNode);
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
            mRaySceneQuery.SetSortByDistance(true, 1);
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

            mRaySceneQuery.SetSortByDistance(true, 1);
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
                //System.Diagnostics.Trace.WriteLine("Not found!" + d.first);
                if (d0.first)
                {
                    Vector3 planePoint = mouseRay.GetPoint(d0.second);
                    if ((Engine.Renderer.Camera.Position - planePoint).Length < MAX_TRANSFORM_DRAG_DISTANCE)
                    {
                        vector = planePoint;
                        return true;
                    }
                }
                if (d1.first)
                {
                    Vector3 planePoint = mouseRay.GetPoint(d1.second);
                    if ((Engine.Renderer.Camera.Position - planePoint).Length < MAX_TRANSFORM_DRAG_DISTANCE)
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

                if (_transformDragging != TransformDragging.NONE)
                {
                    if (selectedNode != null)
                    {
                        Plane plane = new Plane();
                        if (_transformDragging == TransformDragging.X)
                        {
                            Vector3 normal = Vector3.UNIT_Y;
                            plane = new Plane(normal, selectedNode.Position);
                        }
                        else if (_transformDragging == TransformDragging.Y)
                        {
                            Vector3 normal = (Engine.Renderer.Camera.Position - selectedNode.Position).NormalisedCopy;
                            normal.y = 0;
                            plane = new Plane(normal, selectedNode.Position);
                        }
                        else if (_transformDragging == TransformDragging.Z)
                        {
                            Vector3 normal = Vector3.UNIT_Y;
                            plane = new Plane(normal, selectedNode.Position);
                        }

                        Vector3 transformVector;
                        if (GetRaySceneLocationWithPlane(point.X, point.Y, plane, out transformVector, selectedNode))
                        {
                            PositionedInstance instance = Engine.SceneNodeStore.GetInstance(selectedNode);

                            transformVector -= _transformDragDifference;

                            if (_transformDragging == TransformDragging.X)
                            {
                                instance.position.x = (float)System.Math.Round(transformVector.x);
                                PhysicsEngine.GetCollisionVectorResolvement(instance as PhysicsInstance);
                            }
                            else if (_transformDragging == TransformDragging.Y)
                            {
                                instance.position.y = (float)System.Math.Round(transformVector.y);
                                PhysicsEngine.GetCollisionVectorResolvement(instance as PhysicsInstance);
                            }
                            else if (_transformDragging == TransformDragging.Z)
                            {
                                instance.position.z = (float)System.Math.Round(transformVector.z);
                                PhysicsEngine.GetCollisionVectorResolvement(instance as PhysicsInstance);
                            }
                        }
                    }
                }
                Cursor cursor = null;
                if (_transformFreeDragging)
                {
                    if (selectedNode != null)
                    {
                        Plane plane = new Plane(Vector3.UNIT_Y, Engine.Renderer.Camera.Position.y - 16);

                        Vector3 vector;
                        if (GetRaySceneLocationWithPlane(point.X, point.Y, plane, out vector, selectedNode))
                        {
                            PositionedInstance instance = Engine.SceneNodeStore.GetInstance(selectedNode);

                            instance.position.x = (float)System.Math.Round(vector.x);
                            instance.position.y = (float)System.Math.Round(vector.y);
                            instance.position.z = (float)System.Math.Round(vector.z);
                        }

                        cursor = Cursors.NoMove2D;
                    }
                    else
                    {
                        _transformFreeDragging = false;
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
                            if (selectedNode != null && selectedNode.Equals(parentNode) && e.Button.HasFlag(MouseButtons.Left) && _transformDragging == TransformDragging.NONE)
                            {
                                if (System.Math.Abs(deltaDirectionX) >= TRANSFORM_DRAG_THRESHOLD || System.Math.Abs(deltaDirectionY) >= TRANSFORM_DRAG_THRESHOLD)
                                {
                                    _transformFreeDragging = true;
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
