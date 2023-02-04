// Implemention order: 5 - follows Orientation.
// This is the root of the GeoPose inheritance hierarchy.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Positions;
using FrameTransforms;
using Orientations;
using Extras;

namespace GeoPose
{
    /// <summary>
    /// The abstract root of the GeoPose Basic and Advanced classes.
    /// </summary>
    public abstract class GeoPose
    {
        // Optional and non-standard but conforming added property:
        //   an identifier unique within an application.
        public PoseID? poseID { get; set; } = null;

        // Optional and non-standard but conforming added property:
        //  a PoseID type identifier of another GeoPose in the direction of the root of a pose tree.
        public PoseID? parentPoseID { get; set; } = null;

        // Optional and non-standard but conforming added property:
        //   a poseID identifier in the direction of the root of a pose tree.
        public UnixTime? validTime { get; set; } = null;
        public abstract FrameTransform FrameTransform { get; set; }
        public abstract Orientation Orientation { get; set; }
        public abstract string ToJSON(string indent);
    }
}
