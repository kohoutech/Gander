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
        public List<FField> fields;
        public SymbolTable symTable;

        public FStruct(Format _format, String _name)
            : base(_format)
        {
            name = _name;
            fields = new List<FField>();
            symTable = new SymbolTable(format.structs);
        }

        public override void formatEntry(SourceFile src, List<string> lines)
        {
            foreach (FField field in fields)
            {
                field.formatEntry(src, lines);
            }
        }
    }

    //-----------------------------------------------------

    public class FField : FEntry
    {
        public FStruct structure;
        public uint value;

        public FField(FStruct _struct)
            : base(_struct.format)
        {
            structure = _struct;
        }

        public uint getValue()
        {
            return value;
        }
    }

    //-----------------------------------------------------

    public class StructField : FField
    {
        public String name;
        public FStruct ftype;

        public StructField(FStruct _struct, String _name, FStruct _ftype)
            : base(_struct)
        {
            name = _name;
            ftype = _ftype;
        }

        public override void formatEntry(SourceFile src, List<string> lines)
        {
            lines.Add("");
            lines.Add("//" + name);
            ftype.formatEntry(src, lines);
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

        public static IntField loadEntry(FStruct _struct, string name, List<string> fparams)
        {
            int width = Int32.Parse(fparams[1].Trim());
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

        public static FixedString loadEntry(FStruct _struct, string name, List<string> fparams)
        {
            int width = Int32.Parse(fparams[1].Trim());
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

        public static FixedBuffer loadEntry(FStruct _struct, string name, List<string> fparams)
        {
            int width = Int32.Parse(fparams[1].Trim());
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
        public String delim;

        public VariableBuffer(FStruct _struct, string _name, String _delim)
            : base(_struct)
        {
            name = _name;
            delim = _delim;
        }

        public static VariableBuffer loadEntry(FStruct _struct, string name, List<string> fparams)
        {
            String _delim = fparams[1].Trim();
            VariableBuffer f = new VariableBuffer(_struct, name, _delim);
            return f;
        }

        public override void formatEntry(SourceFile src, List<string> lines)
        {
            uint pos = src.getPos();
            FField delimf = (FField)format.vars.getEntry(delim);
            uint width = delimf.getValue() - pos;
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

    //-------------------------------------------------------------------------

    public class VariableTable : FField
    {
        public string name;
        public String delim;
        public FStruct fs;

        public VariableTable(FStruct _struc, String _name, String _delim, FStruct _fs)
            : base(_struc)
        {
            name = _name;
            delim = _delim;
            fs = _fs;
        }

        public static VariableTable loadEntry(FStruct _struc, String name, List<string> parms)
        {
            String delim = parms[1].Trim();
            FStruct fs = (FStruct)_struc.format.structs.getEntry(parms[2].Trim());
            VariableTable f = new VariableTable(_struc, name, delim, fs);
            return f;
        }

        public override void formatEntry(SourceFile src, List<string> lines)
        {
            lines.Add("");
            lines.Add("//" + name);
            FField delimf = (FField)structure.format.vars.getEntry(delim);
            uint count = delimf.value;
            for (int i = 0; i < count; i++)
            {
                if (i > 0) { lines.Add(""); }
                lines.Add("[" + i + "]");
                fs.formatEntry(src, lines);
            }
        }
    }

    //-------------------------------------------------------------------------

    public class SizeBufferTable : FField
    {
        public string name;
        public String delim;

        public SizeBufferTable(FStruct _struc, String _name, String _delim)
            : base(_struc)
        {
            name = _name;
            delim = _delim;
        }

        public static SizeBufferTable loadEntry(FStruct _struc, String name, List<string> parms)
        {
            String delim = parms[1].Trim();
            SizeBufferTable f = new SizeBufferTable(_struc, name, delim);
            return f;
        }

        public override void formatEntry(SourceFile src, List<string> lines)
        {
        }
    }

}
