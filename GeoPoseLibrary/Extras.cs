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

}
