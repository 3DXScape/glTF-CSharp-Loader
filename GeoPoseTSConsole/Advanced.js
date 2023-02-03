"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.Advanced = void 0;
const GeoPose = require("./GeoPose");
// Implemention order: 7 - follows Basic GeoPose.
// This is the most general GeoPose - the largest part of the 20% part of a 80/20 solution.
// The difficult implementation is creating the interface layer between the
// Extrinsic specification and external authorities and data sources.
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