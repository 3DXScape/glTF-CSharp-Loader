using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//-----------------------------------------------------------------------
// <copyright>
// Copyright (c) 2020-2023, The DaniElenga Foundation.
//    Licensed under the MIT License (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//      http://www.opensource.org/licenses/mit-license.php
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// </copyright>
// <author>Steve Smyth</author>
// <website>https://opensiteplan.org</website>
//-----------------------------------------------------------------------

/// <summary>
/// This library is a C# implementation of the three OGC GeoPose 1.0 Basic and Advanced targets
/// with Json serialization of each, conforming to the OGC GeoPose 1.0 standard.
/// <version>
/// Version 0.9.13 
/// </version>
/// <date>
/// 13 December 2022
/// </date>
/// <note>
/// This library was created as a part of the CitySharp encoding of OGC CityGML 3.0.
/// It is used in the prototype encoding of the CityGML semantic model in Khronos glTF 2.0.
/// </note>
/// <note>
/// This version targets the open source and platform-independent .NET 6 Core framework
///  and is an alternative to the GeoPose implementation.
/// This GeoPoseF design differs from the GeoPose design in that it directly follows the template from the standard,
///  where every GeoPose consists of a frame transformation and an orientation.
///  Every transform defines the relationship
///  between an outer and an inner frame and expresses the origin of the inner frame as a
///  distinguished Position. The conceptual advantage of this design is that it follows
///  the conceptual model of the standard exactly, rather than treating the Basic targets
///  as special cases.
///  
///  **There is no practical difference in actual use of this version vs
///  the "GeoPose" namespace version. It makes the derivation of the distinguished point "Position"
///  as the origin of the coordinate system associated with the inner frame explicit.
///  This structure also makes it easy to add new categories of FrameTransfom directly in the inheritance scheme.**
///  
/// </note>
/// </summary>
namespace GeoPoseX
{
    // *******************************************************************************

    /// <summary>
    /// The abstract root of the GeoPose Basic and Advanced classes.
    /// </summary>
    public abstract class GeoPose
    {
        public PoseID? poseID { get; set; } = null;
        public PoseID? parentPoseID { get; set; } = null;
        public UnixTime? validTime { get; set; } = null;
        public abstract FrameTransform? FrameTransform { get; set; }
        public abstract Orientation? Orientation { get; set; }
    }
    /// <summary>
    /// The abstract root of the Position hierarchy.
    /// <note>
    /// Because the various ways to express Position share no underlying structure,
    /// the class definition is simply an empty shell.
    /// </note>
    /// </summary>
    public class UnixTime
    {
        public string timeValue { get; set; } = string.Empty;
    }
    public class PoseID
    {
        public string id { get; set; } = string.Empty;
    }
    public abstract class Basic : GeoPose
    {
        /// <summary>
        /// A Position specified in spherical coordinates with height above a reference surface -
        /// usually an ellipsoid of revolution or a gravitational equipotential surface.
        /// </summary>
        public override FrameTransform FrameTransform { get; set; } = new WGS84ToLTP_ENUTransform();
    }

    /// <summary>
    /// A Basic-YPR GeoPose.
    /// <remark>
    /// See the OGS GeoPose 1.0 standard for a full description.
    /// </remark>
    /// </summary>
    public class BasicYPR : Basic
    {
        /// <summary>
        /// An Orientation specified as three rotations.
        /// </summary>
        public override Orientation Orientation { get; set; } = new YPRAngles();
        /// <summary>
        /// This function returns a Json encoding of a Basic-YPR GeoPose
        /// </summary>
        public string ToJSON(string indent = "")
        {
            StringBuilder sb = new StringBuilder();
            if (FrameTransform != null && Orientation != null)
            {
                sb.Append("{\r\n\t\t" + indent);
                sb.Append("\"position\": {\r\n\t\t\t" + indent + "\"lat\": " + ((WGS84ToLTP_ENUTransform)FrameTransform).Position.lat + ",\r\n\t\t\t" + indent +
                    "\"lon\": " + ((WGS84ToLTP_ENUTransform)FrameTransform).Position.lon + ",\r\n\t\t\t" + indent +
                    "\"h\":   " + ((WGS84ToLTP_ENUTransform)FrameTransform).Position.h);
                sb.Append("\r\n\t\t" + indent + "},");
                sb.Append("\r\n\t\t" + indent);
                sb.Append("\"angles\": {\r\n\t\t\t" + indent + "\"yaw\":   " +((YPRAngles)Orientation).yaw + ",\r\n\t\t\t" + indent +
                    "\"pitch\": " + ((YPRAngles)Orientation).pitch + ",\r\n\t\t\t" + indent +
                    "\"roll\":  " + ((YPRAngles)Orientation).roll);
                sb.Append("\r\n\t\t" + indent + "}");
                sb.Append("\r\n\t" + indent + "}");
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// A Basic-Quaternion GeoPose.
    /// <remark>
    /// See the OGS GeoPose 1.0 standard for a full description.
    /// </remark>
    /// </summary>
    public class BasicQuaternion : Basic
    {
 /*       /// <summary>
        /// A WGS84 (EPSG 4327) Position specified in spherical coordinates with height above the WGS84 ellipsoid.
        /// </summary>
        public WGS84ToLTP_ENUTransform WGS84ToLTP_ENU { get; set; } = new WGS84ToLTP_ENUTransform(); */
        /// <summary>
        /// An Orientation specified as a unit quaternion.
        /// </summary>
        public override Orientation Orientation { get; set; } = new Quaternion();
        /// <summary>
        /// This function returns a Json encoding of a Basic-Quaternion GeoPose
        /// </summary>
        public string ToJSON(string indent = "")
        {
            StringBuilder sb = new StringBuilder();
            if (((WGS84ToLTP_ENUTransform)FrameTransform).Position != null && Orientation != null)
            {
                sb.Append("{\r\n\t\t" + indent);
                sb.Append("\"position\": {\r\n\t\t\t" + indent + "\"lat\": " +
                    ((WGS84ToLTP_ENUTransform)FrameTransform).Position.lat + ",\r\n\t\t\t" + indent +
                    "\"lon\": " + ((WGS84ToLTP_ENUTransform)FrameTransform).Position.lon +
                    ",\r\n\t\t\t" + indent +
                    "\"h\":   " + ((WGS84ToLTP_ENUTransform)FrameTransform).Position.h);
                sb.Append("\r\n\t\t" + indent + "},");
                sb.Append("\r\n\t\t" + indent);
                sb.Append("\"angles\": {\r\n\t\t\t" + indent + "\"x\":   " + ((Quaternion)Orientation).x + ",\r\n\t\t\t" + indent +
                    "\"y\": " + ((Quaternion)Orientation).y + ",\r\n\t\t\t" + indent +
                    "\"z\": " + ((Quaternion)Orientation).z + ",\r\n\t\t\t" + indent +
                    "\"w\":  " + ((Quaternion)Orientation).w);
                sb.Append("\r\n\t\t" + indent + "}");
                sb.Append("\r\n\t" + indent + "}");
            }
            return sb.ToString();
        }
    }
    /// <summary>
    /// A derived pose within an engineering CRS with a Cartesian coordinate system.
    /// This form is the closest to the classical computer graphics pose concept.
    /// <remark>
    /// Not (yet) part of the OGC GeoPose standard and not backwards-compatible.
    /// Useful when operating within a local Cartesian frame defined by a Basic (or other) GeoPose.
    /// </remark>
    /// </summary>
    public class Local : GeoPose
    {
        /// <summary>
        /// The xOffset, yOffset, zOffset from the origin of the rotated inner frame of a "parent" GeoPose.
        /// </summary>
        public override FrameTransform FrameTransform { get; set; } = new Translation();
        /// <summary>
        /// The yaw, pitch, roll orientation.
        /// </summary>
        public override Orientation Orientation { get; set; } = new YPRAngles();
        /// <summary>
        /// This function returns a Json encoding of a Basic-YPR GeoPose
        /// </summary>
        public string ToJSON(string indent = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{\r\n\t\t" + indent);
            sb.Append("\"position\": {\r\n\t\t\t" + indent + "\"east\": " + ((Translation)FrameTransform).xOffset + ",\r\n\t\t\t" + indent +
                "\"north\": " + ((Translation)FrameTransform).yOffset + ",\r\n\t\t\t" + indent +
                "\"up\":   " + ((Translation)FrameTransform).zOffset);
            sb.Append("\r\n\t\t" + indent + "},");
            sb.Append("\r\n\t\t" + indent);
            sb.Append("\"angles\": {\r\n\t\t\t" + indent + "\"yaw\":   " + ((YPRAngles)Orientation).yaw + ",\r\n\t\t\t" + indent +
                "\"pitch\": " + ((YPRAngles)Orientation).pitch + ",\r\n\t\t\t" + indent +
                "\"roll\":  " + ((YPRAngles)Orientation).roll);
            sb.Append("\r\n\t\t" + indent + "}");
            sb.Append("\r\n\t" + indent + "}");
            return sb.ToString();
        }
    }


    /// <summary>
    /// Advanced GeoPose.
    /// </summary>
    public class Advanced : GeoPose
    {
        /// <summary>
        /// A Frame Specification defining a frame with associated coordinate system whose Position is the origin.
        /// </summary>
        public override FrameTransform FrameTransform { get; set; } = new Extrinsic();
        /// <summary>
        /// An Orientation specified as a unit quaternion.
        /// </summary>
        public override Orientation Orientation { get; set; } = new Quaternion();
        /// <summary>
        /// Milliseconds of Unix time ticks (optional).
        /// </summary>
        //public long? ValidTime { get; set; } = 0;
        /// <summary>
        /// This function returns a Json encoding of an Advanced GeoPose
        /// </summary>
        public string ToJSON()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"frameSpecification\":{\"authority\":" +
                ((Extrinsic)FrameTransform).authority + ",\"id\":" +
                ((Extrinsic)FrameTransform).id + ",\"parameters\":" +
                ((Extrinsic)FrameTransform).parameters + "},");
            sb.Append("\"quaternion\":{\"x\":" + ((Quaternion)Orientation).x + ",\"y\":" +
                ((Quaternion)Orientation).y + ",\"z\":" +
                ((Quaternion)Orientation).z + ",\"w\":" +
                ((Quaternion)Orientation).w);
            sb.Append("}}");
            return sb.ToString();
        }
    }

    // *******************************************************************************

    public abstract class Position
    {

    }
    /// <summary>
    /// The abstract root of the Orientation hierarchy.
    /// <note>
    /// Because the various ways to express Orientation share no underlying structure,
    /// the class definition is simply an empty shell.
    /// </note>
    /// </summary>
    public abstract class Orientation
    {

    }
    /// <summary>
    /// A specialization of Position for using two angles and a height for geodetic positions.
    /// </summary>
    public class GeodeticPosition : Position
    {
        /// <summary>
        /// A latitude in degrees, positive north of equator and negative south of equator.
        /// The latitude is the angle between the plane of the equator and a plane tangent to the ellipsoid at the given point.
        /// </summary>
        public double lat { get; set; } = double.NaN;
        /// <summary>
        /// A longitude in degrees, positive east of the prime meridian and negative west of prime meridian.
        /// </summary>
        public double lon { get; set; } = double.NaN;
        /// <summary>
        /// A distance in meters, measured with respect to an implied (Basic) or specified (Advanced) reference surface,
        /// postive opposite the direction of the force of gravity,
        /// and negative in the direction of the force of gravity.
        /// </summary>
        public double h { get; set; } = double.NaN;
    }
    /// <summary>
    /// A specialization of Position for geocentric positions.
    /// </summary>
    public class GeocentricPosition : Position
    {
        /// <summary>
        /// A coordinate value in meters, along an axis (x-axis) that typically has origin at
        /// the center of mass, lies in the same plane as the y axis, and perpendicular to the y axis,
        /// forming a right-hand coordinate system with the z-axis in the up direction.
        /// </summary>
        public double x { get; set; } = double.NaN;
        /// <summary>
        /// A coordinate value in meters, along an axis (y-axis) that typically has origin at
        /// the center of mass, lies in the same plane as the x axis, and perpendicular to the x axis,
        /// forming a right-hand coordinate system with the z-axis in the up direction.
        /// </summary>
        public double y { get; set; } = double.NaN;
        /// <summary>
        /// A coordinate value in meters, along the z-axis.
        /// </summary>
        public double z { get; set; } = double.NaN;
    }
    /// <summary>
    /// A specialization of Position for using two angles and a height for geodetic positions.
    /// </summary>
    /*public class CartesianPosition : Position
    {
        /// <summary>
        /// A coordinate value in meters, along an axis (east-axis) that has origin at
        /// a point tangent to an ellisoid defined by a geodetic frame,
        /// is perpendicular to the north axis,
        /// forms a right-hand coordinate system with the north and up axes, and
        /// which lies in a plane tangent to the ellisoid at that point.
        /// </summary>
        public double east { get; set; } = double.NaN;
        /// <summary>
        /// A coordinate value in meters, along an axis (north-axis) that has origin at
        /// a point tangent to an ellisoid defined by a geodetic frame,
        /// is perpendicular to the east axis,
        /// forms a right-hand coordinate system with the east and up axes, and
        /// which lies in a plane tangent to the ellisoid at that point.
        /// </summary>
        public double north { get; set; } = double.NaN;
        /// <summary>
        /// A coordinate value in meters, measured perpendicular to the tangent plane,
        /// positive in the up direction.
        /// </summary>
        public double up { get; set; } = double.NaN;
    }*/
    /// <summary>
    /// A specialization of Orientation using Yaw, Pitch, and Roll angles in degrees.
    /// <remark>
    /// This style of Orientation is best for easy human interpretation.
    /// It suffers from some computational inefficiencies, awkward interpolation, and singularities.
    /// </remark>
    /// </summary>
    public class YPRAngles : Orientation
    {
        /// <summary>
        /// A left-right angle in degrees.
        /// </summary>
        public double yaw { get; set; } = double.NaN;
        /// <summary>
        /// An up-down angle in degrees.
        /// </summary>
        public double pitch { get; set; } = double.NaN;
        /// <summary>
        /// A side-to-side angle in degrees.
        /// </summary>
        public double roll { get; set; } = double.NaN;
    }
    /// <summary>
    /// A specialization of Orientation using a unit quaternion.
    /// </summary>
    /// <remark>
    /// This style of Orientation is best for computation.
    /// It is not easily interpreted or visualized by humans.
    /// </remark>
    public class Quaternion : Orientation
    {
        /// <summary>
        /// The x component.
        /// </summary>
        public double x { get; set; } = double.NaN;
        /// <summary>
        /// The y component.
        /// </summary>
        public double y { get; set; } = double.NaN;
        /// <summary>
        /// The z component.
        /// </summary>
        public double z { get; set; } = double.NaN;
        /// <summary>
        /// The w component.
        /// </summary>
        public double w { get; set; } = double.NaN;
    }
    /// <summary>
    /// A FrameTransform is a generic container for information that defines mapping between reference frames.
    /// <remark>
    /// </remark>
    /// </summary>
    public abstract class FrameTransform
    {

    }

    /// <summary>
    /// A FrameSpecification is a generic container for information that defines a reference frame.
    /// <remark>
    /// A FrameSpecification can be abstracted as a Position:
    /// The origin of the coordinate system associated with the frame is a Position and serves in that role
    /// in the Advanced GeoPose.
    /// The origin, is in fact the *only* distinguished Position associated with the coodinate system.
    /// </remark>
    /// </summary>
    public class Extrinsic : FrameTransform
    {
        /// <summary>
        /// The name or identification of the definer of the category of frame specification.
        /// A Uri that usually but not always points to a valid web address.
        /// </summary>
        public string authority { get; set; } = "";
        /// <summary>
        /// A string that uniquely identifies a frame type.
        /// The interpretation of the string is determined by the authority.
        /// </summary>
        public string id { get; set; } = "";
        /// <summary>
        /// A string that holds any parameters required by the authority to define a frame of the given type as specified by the id.
        /// The interpretation of the string is determined by the authority.
        /// </summary>
        public string parameters { get; set; } = "";
    }
    /// <summary>
    /// A specialized specification of the WGS84 (EPSG 4326) geodetic frame to a local tangent plane East, North, Up frame.
    /// <remark>
    /// The origin of the coordinate system associated with the frame is a Position - the origin -
    /// which is the *only* distinguished Position associated with the coodinate system associated with the inner frame (range).
    /// </remark>
    /// </summary>
    public class WGS84ToLTP_ENUTransform : FrameTransform
    {
        /// <summary>
        /// A single geodetic position defines the tangent point for a transform to LTP-ENU.
        /// </summary>
        public GeodeticPosition Position { get; set; } = null;
    }

    public class Translation : FrameTransform
    {
        public double xOffset { get; set; } = 0.0;
        public double yOffset { get; set; } = 0.0;
        public double zOffset { get; set; } = 0.0;
    }
}
