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
    public class FileFormatter
    {
        public static Dictionary<String, Format> formatList;
        public static Dictionary<String, String> formatFileList;

        public FileFormatter()
        {
            formatList = new Dictionary<string, Format>();
            formatFileList = new Dictionary<string, string>();
        }

        public Format getFormat(String extention)
        {
            Format format = null;
            if (formatList.ContainsKey(extention))
            {
                format = formatList[extention];
            }
            else
            {
                String filepath = formatFileList[extention];
                format = Format.loadFormatFile(filepath);
                formatList[extention] = format;
            }
            return format;
        }

        public void registerFormat(String extension, String filepath)
        {
            formatFileList.Add(extension, filepath);
        }

        //- format file contents ----------------------------------------------

        public void formatFile(string filename, Format format)
        {            
            SourceFile src = new SourceFile(filename);

            List<String> lines = new List<string>();
            foreach (FEntry entry in format.entries)
            {
                entry.formatEntry(src, lines);
            }

            File.WriteAllLines("outfile.txt", lines);            
        }
    }
}
