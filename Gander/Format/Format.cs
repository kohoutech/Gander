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
        public Dictionary<String, FEntry> entryTable;

        //use hard coded fields for now
        public static Format loadFormatFile(string filepath)
        {
            Format format = new Format();

            //String[] lines = File.ReadAllLines(filepath);
            EnamlData gosling = EnamlData.loadFromFile(filepath);

            string gosVersion = gosling.getStringValue("Gosling.version", "");
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
                            f = IntField.loadEntry(format, fieldname, fparams);
                            break;

                        case "fixedstr":
                            f = FixedString.loadEntry(format, fieldname, fparams);
                            break;

                        case "fixedbuf":
                            f = FixedBuffer.loadEntry(format, fieldname, fparams);
                            break;

                        case "varbuf":
                            f = VariableBuffer.loadEntry(format, fieldname, fparams);
                            break;

                        default:
                            break;
                    }
                    fs.fields.Add(fieldname, f);
                }
                format.entryTable.Add(structname, fs);
            }

            return format;
        }

        public Format()
        {
            fields = new List<FEntry>();
            entryTable = new Dictionary<string, FEntry>();
        }

        public FEntry getEntry(String name)
        {
            FEntry entry = null;
            if (entryTable.ContainsKey(name))
            {
                entry = entryTable[name];
            }
            return entry;
        }
    }
}
