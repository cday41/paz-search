namespace PAZ_Dispersal
{
   public class Male : Animal
   {
      
      
      public Male():base()
      {
         base.sex = "Male";
      }
      public override void dump()
      {
         fw.writeLine("");
         fw.writeLine("my sex is male");
         
         base.dump();

      }
   }
}
