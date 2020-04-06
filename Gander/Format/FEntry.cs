/* ----------------------------------------------------------------------------
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

namespace Gander
{
    //base class
    public class FEntry
    {
        public Format format;
        public uint value;

        public FEntry(Format _format)
        {
            format = _format;
        }

        public virtual void formatEntry(SourceFile src, List<string> lines)
        {
        }

        public uint getValue()
        {
            return value;
        }
    }

    //-----------------------------------------------------
    
    public class IntField : FEntry
    {
        public String name;
        public int width;

        public IntField(Format _format, String _name, int _width)
            : base(_format)
        {
            name = _name;
            width = _width;
        }

        public static IntField loadEntry(Format _format, string fparams)
        {            
            int pos = fparams.IndexOf(':');
            String name = fparams.Substring(0, pos).Trim();
            int width = Int32.Parse(fparams.Substring(pos + 1).Trim());
            IntField f = new IntField(_format, name, width);
            _format.storeEntry(name, f);
            return f;
        }

        public override void formatEntry(SourceFile src, List<string> lines)
        {
            string s = src.getPos().ToString("X6");
            value = 0;
            for (int i = 0; i < width; i++)
            {
                uint v = src.getOne();
                s = s + ':' + v.ToString("X2");
                value = (v << (i * 8)) + value;
            }
            s = s + "\t\t//" + name;
            lines.Add(s);
        }
    }

    //-----------------------------------------------------

    public class FixedBuffer : FEntry
    {
        public int width;

        public FixedBuffer(Format _format, int _width)
            : base(_format)
        {
            width = _width;
        }

        public static FixedBuffer loadEntry(Format _format, string fparams)
        {
            int width = Int32.Parse(fparams.Trim());
            FixedBuffer f = new FixedBuffer(_format, width);
            return f;
        }

        public override void formatEntry(SourceFile src, List<string> lines)
        {
            string s = src.getPos().ToString("X6");
            int count = 0;
            for (int i = 0; i < width; i++)
            {
                uint v = src.getOne();
                s = s + ':' + v.ToString("X2");
                count++;
                if (count == 16)
                {
                    lines.Add(s);
                    s = src.getPos().ToString("X6");
                    count = 0;
                }
            }
            lines.Add(s);
        }
    }

    //-----------------------------------------------------

    public class VariableBuffer : FEntry
    {
        public int width;
        public FEntry delim;

        public VariableBuffer(Format _format, FEntry _delim)
            : base(_format)
        {
            delim = _delim;
        }

        public static VariableBuffer loadEntry(Format _format, string fparams)
        {
            FEntry _delim = _format.getEntry(fparams.Trim());
            VariableBuffer f = new VariableBuffer(_format, _delim);
            return f;
        }

        public override void formatEntry(SourceFile src, List<string> lines)
        {
            uint pos = src.getPos();
            uint width = delim.getValue() - pos;
            string s = pos.ToString("X6");
            int count = 0;
            for (int i = 1; i <= width; i++)
            {
                uint v = src.getOne();
                s = s + ':' + v.ToString("X2");
                count++;
                if ((count == 16) && (i != width))
                {
                    lines.Add(s);
                    s = src.getPos().ToString("X6");
                    count = 0;
                }
            }
            lines.Add(s);
        }
    }

        //-----------------------------------------------------

    public class FixedString : FEntry
    {
        String name;
        String str;
        int width;

        public FixedString(Format _format, String _name, int _width)
            : base(_format)
        {
            name = _name;
            width = _width;
        }

        public static FixedString loadEntry(Format _format, string fparams)
        {
            int pos = fparams.IndexOf(':');
            String name = fparams.Substring(0, pos).Trim();
            int width = Int32.Parse(fparams.Substring(pos + 1).Trim());
            FixedString f = new FixedString(_format, name, width);
            _format.storeEntry(name, f);
            return f;
        }

        public override void formatEntry(SourceFile src, List<string> lines)
        {
            string s = src.getPos().ToString("X6");
            str = "";
            for (int i = 0; i < width; i++)
            {
                uint v = src.getOne();
                if (0x020 <= v && v <= 0x7f)
                {
                    str = str + (char)v;
                }
                else
                {
                    str = str + '\\' + v.ToString("X2");
                }
            }
            s = s + ":\"" + str + "\"\t\t//" + name;
            lines.Add(s);
        }
    }

}
