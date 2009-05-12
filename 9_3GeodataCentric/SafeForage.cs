namespace PAZ_Dispersal
{
   public class SafeForageModifier : Modifier
   {
      
      public SafeForageModifier()
      {
         base.Name = "SafeForageModifier";
         base.CaptureFood = 1.0;
         base.PredationRisk = 1;
         base.MoveSpeed = 1.0;
         base.MoveTurtosity = 1.0;
         base.EnergyUsed = 1.0;
         base.PerceptonModifier = 1.0;
      }
     
   }
}

