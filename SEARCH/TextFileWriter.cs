using System.IO;
using System.Collections.Specialized;
using log4net;
namespace SEARCH
{
   public class TextFileWriter
   {
		#region Fields (4) 

       private ILog eLog = LogManager.GetLogger("Error");
      private string mOutPath;
      private StringCollection sc;
      private System.IO.StreamWriter sw;

		#endregion Fields 

		#region Constructors (1) 


      public TextFileWriter(string path, string fileName)
      {
         bool exists = false;
       //  sc = new StringCollection();
         if (path != null)
         {
             if (!System.IO.Directory.Exists(path))
             {
                 System.IO.Directory.CreateDirectory(path);
             }
             if(System.IO.File.Exists(path + "\\" + fileName + ".csv"))
             {
                 exists = true;
               
             }
             sw = new StreamWriter(path + "\\" + fileName + ".csv", true);
             this.OutPath = path + "\\" + fileName + ".csv";
             sw.AutoFlush = true;
             if (!exists)
             {
                 sw.WriteLine("Year, Day, Time, Animal #, X, Y, Asleep, Behavior Mode, Energy Level, Risk, ProbFoodCap, MVL, MSL, PercptionDist, Percent Step");
             }
         }
      }

		#endregion Constructors 

		#region Properties (1) 

      public string OutPath
      {
         get { return mOutPath; }
         set { mOutPath = value; }
      }

		#endregion Properties 

		#region Methods (3) 

		// Public Methods (3) 

      public void addLine(string inValue)
      {
         try
         {
            this.sw.WriteLine(inValue);
            
         }
         catch (System.Exception ex)
         {

            eLog.Debug(ex);
         }
      }

      public void close()
      {
         sw.Close();
      }

      public string RelocateFile()
      {
         string fileName = Path.GetFileName(this.mOutPath);
         string path = Path.GetDirectoryName(this.mOutPath);
         string newPath = Path.Combine(path, "Resident");
         if (!Directory.Exists(newPath)) Directory.CreateDirectory(newPath);
         sw.Close();
         string newFileName = Path.Combine(newPath, fileName);
         if(File.Exists(newFileName)) File.Delete(newFileName);
         File.Copy(this.mOutPath, newFileName);
         sw = new StreamWriter(newFileName, true);
         File.Delete(mOutPath);
         this.OutPath = newFileName;
         sw.AutoFlush = true;
       
         return newFileName;
      }

      public void WriteOutTimeStep()
      {
         foreach (string s in this.sc)
         {
            this.sw.WriteLine(s);
         }
         this.sc.Clear();
      }

		#endregion Methods 
   }
}
