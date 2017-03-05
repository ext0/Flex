﻿using Flex.Development.Execution.Data;
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
    public static class Vector3Extension
    {
        public static Vector3 SphericalUniformToCartesian(this Vector3 Value)
        {
            Vector3 Result = Vector3.ZERO;

            Result.x = Value.x * Mogre.Math.Sin(Value.y * Mogre.Math.PI) * Mogre.Math.Cos(Value.z * Mogre.Math.PI);
            Result.z = Value.x * Mogre.Math.Sin(Value.y * Mogre.Math.PI) * Mogre.Math.Sin(Value.z * Mogre.Math.PI);
            Result.y = Value.x * Mogre.Math.Cos(Value.y * Mogre.Math.PI);

            return Result;
        }

        public static Vector3 RangeTrimSphericalUniform(this Vector3 Value)
        {
            //Uniform coordinate z should be between 0 & 2.0 and y should be between 0 and +0.5.
            if (Value.z > 2.0f)
                Value.z = Value.z % 2.0f;
            else if (Value.z < 0)
                Value.z = 2.0f + (Value.z % 2.0f);

            if (Value.y > 0.99f)
                Value.y = 0.99f;
            else if (Value.y < 0.01)
                Value.y = 0.01f;

            return Value;
        }
    }

    public class MogreRenderer
    {
        private Root _root;
        private RenderWindow _renderWindow;
        private MogreImage Image;

        private SceneManager _scene;
        private Camera _viewCamera;

        private Timer _timer;

        internal SceneManager Scene
        {
            get
            {
                return _scene;
            }
        }

        public void Init()
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
        }

        public void AttachRenderWindow(IntPtr Handle)
        {
            NameValuePairList config = new NameValuePairList();
            config["externalWindowHandle"] = Handle.ToString();
            config["vsync"] = "False";
            config["FSAA"] = "2";
            config["Multithreaded"] = "true";

            _renderWindow = _root.CreateRenderWindow("Mogre Window", 0, 0, false, config);
            _renderWindow.IsAutoUpdated = false;
        }

        public Camera Camera
        {
            get
            {
                return _viewCamera;
            }
        }

        public SceneNode CreateEntity(out Entity entity, String entityMesh)
        {
            entity = _scene.CreateEntity(entityMesh);
            SceneNode node = _scene.CreateSceneNode();
            node.AttachObject(entity);
            _scene.RootSceneNode.AddChild(node);
            return node;
        }

        public void CreateDefaultScene()
        {
            _scene = _root.CreateSceneManager(SceneType.ST_GENERIC);

            ColourValue color = new ColourValue(
                ActiveScene.Context.ActiveWorld.Sky.ambient.R / 255f,
                ActiveScene.Context.ActiveWorld.Sky.ambient.G / 255f,
                ActiveScene.Context.ActiveWorld.Sky.ambient.B / 255f);

            _scene.AmbientLight = color;

            _scene.SetSkyBox(true, "Skyboxes/Default", 500);

            _viewCamera = _scene.CreateCamera("ViewPoint");
            _viewCamera.ProjectionType = ProjectionType.PT_PERSPECTIVE;

            _viewCamera.Position = Vector3.ZERO;
            _viewCamera.LookAt(Vector3.ZERO);
            _viewCamera.NearClipDistance = 0.01f;
            _viewCamera.FarClipDistance = 1000.0f;
            _viewCamera.FOVy = new Degree(100f);
            _viewCamera.AutoAspectRatio = true;

            _renderWindow.AddViewport(_viewCamera);
        }

        public MogreImage CreateMogreImage(Tuple<uint, uint> Size)
        {
            return Image ?? (Image = new MogreImage(_root, _viewCamera, Size.Item1, Size.Item2));
        }

        [HandleProcessCorruptedStateExceptions]
        public void Loop()
        {
            float elapsed = _timer.Microseconds / 1000000f;
            _timer.Reset();

            Image.PrepareForRender(elapsed);
            try
            {
                _root.RenderOneFrame();
            }
            catch { }

            Engine.RunOnUIThread(Image.PostRender);
        }

        public void ManualRender()
        {
            _root.RenderOneFrame();
        }

        public void Shutdown()
        {
            _renderWindow.Destroy();
            _renderWindow.Dispose();
            _renderWindow = null;

            System.Windows.Forms.Application.DoEvents();

            _root.Shutdown();
            //Commented out to prevent a crash.
            //Root.Dispose();
            //Root = null;
        }
    }
}