namespace SEARCH
{
   public class Male : Animal
   {
		#region Public Members (2) 

		#region Constructors (1) 

      public Male():base()
      {
         base.sex = "Male";
      }

		#endregion Constructors 
		#region Methods (1) 

      public override void dump()
      {
         fw.writeLine("");
         fw.writeLine("my sex is male");
         
         base.dump();

      }

		#endregion Methods 

		#endregion Public Members 
   }
}
