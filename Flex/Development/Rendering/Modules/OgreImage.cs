using Mogre;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace Flex.Development.Rendering.Modules
{
    public class OgreImage : D3DImage, ISupportInitialize
    {
        private delegate void MethodInvoker();

        #region ViewportSize Property

        public static readonly DependencyProperty ViewportSizeProperty =
            DependencyProperty.Register("ViewportSize", typeof(Size), typeof(OgreImage),
                                        new PropertyMetadata(new Size(100, 100), OnViewportProperyChanged)
                );

        private static void OnViewportProperyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var imageSource = (OgreImage)d;

            imageSource._reloadRenderTargetTime = Environment.TickCount;
        }

        public Size ViewportSize
        {
            get { return (Size)GetValue(ViewportSizeProperty); }
            set { SetValue(ViewportSizeProperty, value); }
        }

        #endregion

        #region AutoInitialise Property

        public static readonly DependencyProperty AutoInitialiseProperty =
            DependencyProperty.Register("AutoInitialise", typeof(bool), typeof(OgreImage),
                                        new PropertyMetadata(false));

        public bool AutoInitialise
        {
            get { return (bool)GetValue(AutoInitialiseProperty); }
            set { SetValue(AutoInitialiseProperty, value); }
        }

        #endregion

        #region CreateDefaultScene Property

        public static readonly DependencyProperty CreateDefaultSceneProperty =
            DependencyProperty.Register("CreateDefaultScene", typeof(bool), typeof(OgreImage),
                                        new PropertyMetadata(true));

        public bool CreateDefaultScene
        {
            get { return (bool)GetValue(CreateDefaultSceneProperty); }
            set { SetValue(CreateDefaultSceneProperty, value); }
        }

        #endregion

        #region ResizeRenderTargetDelay Property

        public static readonly DependencyProperty ResizeRenderTargetDelayProperty =
            DependencyProperty.Register("ResizeRenderTargetDelay", typeof(Duration), typeof(OgreImage),
            new PropertyMetadata(new Duration(new TimeSpan(200))));

        public Duration ResizeRenderTargetDelay
        {
            get { return (Duration)GetValue(ResizeRenderTargetDelayProperty); }
            set { SetValue(ResizeRenderTargetDelayProperty, value); }
        }

        #endregion

        #region FrameRate Property

        public static readonly DependencyProperty FrameRateProperty =
            DependencyProperty.Register("FrameRate", typeof(int?), typeof(OgreImage),
            new PropertyMetadata(FrameRate_Changed));

        private static void FrameRate_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((OgreImage)d).OnFrameRateChanged((int?)e.NewValue);
        }

        public int? FrameRate
        {
            get { return (int?)GetValue(FrameRateProperty); }
            set { SetValue(FrameRateProperty, value); }
        }

        #endregion

        private Root _root;
        private TexturePtr _texture;
        private RenderWindow _renderWindow;
        private Viewport _viewport;
        private SceneManager _sceneManager;
        private RenderTarget _renTarget;
        private int _reloadRenderTargetTime;
        private bool _imageSourceValid;
        private Thread _currentThread;
        private DispatcherTimer _timer;
        private bool _eventAttatched;

        #region IDisposable Members

        public void Dispose()
        {
            IsFrontBufferAvailableChanged -= _isFrontBufferAvailableChanged;

            DetachRenderTarget(true, true);

            if (_currentThread != null)
            {
                _currentThread.Abort();
            }

            if (_root != null)
            {
                DisposeRenderTarget();
                CompositorManager.Singleton.RemoveAll();

                _root.Dispose();
                _root = null;
            }

            GC.SuppressFinalize(this);
        }

        #endregion

        #region ISupportInitialize Members

        public void BeginInit()
        {
        }

        public void EndInit()
        {
            if (AutoInitialise)
            {
                InitOgre();
            }
        }

        #endregion

        protected bool _InitOgre()
        {
            lock (this)
            {
                IntPtr hWnd = IntPtr.Zero;

                foreach (PresentationSource source in PresentationSource.CurrentSources)
                {
                    var hwndSource = (source as HwndSource);
                    if (hwndSource != null)
                    {
                        hWnd = hwndSource.Handle;
                        break;
                    }
                }

                if (hWnd == IntPtr.Zero) return false;

                //CallResourceItemLoaded(new ResourceLoadEventArgs("Engine", 0));

                // load the OGRE engine
                //
                _root = new Root("../plugins.cfg", "../ogre.cfg", "../flexrender.log");
                _root.RenderSystem = _root.GetRenderSystemByName("Direct3D9Ex Rendering Subsystem");
                // configure resource paths from : "resources.cfg" file
                //
                var configFile = new ConfigFile();
                configFile.Load("../resources.cfg", "\t:=", true);

                // Go through all sections & settings in the file
                //
                ConfigFile.SectionIterator seci = configFile.GetSectionIterator();

                // Normally we would use the foreach syntax, which enumerates the values, 
                // but in this case we need CurrentKey too;
                while (seci.MoveNext())
                {
                    string secName = seci.CurrentKey;

                    ConfigFile.SettingsMultiMap settings = seci.Current;
                    foreach (var pair in settings)
                    {
                        string typeName = pair.Key;
                        string archName = pair.Value;
                        ResourceGroupManager.Singleton.AddResourceLocation(archName, typeName, secName);
                    }
                }

                _root.RenderSystem.SetConfigOption("Full Screen", "No");
                _root.RenderSystem.SetConfigOption("Video Mode", "640 x 480 @ 32-bit colour");

                _root.Initialise(false);

                var misc = new NameValuePairList();
                misc["externalWindowHandle"] = hWnd.ToString();
                misc["vsync"] = "False";
                misc["FSAA"] = "2";
                misc["Multithreaded"] = "False";

                _renderWindow = _root.CreateRenderWindow("OgreImageSource Windows", 0, 0, false, misc);
                _renderWindow.IsAutoUpdated = false;

                //InitResourceLoad();

                ResourceGroupManager.Singleton.InitialiseAllResourceGroups();

                Dispatcher.Invoke(
                    (MethodInvoker)delegate
                    {
                        IsFrontBufferAvailableChanged += _isFrontBufferAvailableChanged;

                        _sceneManager = _root.CreateSceneManager(SceneType.ST_GENERIC);
                        if (Initialised != null)
                            Initialised(this, new RoutedEventArgs());

                        ReInitRenderTarget();
                        AttachRenderTarget(true);

                        OnFrameRateChanged(this.FrameRate);

                        _currentThread = null;
                    });

                return true;
            }
        }


        public bool InitOgre()
        {
            return _InitOgre();
        }

        public Thread InitOgreAsync(ThreadPriority priority, RoutedEventHandler completeHandler)
        {
            if (completeHandler != null)
                Initialised += completeHandler;

            _currentThread = new Thread(() => _InitOgre())
            {
                Priority = priority
            };
            _currentThread.Start();

            return _currentThread;
        }

        public void InitOgreAsync()
        {
            InitOgreAsync(ThreadPriority.Normal, null);
        }

        public event RoutedEventHandler Initialised;
        public event EventHandler PreRender;
        public event EventHandler PostRender;

        protected void RenderFrame()
        {
            if (PreRender != null)
                PreRender(this, EventArgs.Empty);

            _root.RenderOneFrame();

            if (PostRender != null)
                PostRender(this, EventArgs.Empty);
        }

        protected void DisposeRenderTarget()
        {
            if (_renTarget != null)
            {
                CompositorManager.Singleton.RemoveCompositorChain(_viewport);
                _renTarget.RemoveAllListeners();
                _renTarget.RemoveAllViewports();
                _root.RenderSystem.DestroyRenderTarget(_renTarget.Name);
                _renTarget = null;
                _viewport = null;
            }

            if (_texture != null)
            {
                TextureManager.Singleton.Remove(_texture.Handle);
                _texture.Dispose();
                _texture = null;
            }
        }

        protected void ReInitRenderTarget()
        {
            DetachRenderTarget(true, false);
            DisposeRenderTarget();

            _texture = TextureManager.Singleton.CreateManual(
                "OgreImageSource RenderTarget",
                ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME,
                TextureType.TEX_TYPE_2D,
                (uint)ViewportSize.Width, (uint)ViewportSize.Height,
                0, Mogre.PixelFormat.PF_A8R8G8B8,
                (int)TextureUsage.TU_RENDERTARGET);

            _renTarget = _texture.GetBuffer().GetRenderTarget();

            _reloadRenderTargetTime = 0;
        }

        public Root Root
        {
            get
            {
                return _root;
            }
        }

        public SceneManager SceneManager
        {
            get
            {
                return _sceneManager;
            }
        }

        public RenderWindow RenderWindow
        {
            get
            {
                return _renderWindow;
            }
        }

        protected virtual void AttachRenderTarget(bool attachEvent)
        {
            if (!_imageSourceValid)
            {
                Lock();
                try
                {
                    IntPtr surface;
                    _renTarget.GetCustomAttribute("DDBACKBUFFER", out surface);
                    SetBackBuffer(D3DResourceType.IDirect3DSurface9, surface);

                    _imageSourceValid = true;
                }
                finally
                {
                    Unlock();
                }
            }

            if (attachEvent)
                UpdateEvents(true);
        }

        protected virtual void DetachRenderTarget(bool detatchSurface, bool detatchEvent)
        {
            if (detatchSurface && _imageSourceValid)
            {
                Lock();
                SetBackBuffer(D3DResourceType.IDirect3DSurface9, IntPtr.Zero);
                Unlock();

                _imageSourceValid = false;
            }

            if (detatchEvent)
                UpdateEvents(false);
        }

        protected virtual void UpdateEvents(bool attach)
        {
            _eventAttatched = attach;
            if (attach)
            {
                if (_timer != null)
                    _timer.Tick += _rendering;
                else
                    CompositionTarget.Rendering += _rendering;
            }
            else
            {
                if (_timer != null)
                    _timer.Tick -= _rendering;
                else
                    CompositionTarget.Rendering -= _rendering;
            }
        }

        private void _isFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsFrontBufferAvailable)
                AttachRenderTarget(true); // might not succeed
            else
                // need to keep old surface attached because it's the only way to get the front buffer active event.
                DetachRenderTarget(false, true);
        }

        private void _rendering(object sender, EventArgs e)
        {
            if (_root == null) return;

            if (IsFrontBufferAvailable)
            {
                /*
                if (MogreWpf.Interop.D3D9RenderSystem.IsDeviceLost(_renderWindow))
                {
                    _renderWindow.Update(); // try restore
                    _reloadRenderTargetTime = -1;

                    if (MogreWpf.Interop.D3D9RenderSystem.IsDeviceLost(_renderWindow))
                        return;
                }
                */

                long durationTicks = ResizeRenderTargetDelay.TimeSpan.Ticks;

                // if the new next ReInitRenderTarget() interval is up
                if (((_reloadRenderTargetTime < 0) || (durationTicks <= 0))
                    // negative time will be reloaded immediatly
                    ||
                    ((_reloadRenderTargetTime > 0) &&
                     (Environment.TickCount >= (_reloadRenderTargetTime + durationTicks))))
                {
                    ReInitRenderTarget();
                }

                if (!_imageSourceValid)
                    AttachRenderTarget(false);

                Lock();
                RenderFrame();
                AddDirtyRect(new Int32Rect(0, 0, PixelWidth, PixelHeight));
                Unlock();
            }
        }

        private void OnFrameRateChanged(int? newFrameRate)
        {
            bool wasAttached = _eventAttatched;
            UpdateEvents(false);

            if (newFrameRate == null)
            {
                if (_timer != null)
                {
                    _timer.Tick -= _rendering;
                    _timer.Stop();
                    _timer = null;
                }
            }
            else
            {
                if (_timer == null)
                    _timer = new DispatcherTimer();

                _timer.Interval = new TimeSpan(1000 / newFrameRate.Value);
                _timer.Start();
            }

            if (wasAttached)
                UpdateEvents(true);
        }
    }
}
