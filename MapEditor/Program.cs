﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapEditor
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.Run(new Form1());
        }
    }
}
