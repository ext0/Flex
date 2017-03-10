using Caliburn.Micro;
using Flex.Development.Instances;
using Flex.Modules.Explorer;
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

        private System.Windows.Forms.Integration.WindowsFormsHost _host;
        private Panel _panel;

        private Point _oldMousePosition;

        private Dictionary<SelectionType, List<SceneNode>> _boundingBoxes;

        public MouseHandler(System.Windows.Forms.Integration.WindowsFormsHost host, Panel panel)
        {
            _boundingBoxes = new Dictionary<SelectionType, List<SceneNode>>();
            _panel = panel;
            _host = host;
        }

        public void Initialize()
        {
            _panel.MouseMove += _panel_MouseMove;
            _panel.MouseClick += _panel_MouseClick;
            _panel.MouseWheel += _panel_MouseWheel;
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
            if (_boundingBoxes.ContainsKey(SelectionType.SELECT))
            {
                _boundingBoxes[SelectionType.SELECT].Clear();
            }
        }

        public void SetActiveSelectedNode(SceneNode newNode)
        {
            PositionedInstance instance = Engine.SceneNodeStore.GetInstance(newNode);
            SceneNode node;
            if (ExistsSelectedNode(out node))
            {
                if (!newNode.Equals(node))
                {
                    PositionedInstance oldInstance = Engine.SceneNodeStore.GetInstance(node);
                    oldInstance.IsSelected = false;
                    oldInstance.IsBoundingBoxEnabled = false;
                    ClearSelectedNode();

                    instance.IsBoundingBoxEnabled = true;
                    instance.IsSelected = true;
                    SetSelectedNode(newNode);
                }
            }
            else
            {
                instance.IsBoundingBoxEnabled = true;
                instance.IsSelected = true;
                SetSelectedNode(newNode);
            }
        }

        public void AddToHoverNode(SceneNode newNode)
        {
            if (!IsAlreadyHovered(newNode))
            {
                System.Windows.Application.Current.Dispatcher.InvokeAsync(() => _panel.Cursor = Cursors.Hand, DispatcherPriority.Background);
                if (!IsSelectedNode(newNode))
                {
                    PositionedInstance instance = Engine.SceneNodeStore.GetInstance(newNode);
                    instance.IsBoundingBoxEnabled = true;
                    MarkAsHovered(newNode);
                }
            }
        }

        private void _panel_MouseClick(object sender, MouseEventArgs e)
        {
            Point point = new Point(e.X, e.Y);
            Engine.QueueForRenderDispatcher(() =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    RaySceneQueryResult.Enumerator itr = GetRaySceneQuery(point.X, point.Y);

                    if (itr != null)
                    {
                        if (itr.MoveNext())
                        {
                            RaySceneQueryResultEntry entry = itr.Current;
                            SceneNode parentNode = entry.movable.ParentSceneNode;

                            PositionedInstance instance = Engine.SceneNodeStore.GetInstance(parentNode);

                            Engine.RunOnUIThread(() =>
                            {
                                IoC.Get<IExplorer>().SelectInstance(instance);
                            });

                            SetActiveSelectedNode(parentNode);
                        }
                        else
                        {
                            SceneNode node;
                            if (ExistsSelectedNode(out node))
                            {
                                PositionedInstance oldInstance = Engine.SceneNodeStore.GetInstance(node);
                                oldInstance.IsSelected = false;
                                oldInstance.IsBoundingBoxEnabled = false;
                                ClearSelectedNode();
                            }
                        }
                    }
                }
            });
        }

        private static RaySceneQueryResult.Enumerator GetRaySceneQuery(double x, double y)
        {
            float width = Engine.Renderer.Camera.Viewport.ActualWidth;
            float height = Engine.Renderer.Camera.Viewport.ActualHeight;

            Ray mouseRay = Engine.Renderer.Camera.GetCameraToViewportRay((float)((x - 1) / width), (float)((y - 1) / height));

            RaySceneQuery mRaySceneQuery = Engine.Renderer.Scene.CreateRayQuery(mouseRay);
            mRaySceneQuery.SetSortByDistance(true, 1);
            mRaySceneQuery.QueryTypeMask = SceneManager.ENTITY_TYPE_MASK;

            RaySceneQueryResult result = mRaySceneQuery.Execute();
            RaySceneQueryResult.Enumerator itr = (RaySceneQueryResult.Enumerator)(result.GetEnumerator());

            return itr;
        }

        private void _panel_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta != 0)
            {
                Engine.Renderer.Camera.Move(Engine.Renderer.Camera.Direction * (e.Delta / 10f));
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
                if (e.Button == MouseButtons.Right)
                {
                    double deltaDirectionX = point.X - _oldMousePosition.X;
                    double deltaDirectionY = point.Y - _oldMousePosition.Y;
                    Engine.Renderer.Camera.Pitch((float)-deltaDirectionY / 200f);
                    Engine.Renderer.Camera.Yaw((float)-deltaDirectionX / 200f);
                }

                RaySceneQueryResult.Enumerator itr = GetRaySceneQuery(point.X, point.Y);

                if (itr != null)
                {
                    if (itr.MoveNext())
                    {
                        RaySceneQueryResultEntry entry = itr.Current;
                        SceneNode parentNode = entry.movable.ParentSceneNode;
                        AddToHoverNode(parentNode);
                    }
                    else
                    {
                        System.Windows.Application.Current.Dispatcher.InvokeAsync(() => _panel.Cursor = Cursors.Default, DispatcherPriority.Background);
                        IEnumerable<SceneNode> hovered = GetHoveredNodes();
                        if (hovered != null)
                        {
                            foreach (SceneNode node in GetHoveredNodes())
                            {
                                if (!IsSelectedNode(node))
                                {
                                    PositionedInstance oldInstance = Engine.SceneNodeStore.GetInstance(node);
                                    oldInstance.IsBoundingBoxEnabled = false;
                                }
                            }
                            ClearHovered();
                        }
                    }
                }

                _oldMousePosition = point;
            });
        }
    }
}
