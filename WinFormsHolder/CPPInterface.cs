using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsHolder
{
    internal class CPPInterface
    {
        public delegate void WindowEventDelegate(bool Create, IntPtr win);
        internal const string DllName = "Holder.dll";
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetMainWindow(IntPtr hWnd, WindowEventDelegate callback);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ReleaseMainWindow();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetContainer();

        [DllImport("user32", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DisableProcessWindowsGhosting();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder lpString, int nMaxCount);
        public static string GetWindowText(IntPtr hWnd)
        {
            StringBuilder sb = new StringBuilder(256);
            GetWindowText(hWnd, sb, 256);
            return sb.ToString();
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport(DllName, CharSet = CharSet.Auto)]
        public static extern void ShowExplorerWin(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern bool DestroyWindow(IntPtr hWnd);
        public static void CloseWindow(IntPtr hWnd)
        {
            DestroyWindow(hWnd);
        }
        [DllImport(DllName, CharSet = CharSet.Auto)]
        public static extern IntPtr GetExplorerWindow(int index);
        [DllImport(DllName, CharSet = CharSet.Auto)]
        public static extern int GetExplorerWindowCount();
        [DllImport(DllName, CharSet = CharSet.Auto)]
        public static extern void UpdateMainWinSize();
        [DllImport(DllName, CharSet = CharSet.Auto)]
        public static extern void CloseExplorerWin(IntPtr wnd);
    }
}
