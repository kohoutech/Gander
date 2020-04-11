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

        public FEntry(Format _format)
        {
            format = _format;
        }

        public virtual void formatEntry(SourceFile src, List<string> lines)
        {
        }
    }

    //-----------------------------------------------------
    
    public class FStruct : FEntry
    {
        public String name;
        public SymbolTable fields;
        
        public FStruct(Format _format, String _name)
            : base(_format)
        {
            name = _name;
            fields = new SymbolTable(format.symTable);
        }
    }

    //-----------------------------------------------------

    public class FField : FEntry
    {
        public FStruct structure;
        public uint value;

        public FField(FStruct _struct) : base(_struct.format)
        {
            structure = _struct;
        }

        public uint getValue()
        {
            return value;
        }
    }

    //-----------------------------------------------------

    public class IntField : FField
    {
        public String name;
        public int width;

        public IntField(FStruct _struct, String _name, int _width)
            : base(_struct)
        {
            name = _name;
            width = _width;
        }

        public static IntField loadEntry(FStruct _struct, string name, string fparams)
        {            
            int width = Int32.Parse(fparams.Trim());
            IntField f = new IntField(_struct, name, width);
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

    public class FixedBuffer : FField
    {
        public string name;
        public int width;

        public FixedBuffer(FStruct _struct, string _name, int _width)
            : base(_struct)
        {
            name = _name;
            width = _width;
        }

        public static FixedBuffer loadEntry(FStruct _struct, string name, string fparams)
        {
            int width = Int32.Parse(fparams.Trim());
            FixedBuffer f = new FixedBuffer(_struct, name, width);
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

    public class VariableBuffer : FField
    {
        public string name;
        public int width;
        public FField delim;

        public VariableBuffer(FStruct _struct, string _name, FField _delim)
            : base(_struct)
        {
            name = _name;
            delim = _delim;
        }

        public static VariableBuffer loadEntry(FStruct _struct, string name, string fparams)
        {
            FField delim = (FField)_struct.fields.getEntry(fparams.Trim());
            VariableBuffer f = new VariableBuffer(_struct, name, delim);
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

    public class FixedString : FField
    {
        String name;
        String str;
        int width;

        public FixedString(FStruct _struct, String _name, int _width)
            : base(_struct)
        {
            name = _name;
            width = _width;
        }

        public static FixedString loadEntry(FStruct _struct, string name, string fparams)
        {
            int width = Int32.Parse(fparams.Trim());
            FixedString f = new FixedString(_struct, name, width);
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
