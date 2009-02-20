namespace FileWriter
{
   /// <summary>
   /// The empty class in case no logging needs to be done.
   /// </summary>
   public class EmptyFileWriter : FileWriter 
   {
      public EmptyFileWriter(){}
       public override  void writeLine(string data)
      {
      }
        public override  void close()
      {
      }
   }
   
}
