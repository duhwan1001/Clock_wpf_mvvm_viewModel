using System;

namespace VewModelSample.UtilClass
{
    public class TCP_Properties
    {
        [Serializable]
        public class ChangeTimeValues : Packet
        {
            public string Packet_ChangeHour { get; set; }
            public string Packet_ChangeMin { get; set; }
            public string Packet_ChangeSec { get; set; }
        }

        [Serializable]
        public class ChangeTimeFormatValues : Packet
        {
            public int Packet_TimeFormat;
        }

        [Serializable]
        public class ChangeStandardValues : Packet
        {
            public int Packet_Standard = 0;
        }

        [Serializable]
        public class AddAlarmValues : Packet
        {
            public string Packet_AlarmHour;
            public string Packet_AlarmMin;
            public string Packet_AlarmSec;
        }

        [Serializable]
        public class StopWatchValues : Packet
        {
            public int Packet_StopWatchFlag;
        }
        
        [Serializable]
        public class Result : Packet
        {
            public bool result { get; set; }
            public string reason { get; set; }
        }
    }

}

