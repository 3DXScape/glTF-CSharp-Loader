import * as Extras from './Extras';
import * as FrameTransform from './FrameTransform';
import * as Orientation from './Orientation';
import * as GeoPose from './GeoPose';

// WARNING: Serialization of this form of GeoPose produces JSON data in a form not part of the OGC GeoPose 1.0 standard.
// Implemention order: 8 -a useful GeoPose for working within a local Cartesian (i.e. engineering) frame.
// Local can be expressed as an Advanced form, but the Advanced form is more complex and this implementation is a shortcut.

/// <summary>
/// Local GeoPose is a derived pose within an engineering CRS with a Cartesian coordinate system.
/// This form is the closest to the classical computer graphics pose concept.
/// <remark>
/// WARNING: Local is not (yet) part of the OGC GeoPose standard and not backwards-compatible.
/// Useful when operating within a local Cartesian frame defined by a Basic (or other) GeoPose.
/// It is possible to define Local via the Advanced GeoPose with
///   "authority": "steve@opensiteplan.org-experimental", "id": "translation", "parameters": {<dx>, <dy>, <dz> }
/// </remark>
/// </summary>
export class Local extends GeoPose.GeoPose {
    public constructor(id: string, frameTransform: FrameTransform.Translation, orientation: Orientation.YPRAngles) {
        super();
        this.poseID = new Extras.PoseID(id);
        this.FrameTransform = frameTransform;
        this.Orientation = orientation;
    }
    /// <summary>
    /// The xOffset, yOffset, zOffset from the origin of the rotated inner frame of a "parent" GeoPose.
    /// </summary>
    public override FrameTransform: FrameTransform.Translation;

    /// <summary>
    /// An Orientation specified as three rotations.
    /// </summary>
    public override Orientation: Orientation.YPRAngles;


    /// <summary>
    /// This function returns a Json encoding of an Advanced GeoPose
    /// </summary>
    public toJSON(): string {
        let indent: string = "";
        let sb: string[] = [''];
        {
            sb.push("{\r\n  ");
            if (this.validTime != null) {
                sb.push("\"validTime\": " + this.validTime.toString() + ",\r\n" + indent + "  ");
            }
            if (this.poseID != null && this.poseID.id != "") {
                sb.push("\"poseID\": \"" + this.poseID.id + "\",\r\n" + indent + "  ");
            }
            if (this.parentPoseID != null && this.parentPoseID.id != "") {
                sb.push("\"parentPoseID\": \"" + this.parentPoseID.id + "\",\r\n" + indent + "  ");
            }
            sb.push("\"position\": \r\n  {\r\n    " + "\"x\": " + (this.FrameTransform as FrameTransform.Translation).xOffset + ",\r\n    " +
                "\"y\": " + (this.FrameTransform as FrameTransform.Translation).yOffset + ",\r\n    " +
                "\"z\":   " + (this.FrameTransform as FrameTransform.Translation).zOffset);
            sb.push("\r\n  " + "},");
            sb.push("\r\n  ");
            sb.push("\"angles\": \r\n  {\r\n    " + "\"yaw\":   " + (this.Orientation as Orientation.YPRAngles).yaw + ",\r\n    " +
                "\"pitch\": " + (this.Orientation as Orientation.YPRAngles).pitch + ",\r\n    " +
                "\"roll\":  " + (this.Orientation as Orientation.YPRAngles).roll);
            sb.push("\r\n  " + "}");
            sb.push("\r\n" + "}\r\n");

            return sb.join('');
        }
    }
}
