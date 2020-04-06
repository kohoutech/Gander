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

            String[] lines = File.ReadAllLines(filepath);
            for (int i = 0; i < lines.Length; i++)
            {
                String line = lines[i];
                int pos = line.IndexOf(':');
                String ftype = line.Substring(0, pos).Trim();
                String fparams = line.Substring(pos+1).Trim();
                FEntry f = null;
                switch (ftype)
                {
                    case "INT":
                        f = IntField.loadEntry(format, fparams);
                        break;

                    case "FIXEDSTR":
                        f = FixedString.loadEntry(format, fparams);
                        break;

                    case "FIXEDBUF":
                        f = FixedBuffer.loadEntry(format, fparams);
                        break;

                    case "VARBUF":
                        f = VariableBuffer.loadEntry(format, fparams);
                        break;


                    default:
                        break;
                }
                format.fields.Add(f);

            }

            return format;
        }

        public Format()
        {
            fields = new List<FEntry>();
            entryTable = new Dictionary<string, FEntry>();
        }

        public void storeEntry(string name, FEntry f)
        {
            entryTable[name] = f;
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
