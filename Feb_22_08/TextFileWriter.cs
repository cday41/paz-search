using System.IO;
namespace PAZ_Dispersal
{
   public class TextFileWriter
   {
      private string mOutPath;
      private System.IO.StreamWriter sw;
      public TextFileWriter(string path, string fileName)
      {
         mOutPath = path;
         if (mOutPath != null)
         {
            if (! System.IO.Directory.Exists(mOutPath))
            {
               System.IO.Directory.CreateDirectory(mOutPath);
            }
            
            sw = new StreamWriter(mOutPath + "\\" + fileName + ".txt",true);
            sw.WriteLine("Year,Day,Time,George #, X, Y, Asleep,Behavior Mode,Energy Level,Risk,ProbFoodCap,MVL,MSL");
         }
      }
      public void addLine(string inValue)
      {
         if(sw != null)
         {
            sw.WriteLine(inValue);
            sw.Flush();
         }
      }

      public void close()
      {
         sw.Close();
      }

      
      public string OutPath
		{
			get { return mOutPath; }
			set { mOutPath = value; }
		}

   }
}
