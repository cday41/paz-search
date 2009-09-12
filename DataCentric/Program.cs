using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SEARCH;

namespace DataCentric
{
    static class Program
    {
		#region Non-Public Members (1) 

		#region Methods (1) 

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmInput());
        }

		#endregion Methods 

		#endregion Non-Public Members 
    }
}