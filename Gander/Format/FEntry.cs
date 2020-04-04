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
        public virtual void format(SourceFile src, List<string> lines)
        {

        }
    }

    public class IntField : FEntry
    {
        public String name;
        public int width;
        public long val;

        public IntField(String _name, int _width)
        {
            name = _name;
            width = _width;
        }

        public static IntField loadEntry(string fparams)
        {
            int pos = fparams.IndexOf(':');
            String name = fparams.Substring(0, pos).Trim();
            //pos = fparams.IndexOf(':', pos + 1);
            int width = Int32.Parse(fparams.Substring(pos + 1).Trim());
            IntField f = new IntField(name, width);
            return f;
        }

        public override void format(SourceFile src, List<string> lines)
        {
            string s = src.getPos().ToString("X6");
            val = 0;
            for (int i = 0; i < width; i++)
            {
                uint v = src.getOne();
                s = s + ':' + v.ToString("X2");
                val = (v << (i * 8)) + val;
            }
            s = s + "\t\t//" + name;
            lines.Add(s);
        }
    }
}
