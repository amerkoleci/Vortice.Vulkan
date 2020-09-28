// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Vortice.Win32;
using static Vortice.Win32.Kernel32;
using static Vortice.Win32.User32;

namespace Vortice
{
    public abstract class Application : IDisposable
    {
        internal static readonly string WndClassName = "VorticeWindow";
        private WNDPROC _wndProc;

        private bool _paused;

        protected Application()
        {
        }

        public abstract string Name { get; }

        public Window MainWindow { get; private set; }

        public virtual void Dispose()
        {
        }

        protected virtual void Initialize()
        {

        }

        protected virtual void OnTick()
        {
        }

        public void Run()
        {
            PlatformInitialize();
            Initialize();

            Message msg = default;
            while (msg.Value != (uint)WindowMessage.Quit)
            {
                //if (!_paused)
                {
                    const uint PM_REMOVE = 1;
                    if (PeekMessage(out msg, IntPtr.Zero, 0, 0, PM_REMOVE))
                    {
                        TranslateMessage(ref msg);
                        DispatchMessage(ref msg);
                    }
                    else
                    {
                        OnTick();
                    }
                }
                //else
                //{
                //    var ret = GetMessage(out msg, IntPtr.Zero, 0, 0);
                //    if (ret == 0)
                //    {
                //        //_exitRequested = true;
                //        break;
                //    }
                //    else if (ret == -1)
                //    {
                //        //Log.Error("[Win32] - Failed to get message");
                //        //_exitRequested = true;
                //        break;
                //    }
                //    else
                //    {
                //        TranslateMessage(ref msg);
                //        DispatchMessage(ref msg);
                //    }
                //}
            }
        }

        protected virtual void OnActivated()
        {
        }

        protected virtual void OnDeactivated()
        {
        }

        protected virtual void OnDraw(int width, int height)
        {

        }

        private void PlatformInitialize()
        {
            _wndProc = ProcessWindowMessage;
            var wndClassEx = new WNDCLASSEX
            {
                Size = Unsafe.SizeOf<WNDCLASSEX>(),
                Styles = WindowClassStyles.CS_HREDRAW | WindowClassStyles.CS_VREDRAW | WindowClassStyles.CS_OWNDC,
                WindowProc = _wndProc,
                InstanceHandle = GetModuleHandle(null),
                CursorHandle = LoadCursor(IntPtr.Zero, SystemCursor.IDC_ARROW),
                BackgroundBrushHandle = IntPtr.Zero,
                IconHandle = IntPtr.Zero,
                ClassName = WndClassName,
            };

            var atom = RegisterClassEx(ref wndClassEx);

            if (atom == 0)
            {
                throw new InvalidOperationException(
                    $"Failed to register window class. Error: {Marshal.GetLastWin32Error()}"
                    );
            }

            // Create main window.
            MainWindow = new Window(Name, 1280, 720);
        }

        private IntPtr ProcessWindowMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            if (msg == (uint)WindowMessage.ActivateApp)
            {
                _paused = IntPtrToInt32(wParam) == 0;
                if (IntPtrToInt32(wParam) != 0)
                {
                    OnActivated();
                }
                else
                {
                    OnDeactivated();
                }

                return DefWindowProc(hWnd, msg, wParam, lParam);
            }

            switch ((WindowMessage)msg)
            {
                case WindowMessage.Destroy:
                    PostQuitMessage(0);
                    break;
            }

            return DefWindowProc(hWnd, msg, wParam, lParam);
        }

        private static int SignedLOWORD(int n)
        {
            return (short)(n & 0xFFFF);
        }

        private static int IntPtrToInt32(IntPtr intPtr)
        {
            return (int)intPtr.ToInt64();
        }
    }
}
