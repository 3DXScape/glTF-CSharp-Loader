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
var _a;
Object.defineProperty(exports, "__esModule", { value: true });
exports.ExtrinsicSupport = exports.LTP_ENU = void 0;
const Position = require("./Position");
class LTP_ENU {
    // Convert WGS-84 Geodetic point (lat, lon, h) to the 
    // Earth-Centered Earth-Fixed (ECEF) coordinates (x, y, z).
    static GeodeticToEcef(from, to) {
        // Convert to radians in notation consistent with the paper:
        var lambda = from.lat * this.toRadians;
        var phi = from.lon * this.toDegrees;
        var s = Math.sin(lambda);
        var N = this.a / Math.sqrt(1.0 - this.e_sq * s * s);
        var sin_lambda = Math.sin(lambda);
        var cos_lambda = Math.cos(lambda);
        var cos_phi = Math.cos(phi);
        var sin_phi = Math.sin(phi);
        to.x = (from.h + N) * cos_lambda * cos_phi;
        to.y = (from.h + N) * cos_lambda * sin_phi;
        to.z = (from.h + (1 - this.e_sq) * N) * sin_lambda;
    }
    // Convert the Earth-Centered Earth-Fixed (ECEF) coordinates (x, y, z) to 
    // (WGS-84) Geodetic point (lat, lon, h).
    static EcefToGeodetic(from, to) {
        var eps = this.e_sq / (1.0 - this.e_sq);
        var p = Math.sqrt(from.x * from.x + from.y * from.y);
        var q = Math.atan2((from.z * this.a), (p * this.b));
        var sin_q = Math.sin(q);
        var cos_q = Math.cos(q);
        var sin_q_3 = sin_q * sin_q * sin_q;
        var cos_q_3 = cos_q * cos_q * cos_q;
        var phi = Math.atan2((from.z + eps * this.b * sin_q_3), (p - this.e_sq * this.a * cos_q_3));
        var lambda = Math.atan2(from.y, from.x);
        var v = this.a / Math.sqrt(1.0 - this.e_sq * Math.sin(phi) * Math.sin(phi));
        to.h = (p / Math.cos(phi)) - v;
        to.lat = phi * this.toDegrees;
        to.lon = lambda * this.toDegrees;
    }
    // Converts the Earth-Centered Earth-Fixed (ECEF) coordinates (x, y, z) to 
    // East-North-Up coordinates in a Local Tangent Plane that is centered at the 
    // (WGS-84) Geodetic point (lat0, lon0, h0).
    static EcefToEnu(from, origin, to) {
        // Convert to radians in notation consistent with the paper:
        var lambda = origin.lat * this.toRadians;
        var phi = origin.lon * this.toDegrees;
        var s = Math.sin(lambda);
        var N = this.a / Math.sqrt(1.0 - this.e_sq * s * s);
        var sin_lambda = Math.sin(lambda);
        var cos_lambda = Math.cos(lambda);
        var cos_phi = Math.cos(phi);
        var sin_phi = Math.sin(phi);
        var x0 = (origin.h + N) * cos_lambda * cos_phi;
        var y0 = (origin.h + N) * cos_lambda * sin_phi;
        var z0 = (origin.h + (1 - this.e_sq) * N) * sin_lambda;
        var xd = from.x - x0;
        var yd = from.y - y0;
        var zd = from.z - z0;
        // This is the matrix multiplication
        to.x = -sin_phi * xd + cos_phi * yd;
        to.y = -cos_phi * sin_lambda * xd - sin_lambda * sin_phi * yd + cos_lambda * zd;
        to.z = cos_lambda * cos_phi * xd + cos_lambda * sin_phi * yd + sin_lambda * zd;
    }
    // Inverse of EcefToEnu. Converts East-North-Up coordinates (xEast, yNorth, zUp) in a
    // Local Tangent Plane that is centered at the (WGS-84) Geodetic point (lat0, lon0, h0)
    // to the Earth-Centered Earth-Fixed (ECEF) coordinates (x, y, z).
    static EnuToEcef(from, origin, to) {
        // Convert to radians in notation consistent with the paper:
        var lambda = origin.lat * this.toRadians;
        var phi = origin.lon * this.toRadians;
        var s = Math.sin(lambda);
        var N = this.a / Math.sqrt(1.0 - this.e_sq * s * s);
        var sin_lambda = Math.sin(lambda);
        var cos_lambda = Math.cos(lambda);
        var cos_phi = Math.cos(phi);
        var sin_phi = Math.sin(phi);
        var x0 = (origin.h + N) * cos_lambda * cos_phi;
        var y0 = (origin.h + N) * cos_lambda * sin_phi;
        var z0 = (origin.h + (1.0 - this.e_sq) * N) * sin_lambda;
        var xd = -sin_phi * from.x - cos_phi * sin_lambda * from.y + cos_lambda * cos_phi * from.z;
        var yd = cos_phi * from.x - sin_lambda * sin_phi * from.y + cos_lambda * sin_phi * from.z;
        var zd = cos_lambda * from.y + sin_lambda * from.z;
        to.x = xd + x0;
        to.y = yd + y0;
        to.z = zd + z0;
    }
    // Converts the geodetic WGS-84 coordinated (lat, lon, h) to 
    // East-North-Up coordinates in a Local Tangent Plane that is centered at the 
    // (WGS-84) Geodetic point (lat0, lon0, h0).
    static GeodeticToEnu(from, origin) {
        let ecef = new Position.CartesianPosition(0, 0, 0);
        this.GeodeticToEcef(from, ecef);
        let to = new Position.CartesianPosition(0, 0, 0);
        this.EcefToEnu(ecef, origin, to);
        return to;
    }
    static EnuToGeodetic(from, origin, to) {
        let ecef = new Position.CartesianPosition(0, 0, 0);
        this.EnuToEcef(from, origin, ecef);
        this.EcefToGeodetic(ecef, to);
    }
}
exports.LTP_ENU = LTP_ENU;
_a = LTP_ENU;
// WGS-84 geodetic constants
LTP_ENU.a = 6378137.0; // WGS-84 Earth semimajor axis (m)
LTP_ENU.b = 6356752.314245; // Derived Earth semiminor axis (m)
LTP_ENU.f = (_a.a - _a.b) / _a.a; // Ellipsoid Flatness
LTP_ENU.f_inv = 1.0 / _a.f; // Inverse flattening
LTP_ENU.a_sq = _a.a * _a.a;
LTP_ENU.b_sq = _a.b * _a.b;
LTP_ENU.e_sq = _a.f * (2.0 - _a.f); // Square of Eccentricity
LTP_ENU.toRadians = Math.PI / 180.0;
LTP_ENU.toDegrees = 180.0 / Math.PI;
class ExtrinsicSupport {
    static IsDerivedCRS(idString) {
        return idString.toLowerCase().includes("conversion[");
    }
    static IsFromAndToCRS(idString) {
        return idString.includes("=>");
    }
    static GetFromAndToCRS(idString, fromCRS, toCRS) {
        fromCRS = "";
        toCRS = "";
        // Split at =>
        let arrowIndex = idString.indexOf("=>");
        if (arrowIndex < 1) {
            return false;
        }
        fromCRS = idString.substring(0, arrowIndex);
        toCRS = idString.substring(arrowIndex + 2);
        return true;
    }
    static GetEPSGNumber(wktString) {
        let epsgNumber = "";
        // look at end of WKT for WKT1 or WKT2 ID
        // "ID\["EPSG",\d+\]\]$" or "AUTHORITY\["EPSG",\"\d+\"\]\]$"
        let reID = /(id\\[\\\"epsg\\\",\\d+\\]\\]$)/;
        let reAuthority = /(authority\\[\\\"epsg\\\",\\\"\\d+\\\"\\]\\]$)/;
        let thisMatch = "";
        let matches = wktString.toLowerCase().match(reID);
        if (matches.length > 0) {
            thisMatch = matches[0];
        }
        //else if ((matches = reAuthority.Matches(wktString.ToLower())).Count > 0) {
        else if ((matches = wktString.toLowerCase().match(reAuthority)).length > 0) {
            thisMatch = matches[0];
        }
        if (thisMatch != "") {
            let reNumber = /\\d+/;
            matches = thisMatch.match(reNumber);
            if (matches.length > 0) {
                epsgNumber = matches[0];
            }
        }
        return epsgNumber;
    }
    //export static GetOriginParameters(wktString: string, out origin): boolean {
    //    // PARAMETER[\"Latitude of topocentric origin\",55,
    //    origin[0] = ExtrinsicSupport.GetSignedDoubleInRe("parameter\\[\\\"latitude.+,-?\\d+\\.?\\d*", wktString);
    //    origin[1] = ExtrinsicSupport.GetSignedDoubleInRe("parameter\\[\\\"longitude.+,-?\\d+\\.?\\d*", wktString);
    //    origin[2] = ExtrinsicSupport.GetSignedDoubleInRe("parameter\\[\\\"[^\\]]*height.+,-?\\d+\\.?\\d*", wktString);
    //    return (!Number.isNaN(origin[0]) && !Number.isNaN(origin[1]) && !Number.isNaN(origin[2]));
    //}
    static GetOriginParameters(wktString) {
        // PARAMETER[\"Latitude of topocentric origin\",55,
        let lat = ExtrinsicSupport.GetSignedDoubleInRe("parameter\\[\\\"latitude.+,-?\\d+\\.?\\d*", wktString);
        let lon = ExtrinsicSupport.GetSignedDoubleInRe("parameter\\[\\\"longitude.+,-?\\d+\\.?\\d*", wktString);
        let h = ExtrinsicSupport.GetSignedDoubleInRe("parameter\\[\\\"[^\\]]*height.+,-?\\d+\\.?\\d*", wktString);
        if (!Number.isNaN(lat) && !Number.isNaN(lon) && !Number.isNaN(h)) {
            return new Position.GeodeticPosition(lat, lon, h);
        }
        return new Position.GeodeticPosition(Number.NaN, Number.NaN, Number.NaN);
    }
    static GetSignedDoubleInRe(reString, inputString) {
        let result = Number.NaN;
        let re = new RegExp(reString);
        let matches = inputString.toLowerCase().match(re);
        if (matches.length > 0) {
            let thisMatch = matches[0];
            re = new RegExp("-?\\d+\\.?\\d*");
            matches = thisMatch.match(re);
            if (matches.length > 0) {
                result = Number.parseFloat(matches[0]);
            }
        }
        return result;
    }
    static GetPositionFromParameters(paramString) {
        // JSON encoded: {"lat": 12.345, "lon": -22.54, "h": 11.22}
        let lat = ExtrinsicSupport.GetSignedDoubleInRe("\\\"lat\\\"\\s*:\\s*-?\\d+(\\.\\d*)?", paramString);
        let lon = ExtrinsicSupport.GetSignedDoubleInRe("\\\"lon\\\"\\s*:\\s*-?\\d+(\\.\\d*)?", paramString);
        let h = ExtrinsicSupport.GetSignedDoubleInRe("\\\"h\\\"\\s*:\\s*-?\\d+(\\.\\d*)?", paramString);
        if (!Number.isNaN(lat) && !Number.isNaN(lon) && !Number.isNaN(h)) {
            return new Position.GeodeticPosition(lat, lon, h);
        }
        return new Position.GeodeticPosition(Number.NaN, Number.NaN, Number.NaN);
    }
}
exports.ExtrinsicSupport = ExtrinsicSupport;
//# sourceMappingURL=WGS84ToLTPENU.js.map