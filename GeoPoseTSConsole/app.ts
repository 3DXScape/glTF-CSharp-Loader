import { stdin as input } from 'node:process';
import * as proj4 from 'proj4';
import * as Position from './Position';
import * as FrameTransform from './FrameTransform';
import * as Orientation from './Orientation';
import * as LTPENU from './WGS84ToLTPENU';
import * as Extras from './Extras';


var source = proj4.Proj('EPSG:4326');    //source coordinates will be in Longitude/Latitude, WGS84
var dest = proj4.Proj('EPSG:3785');     //destination coordinates in meters, global spherical mercators projection, see http://spatialreference.org/ref/epsg/3785/

// transforming point coordinates
var p = proj4.toPoint([-76.0, 45.0, 11.0]);   //any object will do as long as it has 'x' and 'y' properties



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

input.read();

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
abstract class GeoPose {
    // Optional and non-standard but conforming added property:
    // an identifier unique within an application.
    public poseID: Extras.PoseID;

    // Optional and non-standard but conforming added property:
    // a PoseID type identifier of another GeoPose in the direction of the root of a pose tree.
    public parentPoseID: Extras.PoseID;

    // Optional and non-standard (except in Advanced) but conforming added property:
    // a validTime with milliseconds of Unix time.
    public validTime: number;
    abstract FrameTransform: FrameTransform.FrameTransform;
    abstract Orientation: Orientation.Orientation;
}
/// <summary>
/// The Basic GeoPoses share the use of a local tangent plane, east-north-up frame transform.
/// The types of Basic GeoPose are distinguished by the method used to specify orientation of the inner frame.
/// </summary>
abstract class Basic extends GeoPose {
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
export class Local extends GeoPose {
    public constructor(id: string, frameTransform: FrameTransform.Translation, orientation: Orientation.Quaternion) {
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
    public override Orientation: Orientation.Quaternion;
}

/// <summary>
/// Advanced GeoPose.
/// </summary>
export class Advanced extends GeoPose {
    public constructor(poseID: Extras.PoseID, frameTransform: FrameTransform.Extrinsic, orientation: Orientation.Quaternion) {
        super();
        this.poseID = poseID;
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
}


let myLocal = new BasicYPR("OS_GB", new Position.GeodeticPosition(51.5, -1.5, 0.0), new Orientation.YPRAngles(0, 0, 0));
input.read();
