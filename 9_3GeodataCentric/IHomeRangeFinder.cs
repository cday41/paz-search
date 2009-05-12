using ESRI.ArcGIS.Geodatabase;
namespace PAZ_Dispersal
{
   public interface IHomeRangeFinder
   {
     // bool FindHomeRange(Animal inAnimal,AnimalMap inMap);
      bool setHomeRangeCenter(Animal inAnimal, IFeatureClass inAnmialMemoryMap);
      bool setHomeRangeCenter(Animal inA, string inFileName);

   }
}