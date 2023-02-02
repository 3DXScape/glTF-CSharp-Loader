"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
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
    WGS84ToLTPENU(tangentPoint) {
        this.origin = tangentPoint;
    }
}
class Position {
}
class GeodeticPosition extends Position {
}
class PoseID {
    PoseID(id) {
        this.id = id;
    }
}
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
/// <summary>
/// A derived pose within an engineering CRS with a Cartesian coordinate system.
/// This form is the closest to the classical computer graphics pose concept.
/// <remark>
/// Not (yet) part of the OGC GeoPose standard and not backwards-compatible.
/// Useful when operating within a local Cartesian frame defined by a Basic (or other) GeoPose.
/// </remark>
/// </summary>
class Local extends GeoPose {
    Local(id, frameTransform, quaternion) {
        this.poseID = new PoseID();
        this.poseID.id = id;
        this.FrameTransform = frameTransform;
        this.Orientation = quaternion;
    }
}
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
//# sourceMappingURL=app.js.map