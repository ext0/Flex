//#define MOGRECOMPOSITOR
#define MOGREBLIT

using Mogre;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Flex.Development.Rendering.Modules
{
    public class MogreImage : System.Windows.Interop.D3DImage
    {
        private Root _root;
        private Camera _camera;
        private uint _width;
        private uint _height;

        private TexturePtr _frontTexture;
        private RenderTarget _frontTarget;
#if MOGRECOMPOSITOR
        private Viewport _frontViewPort;
        private CompositorInstance _renderingCompositor;
        private SceneManager _fakeScene;
        private Camera _fakeCamera;
#endif
        private TexturePtr _backTexture;
        private RenderTarget _backTarget;

        private Duration lockDuration = new Duration(new TimeSpan(0, 0, 0, 0, 100));

        public MogreImage(Root root, Camera camera, uint width, uint height)
        {
            _root = root;
            _camera = camera;
            _width = width;
            _height = height;
#if MOGRECOMPOSITOR
            _fakeScene = root.CreateSceneManager(SceneType.ST_GENERIC);
            _fakeCamera = _fakeScene.CreateCamera("Fake");
#endif
            ReInitRenderTargets();
        }

        protected void DestroyRenderTargets()
        {
            if (_frontTarget == null)
                return;

            //First Detach the render target.
            Lock();
            try
            {
                SetBackBuffer(System.Windows.Interop.D3DResourceType.IDirect3DSurface9, IntPtr.Zero);
            }
            finally
            {
                Unlock();
            }

#if MOGRECOMPOSITOR
            _renderingCompositor.Enabled = false;
            CompositorManager.Singleton.RemoveCompositor(_frontViewPort, "WPFOutput");
            _renderingCompositor.Dispose();
            _renderingCompositor = null;
            CompositorManager.Singleton.ReloadUnreferencedResources();
            _frontViewPort.Dispose();

            //Unload the underlying material so that we can clean up the Textures.
            MaterialManager.Singleton.GetByName("WPF/Compositor/Identity").Target.Unload();
#endif
            _frontTarget.RemoveAllViewports();

            _root.RenderSystem.DestroyRenderTarget(_frontTarget.Name);

            _frontTarget.Dispose();
            _frontTarget = null;
            TextureManager.Singleton.Remove("MogreImageFrontBuffer");
            _frontTexture.Dispose();
            _frontTexture = null;

            _root.RenderSystem.DestroyRenderTarget(_backTarget.Name);
            _backTarget.Dispose();
            _backTarget = null;
            TextureManager.Singleton.Remove("MogreImageBackBuffer");
            _backTexture.Dispose();
            _backTexture = null;
        }

        protected void ReInitRenderTargets()
        {
            DestroyRenderTargets();

            _frontTexture = TextureManager.Singleton.CreateManual("MogreImageFrontBuffer",
                ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME, TextureType.TEX_TYPE_2D,
                _width, _height, 0, PixelFormat.PF_A8R8G8B8,
                (int)TextureUsage.TU_RENDERTARGET | (int)TextureUsage.TU_DYNAMIC);

            _backTexture = TextureManager.Singleton.CreateManual("MogreImageBackBuffer",
                ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME, TextureType.TEX_TYPE_2D,
                _width, _height, 0, PixelFormat.PF_A8R8G8B8,
                (int)TextureUsage.TU_RENDERTARGET | (int)TextureUsage.TU_DYNAMIC);

            _frontTarget = _frontTexture.GetBuffer().GetRenderTarget();
            _backTarget = _backTexture.GetBuffer().GetRenderTarget();

#if MOGRECOMPOSITOR
            _frontViewPort = _frontTarget.AddViewport(_fakeCamera);
            _renderingCompositor = CompositorManager.Singleton.AddCompositor(_frontViewPort, "WPFOutput");

            _renderingCompositor.Enabled = true;
#endif

            _backTarget.AddViewport(_camera);

            _frontTarget.IsAutoUpdated = false;

            MarkFrameDirty();
        }

        public void CompositeToWPF(object sender, EventArgs e)
        {
            Lock();
            try
            {
#if MOGREBLIT
                _backTexture.CopyToTexture(_frontTexture);
#endif
#if MOGRECOMPOSITOR
                _frontTarget.Update();
#endif
                IntPtr surface;
                _frontTarget.GetCustomAttribute("DDBACKBUFFER", out surface);
                SetBackBuffer(System.Windows.Interop.D3DResourceType.IDirect3DSurface9, surface);
                AddDirtyRect(new System.Windows.Int32Rect(0, 0, 1, 1));
            }
            finally
            {
                Unlock();
            }
        }

        public void SetSize(uint Width, uint Height)
        {
            bool NeedsReInit = false;

            if (_width != Width)
            {
                _width = Width;
                NeedsReInit = true;
            }

            if (_height != Height)
            {
                _height = Height;
                NeedsReInit = true;
            }

            if (NeedsReInit)
            {
                DestroyRenderTargets();
                ReInitRenderTargets();
            }
        }

        public void PostRender()
        {
            MarkFrameDirty();
        }

        public void MarkFrameDirty()
        {
            Lock();
            try
            {
                IntPtr surface;
                _frontTarget.GetCustomAttribute("DDBACKBUFFER", out surface);
                SetBackBuffer(System.Windows.Interop.D3DResourceType.IDirect3DSurface9, surface);
                AddDirtyRect(new Int32Rect(0, 0, (int)_frontTarget.Width, (int)_frontTarget.Height));
            }
            finally
            {
                Unlock();
            }
        }

        public void Shutdown()
        {
#if MOGRECOMPOSITOR
            //Delete the fake camera and Scene Manager.
            _fakeCamera.Dispose();
            _fakeCamera = null;
            _fakeScene.DestroyAllCameras();
            _root.DestroySceneManager(_fakeScene);
            _fakeScene.Dispose();
            _fakeScene = null;
#endif
            DestroyRenderTargets();
        }
    }
}
