using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VHClient
{
    //有参数的函数，必须指明函数指针为Cdecl类型，否则程序会跑飞！

    //封装一系列委托，用于代理函数指针
    //实际上不能用泛型
    /*
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate void CdeclAction<T>(T a);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate void CdeclAction<T1,T2>(T1 a1,T2 a2);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate void CdeclAction<T1,T2,T3>(T1 a1,T2 a2,T3 a3);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate void CdeclAction<T1,T2,T3,T4>(T1 a1,T2 a2,T3 a3,T4 a4);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate T CdeclFunc<T>();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate T CdeclFunc<T,T1>(T1 a);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate T CdeclFunc<T,T1, T2>(T1 a1, T2 a2);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate T CdeclFunc<T,T1, T2, T3>(T1 a1, T2 a2, T3 a3);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate T CdeclFunc<T,T1, T2, T3, T4>(T1 a1, T2 a2, T3 a3, T4 a4);
    */
    //传入函数指针的时候不能用泛型，所以这样写

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate void CdeclActionByteByte(byte a,byte b);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    unsafe delegate void CdeclActionByteA(byte* a);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    unsafe delegate void CdeclActionByteAByteA(byte* a,byte* b);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate byte CdeclFuncByteByte(byte x);

}
