"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.GeoPose = void 0;
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
exports.GeoPose = GeoPose;
//# sourceMappingURL=GeoPose.js.map