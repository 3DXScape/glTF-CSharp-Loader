using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace glTFInterface
{
     public class Skin
    {
        // Type: integer
        // The index of the accessor containing the floating-point 4x4 inverse-bind matrices.
        // Required: No
        public int inverseBindMatrices { get; set; } = -1;

        // Type: integer
        // The index of the node used as a skeleton root.
        // Required: No
        public int skeleton { get; set; } = -1;

        // Type: integer[1 - *]
        // Indices of skeleton nodes, used as joints in this skin.
        // Required: Yes
        public List<int>? joints { get; set; } = null;

        // Type: string
        // The user-defined name of this object.
        // Required: No
        public string name { get; set; } = "not set";

        // Type: extension
        // JSON object with extension-specific objects.
        // Required: No
        public System.Collections.Generic.Dictionary<string, object>? extensions { get; set; } = null;

        // Type: extras
        // Application-specific data.
        // Required: No
        public Extra? extras { get; set; } = null;
        /*
         * **********************************************************
         */
        private bool isLocked = false;
        public void Lock()
        {
            isLocked = true;
        }
        public void Unlock()
        {
            isLocked = false;
        }
    }
}
