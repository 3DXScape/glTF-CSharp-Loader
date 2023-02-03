"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.Advanced = void 0;
const GeoPose = require("./GeoPose");
/// <summary>
/// Advanced GeoPose.
/// </summary>
class Advanced extends GeoPose.GeoPose {
    constructor(poseID, frameTransform, orientation) {
        super();
        this.poseID = poseID;
        this.FrameTransform = frameTransform;
        this.Orientation = orientation;
    }
}
exports.Advanced = Advanced;
//# sourceMappingURL=Advanced.js.map