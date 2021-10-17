using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;

namespace Vortice
{

    public class MyGameWindow : GameWindow
    {

        public MyGameWindow(string name) :
            base(new GameWindowSettings { IsMultiThreaded = true }, new NativeWindowSettings { Title = name, API = ContextAPI.NoAPI })
        {
        }

        public override void Run()
        {
            // After accepting PR https://github.com/opentk/opentk/pull/1334
            // we don't need to override the Run method anymore :-)
            while (!IsExiting)
            {
                ProcessEvents();
                OnRenderFrame(new FrameEventArgs());
            }
        }

    }
}