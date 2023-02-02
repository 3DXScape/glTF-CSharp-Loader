import { stdin as input, stdout as output } from 'node:process';


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
export class BasicYPR extends Basic
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
export class BasicQuaternion extends Basic
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
export class Local extends GeoPose
{
    public Local(id: string, frameTransform: Translation, orientation: Quaternion)
    {
        this.poseID = new PoseID();
        this.poseID.id = id;
        this.FrameTransform = frameTransform;
        this.Orientation = orientation;
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
export class Advanced extends GeoPose
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

/// <summary>
/// The abstract root of the Orientation hierarchy.
/// <note>
/// An Orientation is a generic container for information that defines rotation within a coordinate system associated with a reference frame.
/// An Orientation may have a specialized context with necessary ancillary information
/// that parameterizes the rotation.
/// Such context may include, for example, part of the information that may be conveyed in an ISO 19111 CRS specification
/// or a proprietary naming, numbering, or modelling scheme as used by EPSG, NASA Spice, or SEDRIS SRM.
/// Subclasses of Orientation exist precisely to hold this context in conjunction with code
/// implementing a Rotate function.
/// </note>
/// </summary>
abstract class Orientation {
    abstract function Position Rotate(point: Position);
}
/// <summary>
/// A specialization of Orientation using Yaw, Pitch, and Roll angles in degrees.
/// <remark>
/// This style of Orientation is best for easy human interpretation.
/// It suffers from some computational inefficiencies, awkward interpolation, and singularities.
/// </remark>
/// </summary>
export class YPRAngles extends Orientation
{
        public YPRAngles(yaw: number, pitch: number, roll: number)
    {
        this.yaw = yaw;
        this.pitch = pitch;
        this.roll = roll;
    }
        public override Position Rotate(Position point)
    {
            // convert to quaternion and use quaternion rotation
            Quaternion q = YPRAngles.ToQuaternion(this.yaw, this.pitch, this.roll);
        return Quaternion.Transform(point, q);
    }
    public static Quaternion ToQuaternion(yaw: number, pitch: number, roll: number) {
        // GeoPose uses angles in degrees for human readability
        // Convert degrees to radians.
        yaw *= (Math.PI / 180.0);
        pitch *= (Math.PI / 180.0);
        roll *= (Math.PI / 180.0);

        let cosRoll = Math.cos(roll * 0.5);
        let sinRoll = Math.sin(roll * 0.5);
        let cosPitch = Math.cos(pitch * 0.5);
        let sinPitch = Math.sin(pitch * 0.5);
        let cosYaw = Math.cos(yaw * 0.5);
        let sinYaw = Math.sin(yaw * 0.5);

        let w = cosRoll * cosPitch * cosYaw + sinRoll * sinPitch * sinYaw;
        let x = sinRoll * cosPitch * cosYaw - cosRoll * sinPitch * sinYaw;
        let y = cosRoll * sinPitch * cosYaw + sinRoll * cosPitch * sinYaw;
        let z = cosRoll * cosPitch * sinYaw - sinRoll * sinPitch * cosYaw;

        let norm = Math.sqrt(x * x + y * y + z * z + w * w);
        let q = new Quaternion();
        if (norm <= 0.0) {
            q.x = x;
            q.y = y;
            q.z = z;
            q.w = w;

        }
        else {
            q.x = x / norm;
            q.y = y / norm;
            q.z = z / norm;
            q.w = w / norm;
        }
        return q;
    }
        /// <summary>
        /// A left-right angle in degrees.
        /// </summary>
        public yaw: number;
        /// <summary>
        /// An up-down angle in degrees.
        /// </summary>
        public pitch: number;
        /// <summary>
        /// A side-to-side angle in degrees.
        /// </summary>
        public roll: number;
}
/// <summary>
/// A specialization of Orientation using a unit quaternion.
/// </summary>
/// <remark>
/// This style of Orientation is best for computation.
/// It is not easily interpreted or visualized by humans.
/// </remark>
export class Quaternion extends Orientation {
    public Quaternion(x: number, y: number, z: number, w: number) {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }
    public override function Position Rotate(point: Position) {
        return Quaternion.Transform(point, this);
    }
    public YPRAngles ToYPRAngles(q: Quaternion) {
        let yprAngles = new YPRAngles();

        // roll (x-axis rotation)
        let sinRollCosPitch = 2.0 * (q.w * q.x + q.y * q.z);
        let cosRollCosPitch = 1.0 - 2.0 * (q.x * q.x + q.y * q.y);
        yprAngles.roll = Math.atan2(sinRollCosPitch, cosRollCosPitch) * (180.0 / Math.PI); // in degrees

        // pitch (y-axis rotation)
        let sinPitch = Math.sqrt(1.0 + 2.0 * (q.w * q.y - q.x * q.z));
        let cosPitch = Math.sqrt(1.0 - 2.0 * (q.w * q.y - q.x * q.z));
        yprAngles.pitch = (2.0 * Math.atan2(sinPitch, cosPitch) - Math.PI / 2.0) * (180.0 / Math.PI); // in degrees

        // yaw (z-axis rotation)
        let sinYawCosPitch = 2.0 * (q.w * q.z + q.x * q.y);
        let cosYawCosPitch = 1.0 - 2.0 * (q.y * q.y + q.z * q.z);
        yprAngles.yaw = Math.atan2(sinYawCosPitch, cosYawCosPitch) * (180.0 / Math.PI); // in degrees

        return yprAngles;
    }
    public static Transform(inPoint: CartesianPosition, rotation: Quaternion) {
        let point = new CartesianPosition();
        point = inPoint;
        let x2 = rotation.x + rotation.x;
        let y2 = rotation.y + rotation.y;
        let z2 = rotation.z + rotation.z;

        let wx2 = rotation.w * x2;
        let wy2 = rotation.w * y2;
        let wz2 = rotation.w * z2;
        let xx2 = rotation.x * x2;
        let xy2 = rotation.x * y2;
        let xz2 = rotation.x * z2;
        let yy2 = rotation.y * y2;
        let yz2 = rotation.y * z2;
        let zz2 = rotation.z * z2;

        let p = new CartesianPosition();
        p.x = point.x * (1.0 - yy2 - zz2) + point.y * (xy2 - wz2) + point.z * (xz2 + wy2),
            p.y = point.x * (xy2 + wz2) + point.y * (1.0 - xx2 - zz2) + point.z * (yz2 - wx2),
                p.z = point.x * (xz2 - wy2) + point.y * (yz2 + wx2) + point.z * (1.0 - xx2 - yy2));
        return p;
    }
    /// <summary>
    /// The x component.
    /// </summary>
    public x: number;
    /// <summary>
    /// The y component.
    /// </summary>
    public y: number;
    /// <summary>
    /// The z component.
    /// </summary>
    public z: number;
    /// <summary>
    /// The w component.
    /// </summary>
    public w: number;
}
/// <summary>
/// The abstract root of the Position hierarchy.
/// <note>
/// Because the various ways to express Position share no underlying structure,
/// the class definition is simply an empty shell.
/// </note>
/// </summary>
public abstract class Position {

}

/// <summary>
/// A specialization of Position for using two angles and a height for geodetic positions.
/// </summary>
export class GeodeticPosition extends Position
{
     public GeodeticPosition(lat: number, lon: number, h: number)
    {
        this.lat = lat;
        this.lon = lon;
        this.h = h;
    }

        /// <summary>
        /// A latitude in degrees, positive north of equator and negative south of equator.
        /// The latitude is the angle between the plane of the equator and a plane tangent to the ellipsoid at the given point.
        /// </summary>
    public lat: number;
        /// <summary>
        /// A longitude in degrees, positive east of the prime meridian and negative west of prime meridian.
        /// </summary>
    public lon: number;
        /// <summary>
        /// A distance in meters, measured with respect to an implied (Basic) or specified (Advanced) reference surface,
        /// postive opposite the direction of the force of gravity,
        /// and negative in the direction of the force of gravity.
        /// </summary>
        public h: number
}
/// <summary>
/// A specialization of Position for geocentric positions.
/// </summary>
export class CartesianPosition extends Position {
    public CartesianPosition(x: number, y: number, z: number) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    /// <summary>
    /// A coordinate value in meters, along an axis (x-axis) that typically has origin at
    /// the center of mass, lies in the same plane as the y axis, and perpendicular to the y axis,
    /// forming a right-hand coordinate system with the z-axis in the up direction.
    /// </summary>
    public x: number;
    /// <summary>
    /// A coordinate value in meters, along an axis (y-axis) that typically has origin at
    /// the center of mass, lies in the same plane as the x axis, and perpendicular to the x axis,
    /// forming a right-hand coordinate system with the z-axis in the up direction.
    /// </summary>
    public y: number;
    /// <summary>
    /// A coordinate value in meters, along the z-axis.
    /// </summary>
    public z: number;
}
public class NoPosition extends Position {
    /// <summary>
    /// A coordinate value in meters, along an axis (x-axis) that typically has origin at
    /// the center of mass, lies in the same plane as the y axis, and perpendicular to the y axis,
    /// forming a right-hand coordinate system with the z-axis in the up direction.
    /// </summary>
    public x: number;
    /// <summary>
    /// A coordinate value in meters, along an axis (y-axis) that typically has origin at
    /// the center of mass, lies in the same plane as the x axis, and perpendicular to the x axis,
    /// forming a right-hand coordinate system with the z-axis in the up direction.
    /// </summary>
    public y: number;
    /// <summary>
    /// A coordinate value in meters, along the z-axis.
    /// </summary>
    public z: number;
}


class FrameTransform {
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


class PoseID {
    public PoseID(id: string) {
        this.id = id;
    }
    public id: string;
}
