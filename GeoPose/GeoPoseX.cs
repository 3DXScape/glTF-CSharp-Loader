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
/// Version 0.9.23 
/// </version>
/// <date>
/// 18 January 2023
/// </date>
/// <note>
/// This library was created as a part of the CitySharp encoding of OGC CityGML 3.0.
/// It is used in the prototype encoding of the CityGML semantic model in Khronos glTF 2.0.
/// </note>
/// <note>
/// This version targets the open source and platform-independent .NET 6 Core framework
///  and is an alternative to the GeoPose implementation.
/// This GeoPoseF2 design differs from the GeoPose design in that it directly follows the template from the standard,
///  where every GeoPose consists of a frame transformation and an orientation.
///  Every transform defines the relationship
///  between an outer and an inner frame and expresses the origin of the inner frame as a
///  distinguished Position. The conceptual advantage of this design is that it follows
///  the conceptual model of the standard exactly, rather than treating the Basic targets
///  as special cases.
///  
///  There is no practical difference in actual use of this version vs
///  the "GeoPose" namespace version.
///  It makes the derivation of the distinguished point "Position"
///  as the origin of the coordinate system associated with the inner frame explicit.
///  This structure also makes it easy to add new categories of FrameTransfom and Orientation
///  directly in the inheritance scheme.
///
///
///  <note>
///  
///  **WARNING**WARNING**WARNING**WARNING**WARNING**WARNING**WARNING**WARNING**
///  
///  The *Local* GeoPose is NOT defined in the OGC GeoPose 1.0 standard.
///  It *may* appear in a future version of the standard.
///  *Local* is shorthand for a local CRS with
///  a Cartesian 3- coordinate system.
///  Every Local GeoPose can be written as an Advanced GeoPose with a suitable
///  authority, id, parameters, and quaterion orientation.
///  
///  **WARNING**WARNING**WARNING**WARNING**WARNING**WARNING**WARNING**WARNING**
///  
/// </note>
///
/// 
/// </summary>
namespace GeoPoseX
{
    // *******************************************************************************

    /// <summary>
    /// The abstract root of the GeoPose Basic and Advanced classes.
    /// </summary>
    public abstract class GeoPose
    {
        // Optional and non-standard but conforming added property:
        //   an identifier unique within an application.
        public PoseID? poseID { get; set; } = null;
        // Optional and non-standard but conforming added property:
        //  a PoseID type identifier of another GeoPose in the direction of the root of a pose tree.
        public PoseID? parentPoseID { get; set; } = null;
        // Optional and non-standard but conforming added property:
        //   a poseID identifier in the direction of the root of a pose tree.
        public UnixTime? validTime { get; set; } = null;
        public abstract FrameTransform FrameTransform { get; set; }
        public abstract Orientation Orientation { get; set; }
        public abstract string ToJSON(string indent);
    }
    public class FloatTriple
    {
        internal FloatTriple()
        {

        }
        public FloatTriple(double[] v)
        {
            this.v[0] = (float)v[0];
            this.v[1] = (float)v[1];
            this.v[2] = (float)v[2];
        }
        public FloatTriple(float[] v)
        {
            this.v[0] = v[0];
            this.v[1] = v[1];
            this.v[2] = v[2];
        }
        public float[] v { get; set; } = new float[3] { 0.0f, 0.0f, 0.0f };
        public DoubleTriple ToDoubleTriple()
        {
            DoubleTriple vc3d = new DoubleTriple(this.v);
            return vc3d;
        }
    }
    public class DoubleTriple
    {
        internal DoubleTriple()
        {

        }
        public DoubleTriple(float[] v)
        {
            this.v[0] = (double)v[0];
            this.v[1] = (double)v[1];
            this.v[2] = (double)v[2];
        }
        public DoubleTriple(double[] v)
        {
            this.v = v;
        }
        public FloatTriple ToFloatTriple()
        {
            FloatTriple vc3f = new FloatTriple(this.v);
            return vc3f;
        }


        public double[] v { get; set; } = new double[3] { 0.0, 0.0, 0.0 };
    }
    public abstract class Basic : GeoPose
    {
        /// <summary>
        /// A Position specified in spherical coordinates with height above a reference surface -
        /// usually an ellipsoid of revolution or a gravitational equipotential surface.
        /// </summary>
        private WGS84ToLTP_ENU _frameTransform = new WGS84ToLTP_ENU();
        public override FrameTransform FrameTransform
        {
            get
            {
                return _frameTransform;
            }
            set
            {
                if (value.GetType() == typeof(WGS84ToLTP_ENU))
                {
                    _frameTransform = (WGS84ToLTP_ENU)value;
                }
            }
        }
    }

    /// <summary>
    /// A Basic-YPR GeoPose.
    /// <remark>
    /// See the OGS GeoPose 1.0 standard for a full description.
    /// </remark>
    /// </summary>
    public class BasicYPR : Basic
    {
        internal BasicYPR()
        {

        }
        public BasicYPR(string id, GeodeticPosition tangentPoint, YPRAngles yprAngles )
        {

            this.poseID = new PoseID(id);
            this.FrameTransform = new WGS84ToLTP_ENU(tangentPoint);
            this.Orientation = yprAngles;
        }
        private YPRAngles _orientation = new YPRAngles();
        /// <summary>
        /// An Orientation specified as three rotations.
        /// </summary>
        public override Orientation Orientation
        {
            get
            {
                return _orientation;
            }
            set
            {
                if (value.GetType() == typeof(YPRAngles))
                {
                    _orientation = (YPRAngles)value;
                }
            }
        }
        /// <summary>
        /// Return a string containing Json encoding of a Basic-YPR GeoPose
        /// </summary>
        public override string ToJSON(string indent = "")
        {
            StringBuilder sb = new StringBuilder();
            if (FrameTransform != null && Orientation != null)
            {
                sb.Append("{\r\n\t\t" + indent);
                if (validTime != null && validTime.timeValue != String.Empty)
                {
                    sb.Append("\"validTime\": " + validTime.timeValue + ",\r\n" + indent + "  ");
                }
                if (poseID != null && poseID.id != String.Empty)
                {
                    sb.Append("\"poseID\": \"" + poseID.id + "\",\r\n" + indent + "  ");
                }
                if (parentPoseID != null && parentPoseID.id != String.Empty)
                {
                    sb.Append("\"parentPoseID\": \"" + parentPoseID.id + "\",\r\n" + indent + "  ");
                }
                sb.Append("\"position\": {\r\n\t\t\t" + indent + "\"lat\": " + ((WGS84ToLTP_ENU)FrameTransform).Origin.lat + ",\r\n\t\t\t" + indent +
                    "\"lon\": " + ((WGS84ToLTP_ENU)FrameTransform).Origin.lon + ",\r\n\t\t\t" + indent +
                    "\"h\":   " + ((WGS84ToLTP_ENU)FrameTransform).Origin.h);
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
        internal BasicQuaternion()
        {
        }
        public BasicQuaternion(string id, GeodeticPosition tangentPoint, Quaternion quaternion)
        {
            this.poseID = new PoseID(id);
            this.FrameTransform = new WGS84ToLTP_ENU(tangentPoint);
            this.Orientation = quaternion;
        }
        /// <summary>
        /// An Orientation specified as a unit quaternion.
        /// </summary>
        private Quaternion _orientation = new Quaternion();
        public override Orientation Orientation
        {
            get
            {
                return _orientation;
            }
            set
            {
                if (value.GetType() == typeof(Quaternion))
                {
                    _orientation = (Quaternion)value;
                }
            }
        }
        /// <summary>
        /// This function returns a Json encoding of a Basic-Quaternion GeoPose
        /// </summary>
        public override string ToJSON(string indent = "")
        {
            StringBuilder sb = new StringBuilder();
            if (((WGS84ToLTP_ENU)FrameTransform).Origin != null && Orientation != null)
            {
                sb.Append("{\r\n\t\t" + indent);
                if (validTime != null && validTime.timeValue != String.Empty)
                {
                    sb.Append("\"validTime\": " + validTime.timeValue + ",\r\n" + indent + "  ");
                }
                if (poseID != null && poseID.id != String.Empty)
                {
                    sb.Append("\"poseID\": \"" + poseID.id + "\",\r\n" + indent + "  ");
                }
                if (parentPoseID != null && parentPoseID.id != String.Empty)
                {
                    sb.Append("\"parentPoseID\": \"" + parentPoseID.id + "\",\r\n" + indent + "  ");
                }
                sb.Append("\"position\": {\r\n\t\t\t" + indent + "\"lat\": " +
                    ((WGS84ToLTP_ENU)FrameTransform).Origin.lat + ",\r\n\t\t\t" + indent +
                    "\"lon\": " + ((WGS84ToLTP_ENU)FrameTransform).Origin.lon +
                    ",\r\n\t\t\t" + indent +
                    "\"h\":   " + ((WGS84ToLTP_ENU)FrameTransform).Origin.h);
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
        public Local(string id, Translation frameTransform, Orientation quaternion)
        {
            this.poseID = new PoseID(id);
            this.FrameTransform = frameTransform;
            this.Orientation = quaternion;
        }
        /// <summary>
        /// The xOffset, yOffset, zOffset from the origin of the rotated inner frame of a "parent" GeoPose.
        /// </summary>
        public override FrameTransform FrameTransform { get; set; } = new Translation();
        /// <summary>
        /// The yaw, pitch, roll orientation.
        /// </summary>
        private YPRAngles _orientation = new YPRAngles();
        /// <summary>
        /// An Orientation specified as three rotations.
        /// </summary>
        public override Orientation Orientation
        {
            get
            {
                return _orientation;
            }
            set
            {
                if (value.GetType() == typeof(YPRAngles))
                {
                    _orientation = (YPRAngles)value;
                }
            }
        }
        /// <summary>
        /// This function returns a Json encoding of a Basic-YPR GeoPose
        /// </summary>
        public override string ToJSON(string indent = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{\r\n  ");
            if (validTime != null && validTime.timeValue != String.Empty)
            {
                sb.Append("\"validTime\": " + validTime.timeValue + ",\r\n" + indent + "  ");
            }
            if (poseID != null && poseID.id != String.Empty)
            {
                sb.Append("\"poseID\": \"" + poseID.id + "\",\r\n" + indent + "  ");
            }
            if (parentPoseID != null && parentPoseID.id != String.Empty)
            {
                sb.Append("\"parentPoseID\": \"" + parentPoseID.id + "\",\r\n" + indent + "  ");
            }
            sb.Append("\"position\": \r\n  {\r\n    " + "\"x\": " + ((Translation)FrameTransform).xOffset + ",\r\n    " +
                "\"y\": " + ((Translation)FrameTransform).yOffset + ",\r\n    " +
                "\"z\":   " + ((Translation)FrameTransform).zOffset);
            sb.Append("\r\n  " + "},");
            sb.Append("\r\n  ");
            sb.Append("\"angles\": \r\n  {\r\n    " + "\"yaw\":   " + ((YPRAngles)Orientation).yaw + ",\r\n    "  +
                "\"pitch\": " + ((YPRAngles)Orientation).pitch + ",\r\n    " +
                "\"roll\":  " + ((YPRAngles)Orientation).roll);
            sb.Append("\r\n  "  + "}");
            sb.Append("\r\n" + "}\r\n");
            return sb.ToString();
        }
    }


    /// <summary>
    /// Advanced GeoPose.
    /// </summary>
    public class Advanced : GeoPose
    {
        internal Advanced()
        {

        }
        public Advanced(PoseID poseID, Extrinsic frameTransform, Quaternion orientation)
        {
            this.poseID = poseID;
            FrameTransform = frameTransform;
            Orientation = orientation;
        }

        /// <summary>
        /// A Frame Specification defining a frame with associated coordinate system whose Position is the origin.
        /// </summary>
        public override FrameTransform FrameTransform { get; set; } = new Extrinsic();
        /// <summary>
        /// An Orientation specified as a unit quaternion.
        /// </summary>
        private Quaternion _orientation = new Quaternion();
        public override Orientation Orientation
        {
            get
            {
                return _orientation;
            }
            set
            {
                if (value.GetType() == typeof(Quaternion))
                {
                    _orientation = (Quaternion)value;
                }
            }
        }
        /// <summary>
        /// Milliseconds of Unix time ticks (optional).
        /// </summary>
        //public long? ValidTime { get; set; } = 0;
        /// <summary>
        /// This function returns a Json encoding of an Advanced GeoPose
        /// </summary>
        public override string ToJSON(string indent)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{\r\n" + indent + "  ");
            if (validTime != null && validTime.timeValue != String.Empty)
            {
                sb.Append("\"validTime\": " + validTime.timeValue + ",\r\n" + indent + "  ");
            }
            if (poseID != null && poseID.id != String.Empty)
            {
                sb.Append("\"poseID\": \"" + poseID.id + "\",\r\n" + indent + "  ");
            }
            if (parentPoseID != null && parentPoseID.id != String.Empty)
            {
                sb.Append("\"parentPoseID\": \"" + parentPoseID.id + "\",\r\n" + indent + "  ");
            }
            sb.Append("\"frameSpecification\":\r\n" + indent + "  " + "{\r\n" + indent + "    \"authority\": \"" +
                ((Extrinsic)FrameTransform).authority.Replace("\"","\\\"") + "\",\r\n" + indent + "    \"id\": \"" +
                ((Extrinsic)FrameTransform).id.Replace("\"", "\\\"") + "\",\r\n" + indent + "    \"parameters\": \"" +
                ((Extrinsic)FrameTransform).parameters.Replace("\"", "\\\"") + "\"\r\n" + indent + "  },\r\n" + indent + "  ");
            sb.Append("\"quaternion\":\r\n" + indent + "  {\r\n" + indent + "    \"x\":" + ((Quaternion)Orientation).x + ",\"y\":" +
                ((Quaternion)Orientation).y + ",\"z\":" +
                ((Quaternion)Orientation).z + ",\"w\":" +
                ((Quaternion)Orientation).w);
            sb.Append("\r\n" + indent + "  }\r\n" + indent + "}\r\n");
            return sb.ToString();
        }
    }

    // *******************************************************************************
    public class UnixTime
    {
        internal UnixTime()
        {

        }
        // Constructor from long integer count of UNIX Time seconds x 1000
        public UnixTime(long longTime)
        {
            timeValue = longTime.ToString();
        }
        public string timeValue { get; set; } = string.Empty;
    }
    public class PoseID
    {
        internal PoseID()
        {

        }
        public PoseID(string id)
        {
            this.id = id;
        }
        public string id { get; set; } = string.Empty;
    }

    /// <summary>
    /// A specialization of Position for using two angles and a height for geodetic positions.
    /// </summary>
    public class GeodeticPosition : Position
    {
        internal GeodeticPosition()
        {

        }
        public GeodeticPosition(double lat, double lon, double h)
        {
            this.lat = lat;
            this.lon = lon;
            this.h = h;
        }

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
    public class CartesianPosition : Position
    {
        public CartesianPosition(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    
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
    public class NoPosition : Position
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
    /// The abstract root of the Position hierarchy.
    /// <note>
    /// Because the various ways to express Position share no underlying structure,
    /// the class definition is simply an empty shell.
    /// </note>
    /// </summary>
    public abstract class Position
    {

    }
    /// <summary>
    /// The abstract root of the Orientation hierarchy.
    /// <note>
    /// An Orientation is a generic container for information that defines rotation within a coordinate system associated with a reference frame.
    /// An Orientation may have a specialized context with necessary ancillary information
    /// that parameterizes the rotation.
    /// Such context may include, for example, part of the information that may be conveyed in an ISO 19111 CRS specification
    /// or a proprietary naming, numbering, or modelling scheme as used by EPSG, NASA Spice, or SEDRIS SRM.
    /// Subclasses of Orientation exist precisely to hold this context in conjunction with code
    /// implementing a Rotate function.
    /// </note>
    /// </summary>
    public abstract class Orientation
    {
        public virtual Position Rotate(Position point)
        {
            return point;
        }
    }
    /// <summary>
    /// A specialization of Orientation using Yaw, Pitch, and Roll angles in degrees.
    /// <remark>
    /// This style of Orientation is best for easy human interpretation.
    /// It suffers from some computational inefficiencies, awkward interpolation, and singularities.
    /// </remark>
    /// </summary>
    public class YPRAngles : Orientation
    {
        // Prevent public use of empty constructor.
        internal YPRAngles()
        {

        }
        public YPRAngles(double yaw, double pitch, double roll)
        {
            this.yaw = yaw;
            this.pitch = pitch;
            this.roll = roll;
        }
        public override Position Rotate(Position point)
        {
            // convert to quaternion and use quaternion rotation
            Quaternion q = YPRAngles.ToQuaternion(this.yaw, this.pitch, this.roll);
            return Quaternion.Transform(point, q);
        }
        public static Quaternion ToQuaternion(double yaw, double pitch, double roll) 
        {
            // GeoPose uses angles in degrees for human readability
            // /Convert to radians.
            yaw   *= (Math.PI / 180.0);
            pitch *= (Math.PI / 180.0);
            roll  *= (Math.PI / 180.0);

            double cosRoll = Math.Cos(roll * 0.5);
            double sinRoll = Math.Sin(roll * 0.5);
            double cosPitch = Math.Cos(pitch * 0.5);
            double sinPitch = Math.Sin(pitch * 0.5);
            double cosYaw = Math.Cos(yaw * 0.5);
            double sinYaw = Math.Sin(yaw * 0.5);

            double w = cosRoll * cosPitch * cosYaw + sinRoll * sinPitch * sinYaw;
            double x = sinRoll * cosPitch * cosYaw - cosRoll * sinPitch * sinYaw;
            double y = cosRoll * sinPitch * cosYaw + sinRoll * cosPitch * sinYaw;
            double z = cosRoll * cosPitch * sinYaw - sinRoll * sinPitch * cosYaw;

            double norm = Math.Sqrt(x*x + y*y +z*z +w*w);
            if(norm <= double.MinValue)
            {
                return new Quaternion(x, y, z, w);
            }
            return new Quaternion(x/norm, y/norm, z/norm, w/norm);
        }
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
        internal Quaternion()
        {

        }
        public Quaternion(double x, double y, double z, double w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
        public override Position Rotate(Position point)
        {
            return Quaternion.Transform(point, this);
        }
        public YPRAngles ToYPRAngles(Quaternion q)
        {
            YPRAngles yprAngles = new YPRAngles();

            // roll (x-axis rotation)
            double sinRollCosPitch = 2.0 * (q.w * q.x + q.y * q.z);
            double cosRollCosPitch = 1.0 - 2.0 * (q.x * q.x + q.y * q.y);
            yprAngles.roll = Math.Atan2(sinRollCosPitch, cosRollCosPitch) * (180.0 / Math.PI); // in degrees

            // pitch (y-axis rotation)
            double sinPitch = Math.Sqrt(1.0 + 2.0 * (q.w * q.y - q.x * q.z));
            double cosPitch = Math.Sqrt(1.0 - 2.0 * (q.w * q.y - q.x * q.z));
            yprAngles.pitch = (2.0 * Math.Atan2(sinPitch, cosPitch) - Math.PI / 2.0) * (180.0 / Math.PI); // in degrees

            // yaw (z-axis rotation)
            double sinYawCosPitch = 2.0 * (q.w * q.z + q.x * q.y);
            double cosYawCosPitch = 1.0 - 2.0 * (q.y * q.y + q.z * q.z);
            yprAngles.yaw = Math.Atan2(sinYawCosPitch, cosYawCosPitch) * (180.0 / Math.PI); // in degrees

            return yprAngles;
        }
        public static Position Transform(Position inPoint, Quaternion rotation)
        {
            CartesianPosition point = (CartesianPosition)inPoint;
            double x2 = rotation.x + rotation.x;
            double y2 = rotation.y + rotation.y;
            double z2 = rotation.z + rotation.z;

            double wx2 = rotation.w * x2;
            double wy2 = rotation.w * y2;
            double wz2 = rotation.w * z2;
            double xx2 = rotation.x * x2;
            double xy2 = rotation.x * y2;
            double xz2 = rotation.x * z2;
            double yy2 = rotation.y * y2;
            double yz2 = rotation.y * z2;
            double zz2 = rotation.z * z2;

            return new CartesianPosition(
                point.x * (1.0f - yy2 - zz2) + point.y * (xy2 - wz2) + point.z * (xz2 + wy2),
                point.x * (xy2 + wz2) + point.y * (1.0f - xx2 - zz2) + point.z * (yz2 - wx2),
                point.x * (xz2 - wy2) + point.y * (yz2 + wx2) + point.z * (1.0f - xx2 - yy2));
        }
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
    /// Most transformation have a context with necessary ancillary information
    /// that parameterizes the transformation of a Position in one frame to a corresponding Position is another.
    /// Such context may include, for example, some or all of the information that may be conveyed in an ISO 19111 CRS specification
    /// or a proprietary naming, numbering, or modelling scheme as used by EPSG, NASA Spice, or SEDRIS SRM.
    /// Subclasses of FrameTransform exist precisely to hold this context in conjunction with code
    /// implementing a Transform function.
    /// <remark>
    /// </remark>
    /// </summary>
    public abstract class FrameTransform
    {
        public virtual Position Transform(Position point)
        {
            // The defualt is to apply the identity transformation
            Position result = point;
            return result;
        }
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
        internal Extrinsic()
        {

        }
        public Extrinsic(string authority, string id, string parameters)
        {
            this.authority = authority;
            this.id = id;
            this.parameters = parameters;
        }
        /// <summary>
        /// The core function of a transformation is the implement a specific frame transformation
        /// i.e. the transformation of a triple of point coordinates in the outer frame to a triple of point coordinates in the inner frame.
        /// When this is not possible due to lack of an appropriate tranformation procedure,
        /// the triple (NaN, NaN, NaN) [three IEEE 574 not-a-number vales] is returned.
        /// Note that an "authority" is not necessarily a standards organization but rather an entity that provides
        /// a register of some kind for a category of frame- and/or frame transform specifications that is useful and stable enough
        /// for someone to implement transformation functions.
        /// An implementation need not implement all possbile transforms.
        /// </summary>
        /// <note>
        /// This would be a good element to implement as a set of plugin.
        /// </note>
        /// <param name="point"></param>
        /// <returns></returns>
        public override Position Transform(Position point)
        {
            string uri = authority.ToLower().Replace("//www.", "");
            if (uri == "https://proj.org" || uri == "https://osgeo.org")
            {
                return noTransform;
            }
            else if (uri == "https://epsg.org")
            {
                return noTransform;
            }
            else if (uri == "https://iers.org")
            {
                return noTransform;
            }
            else if (uri == "https://naif.jpl.nasa.gov")
            {
                return noTransform;
            }
            else if (uri == "https://sedris.org")
            {
                return noTransform;
            }
            else if (uri == "https://iau.org")
            {
                return noTransform;
            }
            return noTransform;
        }
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
        static Position noTransform = new NoPosition();
    }
    /// <summary>
    /// A specialized specification of the WGS84 (EPSG 4326) geodetic frame to a local tangent plane East, North, Up frame.
    /// <remark>
    /// The origin of the coordinate system associated with the frame is a Position - the origin -
    /// which is the *only* distinguished Position associated with the coodinate system associated with the inner frame (range).
    /// </remark>
    /// </summary>
    public class WGS84ToLTP_ENU : FrameTransform
    {
        internal WGS84ToLTP_ENU()
        {

        }
        public WGS84ToLTP_ENU(GeodeticPosition origin)
        {
            this.Origin = origin;
        }
        public Position Transform(Position point)
        {
            double east, north, up;
            LTP_ENU.LTP_ENU.GeodeticToEnu(((GeodeticPosition)point).lat, ((GeodeticPosition)point).lon, ((GeodeticPosition)point).h,
                Origin.lat, Origin.lon, Origin.h, out east, out north, out up);
            CartesianPosition outPoint = new CartesianPosition(east, north, up);
            return outPoint;
        }

        /// <summary>
        /// A single geodetic position defines the tangent point for a transform to LTP-ENU.
        /// </summary>
        public GeodeticPosition Origin { get; set; } = null;
    }
    // A simple translation frame transform.
    public class Translation : FrameTransform
    {
        internal Translation()
        {
        }
        public Translation (double xOffset, double yOffset, double zOffset)
        {
            this.xOffset = xOffset;
            this.yOffset = yOffset;
            this.zOffset = zOffset; 
        }
        public double xOffset { get; set; } = 0.0;
        public double yOffset { get; set; } = 0.0;
        public double zOffset { get; set; } = 0.0;
    }
}
