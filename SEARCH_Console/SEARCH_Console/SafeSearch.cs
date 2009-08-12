namespace SEARCH_Console
{
   public class SafeSearchModifier : Modifier
   {
		#region Public Members (1) 

		#region Constructors (1) 

      public SafeSearchModifier()
      {
         base.Name = "SafeSearchModifier";
         base.CaptureFood = .45;
         base.PredationRisk = .96;
         base.MoveSpeed = 1;
         base.MoveTurtosity = 1.1;
         base.EnergyUsed = 1.02;
         base.PerceptonModifier = 1.01;
      }

		#endregion Constructors 

		#endregion Public Members 
   }  
}

