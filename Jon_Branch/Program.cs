using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using ESRI.ArcGIS.esriSystem;

using SEARCH;

namespace DataCentric
{
    static class Program
    {
		#region Non-Public Members (1) 

		#region Methods (1) 

        static void usage()
        {
            Console.Error.WriteLine("program xmlInputFile mapOutput textOuput xmlBackup [optional]");
            Console.Error.WriteLine("Options:");
            Console.Error.WriteLine("-c interval unit BackupDirectory");
            Console.Error.WriteLine("   Create backups");
            Console.Error.WriteLine("   interval: how long between backups (number)");
            Console.Error.WriteLine("   unit: how interval is measured: i iterations, m minutes, h hours, d days");
            Console.Error.WriteLine("-l BackupDirectory");
            Console.Error.WriteLine("   Loads backup from directory");
            Console.Error.WriteLine("   BackupDirectory: The directory storing the backup information");            
        }

        static void dirCopy(string sourceDir, string destiDir)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDir);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDir);
            }
            if (!Directory.Exists(destiDir))
            {
                Directory.CreateDirectory(destiDir);
            }

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destiDir, file.Name);
                if (file.Extension != ".lock")
                {
                    file.CopyTo(temppath, true);
                }
            }
            foreach (DirectoryInfo subdir in dirs)
            {
                if (string.Compare(subdir.FullName, destiDir) != 0)
                {
                    string temppath = Path.Combine(destiDir, subdir.Name);
                    dirCopy(subdir.FullName, temppath);
                }
            }
        }


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        //[STAThread] - uncommment this if you want this to run from form, not command line
        static void Main(string[] args)
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run( new frm Input());
            //Console.WriteLine("I'm in is licensed");
            
            if (ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.EngineOrDesktop))
            {
                ESRI.ArcGIS.RuntimeManager.BindLicense(ESRI.ArcGIS.ProductCode.EngineOrDesktop);
            }
            
            //Checks if the command line arugments are less than the minimum required amount
            if (args.Length < 3 )
            {
                usage();
                Console.WriteLine("ERROR: Program was not provided enough arguments.");
                Environment.Exit(-1);
            }
            
            frmInput f = new frmInput();

            string xmlInputFile = args[0];
            string mapOutputDirectory = args[1];
            string textOutputDir = args[2];
            string xmlBackup = null;

            //Check if creating a backup xml file
            if (args.Length > 3)
            {

                string temp = args[3].ToLower();
                temp = temp.Trim();
                if ((temp == "-c") || (temp == "-l"))
                {
                    xmlBackup = null;
                }
                else
                {
                    xmlBackup = args[3];
                }
            }

            string mapDir = mapOutputDirectory;
            string textDir = textOutputDir;

            #region backup configuration
            // These variables describe if and how to save or load backups
            // Maybe there should be a struct or class for this data to make doRun call less bulky?
            // Otherwise, make this a property of frmInput or SimulationManager and use setters to
            // configure.

            string backupSaveName = ""; // base name of backup files
            string backupdir = ""; //name of backup to load
            Boolean backupSave = false; // save backups?
            Boolean backupLoad = false; // load backup?
            int backupSaveInterval = 0; // backup how often? (a number)
            int backupSaveCount = 0; //Doesn't matter
            char backupSaveUnit = '0'; // backup how often? (minutes, hours, days, iterations)


            // Parse optional command line arguments if they exist
            try
            {
                if (args.Length > 3)
                {
                    for (int i = 3; i < args.Length; i++)
                    {
                        //if checkpointing
                        if (args[i] == "-c")
                        {
                            backupSave = true;
                            backupSaveInterval = int.Parse(args[i + 1]);
                            if (args[i + 2] == "i")
                            {
                                backupSaveUnit = 'i';
                            }
                            if (args[i + 2] == "m")
                            {
                                backupSaveUnit = 'm';
                            }
                            if (args[i + 2] == "h")
                            {
                                backupSaveUnit = 'h';
                            }
                            if (args[i + 2] == "d")
                            {
                                backupSaveUnit = 'd';
                            }
                            backupSaveName = args[i + 3];
                            if (backupSaveUnit == 0) throw new Exception("backup save unit must be i, m, h, or d");
                            if (backupSaveInterval < 1) throw new Exception("backup save interval must be greater than 0");
                        }
                        //if loading
                        if (args[i] == "-l")
                        {
                            backupLoad = true;
                            backupdir = args[i + 1];
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                Console.Error.WriteLine("Incorrect backup configuration");
                usage();
                Environment.Exit(-1);
            }

            //When reloading, delete the directory if it exists, and remake it.
            if (System.IO.Directory.Exists(mapOutputDirectory) && System.IO.Directory.Exists(textOutputDir) && System.IO.Directory.Exists(backupdir))
            {
                if (backupLoad)
                {
                    if (mapOutputDirectory == textOutputDir)
                    {
                        System.IO.Directory.Delete(mapOutputDirectory, true);
                        dirCopy(backupdir, mapOutputDirectory);
                    }
                    else
                    {
                        System.IO.Directory.Delete(mapOutputDirectory, true);
                        System.IO.Directory.Delete(textOutputDir, true);
                        dirCopy(backupdir, mapOutputDirectory);
                        System.IO.Directory.CreateDirectory(textOutputDir);
                    }
                }
                else
                {
                    Console.WriteLine("Warning: Directory already exist");
                    Environment.Exit(0);
                }
            }
            else if(System.IO.Directory.Exists(backupdir))
            {
                if (backupLoad)
                {
                    if (mapOutputDirectory == textOutputDir)
                    {
                        dirCopy(backupdir, mapOutputDirectory);
                    }
                    else
                    {
                        dirCopy(backupdir, mapOutputDirectory);
                        System.IO.Directory.CreateDirectory(textOutputDir);
                    }
                }
            }
            #endregion

            f.doRun(xmlInputFile, mapOutputDirectory, textOutputDir, xmlBackup, backupSave, backupSaveName, backupSaveInterval, backupSaveUnit, backupSaveCount, backupLoad, backupdir);

            //Console.WriteLine("Finished doRun");
            
            //Tried disposing form
            //f.Dispose();
            //if (f.IsDisposed)
            //{
            //    Console.WriteLine("Disposed form!");
            //}

            
            //Tried multiple types of exits
            //Do we need to force an exit here?
            //Environment.FailFast(null);
            //Console.WriteLine("Trying return");
            //return;
            //Console.WriteLine("Trying Application.Exit()");
            //Application.Exit();
            //Console.WriteLine("Trying Environment.Exit(0)");
            //Environment.Exit(0);


            //Tried garbage collection
            //GC.Collect();
            //Console.WriteLine("Waiting for finalizers");
            //GC.WaitForPendingFinalizers();

            //Tried sleeping
            //System.Threading.Thread.Sleep(5000);

            //Process thisProc = Process.GetCurrentProcess();
            //ProcessThreadCollection myThreads = thisProc.Threads;
            //foreach (ProcessThread pt in myThreads)
            //{
            //    DateTime startTime = pt.StartTime;
            //    TimeSpan cpuTime = pt.TotalProcessorTime;
                //int priority = pt.BasePriority;
            //    ThreadState ts = pt.ThreadState;

            //    Console.WriteLine("thread:  {0}", pt.Id);
            //    Console.WriteLine("    started: {0}", startTime.ToString());
            //    Console.WriteLine("    CPU time: {0}", cpuTime);
            //    Console.WriteLine("    priority: {0}", priority);
            //    Console.WriteLine("    thread state: {0}", ts.ToString());

            //    if (priority == 10)
            //    {
            //        Console.WriteLine("Disposing Thread!!!!");
            //        pt.Dispose();
            //    }
            //}
            Application.Exit();
            Environment.Exit(0);

            //thisProc.Kill();         
            //Console.WriteLine("End of program");

        }

		#endregion Methods 

		#endregion Non-Public Members 
    }
}