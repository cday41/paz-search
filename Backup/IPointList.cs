using System;
using System.Collections;
using ESRI.ArcGIS.Geometry;

namespace PAZ_Dispersal
{
	/// <summary>
	/// Summary description for IPointList.
	/// </summary>
	public class IPointList
	{
		private ArrayList mPoints;

		public IPointList()
		{
			mPoints = new ArrayList();
		}

		public IPoint this[int i]
		{
			get {return (IPoint)mPoints[i];}
			set {mPoints.Add(value);}
		}
		public void add(IPoint inPoint)
		{
			mPoints.Add(inPoint); 
		}
		public IPoint getPointByIndex(int i)
		{
			return (IPoint)mPoints[i];
		}
		public int getLastIndex()
		{
			return this.mPoints.Count - 1;
		}

      public int Count()
      {
         int i = mPoints.Count;
         i--;
         return i;
      }


	}
}
