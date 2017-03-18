using Flex.Development.Execution.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace Flex.Development.Rendering.Modules
{
    public class KeyboardHandler
    {
        private double _wA = 1;
        private double _aA = 1;
        private double _sA = 1;
        private double _dA = 1;

        private double _defaultSpeed = 0.08d * 4;

        private System.Windows.Forms.Integration.WindowsFormsHost _host;
        private Panel _panel;

        public KeyboardHandler(System.Windows.Forms.Integration.WindowsFormsHost host, Panel panel)
        {
            _host = host;
            _panel = panel;
        }

        public void Initialize()
        {
            _host.KeyDown += _host_KeyDown;
            _host.KeyUp += _host_KeyUp;
        }

        public void KeyboardTick()
        {
            foreach (int key in ActiveScene.GetRegisteredKeyPressKeys())
            {
                bool down = false;
                Engine.RunOnUIThread(() =>
                {
                    down = Keyboard.IsKeyDown((Key)key);
                });
                if (down)
                {
                    ActiveScene.RunKeyCallback(KeyAction.KeyPress, key);
                }
            }
            bool none = true;
            bool aDown = false;
            bool wDown = false;
            bool sDown = false;
            bool dDown = false;
            double shiftMultiplier = 1.0d;
            Engine.RunOnUIThread(() =>
            {
                wDown = Keyboard.IsKeyDown(Key.W);
                aDown = Keyboard.IsKeyDown(Key.A);
                sDown = Keyboard.IsKeyDown(Key.S);
                dDown = Keyboard.IsKeyDown(Key.D);
                shiftMultiplier = Keyboard.IsKeyDown(Key.LeftShift) ? 2.0d : shiftMultiplier;
            });
            if (wDown)
            {
                ActiveScene.Context.ActiveWorld.Camera.move(Engine.Renderer.Camera.Direction * (float)(_defaultSpeed * _wA * shiftMultiplier));
                none = false;
            }
            if (aDown)
            {
                ActiveScene.Context.ActiveWorld.Camera.move(-Engine.Renderer.Camera.Right * (float)(_defaultSpeed * _aA * shiftMultiplier));
                none = false;
            }
            if (sDown)
            {
                ActiveScene.Context.ActiveWorld.Camera.move(-Engine.Renderer.Camera.Direction * (float)(_defaultSpeed * _sA * shiftMultiplier));
                none = false;
            }
            if (dDown)
            {
                ActiveScene.Context.ActiveWorld.Camera.move(Engine.Renderer.Camera.Right * (float)(_defaultSpeed * _dA * shiftMultiplier));
                none = false;
            }
            if (none)
            {
                _wA = 1;
                _aA = 1;
                _sA = 1;
                _dA = 1;
            }
        }

        private void _host_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            int key = (int)e.Key;
            if (ActiveScene.Running)
            {
                ActiveScene.RunKeyCallback(KeyAction.KeyUp, key);
            }
        }

        private void _host_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            int key = (int)e.Key;
            if (ActiveScene.Running)
            {
                ActiveScene.RunKeyCallback(KeyAction.KeyDown, key);
            }
        }
    }
}
