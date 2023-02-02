"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.Translation = exports.GeodeticToEnu = exports.WGS84ToLTP_ENU = exports.Extrinsic = exports.PoseID = exports.NoPosition = exports.CartesianPosition = exports.GeodeticPosition = exports.Quaternion = exports.YPRAngles = exports.Advanced = exports.Local = exports.BasicQuaternion = exports.BasicYPR = void 0;
const proj4 = require("proj4");
const node_process_1 = require("node:process");
var source = proj4.Proj('EPSG:4326'); //source coordinates will be in Longitude/Latitude, WGS84
var dest = proj4.Proj('EPSG:3785'); //destination coordinates in meters, global spherical mercators projection, see http://spatialreference.org/ref/epsg/3785/
// transforming point coordinates
var p = proj4.Point(-76.0, 45.0, 11.0); //any object will do as long as it has 'x' and 'y' properties
//var p = new proj4.Point($("#lng").val(), $("#lat").val());
let q = proj4.transform(source, dest, p);
let r = proj4.transform(dest, source, q);
console.log("X : " + p.x + " \nY : " + p.y + " \nZ : " + p.z);
console.log("X : " + q.x + " \nY : " + q.y + " \nZ : " + q.z);
console.log("X : " + r.x + " \nY : " + r.y + " \nZ : " + r.z);
node_process_1.stdin.read();
class GeoPose {
}
class Basic extends GeoPose {
}
/// <summary>
/// A Basic-YPR GeoPose.
/// <remark>
/// See the OGS GeoPose 1.0 standard for a full description.
/// </remark>
/// </summary>
class BasicYPR extends Basic {
    BasicYPR(id, tangentPoint, yprAngles) {
        this.poseID = new PoseID();
        this.poseID.id = id;
        this.FrameTransform = new WGS84ToLTPENU();
        this.FrameTransform.origin = tangentPoint;
        this.Orientation = yprAngles;
    }
}
exports.BasicYPR = BasicYPR;
/// <summary>
/// A Basic-Quaternion GeoPose.
/// <remark>
/// See the OGS GeoPose 1.0 standard for a full description.
/// </remark>
/// </summary>
class BasicQuaternion extends Basic {
    BasicQuaternion(id, tangentPoint, quaternion) {
        this.poseID = new PoseID();
        this.poseID.id = id;
        this.FrameTransform = new WGS84ToLTPENU();
        this.FrameTransform.origin = tangentPoint;
        this.Orientation = quaternion;
    }
}
exports.BasicQuaternion = BasicQuaternion;
/// <summary>
/// A derived pose within an engineering CRS with a Cartesian coordinate system.
/// This form is the closest to the classical computer graphics pose concept.
/// <remark>
/// Not (yet) part of the OGC GeoPose standard and not backwards-compatible.
/// Useful when operating within a local Cartesian frame defined by a Basic (or other) GeoPose.
/// </remark>
/// </summary>
class Local extends GeoPose {
    Local(id, frameTransform, orientation) {
        this.poseID = new PoseID();
        this.poseID.id = id;
        this.FrameTransform = frameTransform;
        this.Orientation = orientation;
    }
}
exports.Local = Local;
/// <summary>
/// Advanced GeoPose.
/// </summary>
class Advanced extends GeoPose {
    Advanced(poseID, frameTransform, orientation) {
        this.poseID = poseID;
        this.FrameTransform = frameTransform;
        this.Orientation = orientation;
    }
}
exports.Advanced = Advanced;
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
class Orientation {
}
/// <summary>
/// A specialization of Orientation using Yaw, Pitch, and Roll angles in degrees.
/// <remark>
/// This style of Orientation is best for easy human interpretation.
/// It suffers from some computational inefficiencies, awkward interpolation, and singularities.
/// </remark>
/// </summary>
class YPRAngles extends Orientation {
    YPRAngles(yaw, pitch, roll) {
        this.yaw = yaw;
        this.pitch = pitch;
        this.roll = roll;
    }
    Rotate(point) {
        // convert to quaternion and use quaternion rotation
        let q = YPRAngles.ToQuaternion(this.yaw, this.pitch, this.roll);
        return Quaternion.Transform(point, q);
    }
    static ToQuaternion(yaw, pitch, roll) {
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
}
exports.YPRAngles = YPRAngles;
/// <summary>
/// A specialization of Orientation using a unit quaternion.
/// </summary>
/// <remark>
/// This style of Orientation is best for computation.
/// It is not easily interpreted or visualized by humans.
/// </remark>
class Quaternion extends Orientation {
    Quaternion(x, y, z, w) {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }
    Rotate(point) {
        return Quaternion.Transform(point, this);
    }
    ToYPRAngles(q) {
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
    static Transform(inPoint, rotation) {
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
            p.z = point.x * (xz2 - wy2) + point.y * (yz2 + wx2) + point.z * (1.0 - xx2 - yy2);
        return p;
    }
}
exports.Quaternion = Quaternion;
/// <summary>
/// The abstract root of the Position hierarchy.
/// <note>
/// Because the various ways to express Position share no underlying structure,
/// the class definition is simply an empty shell.
/// </note>
/// </summary>
class Position {
}
/// <summary>
/// A specialization of Position for using two angles and a height for geodetic positions.
/// </summary>
class GeodeticPosition extends Position {
    GeodeticPosition(lat, lon, h) {
        this.lat = lat;
        this.lon = lon;
        this.h = h;
    }
}
exports.GeodeticPosition = GeodeticPosition;
/// <summary>
/// A specialization of Position for geocentric positions.
/// </summary>
class CartesianPosition extends Position {
    CartesianPosition(x, y, z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }
}
exports.CartesianPosition = CartesianPosition;
class NoPosition extends Position {
}
exports.NoPosition = NoPosition;
class PoseID {
    PoseID(id) {
        this.id = id;
    }
}
exports.PoseID = PoseID;
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
class FrameTransform {
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
class Extrinsic extends FrameTransform {
    Extrinsic(authority, id, parameters) {
        this.authority = authority;
        this.id = id;
        this.parameters = parameters;
    }
    /// <summary>
    /// The core function of a transformation is the implement a specific frame transformation
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
    Transform(point) {
        let uri = this.authority.toLowerCase().replace("//www.", "");
        if (uri == "https://proj.org" || uri == "https://osgeo.org") {
            var outer = proj4.Proj('EPSG:4326'); //source coordinates will be in Longitude/Latitude, WGS84
            var inner = proj4.Proj('EPSG:3785'); //destination coordinates in meters, global spherical mercato
            var cp = point;
            let p = proj4.Point(cp.x, cp.y, cp.z);
            proj4.transform(outer, inner, p);
            // convert points from one coordinate system to another
            let outP = new CartesianPosition();
            outP.x = p.x;
            outP.y = p.y;
            outP.z = p.z;
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
}
exports.Extrinsic = Extrinsic;
Extrinsic.noTransform = new NoPosition();
/// <summary>
/// A specialized specification of the WGS84 (EPSG 4326) geodetic frame to a local tangent plane East, North, Up frame.
/// <remark>
/// The origin of the coordinate system associated with the frame is a Position - the origin -
/// which is the *only* distinguished Position associated with the coodinate system associated with the inner frame (range).
/// </remark>
/// </summary>
class WGS84ToLTP_ENU extends FrameTransform {
    WGS84ToLTP_ENU(origin) {
        this.Origin = origin;
    }
    Transform(point) {
        let east;
        let north;
        let up = 0;
        let geoPoint = point;
        let outPoint;
        GeodeticToEnu(this.Origin, geoPoint, outPoint);
        return outPoint;
    }
}
exports.WGS84ToLTP_ENU = WGS84ToLTP_ENU;
function GeodeticToEnu(origin, geoPoint, enuPoint) {
    let out = new CartesianPosition();
    return out;
}
exports.GeodeticToEnu = GeodeticToEnu;
// A simple translation frame transform.
// The FrameTransform is created with an offset.
// The Transform adds the offset ot an input Cartesian Position and reurns a Cartesian Position
class Translation extends FrameTransform {
    Translation(xOffset, yOffset, zOffset) {
        this.xOffset = xOffset;
        this.yOffset = yOffset;
        this.zOffset = zOffset;
    }
    Transform(point) {
        let p = new CartesianPosition();
        let cp = point;
        p.x = cp.x + this.xOffset;
        p.y = cp.y + this.yOffset;
        p.z = cp.z + this.zOffset;
        return p;
    }
}
exports.Translation = Translation;
class WGS84ToLTPENU extends FrameTransform {
    WGS84ToLTPENU(tangentPoint) {
        this.origin = tangentPoint;
    }
    Transform(point) {
        return NoPosition;
    }
}
//# sourceMappingURL=app.js.map