﻿/* ----------------------------------------------------------------------------
Gander : a file format viewer
Copyright (C) 1998-2019  George E Greaney

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
    public class FileFormatter
    {
        public static Dictionary<String, FileFormatter> formatterList;

        public static FileFormatter getFormatter(String extention)
        {
            return formatterList[extention];
        }

        public static void registerFormatter(String extension, FileFormatter formatter)
        {
            formatterList.Add(extension, formatter);
        }
    }
}
