import * as Extras from './Extras';
import * as Position from './Position';
import * as FrameTransform from './FrameTransform';
import * as Orientation from './Orientation';
import * as GeoPose from './GeoPose';

// Implemention order: 6 - follows GeoPose.
// This is the simplest family of GeoPoses - the 80% part of a 80/20 solution.

/// <summary>
/// The Basic GeoPoses share the use of a local tangent plane, east-north-up frame transform.
/// The types of Basic GeoPose are distinguished by the method used to specify orientation of the inner frame.
/// </summary>
export abstract class Basic extends GeoPose.GeoPose {
    /// <summary>
    /// A Position specified in geographic coordinates with height above a reference surface -
    /// usually an ellipsoid of revolution or a gravitational equipotential surface is
    /// transformed to a local Cartesian frame, suitable for use over an extent of a few km.
    /// </summary>
    public override FrameTransform: FrameTransform.WGS84ToLTPENU;
}

/// <summary>
/// A Basic-YPR GeoPose uses yaw, pitch, and roll angles measured in degrees to define the orientation of the inner frame..
/// </summary>
export class BasicYPR extends Basic {
    public constructor(id: string, tangentPoint: Position.GeodeticPosition, yprAngles: Orientation.YPRAngles) {
        super();
        this.poseID = new Extras.PoseID(id);
        this.FrameTransform = new FrameTransform.WGS84ToLTPENU(tangentPoint);
        this.Orientation = yprAngles;
    }
    /// <summary>
    /// An Orientation specified as three successive rotations about the local Z, Y, and X axes, in that order..
    /// </summary>
    public override Orientation: Orientation.YPRAngles;
}

/// <summary>
/// A Basic-Quaternion GeoPose uses a unit quaternions to define the orientation of the inner frame..
/// <remark>
/// See the OGS GeoPose 1.0 standard for a full description.
/// </remark>
/// </summary>
export class BasicQuaternion extends Basic {
    public constructor(id: string, tangentPoint: Position.GeodeticPosition, quaternion: Orientation.Quaternion) {
        super();
        this.poseID = new Extras.PoseID(id);
        this.FrameTransform = new FrameTransform.WGS84ToLTPENU(tangentPoint);
        this.Orientation = quaternion;
    }

    /// <summary>
    /// An Orientation specified as a unit quaternion.
    /// </summary>
    public override Orientation: Orientation.Quaternion;
}
