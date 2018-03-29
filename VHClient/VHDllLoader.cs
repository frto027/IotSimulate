using System;
using System.Runtime.InteropServices;


namespace VHClient
{
    class VHDllLoader
    {
        public enum CallBacks
        {
            CB_USART_READ,
            CB_USART_WRITE,
            CB_COUNT
        };

        [DllImport("kernel32.dll")]
        private extern static IntPtr LoadLibrary(string path);

        [DllImport("kernel32.dll")]
        private extern static IntPtr GetProcAddress(IntPtr lib, string funcName);

        [DllImport("kernel32.dll")]
        private extern static bool FreeLibrary(IntPtr lib);

        IntPtr DllPtr;

        public VHDllLoader(string dllPath)
        {
            DllPtr = LoadLibrary(dllPath);
            if (DllPtr == IntPtr.Zero)
                throw new ArgumentException("load failed:" + dllPath);
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void VHPipe_SetupLinks_TYPE(sbyte id, Delegate callback);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int Main_Type();

        public void SetupLinks(CallBacks id, Delegate func)
        {
            if (DllPtr == IntPtr.Zero)
                throw new ArgumentException("no dll loaded");
            IntPtr fptr = GetProcAddress(DllPtr, "VHPipe_SetupLinks");
            if (fptr == IntPtr.Zero)
                throw new Exception("invalid dll:can not load function VHPipe_SetupLinks");
            Marshal.GetDelegateForFunctionPointer<VHPipe_SetupLinks_TYPE>(fptr)((sbyte)id, func);
        }

        public int RunMain()
        {
            if (DllPtr == IntPtr.Zero)
                throw new ArgumentException("no dll loaded");
            IntPtr fptr = GetProcAddress(DllPtr, "main");
            if (fptr == IntPtr.Zero)
                throw new Exception("invalid dll:can not load function main");
            return Marshal.GetDelegateForFunctionPointer<Main_Type>(fptr)();
        }

        public void Close()
        {
            if (DllPtr != IntPtr.Zero)
            {
                FreeLibrary(DllPtr);
                DllPtr = IntPtr.Zero;
            }
        }
    }
}
