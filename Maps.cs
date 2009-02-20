/******************************************************************************
 * CHANGE LOG
 * DATE:          Sunday, September 09, 2007 12:06:34 PM
 * Author:        Bob Cummings
 * Description:   Worked on switching out maps.  Found issue with hourly maps
 *                switching on time. But next day would not reset. Added logic 
 *                move start time ahead one day, if there is not another trigger
 *                with same start date.
 * 
 *                That created issue with swapping out social maps. Not sure if 
 *                corrected or not.
 * 
 *                Modified read and write xml nodes for changes.  Still an issue
 *                with start dates for some reason.
 ******************************************************************************
 * CHANGE LOG
 * DATE:          Monday, September 24, 2007 7:26:35 AM
 * Author:        Bob Cummings
 * Description:   Modified the condition in public void changeMap(DateTime now)
 *                from checking if the trigger start time was less than the 
 *                current time to less than or equal to.
 * ***************************************************************************
* CHANGE LOG
* DATE:          Thursday, September 27, 2007 10:20:20 AM
* Author:        Bob Cummings
* Description:   Added setting the orignial start time in 
*                public void loadXMLTriggers(XPathNodeIterator inIterator)
*                This should fix the problem with the XML file writing out the 
*                wrong start time.   
* ***************************************************************************
*  Description: Also changed  public void writeXMLTriggers(ref XmlTextWriter xw)
*                 to write out the OriginalStartDate.Date and .Hour
*******************************************************************************
*DATE:            Saturday, October 13, 2007 1:07:31 PM
*Author:          Bob Cummings
*Description:     Added if statement in changeMap(DateTime now, AnimalManager am)
*                 We were having an issue on yearly swaps of release maps, double
*                 creation of new disperser maps.
******************************************************************************/
              


using System;
using System.Text;
using System.Collections;
using System.Xml;
using System.Xml.XPath;
using DesignByContract;
namespace PAZ_Dispersal
{
   public class Maps
   {


      #region private members
      private bool changeMaps;
      private MapSwapTrigger[] mMyTriggers;
      private int previousIndex; //used when switching out maps to see if the new one is the 
      //the same as the last one.  If so we do not need to reload.
      private FileWriter.FileWriter fw;
      private String mMapType;
      private string mMyPath;
      
      #endregion

      #region publicMethods

      public Maps()
      {
         buildLogger();
         //set to negative one, because the first time through the current index in map change will be zero or positive.
         //so we will always want to load a map then.
         this.previousIndex = -1;
      }
      public Maps(String type) : this()
      {
         mMapType = type;
      }

      
      public void changeMap(DateTime now)
      {
         try
         {
            changeMaps = false;
            int currIndex = this.mMyTriggers.Length - 1;
            fw.writeLine("");
            fw.writeLine("inside Maps changeMap for map type " + mMapType + " at simulation datetime " + now.ToShortDateString() + " " + now.ToShortTimeString());
            this.dumpTriggersHere();
            if (this.mMyTriggers.Length > 1)
            {
               //start at the latest in the time frame and work backwards to find the current time in the simulation
               while (this.mMyTriggers[currIndex].StartDate > now && currIndex > 0)
               {
                  fw.writeLine("current index is " + currIndex.ToString());
                  fw.writeLine(this.mMyTriggers[currIndex].ToString());
                  currIndex--;
               }
            }
            fw.writeLine("outside of loop");
            fw.writeLine("Check to see if this index is the same as the last one.");
            fw.writeLine("current index is " + currIndex.ToString());
            fw.writeLine("Previous index was " + this.previousIndex.ToString());
            //Monday, September 24, 2007 Author: Bob Cummings 
            //changed condition from less than to less than or equal.
            if (this.mMyTriggers[currIndex].StartDate <= now && currIndex != this.previousIndex)
            {
               this.previousIndex = currIndex;
               fw.writeLine(this.mMyTriggers[currIndex].ToString());
               fw.writeLine("new start date year, day, and hour is less than now, so loading " + this.mMyTriggers[currIndex].Path + this.mMyTriggers[currIndex].Filename);
               fw.writeLine("which has the starting time of " + this.mMyTriggers[currIndex].StartDate.ToShortDateString() + " " + this.mMyTriggers[currIndex].StartDate.ToShortTimeString());
                MapManager.GetUniqueInstance().loadOneMap(mMapType, this.mMyTriggers[currIndex].Filename, this.mMyTriggers[currIndex].Path);
               fw.writeLine("Now using new map");
               changeMaps = true;
               setNewStartDate();
            }
            fw.writeLine("");

         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         
         
      }
   
      public void changeMap(DateTime now, AnimalManager am)
      {
         try
         {
            fw.writeLine("inside changeMap(DateTime now, AnimalManager am) calling changeMap(now)");
            string previousMapName = MapManager.GetUniqueInstance().SocialMap.fullFileName;
            this.changeMap(now);
            if (this.mMapType == "Social" && changeMaps == true)
            {

               am.adjustNewSocialMap(MapManager.GetUniqueInstance().SocialMap);
               
            }
            else if (this.mMapType == "Dispersal" && changeMaps == true)
            {
                fw.writeLine("MapType is Dispersal and changeMaps == true so need to make newly released animals");
               InitialAnimalAttributes[] inIAA;
               MapManager.GetUniqueInstance().GetInitialAnimalAttributes(out inIAA);
              
               am.addNewDispersers(inIAA, now);
               fw.writeLine("Back from the animal manager making new animals");
                fw.writeLine("trigger type is " + this.mMyTriggers[0].MyTriggerType.ToString());
               //Bob Cummings Saturday, October 13, 2007
               //added check to see if this was a yearly swap, if so then the simulation
               //manager will handle making the new maps just like breeding
               if (! (this.mMyTriggers[0].MyTriggerType == MapSwapTrigger.mTriggerType.YEARLY))
               {  
                 fw.writeLine("so ask the map manager to add to the list");
                  MapManager.GetUniqueInstance().makeNewDisperserAnimalMaps(am);
               }
            }
         }
         catch (System.Exception ex)
         {
            FileWriter.FileWriter.WriteErrorFile(ex);
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif

         }
      }

      public void setUpNewYearDispersalMap(DateTime now, AnimalManager am)
      {
         //Because this is happening at the yearly time change the simulation
         //manager will handle making the new animal maps.
         try
         {
            fw.writeLine("inside setUpNewYearDispersalMap(DateTime now, AnimalManager am) calling changeMap(now)");
            this.changeMap(now);
            if(changeMaps == true)
            {
               InitialAnimalAttributes[] inIAA;
               MapManager.GetUniqueInstance().GetInitialAnimalAttributes(out inIAA);
               am.addNewDispersers(inIAA, now);
            }
         }
         catch (System.Exception ex)
         {
            FileWriter.FileWriter.WriteErrorFile(ex);
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif

         }
      }
      public string dumpTriggers()
      {
         StringBuilder sb = new StringBuilder();
         foreach (MapSwapTrigger m in mMyTriggers)
            if (m != null)     
               sb.Append(m.ToString() + System.Environment.NewLine);

         return sb.ToString();
      }
      public void dumpTriggersHere()
      {
         foreach (MapSwapTrigger m in mMyTriggers)
            if (m != null)   
               fw.writeLine(m.ToString());
      }
      public void addYearToMaps()
      {
         if(this.mMyTriggers[0].MyTriggerType == MapSwapTrigger.mTriggerType.HOURLY)
         {
            int index = 0;
            foreach(MapSwapTrigger mst in this.mMyTriggers)
            {
               //reset the start date back to the orginal for this year/season
               mst.StartDate = mst.OriginalStartDate;
               this.setNewDailyStart(index++);
               mst.OriginalStartDate = mst.StartDate;
            }
         }

      }
      public void loadXMLTriggers(XPathNodeIterator inIterator)
      {
         try
         {
            fw.writeLine("inside loadXMLTriggers for " + this.mMapType + " Maps");
            mMyTriggers = new MapSwapTrigger[inIterator.Count];
            int index = -1;
            while (inIterator.MoveNext())
            {
               index++;
               MapSwapTrigger mst = new MapSwapTrigger();
               XPathNodeIterator tempIT = inIterator.Current.Select("StartDate");
               tempIT.MoveNext();
               mst.StartDate = System.Convert.ToDateTime(tempIT.Current.Value);
               tempIT = inIterator.Current.Select("StartTime");
               tempIT.MoveNext();
               mst.StartDate = mst.StartDate.AddHours(System.Convert.ToInt32(tempIT.Current.Value));
               //Author: Bob Cummings  added setting the orginal start date.  
               //will also modify writting out the XML file to using original start date.
               mst.OriginalStartDate = mst.StartDate;
               tempIT = inIterator.Current.Select("Path");
               tempIT.MoveNext();
               mst.Path = tempIT.Current.Value;
               tempIT = inIterator.Current.Select("FileName");
               tempIT.MoveNext();
               mst.Filename = tempIT.Current.Value;
               tempIT = inIterator.Current.Select("TriggerType");
               tempIT.MoveNext();
               switch (tempIT.Current.Value)
               {
                  case "HOURLY":
                     mst.MyTriggerType = MapSwapTrigger.mTriggerType.HOURLY;
                     break;
                  case "DAILY":
                     mst.MyTriggerType = MapSwapTrigger.mTriggerType.DAILY;
                     break;
                  case "YEARLY":
                     mst.MyTriggerType = MapSwapTrigger.mTriggerType.YEARLY;
                     break;
                  case "STATIC":
                     mst.MyTriggerType = MapSwapTrigger.mTriggerType.STATIC;
                     break;
               }

               
               mMyTriggers[index] = mst;
            }
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
      }

      public void writeXMLTriggers(ref XmlTextWriter xw)
      {
         
         foreach (MapSwapTrigger mst in this.mMyTriggers)
         {
            xw.WriteStartElement("MapSwitchTriggers");
            xw.WriteAttributeString("name",this.mMapType);
            //Author: Bob Cummings
            //Since we had already loaded the maps, the start date was advanced by
            //whatever time amount.  This was causing the xml file to incorrectly
            //report on the start time.  The maps were acutally starting on time, but 
            //file was not being written correctly.
            xw.WriteElementString("StartDate",mst.OriginalStartDate.Date.ToShortDateString());
            xw.WriteElementString("StartTime",mst.OriginalStartDate.Hour.ToString());
            xw.WriteElementString("Path",mst.Path);
            xw.WriteElementString("FileName",mst.Filename);
            xw.WriteElementString("TriggerType",mst.MyTriggerType.ToString());
            xw.WriteEndElement();
         }
         
         xw.Flush();
      }

      #endregion

      #region privateMethods
   
      private void buildLogger()
      {
         string s;
         System.IO.StreamReader sr;
         bool foundPath = false;
         string path = System.Windows.Forms.Application.StartupPath;
         if (System.IO.File.Exists(path + "\\logFile.dat"))      
         {
            sr = new System.IO.StreamReader(path + "\\logFile.dat");
            while (sr.Peek() > -1)
            {
               s = sr.ReadLine();
               if (s.IndexOf("MapsLogPath") == 0)
               {
                  fw = FileWriter.FileWriter.getMapsLogger(s.Substring(s.IndexOf(" ")));
                  foundPath = true;
                  break;
               }
            }
            sr.Close();
         }
         if (!foundPath)
         {
            fw = new FileWriter.EmptyFileWriter();
         }


      }
      private void setNewStartDate()
      {
         
         try
         {
            fw.writeLine("inside new start date trigger type is " + this.mMyTriggers[this.previousIndex].MyTriggerType.ToString());
            switch (this.mMyTriggers[this.previousIndex].MyTriggerType)
            {
               case(MapSwapTrigger.mTriggerType.HOURLY) : 
                  setNewHourlyStart();
                  break;
               case(MapSwapTrigger.mTriggerType.DAILY) : 
                  this.setNewDailyStart(this.previousIndex);
                  break;
               case(MapSwapTrigger.mTriggerType.YEARLY) : 
                  break;
               case(MapSwapTrigger.mTriggerType.STATIC) : 
                  //do nothing but have to include to use the default without error
                  break;
               default : 
                  throw new Exception("Unexpected Trigger Type");
            }

         }
         catch (System.Exception ex)
         {
            FileWriter.FileWriter.WriteErrorFile(ex);
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
         }
      }
      /********************************************************************************
       *  Function name   : setNewHourlyStart
       *  Description     : We need to advance the start time on the hourly maps for the 
       *                    next day.  But we can not advance past the next set of hourly 
       *                    maps.  So we loop through all the maps and check to see if 
       *                    advancing the day by one will match any of the others in the
       *                    list.  If there is a match then forget it.  It is time for the 
       *                    new maps to start.  Other wise move ahead one day.
       *  Return type     : void 
       * *******************************************************************************/
      
      private void setNewHourlyStart()
      {
         try
         {
            fw.writeLine("inside setNewHourlyStart");
            Check.Require(this.MyTriggers[this.previousIndex].MyTriggerType == MapSwapTrigger.mTriggerType.HOURLY,"Wrong type of trigger type in setNewHourlyStart expected HOURLY got " + this.MyTriggers[this.previousIndex].MyTriggerType.ToString());
            bool foundMatch = false;
            DateTime tempDay = this.MyTriggers[this.previousIndex].StartDate.AddDays(1);
            fw.writeLine("old start date was " + this.MyTriggers[this.previousIndex].StartDate.ToShortDateString());
            for (int i = this.previousIndex; i < this.mMyTriggers.Length - 1; i++)
            {
               fw.writeLine("inside loop checking against " + this.mMyTriggers[i].StartDate.ToShortDateString());
               if (this.mMyTriggers[i].StartDate.DayOfYear == tempDay.DayOfYear)
               {
                  fw.writeLine("found match");
                  foundMatch = true;
                  break;
               }
            }
            if (!foundMatch)
               this.MyTriggers[this.previousIndex].StartDate = tempDay;

            fw.writeLine("leaving setNewHourlyStart maptrigger start date is now " +  this.MyTriggers[this.previousIndex].StartDate.ToShortDateString());
         }
         catch (System.Exception ex)
         {
            FileWriter.FileWriter.WriteErrorFile(ex);
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
         }
      }
      /********************************************************************************
       *  Function name   : setNewDailyStart
       *  Description     : After a map is switched out, it needs to have it's start 
       *                    adjusted.  This will create a temp start equal to the current
       *                    map's start day plus one year.  If there are no other maps in 
       *                    this collection with a start date in the same year it will
       *                    advance the start date of the current map by one year.  If 
       *                    there are other maps with a start date for the next year 
       *                    nothing is done so the next map will load at the appropiate
       *                    time.
       *                    
       *  Return type     : void 
       * *******************************************************************************/
      
      private void setNewDailyStart(int index)
      {
         
         try
         {
            fw.writeLine("inside setNewDailyStart");
            bool foundMatch = false;
            DateTime tempDay = this.MyTriggers[index].StartDate.AddYears(1);
            fw.writeLine("old start date was " + this.MyTriggers[index].StartDate.ToShortDateString() + " " + this.mMyTriggers[index].StartDate.ToShortTimeString());
            for (int i = this.previousIndex; i < this.mMyTriggers.Length - 1; i++)
            {
               fw.writeLine("inside loop checking " + tempDay.ToShortDateString() + " against " + this.mMyTriggers[i].StartDate.ToShortDateString());
               if (this.mMyTriggers[i].StartDate.Year == tempDay.Year)
               {
                  fw.writeLine("found match");
                  this.mMyTriggers[index].StartDate = tempDay.AddYears(1000);
                  //TODO REMOVE MAP TRIGGER
                  foundMatch = true;
                  break;
               }
            }
            if (!foundMatch)
               this.MyTriggers[index].StartDate = tempDay;

            fw.writeLine("leaving setNewDailyStart maptrigger start date is now " +  this.MyTriggers[index].StartDate.ToShortDateString() + " " + this.mMyTriggers[index].StartDate.ToShortTimeString());
         }
            

         
         catch(System.Exception ex)
         {
            FileWriter.FileWriter.WriteErrorFile(ex);
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
         }
      }
      #endregion

      #region getters & setters
      public MapSwapTrigger[] MyTriggers
      {
         get { return mMyTriggers; }
         set { mMyTriggers = value; }
      }
      public string MyPath
      {
         get { return mMyPath; }
         set { mMyPath = value; }
      }

		

      #endregion
      
   }
}
