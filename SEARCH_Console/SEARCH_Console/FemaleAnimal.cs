namespace SEARCH_Console
{
   public class Female : Animal
   {
		#region Public Members (2) 

		#region Constructors (1) 

      public Female():base()
      {
         base.sex = "Female";
      }

		#endregion Constructors 
		#region Methods (1) 

      public override void dump()
      {
         fw.writeLine("");
         fw.writeLine("my sex is female");
         base.dump();

      }

		#endregion Methods 

		#endregion Public Members 
   }
   
}
