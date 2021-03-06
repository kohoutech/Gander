﻿/* ----------------------------------------------------------------------------
Gander : a file format viewer
Copyright (C) 1998-2020  George E Greaney

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
----------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Gander
{
    public class SourceFile
    {
        Byte[] srcbuf;
        uint srclen;
        uint srcpos;

        public SourceFile(String fname)
        {

            srcbuf = File.ReadAllBytes(fname);
            srclen = (uint)srcbuf.Length;
            srcpos = 0;
        }

        public uint getPos()
        {
            return srcpos;
        }

        public byte[] getRange(uint ofs, uint len)
        {
            byte[] result = new byte[len];
            Array.Copy(srcbuf, ofs, result, 0, len);
            return result;
        }

        public uint getOne()
        {
            byte a = srcbuf[srcpos++];
            uint result = (uint)(a);
            return result;
        }

        public uint getTwo()
        {
            byte b = srcbuf[srcpos++];
            byte a = srcbuf[srcpos++];
            uint result = (uint)(a * 256 + b);
            return result;
        }

        public uint getFour()
        {
            byte d = srcbuf[srcpos++];
            byte c = srcbuf[srcpos++];
            byte b = srcbuf[srcpos++];
            byte a = srcbuf[srcpos++];
            uint result = (uint)(a * 256 + b);
            result = (result * 256 + c);
            result = (result * 256 + d);
            return result;
        }

        public ulong getEight()
        {
            ulong a = getFour();
            ulong b = getFour();
            b *= 65536;
            b *= 65536;
            ulong result = a + b;
            return result;
        }

        //fixed len string
        public String getAsciiString(int width)
        {
            String result = "";
            for (int i = 0; i < width; i++)
            {
                byte a = srcbuf[srcpos++];
                if ((a >= 0x20) && (a <= 0x7E))
                {
                    result += (char)a;
                }
            }
            return result;
        }

        public String getAsciiZString()
        {
            String result = "";
            byte a = srcbuf[srcpos++];
            while (a != 0x00)
            {
                result += (char)a;
                a = srcbuf[srcpos++];
            }
            return result;
        }

        public void skip(uint delta)
        {
            srcpos += delta;
        }

        public void backup(uint delta)
        {
            srcpos -= delta;
        }

        public void seek(uint pos)
        {
            srcpos = pos;
        }
    }
}

//Console.WriteLine("there's no sun in the shadow of the wizard");
