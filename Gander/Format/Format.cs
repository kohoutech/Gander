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
        public List<FEntry> fields;
        public SymbolTable symTable;

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
                    FEntry f = null;
                    String fline = gosling.getStringValue(fieldpath + fieldname, "");
                    int pos = fline.IndexOf(':');
                    String ftype = (pos != -1) ? fline.Substring(0, pos).Trim() : fline;
                    String fparams = (pos != -1) ? fline.Substring(pos + 1).Trim() : "";
                    ftype = ftype.ToLower();
                    switch (ftype)
                    {
                        case "int":
                            f = IntField.loadEntry(fs, fieldname, fparams);
                            break;

                        case "fixedstr":
                            f = FixedString.loadEntry(fs, fieldname, fparams);
                            break;

                        case "fixedbuf":
                            f = FixedBuffer.loadEntry(fs, fieldname, fparams);
                            break;

                        case "varbuf":
                            f = VariableBuffer.loadEntry(fs, fieldname, fparams);
                            break;

                        default:
                            break;
                    }
                    fs.fields.addEntry(fieldname, f);
                }
                format.symTable.addEntry(structname, fs);
            }

            return format;
        }

        public Format()
        {
            fields = new List<FEntry>();
            symTable = new SymbolTable(null);
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
            if (entries.ContainsKey(name))
            {
                entry = entries[name];
            }
            if (entry == null && parent != null)
            {
                entry = parent.getEntry(name);          //follow sym tbl chain up to root
            }
            return entry;
        }
    }
}
