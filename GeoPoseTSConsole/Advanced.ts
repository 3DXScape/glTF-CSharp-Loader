import * as Extras from './Extras';
import * as FrameTransform from './FrameTransform';
import * as Orientation from './Orientation';
import * as GeoPose from './GeoPose';

// Implemention order: 7 - follows Basic GeoPose.
// This is the most general GeoPose - the largest part of the 20% part of a 80/20 solution.
// The difficult implementation is creating the interface layer between the
// Extrinsic specification and external authorities and data sources.

/// <summary>
/// Advanced GeoPose.
/// </summary>
export class Advanced extends GeoPose.GeoPose {
    public constructor(id: string, frameTransform: FrameTransform.Extrinsic, orientation: Orientation.Quaternion) {
        super();
        this.poseID = new Extras.PoseID(id);
        this.FrameTransform = frameTransform;
        this.Orientation = orientation;
    }

    /// <summary>
    /// A Frame Specification defining a frame with associated coordinate system whose Position is the origin.
    /// </summary>
    public override FrameTransform: FrameTransform.Extrinsic;

    /// <summary>
    /// An Orientation specified as a unit quaternion.
    /// </summary>
    public override Orientation: Orientation.Quaternion;

    /// <summary>
    /// This function returns a Json encoding of an Advanced GeoPose
    /// </summary>
    public toJSON(): string {
        let indent: string = "";
        let sb: string[] = [''];
        {
            sb.push("{\r\n" + indent + "  ");
            if (this.validTime != null) {
                sb.push("\"validTime\": " + this.validTime.toString() + ",\r\n" + indent + "  ");
            }
            if (this.poseID != null && this.poseID.id != "") {
                sb.push("\"poseID\": \"" + this.poseID.id + "\",\r\n" + indent + "  ");
            }
            if (this.parentPoseID != null && this.parentPoseID.id != "") {
                sb.push("\"parentPoseID\": \"" + this.parentPoseID.id + "\",\r\n" + indent + "  ");
            }
            sb.push("\"frameSpecification\":\r\n" + indent + "  " + "{\r\n" + indent + "    \"authority\": \"" +
                (this.FrameTransform as FrameTransform.Extrinsic).authority.replace("\"", "\\\"") + "\",\r\n" + indent + "    \"id\": \"" +
                (this.FrameTransform as FrameTransform.Extrinsic).id.replace("\"", "\\\"") + "\",\r\n" + indent + "    \"parameters\": \"" +
                (this.FrameTransform as FrameTransform.Extrinsic).parameters.replace("\"", "\\\"") + "\"\r\n" + indent + "  },\r\n" + indent + "  ");
            sb.push("\"quaternion\":\r\n" + indent + "  {\r\n" + indent + "    \"x\":" + (this.Orientation as Orientation.Quaternion).x + ",\"y\":" +
                (this.Orientation as Orientation.Quaternion).y + ",\"z\":" +
                (this.Orientation as Orientation.Quaternion).z + ",\"w\":" +
                (this.Orientation as Orientation.Quaternion).w);
            sb.push("\r\n" + indent + "  }\r\n" + indent + "}\r\n");
            return sb.join('');
        }
    }
}
