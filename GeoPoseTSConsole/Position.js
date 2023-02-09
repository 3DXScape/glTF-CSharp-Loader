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
exports.NoPosition = exports.CartesianPosition = exports.GeodeticPosition = exports.Position = void 0;
// Implemention order: 2 - follows Extras.
// These classes define positions in a 3D frame using different conventions.
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