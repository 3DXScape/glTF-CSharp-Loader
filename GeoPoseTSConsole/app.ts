import { stdin as input, stdout as output } from 'node:process';

class Orientation {
}
class FrameTransform {
}
class YPRAngles extends Orientation {
}
class Quaternion extends Orientation {
}
class Translation extends FrameTransform {
}
class Extrinsic extends FrameTransform {
}
class WGS84ToLTPENU {
    public WGS84ToLTPENU(tangentPoint: GeodeticPosition) {
        this.origin = tangentPoint;
    }
    public origin: GeodeticPosition;
}
class Position {
}
class GeodeticPosition extends Position {
}
class PoseID {
    public PoseID(id: string) {
        this.id = id;
    }
    public id: string;
}

abstract class GeoPose {
    // Optional and non-standard but conforming added property:
    //   an identifier unique within an application.
    public poseID: PoseID;

    // Optional and non-standard but conforming added property:
    //  a PoseID type identifier of another GeoPose in the direction of the root of a pose tree.
    public parentPoseID: PoseID;

    // Optional and non-standard but conforming added property:
    //   a poseID identifier in the direction of the root of a pose tree.
    public validTime: number;
    abstract FrameTransform: FrameTransform;
    abstract Orientation: Orientation;
}
abstract class Basic extends GeoPose
{
        /// <summary>
        /// A Position specified in geographic coordinates with height above a reference surface -
        /// usually an ellipsoid of revolution or a gravitational equipotential surface.
        /// </summary>
    //private WGS84ToLTP_ENU _frameTransform = new WGS84ToLTP_ENU();
    public override FrameTransform: WGS84ToLTPENU;
}

/// <summary>
/// A Basic-YPR GeoPose.
/// <remark>
/// See the OGS GeoPose 1.0 standard for a full description.
/// </remark>
/// </summary>
class BasicYPR extends Basic
{
    public BasicYPR(id: string, tangentPoint: GeodeticPosition, yprAngles: YPRAngles)
    {
        this.poseID = new PoseID();
        this.poseID.id = id;

        this.FrameTransform = new WGS84ToLTPENU();
        this.FrameTransform.origin = tangentPoint;
        this.Orientation = yprAngles;
    }
        /// <summary>
        /// An Orientation specified as three rotations.
        /// </summary>
    public override Orientation: YPRAngles;
}

/// <summary>
/// A Basic-Quaternion GeoPose.
/// <remark>
/// See the OGS GeoPose 1.0 standard for a full description.
/// </remark>
/// </summary>
class BasicQuaternion extends Basic
{
    public BasicQuaternion(id: string, tangentPoint: GeodeticPosition, quaternion: Quaternion)
    {
        this.poseID = new PoseID();
        this.poseID.id = id;
        this.FrameTransform = new WGS84ToLTPENU();
        this.FrameTransform.origin = tangentPoint;
        this.Orientation = quaternion;
    }

        /// <summary>
        /// An Orientation specified as a unit quaternion.
        /// </summary>
    public override Orientation: Quaternion;
}

/// <summary>
/// A derived pose within an engineering CRS with a Cartesian coordinate system.
/// This form is the closest to the classical computer graphics pose concept.
/// <remark>
/// Not (yet) part of the OGC GeoPose standard and not backwards-compatible.
/// Useful when operating within a local Cartesian frame defined by a Basic (or other) GeoPose.
/// </remark>
/// </summary>
class Local extends GeoPose
{
    public Local(id: string, frameTransform: Translation, quaternion: Orientation)
    {
        this.poseID = new PoseID();
        this.poseID.id = id;
        this.FrameTransform = frameTransform;
        this.Orientation = quaternion;
    }
        /// <summary>
        /// The xOffset, yOffset, zOffset from the origin of the rotated inner frame of a "parent" GeoPose.
        /// </summary>
        public override FrameTransform: Translation;

        /// <summary>
        /// An Orientation specified as three rotations.
        /// </summary>
        public override Orientation: Quaternion;
     
}

/// <summary>
/// Advanced GeoPose.
/// </summary>
class Advanced extends GeoPose
{
    public Advanced(poseID: PoseID, frameTransform: Extrinsic, orientation: Quaternion)
    {
        this.poseID = poseID;
        this.FrameTransform = frameTransform;
        this.Orientation = orientation;
    }

        /// <summary>
        /// A Frame Specification defining a frame with associated coordinate system whose Position is the origin.
        /// </summary>
    public override FrameTransform: Extrinsic;

        /// <summary>
        /// An Orientation specified as a unit quaternion.
        /// </summary>
    public override Orientation: Quaternion;
}

