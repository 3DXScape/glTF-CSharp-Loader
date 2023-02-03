// Implemention order: 2 - follows Extras.
// These classes define positions in a 3D frame using different conventions.

/// <summary>
/// The abstract root of the Position hierarchy.
/// <note>
/// Because these various ways to express Position share no underlying structure,
/// the abstract root class definition is simply an empty shell.
/// </note>
/// </summary>
export abstract class Position {
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

