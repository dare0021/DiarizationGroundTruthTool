using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiarizationGroundTruthTool
{
    /// <summary>
    /// Container class for dialog data
    /// </summary>
    class DialogEntry
    {
        public int id { get; }
        public TimeSpan startingTime { get; set; }
        public TimeSpan endTime { get; set; }

        public DialogEntry(int speakerID, TimeSpan startingTime)
        {
            id = speakerID;
            this.startingTime = startingTime;
        }

        override
        public String ToString()
        {
            return "Speaker " + id + ": (" + startingTime.ToString(@"hh\:mm\:ss") + " ~ " + endTime.ToString(@"hh\:mm\:ss") + ")\n";
        }
    }
}
