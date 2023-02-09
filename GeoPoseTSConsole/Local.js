"use strict";
// * Copyright(c) 2023 The Dani Elenga Foundation
// *
// * Permission is hereby granted, free of charge, to any person obtaining a copy
// * of this software and associated documentation files(the "Software"), to deal
// *     in the Software without restriction, including without limitation the rights
// * to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// * copies of the Software, and to permit persons to whom the Software is
// * furnished to do so, subject to the following conditions:
// *
// * The above copyright notice and this permission notice shall be included in all
// * copies or substantial portions of the Software.
// *
// * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// *     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// *     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// * SOFTWARE.
// *
Object.defineProperty(exports, "__esModule", { value: true });
exports.Local = void 0;
const Extras = require("./Extras");
const GeoPose = require("./GeoPose");
// WARNING: Serialization of this form of GeoPose produces JSON data in a form not part of the OGC GeoPose 1.0 standard.
// Implemention order: 8 -a useful GeoPose for working within a local Cartesian (i.e. engineering) frame.
// Local can be expressed as an Advanced form, but the Advanced form is more complex and this implementation is a shortcut.
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
class Local extends GeoPose.GeoPose {
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
            sb.push("{\r\n  ");
            if (this.validTime != null) {
                sb.push("\"validTime\": " + this.validTime.toString() + ",\r\n" + indent + "  ");
            }
            if (this.poseID != null && this.poseID.id != "") {
                sb.push("\"poseID\": \"" + this.poseID.id + "\",\r\n" + indent + "  ");
            }
            if (this.parentPoseID != null && this.parentPoseID.id != "") {
                sb.push("\"parentPoseID\": \"" + this.parentPoseID.id + "\",\r\n" + indent + "  ");
            }
            sb.push("\"position\": \r\n  {\r\n    " + "\"x\": " + this.FrameTransform.xOffset + ",\r\n    " +
                "\"y\": " + this.FrameTransform.yOffset + ",\r\n    " +
                "\"z\":   " + this.FrameTransform.zOffset);
            sb.push("\r\n  " + "},");
            sb.push("\r\n  ");
            sb.push("\"angles\": \r\n  {\r\n    " + "\"yaw\":   " + this.Orientation.yaw + ",\r\n    " +
                "\"pitch\": " + this.Orientation.pitch + ",\r\n    " +
                "\"roll\":  " + this.Orientation.roll);
            sb.push("\r\n  " + "}");
            sb.push("\r\n" + "}\r\n");
            return sb.join('');
        }
    }
}
exports.Local = Local;
//# sourceMappingURL=Local.js.map