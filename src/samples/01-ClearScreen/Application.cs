using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Vortice
{

    public abstract class Application
    {
        protected Application()
        {
        }

        public abstract string Name { get; }

        [NotNull]
        public MyGameWindow MainWindow { get; private set; } = default!;

        protected virtual void Initialize()
        {
            MainWindow = new MyGameWindow(Name);
            MainWindow.RenderFrame += (e) =>
            {
                OnRenderFrame();
            };
        }

        protected virtual void OnRenderFrame()
        {
        }

        public void Run()
        {
            Initialize();
            MainWindow.Run();
        }

    }
}