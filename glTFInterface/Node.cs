using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace glTFInterface
{
    public class Node
    {
        // The user-defined name of this object.
        // Type: string
        // The user-defined name of this object.
        // Required: No
        public string name { get; set; } = "not set";

        // The index of the camera referenced by this node.
        // Type: integer
        // The index of the camera referenced by this node.
        // Required: No
        public Camera camera { get; set; } = new Camera();

        // child node indices - note that all nodes are stored in the top level glTF container
        // Type: integer [1-*]
        // The indices of this node’s children.
        // Required:  No
        public int[] children = new int[0];

        // The following four transformations are either a matrix or one or more of the other three
        // This can be tested by checking the length of the arrays

        // the matrix is in column major order default is 1, 0, 0, 0,  0, 1, 0, 0,   0, 0, 1, 0,  0, 0, 0, 1
        // Type: number [16]
        // A floating-point 4x4 transformation matrix stored in column-major order.
        // Required: No, default: [1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1]
        public double[] matrix { get; set; } = new double[0];

        // translation is dx, dy, dz with default is 0, 0, 0
        // Type: number [3]
        // The node’s translation along the x, y, and z axes.
        // Required: No, default: [0,0,0]
        public double[] translation { get; set; } = new double[0];

        // rotation is a unit quaternion x, y, z, w default is 0, 0, 0, 1
        // Type: number [4]
        // The node’s unit quaternion rotation in the order(x, y, z, w), where w is the scalar.
        // Required: No, default: [0,0,0,1]
        public double[] rotation { get; set; } = new double[0];

        // scale is in x, y, z axis order default is 1, 1, 1
        // Type: number [3]
        // The node’s non-uniform scale, given as the scaling factors along the x, y, and z axes.
        // Required: No, default: [1,1,1]
        public double[] scale { get; set; } = new double[0];

        // Type: integer
        // The index of the mesh in this node.
        // Required: No
        public int mesh { get; set; } = -1;

        // The index of the skin referenced by this node.
        // Type: integer
        // The index of the skin referenced by this node.
        // Required: No
        public int skin { get; set; } = -1;

        // The weights of the instantiated morph target.
        // The number of array elements MUST match the number of morph targets of the referenced mesh.
        // When defined, mesh MUST also be defined.
        // Type: number [1-*]
        // The weights of the instantiated morph target.
        // The number of array elements MUST match the number of morph targets of the referenced mesh.
        // When defined, mesh MUST also be defined.
        // Required: No
        public double[] weights { get; set; } = new double[0];

        // Type: extension
        // JSON object with extension-specific objects.
        // Required: No
        public Extension[] extensions { get; set; } = new Extension[0];

        // Type: extras
        // Application-specific data.
        // Required: No
        public Extra[] extras { get; set; } = new Extra[0];


    }
}