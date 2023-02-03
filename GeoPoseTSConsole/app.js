"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.Advanced = exports.Local = exports.BasicQuaternion = exports.BasicYPR = void 0;
const node_process_1 = require("node:process");
const proj4 = require("proj4");
const Position = require("./Position");
const FrameTransform = require("./FrameTransform");
const Orientation = require("./Orientation");
const LTPENU = require("./WGS84ToLTPENU");
const Extras = require("./Extras");
var source = proj4.Proj('EPSG:4326'); //source coordinates will be in Longitude/Latitude, WGS84
var dest = proj4.Proj('EPSG:3785'); //destination coordinates in meters, global spherical mercators projection, see http://spatialreference.org/ref/epsg/3785/
// transforming point coordinates
var p = proj4.toPoint([-76.0, 45.0, 11.0]); //any object will do as long as it has 'x' and 'y' properties
//var p = new proj4.Point($("#lng").val(), $("#lat").val());
let q = proj4.transform(source, dest, p);
let r = proj4.transform(dest, source, q);
console.log("X : " + p.x + " \nY : " + p.y + " \nZ : " + p.z);
console.log("X : " + q.x + " \nY : " + q.y + " \nZ : " + q.z);
console.log("X : " + r.x + " \nY : " + r.y + " \nZ : " + r.z);
let d = new LTPENU.LTP_ENU();
let from = new Position.GeodeticPosition(-1.0, 52.0, 15.0);
let origin = new Position.GeodeticPosition(-1.00005, 52.0, 15.3);
let to = new Position.CartesianPosition(0, 0, 0);
d.GeodeticToEnu(from, origin, to);
node_process_1.stdin.read();
/// <summary>
/// A GeoPose has a position and an orientation.
/// The position is abstracted as a transformation between one reference frame (outer frame)
/// and another (inner frame).
/// The position is the origin of the coordinate system of the inner frame.
/// The orientation is applied to the coordinate system of the inner frame.
/// <remark>
/// See the OGS GeoPose 1.0 standard for a full description.
/// </remark>
/// <remark>
/// This implementation includes some optional properties not define in the 1.0 standard
/// but allowed by JSON serializations of all but the Basic-Quaternion(Strict) standardization target.
/// The optional properties are identifiers and time values that are useful in practice.
/// They may be part of a future version of the standard but, as of February 2023, they are optianl add-ons.
/// </remark>
/// </summary>
class GeoPose {
}
/// <summary>
/// The Basic GeoPoses share the use of a local tangent plane, east-north-up frame transform.
/// The types of Basic GeoPose are distinguished by the method used to specify orientation of the inner frame.
/// </summary>
class Basic extends GeoPose {
}
/// <summary>
/// A Basic-YPR GeoPose uses yaw, pitch, and roll angles measured in degrees to define the orientation of the inner frame..
/// </summary>
class BasicYPR extends Basic {
    constructor(id, tangentPoint, yprAngles) {
        super();
        this.poseID = new Extras.PoseID(id);
        this.FrameTransform = new FrameTransform.WGS84ToLTPENU(tangentPoint);
        this.Orientation = yprAngles;
    }
}
exports.BasicYPR = BasicYPR;
/// <summary>
/// A Basic-Quaternion GeoPose uses a unit quaternions to define the orientation of the inner frame..
/// <remark>
/// See the OGS GeoPose 1.0 standard for a full description.
/// </remark>
/// </summary>
class BasicQuaternion extends Basic {
    constructor(id, tangentPoint, quaternion) {
        super();
        this.poseID = new Extras.PoseID(id);
        this.FrameTransform = new FrameTransform.WGS84ToLTPENU(tangentPoint);
        this.Orientation = quaternion;
    }
}
exports.BasicQuaternion = BasicQuaternion;
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
class Local extends GeoPose {
    constructor(id, frameTransform, orientation) {
        super();
        this.poseID = new Extras.PoseID(id);
        this.FrameTransform = frameTransform;
        this.Orientation = orientation;
    }
}
exports.Local = Local;
/// <summary>
/// Advanced GeoPose.
/// </summary>
class Advanced extends GeoPose {
    constructor(poseID, frameTransform, orientation) {
        super();
        this.poseID = poseID;
        this.FrameTransform = frameTransform;
        this.Orientation = orientation;
    }
}
exports.Advanced = Advanced;
let myLocal = new BasicYPR("OS_GB", new Position.GeodeticPosition(51.5, -1.5, 0.0), new Orientation.YPRAngles(0, 0, 0));
node_process_1.stdin.read();
//# sourceMappingURL=app.js.map