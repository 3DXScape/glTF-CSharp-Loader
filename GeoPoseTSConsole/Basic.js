"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.BasicQuaternion = exports.BasicYPR = exports.Basic = void 0;
const Extras = require("./Extras");
const FrameTransform = require("./FrameTransform");
const GeoPose = require("./GeoPose");
// Implemention order: 6 - follows GeoPose.
// This is the simplest family of GeoPoses - the 80% part of a 80/20 solution.
/// <summary>
/// The Basic GeoPoses share the use of a local tangent plane, east-north-up frame transform.
/// The types of Basic GeoPose are distinguished by the method used to specify orientation of the inner frame.
/// </summary>
class Basic extends GeoPose.GeoPose {
}
exports.Basic = Basic;
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
//# sourceMappingURL=Basic.js.map