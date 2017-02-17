using Ab3d.Cameras;
using Ab3d.Common.EventManager3D;
using Ab3d.DirectX;
using Ab3d.DirectX.Controls;
using Ab3d.Utilities;
using Ab3d.Visuals;
using Caliburn.Micro;
using Flex.Development.Execution.Data;
using Flex.Development.Instances;
using Flex.Misc.Utility;
using Flex.Modules.Scene.Views;
using Gemini.Modules.Output;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using Flex.Development.Physics;
using Jitter.Collision;
using Gemini.Modules.PropertyGrid;
using System.ComponentModel;
using System.Threading;
using System.Windows;

namespace Flex.Development.Rendering
{
    public class MainDXScene : IDisposable
    {
        private static readonly TimeSpan EventUnselectCooldown = new TimeSpan(0, 0, 0, 0, 50);
        private static readonly TimeSpan ModelMoveUnselectCooldown = new TimeSpan(0, 0, 0, 0, 50);

        public static MainDXScene Scene = null;
        public List<Instance> VisualInstances;

        private VarianceShadowRenderingProvider _varianceShadowRenderingProvider;
        private Model3DGroup _lightsModel3DGroup;

        private int _shadowMapSize;
        private int _shadowDepthBluringSize;
        private float _shadowTreshold;

        private float _lightHorizontalAngle;
        private float _lightVerticalAngle;
        private float _lightDistance;

        private DirectionalLight _directionalLight;

        private DXViewportView _viewport;

        private FirstPersonCamera _camera;

        private ModelVisual3D _modelVisual3D;
        public ContainerUIElement3D RootContainer;

        private PositionedInstance _selectedPhysicalInstance;

        private DateTime _selectedPhysicalInstanceTime;

        private Point3D _startModelMoverPosition;

        public ModelMoverVisual3D ModelMover;
        private DateTime _modelMoveEndTime;

        private EventManager3D _eventManager;
        private EventManager3D _skyboxEventManager;

        private TranslateTransform3D _currentTranslateTransform3D;

        private DataContext _context;
        private SceneView _sceneView;

        private PolyLineVisual3D _boundingBoxSelected;
        private PolyLineVisual3D _boundingBoxHover;

        private IOutput _output;

        public MainDXScene(DataContext context, SceneView sceneView)
        {
            if (Scene == null)
            {
                Scene = this;
            }
            else
            {
                throw new Exception("Cannot create multiple DXScenes!");
            }
            //Careful with cross threading!
            _sceneView = sceneView;
            _context = context;
            _output = IoC.Get<IOutput>();

            VisualInstances = new List<Instance>();
            _viewport = _sceneView.MainDXViewportView;
            _camera = _sceneView.Camera;

            _shadowMapSize = 256;
            _shadowDepthBluringSize = 4;
            _shadowTreshold = 0.2f;

            _boundingBoxSelected = new PolyLineVisual3D();
            _boundingBoxSelected.LineThickness = 5.0;
            _boundingBoxSelected.LineColor = System.Windows.Media.Color.FromRgb(0, 0, 255);

            _boundingBoxSelected.Positions = new Point3DCollection();

            _boundingBoxHover = new PolyLineVisual3D();
            _boundingBoxHover.LineThickness = 5.0;
            _boundingBoxHover.LineColor = System.Windows.Media.Color.FromArgb(128, 0, 0, 255);

            _boundingBoxHover.Positions = new Point3DCollection();

            _viewport.DXSceneInitialized += DXSceneInitialized;
        }

        private void DXSceneInitialized(object sender, EventArgs e)
        {
            if (_viewport.DXScene == null)
            {
                _output.AppendLine("Failed to initialize DX!");
                return;
            }

            //_viewport.DXScene.AfterUpdated += DXSceneAfterUpdated;

            _eventManager = new EventManager3D(_viewport.Viewport3D);
            _eventManager.CustomEventsSourceElement = _sceneView.ViewportBorder;

            _skyboxEventManager = new EventManager3D(_sceneView.SkyboxViewport);
            _skyboxEventManager.CustomEventsSourceElement = _sceneView.ViewportBorder;

            _varianceShadowRenderingProvider = new VarianceShadowRenderingProvider();
            _varianceShadowRenderingProvider.ShadowMapSize = _shadowMapSize;
            _varianceShadowRenderingProvider.ShadowDepthBluringSize = _shadowDepthBluringSize;
            _varianceShadowRenderingProvider.ShadowTreshold = _shadowTreshold;

            _viewport.DXScene.InitializeShadowRendering(_varianceShadowRenderingProvider);

            _lightsModel3DGroup = new Model3DGroup();

            AmbientLight ambientLight = new AmbientLight(System.Windows.Media.Color.FromRgb(25, 25, 25));

            _lightsModel3DGroup.Children.Add(ambientLight);

            _modelVisual3D = new ModelVisual3D();

            _modelVisual3D.Content = _lightsModel3DGroup;

            _viewport.Viewport3D.Children.Add(_modelVisual3D);

            VisualEventSource3D visualEventSource3DSkybox = new VisualEventSource3D(_sceneView.Skybox);
            visualEventSource3DSkybox.MouseClick += VisualEventSource3D_MouseClick;

            RootContainer = _sceneView.SceneObjectsContainer;

            VisualEventSource3D visualEventSource3D = new VisualEventSource3D(RootContainer);
            visualEventSource3D.MouseMove += VisualEventSource3DOnMouseMove;
            visualEventSource3D.MouseEnter += VisualEventSource3D_MouseEnter;
            visualEventSource3D.MouseLeave += VisualEventSource3DOnMouseLeave;
            visualEventSource3D.MouseClick += VisualEventSource3D_MouseClick;

            _eventManager.RegisterEventSource3D(visualEventSource3D);
            _skyboxEventManager.RegisterEventSource3D(visualEventSource3DSkybox);

            InitializeLights();

            _output.AppendLine("Welcome to Flex!");
        }

        public void PhysicsStep()
        {
            if (ActiveScene.Running)
            {
                PhysicsEngine.Step();
            }
        }

        public void AddChildVisual(Visual3D visual)
        {
            if (IsOnDispatcher())
            {
                Scene.RootContainer.Children.Add(visual);
            } else
            {
                RunOnUIThread(() =>
                {
                    Scene.RootContainer.Children.Add(visual);
                });
            }
        }

        public void RemoveChildVisual(Visual3D visual)
        {
            if (IsOnDispatcher())
            {
                Scene.RootContainer.Children.Remove(visual);
            } else
            {
                RunOnUIThread(() =>
                {
                    Scene.RootContainer.Children.Remove(visual);
                });
            }
        }

        public void RunOnUIThread(System.Action action)
        {
            Scene.RootContainer.Dispatcher.Invoke(action);
        }

        public bool IsOnDispatcher()
        {
            return Thread.CurrentThread == Scene.RootContainer.Dispatcher.Thread;
        }

        private void SetupModelMover()
        {
            ModelMover = new ModelMoverVisual3D();

            ModelMover.SubscribeWithEventManager3D(_eventManager);

            ModelMover.Position = GetVisualCenter(_selectedPhysicalInstance.Visual3D, _selectedPhysicalInstance.Model);
            Rect3D modelBounds = _selectedPhysicalInstance.Model.Bounds;
            double axisLength = Math.Max(modelBounds.Size.X, Math.Max(modelBounds.Size.Y, modelBounds.Size.Z));

            ModelMover.AxisLength = axisLength;

            ModelMover.AxisRadius = 0.3;
            ModelMover.AxisArrowRadius = 0.7;

            // Setup event handlers
            ModelMover.ModelMoveStarted += delegate (object o, EventArgs eventArgs)
            {
                if (_selectedPhysicalInstance == null)
                {
                    return;
                }
                _currentTranslateTransform3D = new TranslateTransform3D();
                Transform3D currentTransform = _selectedPhysicalInstance.Visual3D.Transform;
                if (currentTransform == null)
                {
                    _selectedPhysicalInstance.Visual3D.Transform = _currentTranslateTransform3D;
                }
                else
                {
                    /*
                    Transform3DGroup currentTransformGroup = currentTransform as Transform3DGroup;
                    if (currentTransformGroup == null)
                    {
                        currentTransformGroup = new Transform3DGroup();
                        currentTransformGroup.Children.Add(_selectedPhysicalInstance.Visual3D.Transform);
                        _selectedPhysicalInstance.Visual3D.Transform = currentTransformGroup;
                    }
                    currentTransformGroup.Children.Add(_currentTranslateTransform3D);
                    */
                }

                _startModelMoverPosition = GetVisualCenter(_selectedPhysicalInstance.Visual3D, _selectedPhysicalInstance.Model);
                ModelMover.Position = _startModelMoverPosition;
            };

            ModelMover.ModelMoved += delegate (object o, Ab3d.Common.ModelMovedEventArgs e)
            {
                if (_selectedPhysicalInstance == null)
                    return;

                Vector3D movement = FlexUtility.SnapToGrid(e.MoveVector3D, 1);

                Point3D newCenterPosition = _startModelMoverPosition + movement;

                if (Math.Abs(newCenterPosition.X) > 2000 ||
                    Math.Abs(newCenterPosition.Y) > 2000 ||
                    Math.Abs(newCenterPosition.Z) > 2000)
                {
                    return;
                }

                // When model is moved we get the updated MoveVector3D
                // We use MoveVector3D to change the _currentTranslateTransform3D that is used on the currently selected model and on the ModelMover object
                /*
                _currentTranslateTransform3D.OffsetX = movement.X;
                _currentTranslateTransform3D.OffsetY = movement.Y;
                _currentTranslateTransform3D.OffsetZ = movement.Z;
                */

                _selectedPhysicalInstance.position.setTo(newCenterPosition.X, newCenterPosition.Y, newCenterPosition.Z);

                ModelMover.Position = newCenterPosition;
            };

            ModelMover.ModelMoveEnded += delegate (object o, EventArgs e)
            {
                _modelMoveEndTime = DateTime.Now;
            };

            _viewport.Viewport3D.Children.Insert(0, ModelMover);
        }

        private PositionedInstance GetInstanceFromVisual(Visual3D visual)
        {
            foreach (PositionedInstance positionedInstance in VisualInstances)
            {
                if (positionedInstance.Visual3D.Equals(visual))
                {
                    return positionedInstance;
                }
            }
            return null;
        }

        private Point3D GetVisualCenter(Visual3D visual, Model3D model)
        {
            Rect3D modelBounds = model.Bounds;

            Point3D modelCenter = new Point3D(modelBounds.X + modelBounds.SizeX / 2, modelBounds.Y + modelBounds.SizeY / 2, modelBounds.Z + modelBounds.SizeZ / 2);

            if (visual.Transform != null)
            {
                modelCenter = visual.Transform.Transform(modelCenter);
            }

            return modelCenter;
        }

        private void VisualEventSource3D_MouseClick(object sender, MouseButton3DEventArgs e)
        {
            Visual3D hitModel = e.RayHitResult.VisualHit;

            if (hitModel.GetType().Equals(typeof(MultiMaterialBoxVisual3D)))
            {
                if (_selectedPhysicalInstanceTime == null || ((DateTime.Now - _selectedPhysicalInstanceTime) < EventUnselectCooldown) || (_modelMoveEndTime != null && ((DateTime.Now - _modelMoveEndTime) < ModelMoveUnselectCooldown)))
                {
                    return;
                }
                IoC.Get<IPropertyGrid>().SelectedObject = null;
                if (_selectedPhysicalInstance != null)
                {
                    _selectedPhysicalInstance.PropertyChanged -= SelectPolylineBox;
                }
                _selectedPhysicalInstance = null;

                _boundingBoxSelected.Positions.Clear();

                _viewport.Viewport3D.Children.Remove(ModelMover);
                ModelMover = null;
                return;
            }
            if (hitModel != null)
            {
                ContainerUIElement3D hit = e.HitObject as ContainerUIElement3D;

                if (_selectedPhysicalInstance == null || !_selectedPhysicalInstance.Visual3D.Equals(e.RayHitResult.VisualHit))
                {
                    _selectedPhysicalInstance = GetInstanceFromVisual(e.RayHitResult.VisualHit);
                }
                else
                {
                    return;
                }

                IoC.Get<IPropertyGrid>().SelectedObject = _selectedPhysicalInstance;

                _eventManager.RegisterExcludedVisual3D(_selectedPhysicalInstance.Visual3D);
                _eventManager.RemoveExcludedVisual3D(_selectedPhysicalInstance.Visual3D);

                if (ModelMover == null)
                    SetupModelMover();

                _startModelMoverPosition = GetVisualCenter(_selectedPhysicalInstance.Visual3D, _selectedPhysicalInstance.Model);
                ModelMover.Position = _startModelMoverPosition;

                _selectedPhysicalInstanceTime = DateTime.Now;

                FlexUtility.RunWindowAction(() =>
                {
                    /*
                    _mainWindow.SelectInstance(_selectedPhysicalInstance.Instance);
                    */
                }, DispatcherPriority.Normal);

                if (_selectedPhysicalInstance != null)
                {
                    _selectedPhysicalInstance.PropertyChanged += SelectPolylineBox;
                    PolyLineBoundingBox(_boundingBoxSelected, _selectedPhysicalInstance.position.Vector3D, _selectedPhysicalInstance.Model.Bounds);
                }
                // Tell ModelDecoratorVisual3D which Model3D to show
            }
        }

        private void SelectPolylineBox(object sender, PropertyChangedEventArgs e)
        {
            PolyLineBoundingBox(_boundingBoxSelected, _selectedPhysicalInstance.position.Vector3D, _selectedPhysicalInstance.Model.Bounds);
        }

        public void PolyLineBoundingBox(PolyLineVisual3D polyLine, Vector3D position, Rect3D bounds)
        {
            if (!RootContainer.Children.Contains(polyLine))
            {
                RootContainer.Children.Add(polyLine);
            }

            polyLine.Positions.Clear();

            double[] xValues = new double[2];
            double[] yValues = new double[2];
            double[] zValues = new double[2];

            xValues[0] = bounds.Location.X + position.X;
            xValues[1] = bounds.Location.X + bounds.SizeX + position.X;

            yValues[0] = bounds.Location.Y + position.Y;
            yValues[1] = bounds.Location.Y + bounds.SizeY + position.Y;

            zValues[0] = bounds.Location.Z + position.Z;
            zValues[1] = bounds.Location.Z + bounds.SizeZ + position.Z;

            polyLine.Positions.Add(new Point3D(xValues[0], yValues[0], zValues[0]));
            polyLine.Positions.Add(new Point3D(xValues[1], yValues[0], zValues[0]));
            polyLine.Positions.Add(new Point3D(xValues[1], yValues[0], zValues[1]));
            polyLine.Positions.Add(new Point3D(xValues[0], yValues[0], zValues[1]));
            polyLine.Positions.Add(new Point3D(xValues[0], yValues[0], zValues[0]));
            polyLine.Positions.Add(new Point3D(xValues[0], yValues[1], zValues[0]));
            polyLine.Positions.Add(new Point3D(xValues[1], yValues[1], zValues[0]));
            polyLine.Positions.Add(new Point3D(xValues[1], yValues[1], zValues[1]));
            polyLine.Positions.Add(new Point3D(xValues[0], yValues[1], zValues[1]));
            polyLine.Positions.Add(new Point3D(xValues[0], yValues[1], zValues[0]));
            polyLine.Positions.Add(new Point3D(xValues[0], yValues[1], zValues[1]));
            polyLine.Positions.Add(new Point3D(xValues[0], yValues[0], zValues[1]));
            polyLine.Positions.Add(new Point3D(xValues[0], yValues[1], zValues[1]));
            polyLine.Positions.Add(new Point3D(xValues[1], yValues[1], zValues[1]));
            polyLine.Positions.Add(new Point3D(xValues[1], yValues[0], zValues[1]));
            polyLine.Positions.Add(new Point3D(xValues[1], yValues[1], zValues[1]));
            polyLine.Positions.Add(new Point3D(xValues[1], yValues[1], zValues[0]));
            polyLine.Positions.Add(new Point3D(xValues[1], yValues[0], zValues[0]));
        }

        private void VisualEventSource3DOnMouseMove(object sender, Mouse3DEventArgs mouse3DEventArgs)
        {
            Model3D hitModel = mouse3DEventArgs.RayHitResult.ModelHit;

            if ((_selectedPhysicalInstance == null) || (_selectedPhysicalInstance != null && !mouse3DEventArgs.RayHitResult.VisualHit.Equals(_selectedPhysicalInstance.Visual3D) && mouse3DEventArgs.RayHitResult.VisualHit is BoxVisual3D))
            {
                PositionedInstance instance = GetInstanceFromVisual(mouse3DEventArgs.RayHitResult.VisualHit);
                if (instance != null)
                {
                    PolyLineBoundingBox(_boundingBoxHover, instance.position.Vector3D, hitModel.Bounds);
                }
                //_modelDecorator.TargetModel3D = hitModel;
            }
        }


        private void VisualEventSource3D_MouseEnter(object sender, Mouse3DEventArgs e)
        {
            _sceneView.Cursor = Cursors.Hand;
        }

        private void VisualEventSource3DOnMouseLeave(object sender, Mouse3DEventArgs mouse3DEventArgs)
        {
            _sceneView.Cursor = Cursors.Arrow;

            _boundingBoxHover.Positions.Clear();

            if (_selectedPhysicalInstance != null)
            {
                return;
            }
            else
            {
                _boundingBoxHover.Positions.Clear();
            }
        }

        private void InitializeLights()
        {
            _context.ActiveWorld.Sky.OnChanged += (sender, e) =>
            {
                _lightHorizontalAngle = _context.ActiveWorld.Sky.sunHorizontalAngle;
                _lightVerticalAngle = _context.ActiveWorld.Sky.sunVerticalAngle;
                _lightDistance = _context.ActiveWorld.Sky.sunDistance;

                _directionalLight.Direction = GetSunDirection();
                _directionalLight.SetDXAttribute(DXAttributeType.IsCastingShadow, _context.ActiveWorld.Sky.castShadows);
            };

            _lightHorizontalAngle = _context.ActiveWorld.Sky.sunHorizontalAngle;
            _lightVerticalAngle = _context.ActiveWorld.Sky.sunVerticalAngle;

            _lightDistance = _context.ActiveWorld.Sky.sunDistance;

            _directionalLight = new DirectionalLight();

            _directionalLight.Direction = GetSunDirection();

            _directionalLight.SetDXAttribute(DXAttributeType.IsCastingShadow, false);

            _lightsModel3DGroup.Children.Add(_directionalLight);
        }


        private Vector3D GetSunDirection()
        {
            Point3D position = CalculateLightPosition();

            Vector3D lightDirection = new Vector3D(-position.X, -position.Y, -position.Z);
            lightDirection.Normalize();

            return lightDirection;
        }

        private Point3D CalculateLightPosition()
        {
            float xRad = MathUtil.DegreesToRadians(_lightHorizontalAngle);
            float yRad = MathUtil.DegreesToRadians(_lightVerticalAngle);

            float x = (float)Math.Sin(xRad) * _lightDistance;
            float y = (float)Math.Sin(yRad) * _lightDistance;
            float z = (float)(Math.Cos(xRad) * Math.Cos(yRad)) * _lightDistance;

            return new Point3D(x, y, z);
        }

        public void Dispose()
        {
            _viewport.Dispose();
        }
    }
}
