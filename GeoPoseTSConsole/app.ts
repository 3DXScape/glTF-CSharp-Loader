import * as proj4 from 'proj4';
import * as LTP_ENU from 'WGS84ToLTPENU';
import { stdin as input, stdout as output } from 'node:process';

var source = proj4.Proj('EPSG:4326');    //source coordinates will be in Longitude/Latitude, WGS84
var dest = proj4.Proj('EPSG:3785');     //destination coordinates in meters, global spherical mercators projection, see http://spatialreference.org/ref/epsg/3785/

// transforming point coordinates
var p = proj4.Point(-76.0, 45.0, 11.0);   //any object will do as long as it has 'x' and 'y' properties



    //var p = new proj4.Point($("#lng").val(), $("#lat").val());
let q = proj4.transform(source, dest, p);
let r = proj4.transform(dest, source, q);
console.log("X : " + p.x + " \nY : " + p.y + " \nZ : " + p.z);
console.log("X : " + q.x + " \nY : " + q.y + " \nZ : " + q.z);
console.log("X : " + r.x + " \nY : " + r.y + " \nZ : " + r.z);
input.read();



abstract class GeoPose {
    // Optional and non-standard but conforming added property:
    //   an identifier unique within an application.
    public poseID: PoseID;

    // Optional and non-standard but conforming added property:
    //  a PoseID type identifier of another GeoPose in the direction of the root of a pose tree.
    public parentPoseID: PoseID;

    // Optional and non-standard (except in Advanced) but conforming added property:
    //   a validTime with milliseconds of Unix time.
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
    public constructor(id: string, tangentPoint: GeodeticPosition, yprAngles: YPRAngles)
    {
        super();
        this.poseID = new PoseID(id);
        this.FrameTransform = new WGS84ToLTPENU(tangentPoint);
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
export class BasicQuaternion extends Basic {
    public constructor(id: string, tangentPoint: GeodeticPosition, quaternion: Quaternion) {
        super();
        this.poseID = new PoseID(id);
        this.FrameTransform = new WGS84ToLTPENU(tangentPoint);
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
export class Local extends GeoPose {
    public constructor(id: string, frameTransform: Translation, orientation: Quaternion) {
        super();
        this.poseID = new PoseID(id);
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
export class Advanced extends GeoPose {
    public constructor(poseID: PoseID, frameTransform: Extrinsic, orientation: Quaternion) {
        super();
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
    abstract Rotate(point: CartesianPosition): Position;
}

/// <summary>
/// A specialization of Orientation using Yaw, Pitch, and Roll angles measured in degrees.
/// <remark>
/// This style of Orientation is best for easy human interpretation.
/// It suffers from some computational inefficiencies, awkward interpolation, and singularities.
/// </remark>
/// </summary>
export class YPRAngles extends Orientation {
    public constructor(yaw: number, pitch: number, roll: number) {
        super();
        this.yaw = yaw;
        this.pitch = pitch;
        this.roll = roll;
    }

    /// <summary>
    /// The function is to apply a YPR transformation
    /// </summary>
    public override Rotate(point: CartesianPosition): Position {
        // convert to quaternion and use quaternion rotation
        let q = YPRAngles.ToQuaternion(this.yaw, this.pitch, this.roll);
        return Quaternion.Transform(point, q);
    }
    public static ToQuaternion(yaw: number, pitch: number, roll: number): Quaternion {
        // GeoPose angles are measured in degrees for human readability
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
        let q = new Quaternion(x, y, z, w);
        if (norm > 0.0) {

            q.x = q.x / norm;
            q.y = q.y / norm;
            q.z = q.z / norm;
            q.w = q.w / norm;
        }
        return q;
    }
    /// <summary>
    /// A left-right angle in degrees.
    /// </summary>
    public yaw: number;
    /// <summary>
    /// A forward-looking up-down angle in degrees.
    /// </summary>
    public pitch: number;
    /// <summary>
    /// A side-to-side angle in degrees.
    /// </summary>
    public roll: number;
}
/// <summary>
/// Quaternion is a specialization of Orientation using a unit quaternion.
/// </summary>
/// <remark>
/// This style of Orientation is best for computation.
/// It is not easily interpreted or visualized by humans.
/// </remark>
export class Quaternion extends Orientation {
    public constructor(x: number, y: number, z: number, w: number) {
        super();
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }
    public override Rotate(point: CartesianPosition): Position  {
        return Quaternion.Transform(point, this);
    }
    public ToYPRAngles(q: Quaternion): YPRAngles {

        // roll (x-axis rotation)
        let sinRollCosPitch = 2.0 * (q.w * q.x + q.y * q.z);
        let cosRollCosPitch = 1.0 - 2.0 * (q.x * q.x + q.y * q.y);
        let roll = Math.atan2(sinRollCosPitch, cosRollCosPitch) * (180.0 / Math.PI); // in degrees

        // pitch (y-axis rotation)
        let sinPitch = Math.sqrt(1.0 + 2.0 * (q.w * q.y - q.x * q.z));
        let cosPitch = Math.sqrt(1.0 - 2.0 * (q.w * q.y - q.x * q.z));
        let pitch = (2.0 * Math.atan2(sinPitch, cosPitch) - Math.PI / 2.0) * (180.0 / Math.PI); // in degrees

        // yaw (z-axis rotation)
        let sinYawCosPitch = 2.0 * (q.w * q.z + q.x * q.y);
        let cosYawCosPitch = 1.0 - 2.0 * (q.y * q.y + q.z * q.z);
        let yaw = Math.atan2(sinYawCosPitch, cosYawCosPitch) * (180.0 / Math.PI); // in degrees
        let yprAngles = new YPRAngles(yaw, pitch, roll);
        return yprAngles;
    }
    public static Transform(inPoint: CartesianPosition, rotation: Quaternion): CartesianPosition {
        let point = new CartesianPosition(inPoint.x, inPoint.y, inPoint.z);
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

        let p = new CartesianPosition(
            point.x * (1.0 - yy2 - zz2) + point.y * (xy2 - wz2) + point.z * (xz2 + wy2),
            point.x * (xy2 + wz2) + point.y * (1.0 - xx2 - zz2) + point.z * (yz2 - wx2),
            point.x * (xz2 - wy2) + point.y * (yz2 + wx2) + point.z * (1.0 - xx2 - yy2));
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
/// the abstract root class definition is simply an empty shell.
/// </note>
/// </summary>
abstract class Position {
}

/// <summary>
/// GeodeticPosition is a specialization of Position for using two angles and a height for geodetic reference systems.
/// </summary>
export class GeodeticPosition extends Position {
    public constructor(lat: number, lon: number, h: number) {
        super();
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
/// CartesianPosition is a specialization of Position for geocentric, topocentric, and engineering reference systems.
/// </summary>
export class CartesianPosition extends Position {
    public constructor(x: number, y: number, z: number) {
        super();
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

/// <summary>
/// NoPosition is a specialization of Position for a Position that can be easily identified as non-existent.
/// </summary>
export class NoPosition extends Position {
    public constructor(){
        super();
        this.x = this.y = this.z = NaN;
    }
    /// <summary>
    /// A coordinate value in meters, along an axis (x-axis) that typically has origin at
    /// the center of mass, lies in the same plane as the y axis, and perpendicular to the y axis,
    /// forming a right-hand coordinate system with the z-axis in the up direction.
    /// </summary>
    public readonly x: number = NaN;
    /// <summary>
    /// A coordinate value in meters, along an axis (y-axis) that typically has origin at
    /// the center of mass, lies in the same plane as the x axis, and perpendicular to the x axis,
    /// forming a right-hand coordinate system with the z-axis in the up direction.
    /// </summary>
    public readonly y: number = NaN;
    /// <summary>
    /// A coordinate value in meters, along the z-axis.
    /// </summary>
    public readonly z: number = NaN;
}

export class PoseID {
    public constructor(id: string) {
        this.id = id;
    }
    public id: string;
}

/// <summary>
/// A FrameTransform is a generic container for information that defines mapping between reference frames.
/// Most transformation have a context with necessary ancillary information
/// that parameterizes the transformation of a Position in one frame to a corresponding Position is another.
/// Such context may include, for example, some or all of the information that may be conveyed in an ISO 19111 CRS specification
/// or a proprietary naming, numbering, or modelling scheme as used by EPSG, NASA Spice, or SEDRIS SRM.
/// Subclasses of FrameTransform exist precisely to hold this context in conjunction with code
/// implementing a Transform function.
/// <remark>
/// </remark>
/// </summary>
abstract class FrameTransform {
    public abstract Transform(point: Position): Position;
}


/// <summary>
/// A FrameSpecification is a generic container for information that defines a reference frame.
/// <remark>
/// A FrameSpecification can be abstracted as a Position:
/// The origin of the coordinate system associated with the frame is a Position and serves in that role
/// in the Advanced GeoPose.
/// The origin, is in fact the *only* distinguished Position associated with the coodinate system.
/// </remark>
/// </summary>
export class Extrinsic extends FrameTransform {
    public constructor(authority: string, id: string, parameters: string) {
        super();
        this.authority = authority;
        this.id = id;
        this.parameters = parameters;
    }
    /// <summary>
    /// The core function of a transformation is to implement a specific frame transformation
    /// i.e. the transformation of a triple of point coordinates in the outer frame to a triple of point coordinates in the inner frame.
    /// When this is not possible due to lack of an appropriate tranformation procedure,
    /// the triple (NaN, NaN, NaN) [three IEEE 574 not-a-number vales] is returned.
    /// Note that an "authority" is not necessarily a standards organization but rather an entity that provides
    /// a register of some kind for a category of frame- and/or frame transform specifications that is useful and stable enough
    /// for someone to implement transformation functions.
    /// An implementation need not implement all possbile transforms.
    /// </summary>
    /// <note>
    /// This would be a good element to implement as a set of plugin.
    /// </note>
    /// <param name="point"></param>
    /// <returns></returns>
    public override Transform(point: Position): Position {
        let uri = this.authority.toLowerCase().replace("//www.", "");
        if (uri == "https://proj.org" || uri == "https://osgeo.org") {
            var outer = proj4.Proj('EPSG:4326');    //source coordinates will be in Longitude/Latitude, WGS84
            var inner = proj4.Proj('EPSG:3785');     //destination coordinates in meters, global spherical mercato
            var cp = point as CartesianPosition;
            let p = proj4.Point(cp.x, cp.y, cp.z);
            proj4.transform(outer, inner, p);
            // convert points from one coordinate system to another
            let outP = new CartesianPosition(p.x, p.y, p.z);
            return outP;
        }
        else if (uri == "https://epsg.org") {
            return NoPosition;
        }
        else if (uri == "https://iers.org") {
            return NoPosition;
        }
        else if (uri == "https://naif.jpl.nasa.gov") {
            return NoPosition;
        }
        else if (uri == "https://sedris.org") {
            return NoPosition;
        }
        else if (uri == "https://iau.org") {
            return NoPosition;
        }
        return NoPosition;
    }
    /// <summary>
    /// The name or identification of the definer of the category of frame specification.
    /// A Uri that usually but not always points to a valid web address.
    /// </summary>
    public authority: string;
    /// <summary>
    /// A string that uniquely identifies a frame type.
    /// The interpretation of the string is determined by the authority.
    /// </summary>
    public id: string;
    /// <summary>
    /// A string that holds any parameters required by the authority to define a frame of the given type as specified by the id.
    /// The interpretation of the string is determined by the authority.
    /// </summary>
    public parameters: string;
    public static noTransform: Position = new NoPosition();
}
/// <summary>
/// A specialized specification of the WGS84 (EPSG 4326) geodetic frame to a local tangent plane East, North, Up frame.
/// <remark>
/// The origin of the coordinate system associated with the frame is a Position - the origin -
/// which is the *only* distinguished Position associated with the coodinate system associated with the inner frame (range).
/// </remark>
/// </summary>
export class WGS84ToLTPENU extends FrameTransform {
    public constructor(origin: GeodeticPosition) {
        super();
        this.Origin = origin;
    }
    public override Transform(point: Position): Position {
        let geoPoint = point as GeodeticPosition;
        let outPoint: CartesianPosition;
        GeodeticToEnu(this.Origin, geoPoint, outPoint);
        return outPoint;
    }

    /// <summary>
    /// A single geodetic position defines the tangent point for a transform to LTP-ENU.
    /// </summary>
    public Origin: GeodeticPosition;
}

export function GeodeticToEnu(origin: GeodeticPosition, geoPoint: GeodeticPosition, enuPoint: CartesianPosition) {
    let out = new CartesianPosition(0, 0, 0);
    return out;
}

// A simple translation frame transform.
// The FrameTransform is created with an offset.
// The Transform adds the offset ot an input Cartesian Position and reurns a Cartesian Position
export class Translation extends FrameTransform {
    public constructor(xOffset: number, yOffset: number, zOffset: number) {
        super();
        this.xOffset = xOffset;
        this.yOffset = yOffset;
        this.zOffset = zOffset;
    }
    public override Transform(point: Position): Position {
        let cp = point as CartesianPosition;
        let p = new CartesianPosition(cp.x + this.xOffset, cp.y + this.yOffset, cp.z + this.zOffset);
        return p;
    }
    public xOffset: number;
    public yOffset: number;
    public zOffset: number;
}


