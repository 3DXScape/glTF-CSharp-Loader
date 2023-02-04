// Implemention order: 8 -a useful GeoPose for working within a local Cartesian (i.e. engineering) frame.
// Local can be expressed as an Advanced form, but the Advanced form is more complex and this implementation is a shortcut.

using System;
using System.Text;

using GeoPose;
using Positions;
using FrameTransforms;
using Orientations;
using Extras;

/// <summary>
/// A derived pose within an engineering CRS with a Cartesian coordinate system.
/// This form is the closest to the classical computer graphics pose concept.
/// <remark>
/// Not (yet) part of the OGC GeoPose standard and not backwards-compatible.
/// Useful when operating within a local Cartesian frame defined by a Basic (or other) GeoPose.
/// </remark>
/// </summary>
public class Local : GeoPose.GeoPose
{
    public Local(string id, Translation frameTransform, Orientation quaternion)
    {
        this.poseID = new PoseID(id);
        this.FrameTransform = frameTransform;
        this.Orientation = quaternion;
    }
    /// <summary>
    /// The xOffset, yOffset, zOffset from the origin of the rotated inner frame of a "parent" GeoPose.
    /// </summary>
    private Translation _frameTransform = new Translation();
    public override FrameTransform FrameTransform
    {
        get
        {
            return _frameTransform;
        }
        set
        {
            if (value.GetType() == typeof(Translation))
            {
                _frameTransform = (Translation)value;
            }
            // else throw expected translation exception
        }
    }
    /// <summary>
    /// Local uses the yaw, pitch, roll orientation.
    /// </summary>
    private YPRAngles _orientation = new YPRAngles();
    /// <summary>
    /// An Orientation specified as three rotations.
    /// </summary>
    public override Orientation Orientation
    {
        get
        {
            return _orientation;
        }
        set
        {
            if (value.GetType() == typeof(YPRAngles))
            {
                _orientation = (YPRAngles)value;
            }
            // else throw expected YPRAngles exception
        }
    }
    /// <summary>
    /// This function returns a Json encoding of a Basic-YPR GeoPose
    /// </summary>
    public override string ToJSON(string indent = "")
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("{\r\n  ");
        if (validTime != null && validTime.timeValue != String.Empty)
        {
            sb.Append("\"validTime\": " + validTime.timeValue + ",\r\n" + indent + "  ");
        }
        if (poseID != null && poseID.id != String.Empty)
        {
            sb.Append("\"poseID\": \"" + poseID.id + "\",\r\n" + indent + "  ");
        }
        if (parentPoseID != null && parentPoseID.id != String.Empty)
        {
            sb.Append("\"parentPoseID\": \"" + parentPoseID.id + "\",\r\n" + indent + "  ");
        }
        sb.Append("\"position\": \r\n  {\r\n    " + "\"x\": " + ((Translation)FrameTransform).xOffset + ",\r\n    " +
            "\"y\": " + ((Translation)FrameTransform).yOffset + ",\r\n    " +
            "\"z\":   " + ((Translation)FrameTransform).zOffset);
        sb.Append("\r\n  " + "},");
        sb.Append("\r\n  ");
        sb.Append("\"angles\": \r\n  {\r\n    " + "\"yaw\":   " + ((YPRAngles)Orientation).yaw + ",\r\n    " +
            "\"pitch\": " + ((YPRAngles)Orientation).pitch + ",\r\n    " +
            "\"roll\":  " + ((YPRAngles)Orientation).roll);
        sb.Append("\r\n  " + "}");
        sb.Append("\r\n" + "}\r\n");
        return sb.ToString();
    }
}

