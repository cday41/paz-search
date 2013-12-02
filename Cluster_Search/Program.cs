using System;
using System.Collections.Generic;
//using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.ADF.COMSupport;

using SEARCH;

namespace DataCentric
{
    static class Program
    {
      #region Methods (5) 

      #region Private Methods (5) 

       private static void CheckLicense ()
        {
           if (ESRI.ArcGIS.RuntimeManager.Bind (ESRI.ArcGIS.ProductCode.EngineOrDesktop))
           {
              ESRI.ArcGIS.RuntimeManager.BindLicense (ESRI.ArcGIS.ProductCode.EngineOrDesktop);
           }
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

        private static BackUpParams DoBackUpLoad (string[] args, string mapOutputDirectory, string textOutputDir)
        {

           BackUpParams backupParms = new BackUpParams ();
           // Parse optional command line arguments if they exist
           try
           {
              if (args.Length > 3)
              {
                 for (int i = 3; i < args.Length; i++)
                 {
                    //if checkpointing
                    if (args[i].ToLower() == "-c")
                    {
                       backupParms.backupSave = true;
                       backupParms.backupSaveInterval = int.Parse (args[i + 1]);
                       backupParms.backupSaveUnit = 'z';
                       switch (args[i + 2].ToLower())
                       {
                          case "i":
                             backupParms.backupSaveUnit = 'i';
                             break;
                          case "m":
                             backupParms.backupSaveUnit = 'm';
                             break;
                          case "h":
                             backupParms.backupSaveUnit = 'h';
                             break;
                          case "d":
                             backupParms.backupSaveUnit = 'd';
                             break;
                       }

                       backupParms.backupSaveName = args[i + 3];
                       if (backupParms.backupSaveUnit == 'z') throw new Exception ("backup save unit must be i, m, h, or d");
                       if (backupParms.backupSaveInterval < 1) throw new Exception ("backup save interval must be greater than 0");
                    }
                    //if loading
                    if (args[i].ToLower() == "-l")
                    {
                       backupParms.backupLoad = true;
                       backupParms.backupdir = args[i + 1];
                    }
                 }
              }
           }
           catch (Exception e)
           {
              Console.Error.WriteLine (e.Message);
              Console.Error.WriteLine ("Incorrect backup configuration");
              usage ();
              Environment.Exit (-1);
           }

           //When reloading, delete the directory if it exists, and remake it.
           if (System.IO.Directory.Exists (mapOutputDirectory) && System.IO.Directory.Exists (textOutputDir) && System.IO.Directory.Exists (backupParms.backupdir))
           {
              if (backupParms.backupLoad)
              {
                 if (mapOutputDirectory == textOutputDir)
                 {
                    Directory.GetFiles(mapOutputDirectory, "*", SearchOption.AllDirectories);
                    System.IO.Directory.Delete (mapOutputDirectory, true);
                    dirCopy (backupParms.backupdir, mapOutputDirectory);
                 }
                 else
                 {
                    System.IO.Directory.Delete (mapOutputDirectory, true);
                    System.IO.Directory.Delete (textOutputDir, true);
                    dirCopy (backupParms.backupdir, mapOutputDirectory);
                    System.IO.Directory.CreateDirectory (textOutputDir);
                 }
              }
              else
              {
                 Console.WriteLine ("Warning: Directory already exist");
                 Environment.Exit (0);
              }
           }
           else if (System.IO.Directory.Exists (backupParms.backupdir))
           {
              if (backupParms.backupLoad)
              {
                 if (mapOutputDirectory == textOutputDir)
                 {
                    dirCopy (backupParms.backupdir, mapOutputDirectory);
                 }
                 else
                 {
                    dirCopy (backupParms.backupdir, mapOutputDirectory);
                    System.IO.Directory.CreateDirectory (textOutputDir);
                 }
              }
           }
           return backupParms;
        }

        private static void LoadArgs (string[] args, out string xmlInputFile, out string mapOutputDirectory, out string textOutputDir, out string xmlBackup)
        {
           //Checks if the command line arugments are less than the minimum required amount
           if (args.Length < 3)
           {
              usage ();
              Console.WriteLine ("ERROR: Program was not provided enough arguments.");
              Environment.Exit (-1);
           }



           xmlInputFile = args[0];
           mapOutputDirectory = args[1];
           textOutputDir = args[2];
           xmlBackup = null;

           //Check if creating a backup xml file
           if (args.Length > 3)
           {

              string temp = args[3].ToLower ();
              temp = temp.Trim ();
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
        }

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

      #endregion Private Methods 

      #endregion Methods 

      #region Main()
       /// <summary>
       /// The main entry point for the application.
       /// </summary>
       // [STAThread] //Runs faster with the tag, but seems to have some issue with exiting
       static void Main (string[] args)
       {
          string xmlInputFile;
          string mapOutputDirectory;
          string textOutputDir;
          string xmlBackup;
#if DEBUG
          Console.WriteLine ("calling check license");
#endif

          CheckLicense ();
          frmInput f = new frmInput ();

#if DEBUG
          Console.WriteLine ("calling LoadArgs");
#endif
          LoadArgs (args, out xmlInputFile, out mapOutputDirectory, out textOutputDir, out xmlBackup);
#if DEBUG
          Console.WriteLine ("Calling DoBackUpLoad");
#endif       
          BackUpParams backupParms = DoBackUpLoad (args, mapOutputDirectory, textOutputDir);
#if DEBUG
          Console.WriteLine ("Calling DoRun");
#endif          
          f.doRun (xmlInputFile, mapOutputDirectory, textOutputDir, xmlBackup, backupParms);

          //ESRI.ArcGIS.ADF.COMSupport.AOUninitialize.Shutdown();
          System.Environment.Exit (0);

          //thisProc.Kill();         
          //Console.WriteLine("End of program");

       }
       #endregion
    }
}