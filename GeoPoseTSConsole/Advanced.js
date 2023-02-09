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