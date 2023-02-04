"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.Advanced = void 0;
const Extras = require("./Extras");
const GeoPose = require("./GeoPose");
// Implemention order: 7 - follows Basic GeoPose.
// This is the most general GeoPose - the largest part of the 20% part of a 80/20 solution.
// The difficult implementation is creating the interface layer between the
// Extrinsic specification and external authorities and data sources.
/// <summary>
/// Advanced GeoPose.
/// </summary>
class Advanced extends GeoPose.GeoPose {
    constructor(id, frameTransform, orientation) {
        super();
        this.poseID = new Extras.PoseID(id);
        this.FrameTransform = frameTransform;
        this.Orientation = orientation;
    }
    /// <summary>
    /// This function returns a Json encoding of an Advanced GeoPose
    /// </summary>
    toJSON() {
        let indent = "";
        let sb = [''];
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
                this.FrameTransform.authority.replace("\"", "\\\"") + "\",\r\n" + indent + "    \"id\": \"" +
                this.FrameTransform.id.replace("\"", "\\\"") + "\",\r\n" + indent + "    \"parameters\": \"" +
                this.FrameTransform.parameters.replace("\"", "\\\"") + "\"\r\n" + indent + "  },\r\n" + indent + "  ");
            sb.push("\"quaternion\":\r\n" + indent + "  {\r\n" + indent + "    \"x\":" + this.Orientation.x + ",\"y\":" +
                this.Orientation.y + ",\"z\":" +
                this.Orientation.z + ",\"w\":" +
                this.Orientation.w);
            sb.push("\r\n" + indent + "  }\r\n" + indent + "}\r\n");
            return sb.join('');
        }
    }
}
exports.Advanced = Advanced;
//# sourceMappingURL=Advanced.js.map