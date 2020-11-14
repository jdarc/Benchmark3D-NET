using System;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Benchmark.Engine
{
    internal static class ArrayExtensions
    {
        public static void Fill<T>(this T[] array, T value) where T : unmanaged
        {
            if (Sse.IsSupported)
                unsafe
                {
                    fixed (void* mem = array)
                    {
                        var ptr = (Vector256<int>*) mem;
                        var val = Vector256.Create(*(int*) &value);
                        for (int x = 0, length = array.Length >> 3; x < length; ++x) ptr[x] = val;
                    }
                }
            else Array.Fill(array, value);
        }

        public static void CopyTo(this int[] source, IntPtr dstPtr)
        {
            if (Sse.IsSupported)
                unsafe
                {
                    fixed (void* srcPtr = source)
                    {
                        var dst = (Vector256<int>*) dstPtr.ToPointer();
                        var src = (Vector256<int>*) srcPtr;
                        var total = source.Length >> 3;
                        for (var x = 0; x < total; ++x) dst[x] = src[x];
                    }
                }
            else Marshal.Copy(source, 0, dstPtr, source.Length);
        }
    }
}