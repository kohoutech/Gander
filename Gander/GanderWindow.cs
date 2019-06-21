/* ----------------------------------------------------------------------------
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Gander
{
    public partial class GanderWindow : Form
    {
        Gander gander;
        String currentPath;

        public GanderWindow()
        {
            gander = new Gander(this);

            InitializeComponent();
        }

//- data operations -----------------------------------------------------------

        public void openFile(String filename)
        {
            ganderStatusLabel.Text = "Loading...";
            gander.openSourceFile(filename);

            Text = "Gander [" + filename + "]";
            ganderStatusLabel.Text = "";
        }


        private void showOpenFileDialog()
        {
            String filename = "";
            //if (currentPath != null)
            //{
            //    ganderOpenFileDialog.InitialDirectory = currentPath;
            //}
            //else
            //{
            //    ganderOpenFileDialog.InitialDirectory = Application.StartupPath;                
            //}
            //ganderOpenFileDialog.FileName = "";
            //ganderOpenFileDialog.DefaultExt = "*.exe";
            //ganderOpenFileDialog.Filter = "Executable files|*.exe|DLL files|*.dll|All files|*.*";
            //fluoroOpenFileDialog.ShowDialog();
            //filename = fluoroOpenFileDialog.FileName;
            filename = "test.dll";
            if (filename.Length != 0)
            {
                //currentPath = Path.GetDirectoryName(filename);
                //openFile(filename);
            }
        }


//- file menu -----------------------------------------------------------------

        private void openFileMenuItem_Click(object sender, EventArgs e)
        {
            showOpenFileDialog();
        }

        private void saveFileMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exitFileMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

//- edit menu -----------------------------------------------------------------

        private void copyEditMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void selectAllEditMenuItem_Click(object sender, EventArgs e)
        {

        }

//- format menu -----------------------------------------------------------------

        private void setFormatMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void manageFormatMenuItem_Click(object sender, EventArgs e)
        {

        }

//- help menu -----------------------------------------------------------------

        private void aboutHelpMenuItem_Click(object sender, EventArgs e)
        {
            String msg = "Gander\nversion 1.0.0\n" + "\xA9 Origami Software 1998-2018\n" + "http://origami.kohoutech.com";
            MessageBox.Show(msg, "About");
        }
    }
}

//Console.WriteLine("there's no sun in the shadow of the wizard");
