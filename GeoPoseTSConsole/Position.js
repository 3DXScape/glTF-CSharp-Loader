"use strict";
// Implemention order: 2 - follows Extras.
// These classes define positions in a 3D frame using different conventions.
Object.defineProperty(exports, "__esModule", { value: true });
exports.NoPosition = exports.CartesianPosition = exports.GeodeticPosition = exports.Position = void 0;
/// <summary>
/// The abstract root of the Position hierarchy.
/// <note>
/// Because these various ways to express Position share no underlying structure,
/// the abstract root class definition is simply an empty shell.
/// </note>
/// </summary>
class Position {
}
exports.Position = Position;
/// <summary>
/// GeodeticPosition is a specialization of Position for using two angles and a height for geodetic reference systems.
/// </summary>
class GeodeticPosition extends Position {
    constructor(lat, lon, h) {
        super();
        this.lat = lat;
        this.lon = lon;
        this.h = h;
    }
}
exports.GeodeticPosition = GeodeticPosition;
/// <summary>
/// CartesianPosition is a specialization of Position for geocentric, topocentric, and engineering reference systems.
/// </summary>
class CartesianPosition extends Position {
    constructor(x, y, z) {
        super();
        this.x = x;
        this.y = y;
        this.z = z;
    }
}
exports.CartesianPosition = CartesianPosition;
class NoPosition extends Position {
    constructor() {
        super();
        this.x = this.y = this.z = NaN;
    }
}
exports.NoPosition = NoPosition;
//# sourceMappingURL=Position.js.map