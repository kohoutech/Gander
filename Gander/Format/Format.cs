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
using System.IO;
using Origami.ENAML;

namespace Gander
{
    public class Format
    {
        public List<FEntry> entries;
        public SymbolTable structs;
        public SymbolTable vars;
        public FStruct fyle;

        //use hard coded fields for now
        public static Format loadFormatFile(string filepath)
        {
            Format format = new Format();

            EnamlData gosling = EnamlData.loadFromFile(filepath);

            string gosVersion = gosling.getStringValue("Gosling.version", "");

            //read in structure data
            List<String> structs = gosling.getPathKeys("structs");
            foreach (String structname in structs)
            {
                FStruct fs = new FStruct(format, structname);
                List<String> fields = gosling.getPathKeys("structs." + structname);
                String fieldpath = "structs." + structname + ".";
                foreach (String fieldname in fields)
                {
                    String fline = gosling.getStringValue(fieldpath + fieldname, "");
                    List<String> parms = getParams(fline);
                    FField f = loadField(fs, fieldname, parms);
                    fs.fields.Add(f);
                    fs.symTable.addEntry(fieldname, f);
                }
                format.structs.addEntry(structname, fs);
            }

            //read in file structure data
            format.fyle = new FStruct(format, "file");
            List<String> ffields = gosling.getPathKeys("file");
            foreach (String fieldname in ffields)
            {
                String fline = gosling.getStringValue("file." + fieldname, "").Trim();
                List<String> parms = getParams(fline);
                FField f = loadField(format.fyle, fieldname, parms);
                format.fyle.fields.Add(f);
                format.fyle.symTable.addEntry(fieldname, f);
            }

            return format;
        }

        public static List<String> getParams(string line)
        {
            List<String> parms = new List<String>();
            while (line.Length != 0)
            {
                int pos = line.IndexOf(':');
                String parm = (pos != -1) ? line.Substring(0, pos).Trim() : line;
                parms.Add(parm);
                line = (pos != -1) ? line.Substring(pos + 1).Trim() : "";
            }
            return parms;
        }

        public static FField loadField(FStruct fs, String fname, List<String> parms)
        {
            FField f = null;

            //check for loaded struct
            FEntry ftype = fs.format.structs.getEntry(parms[0].ToLower());
            if (ftype != null)
            {
                f = new StructField(fs, fname, (FStruct)ftype);
            }

            //else is a predefined struct
            else switch (parms[0])
                {
                    case "int":
                        f = IntField.loadEntry(fs, fname, parms);
                        break;

                    case "fixedstr":
                        f = FixedString.loadEntry(fs, fname, parms);
                        break;

                    case "fixedbuf":
                        f = FixedBuffer.loadEntry(fs, fname, parms);
                        break;

                    case "varbuf":
                        f = VariableBuffer.loadEntry(fs, fname, parms);
                        break;

                    case "vartable":
                        f = VariableTable.loadEntry(fs, fname, parms);
                        break;

                    case "sizebuftable":
                        f = SizeBufferTable.loadEntry(fs, fname, parms);
                        break;

                    default:
                        break;
                }

            return f;
        }

        //---------------------------------------------------------------------

        public Format()
        {
            entries = new List<FEntry>();
            structs = new SymbolTable(null);
            vars = new SymbolTable(null);
        }
    }

    //-------------------------------------------------------------------------

    public class SymbolTable
    {
        Dictionary<String, FEntry> entries;
        SymbolTable parent;

        public SymbolTable(SymbolTable _parent)
        {
            parent = _parent;
            entries = new Dictionary<string, FEntry>();
        }

        public void addEntry(String name, FEntry entry)
        {
            entries[name] = entry;
        }

        public FEntry getEntry(String name)
        {
            FEntry entry = null;
            int pos = name.IndexOf('.');
            String start = (pos != -1) ? name.Substring(0, pos).Trim() : name;
            int index = -1;
            if (start.EndsWith("]"))
            {
                int indexpos = start.IndexOf('[');
                String indexstr = start.Substring(indexpos + 1, start.Length - indexpos - 2);
                start = start.Substring(0, indexpos);
            }
            if (entries.ContainsKey(start))
            {
                entry = entries[start];
            }
            if (entry == null && parent != null)
            {
                entry = parent.getEntry(start);          //follow sym tbl chain up to root
            }
            while (pos != -1)
            {
                name = name.Substring(pos + 1);
                entry = ((StructField)entry).ftype.symTable.getEntry(name);
                pos = name.IndexOf('.');
            }
            return entry;
        }
    }
}
