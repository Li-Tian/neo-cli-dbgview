﻿using DbgViewTR;
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Neo
{
    internal static class Helper
    {
        public static bool CompareTo(this SecureString s1, SecureString s2)
        {
            TR.enter();
            if (s1.Length != s2.Length)
                return TR.exit(false);
            IntPtr p1 = IntPtr.Zero;
            IntPtr p2 = IntPtr.Zero;
            try
            {
                p1 = SecureStringMarshal.SecureStringToGlobalAllocAnsi(s1);
                p2 = SecureStringMarshal.SecureStringToGlobalAllocAnsi(s2);
                int i = 0;
                while (true)
                {
                    byte b1 = Marshal.ReadByte(p1, i);
                    byte b2 = Marshal.ReadByte(p2, i++);
                    if (b1 == 0 && b2 == 0)
                        return TR.exit(true);
                    if (b1 != b2)
                        return TR.exit(false);
                    if (b1 == 0 || b2 == 0)
                        return TR.exit(false);
                }
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocAnsi(p1);
                Marshal.ZeroFreeGlobalAllocAnsi(p2);
            }
        }
    }
}