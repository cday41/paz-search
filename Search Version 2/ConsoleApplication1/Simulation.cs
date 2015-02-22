using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ModularSearch
{
    [DataContract]
    public class Simulation
    {
        #region privateFields
        private DateTime _StartDate;
        private DateTime _EndSeasonDate;
        private int _NumYears;
        private int _TimeBetweenSteps;
        private int _StartTime;
        #endregion

        #region Properties
        [DataMember(Name = "StartDate")]
        public DateTime StartDate
        {
            get { return _StartDate; }
            set { _StartDate = value; }
        }

        [DataMember(Name = "EndSeasonDate")]
        public DateTime EndSeasonDate
        {
            get { return _EndSeasonDate; }
            set { _EndSeasonDate = value; }
        }

        [DataMember(Name = "NumYears")]
        public int NumYears
        {
            get { return _NumYears; }
            set { _NumYears = value; }
        }

        [DataMember(Name = "TimeBetweenSteps")]
        public int TimeBetweenSteps
        {
            get { return _TimeBetweenSteps; }
            set { _TimeBetweenSteps = value; }
        }

        [DataMember(Name = "StartTime")]
        public int StartTime
        {
            get { return _StartTime; }
            set { _StartTime = value; }
        }
        #endregion
    }
}
