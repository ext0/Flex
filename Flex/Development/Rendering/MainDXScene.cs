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

namespace Flex.Development.Rendering
{
    public class MainDXScene : IDisposable
    {
        private static readonly TimeSpan EventUnselectCooldown = new TimeSpan(0, 0, 0, 0, 50);
        private static readonly TimeSpan ModelMoveUnselectCooldown = new TimeSpan(0, 0, 0, 0, 50);

        private List<VisualInstance> _visualInstances;

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

        private ModelDecoratorVisual3D _modelDecorator;

        private ModelVisual3D _modelVisual3D;
        private ContainerUIElement3D _rootContainer;

        private PhysicalInstance _selectedPhysicalInstance;
        private DateTime _selectedPhysicalInstanceTime;

        private Point3D _startModelMoverPosition;

        private ModelMoverVisual3D _modelMover;
        private DateTime _modelMoveEndTime;

        private EventManager3D _eventManager;
        private EventManager3D _skyboxEventManager;

        private TranslateTransform3D _currentTranslateTransform3D;

        private DataContext _context;
        private SceneView _sceneView;

        private PhysicsEngine _physics;

        private IOutput _output;

        public MainDXScene(DataContext context, SceneView sceneView)
        {
            //Careful with cross threading!
            _sceneView = sceneView;
            _context = context;
            _physics = new PhysicsEngine();
            _output = IoC.Get<IOutput>();

            _visualInstances = new List<VisualInstance>();
            _viewport = _sceneView.MainDXViewportView;
            _camera = _sceneView.Camera;

            _shadowMapSize = 256;
            _shadowDepthBluringSize = 4;
            _shadowTreshold = 0.2f;

            _modelDecorator = _sceneView.ModelDecorator;

            _viewport.DXSceneInitialized += DXSceneInitialized;
        }

        private void DXSceneInitialized(object sender, EventArgs e)
        {
            if (_viewport.DXScene == null)
            {
                _output.AppendLine("Failed to initialize DX!");
                return;
            }

            _viewport.DXScene.AfterUpdated += DXSceneAfterUpdated;

            _eventManager = new EventManager3D(_viewport.Viewport3D);
            _eventManager.CustomEventsSourceElement = _sceneView.ViewportBorder;

            _skyboxEventManager = new EventManager3D(_sceneView.SkyboxViewport);
            _skyboxEventManager.CustomEventsSourceElement = _sceneView.ViewportBorder;

            _output.AppendLine("Initializing DXScene!");

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

            _rootContainer = _sceneView.SceneObjectsContainer;

            VisualEventSource3D visualEventSource3D = new VisualEventSource3D(_rootContainer);
            visualEventSource3D.MouseMove += VisualEventSource3DOnMouseMove;
            visualEventSource3D.MouseEnter += VisualEventSource3D_MouseEnter;
            visualEventSource3D.MouseLeave += VisualEventSource3DOnMouseLeave;
            visualEventSource3D.MouseClick += VisualEventSource3D_MouseClick;

            _eventManager.RegisterEventSource3D(visualEventSource3D);
            _skyboxEventManager.RegisterEventSource3D(visualEventSource3DSkybox);

            InitializeLights();

            _output.AppendLine("Successfully initialized DXScene!");
        }

        private void DXSceneAfterUpdated(object sender, UpdateStatusEventArgs e)
        {
            if (ActiveScene.Running)
            {
                //_physics.Step();
            }
        }

        private void SetupModelMover()
        {
            _modelMover = new ModelMoverVisual3D();

            _modelMover.SubscribeWithEventManager3D(_eventManager);

            _modelMover.Position = GetVisualCenter(_selectedPhysicalInstance.Visual3D, _selectedPhysicalInstance.Model3D);
            Rect3D modelBounds = _selectedPhysicalInstance.Model3D.Bounds;
            double axisLength = Math.Max(modelBounds.Size.X, Math.Max(modelBounds.Size.Y, modelBounds.Size.Z));

            _modelMover.AxisLength = axisLength;

            _modelMover.AxisRadius = axisLength / 30;
            _modelMover.AxisArrowRadius = _modelMover.AxisRadius * 3;

            // Setup event handlers
            _modelMover.ModelMoveStarted += delegate (object o, EventArgs eventArgs)
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

                _startModelMoverPosition = GetVisualCenter(_selectedPhysicalInstance.Visual3D, _selectedPhysicalInstance.Model3D);
                _modelMover.Position = _startModelMoverPosition;
            };

            _modelMover.ModelMoved += delegate (object o, Ab3d.Common.ModelMovedEventArgs e)
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

                (_selectedPhysicalInstance.Instance as PositionedInstance).Position.SetTo(newCenterPosition.X, newCenterPosition.Y, newCenterPosition.Z);

                _modelMover.Position = newCenterPosition;
            };

            _modelMover.ModelMoveEnded += delegate (object o, EventArgs e)
            {
                _modelMoveEndTime = DateTime.Now;
            };

            _viewport.Viewport3D.Children.Insert(0, _modelMover);
        }

        private Instance GetInstanceFromVisual(Visual3D visual)
        {
            foreach (VisualInstance visualInstance in _visualInstances)
            {
                if (visualInstance.Visual3D.Equals(visual))
                {
                    return visualInstance.Instance;
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
                _selectedPhysicalInstance = null;
                _viewport.Viewport3D.Children.Remove(_modelMover);
                _modelMover = null;
                return;
            }
            if (hitModel != null)
            {
                ContainerUIElement3D hit = e.HitObject as ContainerUIElement3D;
                ModelVisual3D foundVisual = null;
                Model3D foundModel = null;

                foreach (ModelVisual3D visual in hit.Children)
                {
                    if (visual.Equals(e.RayHitResult.VisualHit))
                    {
                        foundVisual = visual;
                        foundModel = visual.Content;
                        break;
                    }
                }

                if (_selectedPhysicalInstance == null || !_selectedPhysicalInstance.Visual3D.Equals(e.RayHitResult.VisualHit))
                {
                    _selectedPhysicalInstance = new PhysicalInstance(foundVisual, foundModel, GetInstanceFromVisual(e.RayHitResult.VisualHit));
                }
                else
                {
                    return;
                }

                _eventManager.RegisterExcludedVisual3D(_selectedPhysicalInstance.Visual3D);
                _eventManager.RemoveExcludedVisual3D(_selectedPhysicalInstance.Visual3D);

                if (_modelMover == null)
                    SetupModelMover();

                _startModelMoverPosition = GetVisualCenter(_selectedPhysicalInstance.Visual3D, _selectedPhysicalInstance.Model3D);
                _modelMover.Position = _startModelMoverPosition;

                _selectedPhysicalInstanceTime = DateTime.Now;

                FlexUtility.RunWindowAction(() =>
                {
                    /*
                    _mainWindow.SelectInstance(_selectedPhysicalInstance.Instance);
                    */
                }, DispatcherPriority.Normal);

                // Tell ModelDecoratorVisual3D which Model3D to show

                _modelDecorator.TargetModel3D = _selectedPhysicalInstance.Model3D;

            }
        }

        private void VisualEventSource3DOnMouseMove(object sender, Mouse3DEventArgs mouse3DEventArgs)
        {
            Model3D hitModel = mouse3DEventArgs.RayHitResult.ModelHit;

            if (_modelDecorator.TargetModel3D != hitModel /*&& _selectedPhysicalInstance != null && !mouse3DEventArgs.RayHitResult.VisualHit.Equals(_selectedPhysicalInstance.Visual3D) */)
            {
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
            if (_modelDecorator.TargetModel3D != null && _selectedPhysicalInstance != null && _modelDecorator.TargetModel3D.Equals(_selectedPhysicalInstance.Model3D))
            {
                //Stop here, because the user is dragging and we'd like them to still see the bounding box
                return;
            }

            //_modelDecorator.TargetModel3D = null;
        }

        public void AddInstance(Instance instance)
        {
            if (instance is Part)
            {
                Part partInstance = instance as Part;
                BoxVisual3D part = new BoxVisual3D();

                TranslateTransform3D translateTransform = new TranslateTransform3D(partInstance.Position.Vector3D);
                RotateTransform3D rotateTransform = new RotateTransform3D(new QuaternionRotation3D());

                partInstance.OnChanged += (sender, e) =>
                {
                    FlexUtility.RunWindowAction(() =>
                    {
                        /*
                        if (_selectedPhysicalInstance != null && !_selectedPhysicalInstance.Instance.Equals(partInstance))
                        {
                            //Glitches occur if you modify the Vector3 values for position while dragging.
                            part.CenterPosition = partInstance.Position.Point3D;
                        }
                        else if (_selectedPhysicalInstance == null)
                        {
                            part.CenterPosition = partInstance.Position.Point3D;
                        }
                        */
                        if (_selectedPhysicalInstance != null)
                        {
                            _modelMover.Position = partInstance.Position.Point3D;
                        }

                        System.Windows.Media.Media3D.Quaternion rotation = FlexUtility.FromYawPitchRoll((float)partInstance.Rotation.X, (float)partInstance.Rotation.Y, (float)partInstance.Rotation.Z);

                        (part.Transform as Transform3DGroup).Children[1] = new TranslateTransform3D(partInstance.Position.Vector3D);
                        (part.Transform as Transform3DGroup).Children[0] = new RotateTransform3D(new QuaternionRotation3D(rotation));

                        part.Size = partInstance.Size.Size3D;
                        part.Material = partInstance.Material;
                    }, DispatcherPriority.Render);

                };

                Transform3DGroup group = new Transform3DGroup();
                group.Children.Add(rotateTransform);
                group.Children.Add(translateTransform);

                part.Transform = group;

                part.Size = partInstance.Size.Size3D;

                part.Material = partInstance.Material;

                VisualInstance visualInstance = new VisualInstance(part, partInstance);
                _visualInstances.Add(visualInstance);

                _physics.AddVisualInstance(visualInstance);

                _rootContainer.Children.Add(part);
            }
        }

        public bool RemoveInstance(Instance instance)
        {
            if (instance is Part)
            {
                Part partInstance = instance as Part;

                foreach (VisualInstance visualInstance in _visualInstances)
                {
                    if (visualInstance.Instance.Equals(instance))
                    {
                        _rootContainer.Children.Remove(visualInstance.Visual3D);
                        return true;
                    }
                }

                return false;
            }
            else
            {
                return false;
            }
        }

        private void InitializeLights()
        {
            _context.ActiveWorld.Sky.OnChanged += (sender, e) =>
            {
                _lightHorizontalAngle = _context.ActiveWorld.Sky.SunHorizontalAngle;
                _lightVerticalAngle = _context.ActiveWorld.Sky.SunVerticalAngle;
                _lightDistance = _context.ActiveWorld.Sky.SunDistance;

                _directionalLight.Direction = GetSunDirection();
                _directionalLight.SetDXAttribute(DXAttributeType.IsCastingShadow, _context.ActiveWorld.Sky.CastShadows);
            };

            _lightHorizontalAngle = _context.ActiveWorld.Sky.SunHorizontalAngle;
            _lightVerticalAngle = _context.ActiveWorld.Sky.SunVerticalAngle;

            _lightDistance = _context.ActiveWorld.Sky.SunDistance;

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
