using System;
// Implemention order: 1 - start here.
// These classes are non-structural elements - these are part of optional elements that are allowed but not standardized.
namespace Extras
{
    public class PoseID
    {
        internal PoseID()
        {

        }
        public PoseID(string id)
        {
            this.id = id;
        }
        public string id { get; set; } = string.Empty;
    }
    public class UnixTime
    {
        internal UnixTime()
        {

        }
        // Constructor from long integer count of UNIX Time seconds x 1000
        public UnixTime(long longTime)
        {
            timeValue = longTime.ToString();
        }
        public string timeValue { get; set; } = string.Empty;
    }

}
