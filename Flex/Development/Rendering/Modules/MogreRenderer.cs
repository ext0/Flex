using Flex.Development.Execution.Data;
using Flex.Development.Instances;
using Mogre;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

/*
    Bootstrap code courtesy of https://code.google.com/archive/p/mogresdk/
*/

namespace Flex.Development.Rendering.Modules
{
    public class MogreRenderer
    {
        private Root _root;
        private RenderWindow _renderWindow;

        private SceneManager _scene;
        private Mogre.Camera _viewCamera;

        private Timer _timer;

        internal SceneManager Scene
        {
            get
            {
                return _scene;
            }
        }

        internal RenderWindow RenderWindow
        {
            get
            {
                return _renderWindow;
            }
        }

        public MogreRenderer(String handle, uint width, uint height)
        {
            _root = new Root("../plugins.cfg", "../ogre.cfg", "../flexrender.log");
            _root.RenderSystem = _root.GetRenderSystemByName("Direct3D9 Rendering Subsystem");
            _root.Initialise(false);

            ConfigFile file = new ConfigFile();
            file.Load("../resources.cfg", "\t:=", true);
            ConfigFile.SectionIterator secIter = file.GetSectionIterator();
            while (secIter.MoveNext())
            {
                String secName = secIter.CurrentKey;
                if (secIter.Current.IsEmpty)
                    continue;
                foreach (var entry in secIter.Current)
                    ResourceGroupManager.Singleton.AddResourceLocation(entry.Value, entry.Key, secName);
                ResourceGroupManager.Singleton.InitialiseResourceGroup(secName);
            }

            _timer = new Timer();

            NameValuePairList config = new NameValuePairList();
            config["externalWindowHandle"] = handle;
            /*
            config["vsync"] = "False";
            config["FSAA"] = "2";
            config["Multithreaded"] = "False";
            */

            _renderWindow = _root.CreateRenderWindow("Mogre Window", width, height, false, config);
            _renderWindow.IsAutoUpdated = false;
        }

        public void ReinitRenderWindow(String handle, uint width, uint height)
        {
            if (_root == null)
            {
                return;
            }
            NameValuePairList config = new NameValuePairList();
            config["externalWindowHandle"] = handle;
            /*
            config["vsync"] = "False";
            config["FSAA"] = "2";
            config["Multithreaded"] = "False";
            */

            _renderWindow = _root.CreateRenderWindow("Mogre Window", width, height, false, config);
            _renderWindow.IsAutoUpdated = false;
            _renderWindow.AddViewport(_viewCamera);
        }

        public Mogre.Camera Camera
        {
            get
            {
                return _viewCamera;
            }
            internal set
            {
                _viewCamera = value;
            }
        }

        public SceneNode CreateEntity(out Entity entity, String entityMesh, PositionedInstance instance)
        {
            entity = _scene.CreateEntity(entityMesh);
            entity.CastShadows = true;
            entity.QueryFlags = (uint)QueryFlags.INSTANCE_ENTITY;
            SceneNode node = _scene.CreateSceneNode();
            node.AttachObject(entity);
            _scene.RootSceneNode.AddChild(node);
            Engine.SceneNodeStore.AddSceneNode(node, instance);
            return node;
        }

        public SceneNode CreateEntity(out Entity entity, String entityMesh)
        {
            entity = _scene.CreateEntity(entityMesh);
            entity.QueryFlags = (uint)QueryFlags.NON_INSTANCE_ENTITY;
            SceneNode node = _scene.CreateSceneNode();
            node.AttachObject(entity);
            return node;
        }

        public void CreateScene()
        {
            _scene = _root.CreateSceneManager(SceneType.ST_GENERIC);

            _scene.AmbientLight = new ColourValue(
                ActiveScene.Context.ActiveWorld.Sky.ambient.R / 255f,
                ActiveScene.Context.ActiveWorld.Sky.ambient.G / 255f,
                ActiveScene.Context.ActiveWorld.Sky.ambient.B / 255f);

            _scene.SetSkyBox(true, "Skyboxes/Default", 500);
            _scene.ShadowTechnique = ShadowTechnique.SHADOWTYPE_STENCIL_MODULATIVE;
        }

        [HandleProcessCorruptedStateExceptions]
        public void Loop()
        {
            if (_viewCamera == null)
            {
                return;
            }
            _root.RenderSystem._setViewport(_viewCamera.Viewport);
            _root.RenderSystem.ClearFrameBuffer((uint)FrameBufferType.FBT_COLOUR | (uint)FrameBufferType.FBT_DEPTH);

            _scene._renderScene(_viewCamera, _viewCamera.Viewport, true);
            _renderWindow.SwapBuffers(true);

            _root.RenderOneFrame();
        }

        public void OnResize()
        {
            if (_renderWindow != null)
            {
                _renderWindow.WindowMovedOrResized();
            }
        }

        public void Shutdown()
        {
            _renderWindow.Destroy();
            _renderWindow.Dispose();
            _renderWindow = null;

            _root.Shutdown();
            //Commented out to prevent a crash.
            //Root.Dispose();
            //Root = null;
        }
    }
}
