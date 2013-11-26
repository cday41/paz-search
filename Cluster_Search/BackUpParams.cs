using System;

namespace DataCentric
{
   public struct BackUpParams
   {
     
      public string backupSaveName; // base name of backup files
      public string backupdir; //name of backup to load
      public Boolean backupSave; // save backups?
      public Boolean backupLoad; // load backup?
      public int backupSaveInterval; // backup how often? (a number)
      public int backupSaveCount; //Doesn't matter
      public char backupSaveUnit; // backup how often? (minutes, hours, days, iterations)
   }
}
