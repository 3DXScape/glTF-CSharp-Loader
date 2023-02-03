// This is an implementation of EPSG method 9837 using sections 4.1.1 and 4.1.2 of https://www.iogp.org/wp-content/uploads/2019/09/373-07-02.pdf.

/// <summary>
/// The abstract root of the Position hierarchy.
/// <note>
/// Because the various ways to express Position share no underlying structure,
/// the abstract root class definition is simply an empty shell.
/// </note>
/// </summary>
abstract class Position {
}

/// <summary>
/// GeodeticPosition is a specialization of Position for using two angles and a height for geodetic reference systems.
/// </summary>
export class GeodeticPosition extends Position {
    public constructor(lat: number, lon: number, h: number) {
        super();
        this.lat = lat;
        this.lon = lon;
        this.h = h;
    }

    /// <summary>
    /// A latitude in degrees, positive north of equator and negative south of equator.
    /// The latitude is the angle between the plane of the equator and a plane tangent to the ellipsoid at the given point.
    /// </summary>
    public lat: number;
    /// <summary>
    /// A longitude in degrees, positive east of the prime meridian and negative west of prime meridian.
    /// </summary>
    public lon: number;
    /// <summary>
    /// A distance in meters, measured with respect to an implied (Basic) or specified (Advanced) reference surface,
    /// postive opposite the direction of the force of gravity,
    /// and negative in the direction of the force of gravity.
    /// </summary>
    public h: number
}
/// <summary>
/// CartesianPosition is a specialization of Position for geocentric, topocentric, and engineering reference systems.
/// </summary>
export class CartesianPosition extends Position {
    public constructor(x: number, y: number, z: number) {
        super();
        this.x = x;
        this.y = y;
        this.z = z;
    }

    /// <summary>
    /// A coordinate value in meters, along an axis (x-axis) that typically has origin at
    /// the center of mass, lies in the same plane as the y axis, and perpendicular to the y axis,
    /// forming a right-hand coordinate system with the z-axis in the up direction.
    /// </summary>
    public x: number;
    /// <summary>
    /// A coordinate value in meters, along an axis (y-axis) that typically has origin at
    /// the center of mass, lies in the same plane as the x axis, and perpendicular to the x axis,
    /// forming a right-hand coordinate system with the z-axis in the up direction.
    /// </summary>
    public y: number;
    /// <summary>
    /// A coordinate value in meters, along the z-axis.
    /// </summary>
    public z: number;
}

export class NoPosition extends Position {
    public constructor() {
        super();
        this.x = this.y = this.z = NaN;
    }
    /// <summary>
    /// A coordinate value in meters, along an axis (x-axis) that typically has origin at
    /// the center of mass, lies in the same plane as the y axis, and perpendicular to the y axis,
    /// forming a right-hand coordinate system with the z-axis in the up direction.
    /// </summary>
    public x: number;
    /// <summary>
    /// A coordinate value in meters, along an axis (y-axis) that typically has origin at
    /// the center of mass, lies in the same plane as the x axis, and perpendicular to the x axis,
    /// forming a right-hand coordinate system with the z-axis in the up direction.
    /// </summary>
    public y: number;
    /// <summary>
    /// A coordinate value in meters, along the z-axis.
    /// </summary>
    public z: number;
}

export class LTP_ENU {
    // WGS-84 geodetic constants
    const a: number = 6378137.0;         // WGS-84 Earth semimajor axis (m)
    const  b: number = 6356752.314245;     // Derived Earth semiminor axis (m)
    const  f: number = (this.a - this.b) / this.a;           // Ellipsoid Flatness
    const  f_inv: number = 1.0 / this.f;       // Inverse flattening
    const  a_sq: number = this.a * this.a;
    const  b_sq: number = this.b * this.b;
    const  e_sq: number = this.f * (2.0 - this.f);    // Square of Eccentricity
    const  toRadians: number = Math.PI / 180.0;
    const  toDegrees: number = 180.0 / Math.PI;

    // Convert WGS-84 Geodetic point (lat, lon, h) to the 
    // Earth-Centered Earth-Fixed (ECEF) coordinates (x, y, z).
    public GeodeticToEcef(from: GeodeticPosition, to: CartesianPosition): void {
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
    public EcefToGeodetic(from: CartesianPosition, to: GeodeticPosition): void {
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
    public EcefToEnu(from: CartesianPosition, origin: GeodeticPosition, to: CartesianPosition):
        //double x, double y, double z,
        //double lat0, double lon0, double h0,
        //out double xEast, out double yNorth, out double zUp):
        void {
        // Convert to radians in notation consistent with the paper:
        var lambda = origin.lat * this.toRadians;
        var phi = origin.lon * this.toDegrees;
        var s = Math.sin(lambda);
        var N = this.a / Math.sqrt(1.0 - this.e_sq * s * s);

        var sin_lambda = Math.sin(lambda);
        var cos_lambda = Math.cos(lambda);
        var cos_phi = Math.cos(phi);
        var sin_phi = Math.sin(phi);

        var x0: number = (origin.h + N) * cos_lambda * cos_phi;
        var y0: number = (origin.h + N) * cos_lambda * sin_phi;
        var z0: number = (origin.h + (1 - this.e_sq) * N) * sin_lambda;

        var xd: number = from.x - x0;
        var yd: number = from.y - y0;
        var zd: number = from.z - z0;

        // This is the matrix multiplication
        to.x = -sin_phi * xd + cos_phi * yd;
        to.y = -cos_phi * sin_lambda * xd - sin_lambda * sin_phi * yd + cos_lambda * zd;
        to.z = cos_lambda * cos_phi * xd + cos_lambda * sin_phi * yd + sin_lambda * zd;
    }

    // Inverse of EcefToEnu. Converts East-North-Up coordinates (xEast, yNorth, zUp) in a
    // Local Tangent Plane that is centered at the (WGS-84) Geodetic point (lat0, lon0, h0)
    // to the Earth-Centered Earth-Fixed (ECEF) coordinates (x, y, z).
    public EnuToEcef(from: CartesianPosition, origin: GeodeticPosition, to: CartesianPosition): void {
        // Convert to radians in notation consistent with the paper:
        var lambda = origin.lat * this.toRadians;
        var phi    = origin.lon * this.toRadians;
        var s = Math.sin(lambda);
        var N = this.a / Math.sqrt(1.0 - this.e_sq * s * s);

        var sin_lambda = Math.sin(lambda);
        var cos_lambda = Math.cos(lambda);
        var cos_phi = Math.cos(phi);
        var sin_phi = Math.sin(phi);

        var x0: number = (origin.h + N) * cos_lambda * cos_phi;
        var y0: number = (origin.h + N) * cos_lambda * sin_phi;
        var z0: number = (origin.h + (1.0 - this.e_sq) * N) * sin_lambda;

        var xd: number = -sin_phi * from.x - cos_phi * sin_lambda * from.y + cos_lambda * cos_phi * from.z;
        var yd: number = cos_phi * from.x - sin_lambda * sin_phi * from.y + cos_lambda * sin_phi * from.z;
        var zd: number = cos_lambda * from.y + sin_lambda * from.z;

        to.x = xd + x0;
        to.y = yd + y0;
        to.z = zd + z0;
    }

    // Converts the geodetic WGS-84 coordinated (lat, lon, h) to 
    // East-North-Up coordinates in a Local Tangent Plane that is centered at the 
    // (WGS-84) Geodetic point (lat0, lon0, h0).
    public GeodeticToEnu(from: GeodeticPosition, origin: GeodeticPosition, to: CartesianPosition):void
        //double lat0, double lon0, double h0,
    //out double xEast, out double yNorth, out double zUp)
    {
        let ecef = new CartesianPosition(0, 0, 0);
        this.GeodeticToEcef(from, ecef);
        this.EcefToEnu(ecef, origin, to);
    }
    public EnuToGeodetic(from: CartesianPosition, origin: GeodeticPosition, to: GeodeticPosition): void
    //double xEast, double yNorth, double zUp,
    //double lat0, double lon0, double h0,
    //out double lat, out double lon, out double h
    {
        let ecef = new CartesianPosition(0, 0, 0);
        this.EnuToEcef(from, origin, ecef);
        this.EcefToGeodetic(ecef, to);
    }
}