using ESRI.ArcGIS.Geodatabase;
namespace SEARCH
{
   public abstract class IHomeRangeFinder
   {
		#region Operations (2) 

     // bool FindHomeRange(Animal inAnimal,AnimalMap inMap);
      public abstract bool setHomeRangeCenter(Animal inAnimal, IFeatureClass inAnmialMemoryMap);

      public abstract bool setHomeRangeCenter(Animal inA, string inFileName);

		#endregion Operations 
   }
}