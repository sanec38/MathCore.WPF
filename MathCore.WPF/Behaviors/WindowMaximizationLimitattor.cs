﻿using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using MathCore.Annotations;
using MathCore.WPF.pInvoke;
using Microsoft.Xaml.Behaviors;

namespace MathCore.WPF.Behaviors
{
    public class WindowMaximizationLimitattor : Behavior<UIElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.ForWindowFromTemplate(SetHandler);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.ForWindowFromTemplate(ResetHandler);
        }

        private void SetHandler(Window window) => window.SourceInitialized += OnWindowOnSourceInitialized;

        private void ResetHandler(Window window) => window.SourceInitialized -= OnWindowOnSourceInitialized;

        private void OnWindowOnSourceInitialized([NotNull] object sender, [CanBeNull] EventArgs e) => HwndSource.FromHwnd(new WindowInteropHelper((Window)sender).Handle)?.AddHook(WindowProc);

        private static IntPtr WindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_GETMINMAXINFO = 0x0024;
            if (msg == WM_GETMINMAXINFO)
            {
                WmGetMinMaxInfo(hWnd, lParam);
                handled = true;
            }
            return (IntPtr)0;
        }

        private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            var mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            // Adjust the maximized size and position to fit the work area of the correct monitor
            const int MONITOR_DEFAULTTONEAREST = 0x00000002;
            var monitor = hwnd.MonitorFromWindow(MONITOR_DEFAULTTONEAREST);

            if (monitor != IntPtr.Zero)
            {
                var monitorInfo = new MONITORINFO();
                monitor.GetMonitorInfo(monitorInfo);
                var rcWorkArea = monitorInfo.rcWork;
                var rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
            }
            Marshal.StructureToPtr(mmi, lParam, true);
        }
    }
}