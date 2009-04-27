namespace PAZ_Dispersal
{
   public class Female : Animal
   {
        
      
      public Female():base()
      {
         base.sex = "Female";
      }
      public override void dump()
      {
         fw.writeLine("");
         fw.writeLine("my sex is female");
         base.dump();

      }
   }
   
}
