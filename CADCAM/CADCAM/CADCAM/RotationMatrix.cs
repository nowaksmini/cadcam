using System;

namespace CADCAM
{
    public class RotationMatrix
    {
        /* How close a float must be to a float to be "equal". */
        public static float Tolerance = 1E-9f;

        /* The rotation matrix.  This is a 4x4 matrix. */
        private readonly float[][] _matrix;
       
        /// <summary>
        /// Initializes a new instance of the <see cref="RotationMatrix"/> class.
        /// </summary>
        /// <param name="a">x-coordinate of a point on the line of rotation.</param>
        /// <param name="b">y-coordinate of a point on the line of rotation.</param>
        /// <param name="c">z-coordinate of a point on the line of rotation.</param>
        /// <param name="uUn">x-coordinate of the line's direction vector (unnormalized).</param>
        /// <param name="vUn">y-coordinate of the line's direction vector (unnormalized).</param>
        /// <param name="wUn">z-coordinate of the line's direction vector (unnormalized).</param>
        /// <param name="theta">The angle of rotation, in radians.</param>
        public RotationMatrix(float a, float b, float c, float uUn, float vUn, float wUn, float theta)
        {
            float l;
            if ((l = LongEnough(uUn, vUn, wUn)) < 0)
            {
                Console.WriteLine("RotationMatrix: direction vector too short!");
                return;
            }

            // In this instance we normalize the direction vector.
            float u = uUn/l;
            float v = vUn/l;
            float w = wUn/l;

            float u2 = u*u;
            float v2 = v*v;
            float w2 = w*w;
            float cosT = (float) Math.Cos(theta);
            float oneMinusCosT = 1 - cosT;
            float sinT = (float) Math.Sin(theta);

            // Build the matrix entries element by element.
            float m11 = u2 + (v2 + w2)*cosT;
            float m12 = u*v*oneMinusCosT - w*sinT;
            float m13 = u*w*oneMinusCosT + v*sinT;
            float m14 = (a*(v2 + w2) - u*(b*v + c*w))*oneMinusCosT
                        + (b*w - c*v)*sinT;

            float m21 = u*v*oneMinusCosT + w*sinT;
            float m22 = v2 + (u2 + w2)*cosT;
            float m23 = v*w*oneMinusCosT - u*sinT;
            float m24 = (b*(u2 + w2) - v*(a*u + c*w))*oneMinusCosT
                        + (c*u - a*w)*sinT;

            float m31 = u*w*oneMinusCosT - v*sinT;
            float m32 = v*w*oneMinusCosT + u*sinT;
            float m33 = w2 + (u2 + v2)*cosT;
            float m34 = (c*(u2 + v2) - w*(a*u + b*v))*oneMinusCosT
                        + (a*v - b*u)*sinT;
            if (_matrix == null)
            {
                _matrix = new[]
                {
                    new[] {m11, m12, m13, m14},
                    new[] {m21, m22, m23, m24},
                    new[] {m31, m32, m33, m34},
                    new float[] {0, 0, 0, 1}
                };
            }
        }

        /// <summary>
        /// Check whether a vector's length is less than <see ref="TOLERANCE"/> 
        /// </summary>
        /// <param name="u">The vector's x-coordinate.</param>
        /// <param name="v">The vector's y-coordinate.</param>
        /// <param name="w">The vector's z-coordinate.</param>
        /// <returns>length = Math.sqrt(u^2 + v^2 + w^2) if it is greater than  <see ref="TOLERANCE"/> or -1 if not.</returns>
        private float LongEnough(float u, float v, float w)
        {
            float l = (float) Math.Sqrt(u*u + v*v + w*w);
            if (l > Tolerance)
            {
                return l;
            }
            return -1;
        }

        public float[][] GetMatrix()
        {
            return _matrix;
        }
    }
}
