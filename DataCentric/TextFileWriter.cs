using System.IO;
using System.Collections.Specialized;
namespace PAZ_Dispersal
{
   public class TextFileWriter
   {

      #region Fields (2)


      private string mOutPath;
      private StringCollection sc;
      private System.IO.StreamWriter sw;

      #endregion Fields

      #region Constructors (1)

      public TextFileWriter(string path, string fileName)
      {
         mOutPath = path;
         sc = new StringCollection();
         if (mOutPath != null)
         {
            if (!System.IO.Directory.Exists(mOutPath))
            {
               System.IO.Directory.CreateDirectory(mOutPath);
            }

            sw = new StreamWriter(mOutPath + "\\" + fileName + ".txt", true);
            sw.AutoFlush = true;
            sw.WriteLine("Year,Day,Time,George #, X, Y, Asleep,Behavior Mode,Energy Level,Risk,ProbFoodCap,MVL,MSL,PercptionDist,Percent Step");
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

      #region Public Methods (3)

      public void WriteOutTimeStep()
      {
         foreach (string s in this.sc)
         {
            this.sw.WriteLine(s);
         }
         this.sc.Clear();
      }

      public void addLine(string inValue)
      {
         try
         {
            this.sc.Add(inValue);
            
         }
         catch (System.Exception ex)
         {

            FileWriter.FileWriter.WriteErrorFile(ex);
         }
      }

      public void writeLine(string inValue)
      {
         this.sw.WriteLine(inValue);
      }

      public void close()
      {
         sw.Close();
      }

      #endregion Public Methods

   }
}
