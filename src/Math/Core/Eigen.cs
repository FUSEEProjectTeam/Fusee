using System.Linq;

namespace Fusee.Math.Core
{
    /// <summary>
    /// Eigen data structure with values and vectors in double precision.
    /// </summary>
    public struct Eigen
    {
        /// <summary>
        /// Eigen values.
        /// </summary>
        public double[] Values;

        /// <summary>
        /// Eigen vectors.
        /// </summary>
        public double3[] Vectors;

        /// <summary>
        /// Generates the rotation matrix from eigen vectors
        /// </summary>
        public double4x4 RotationMatrix => new(new double4(Vectors[0]), new double4(Vectors[1]), new double4(Vectors[2]), new double4(0, 0, 0, 1));

        /// <summary>
        /// Checks if two EigenD values are the same
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj) => obj is Eigen e && e.Values.Equals(Values) && e.Vectors.Equals(Vectors);

        /// <summary>
        /// Returns hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => Values.GetHashCode() + Vectors.GetHashCode();

        /// <summary>
        /// Checks if two EigenD values are the same
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Eigen left, Eigen right) => left.Equals(right);

        /// <summary>
        /// Checks if two EigenD values aren't the same
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(Eigen left, Eigen right) => !(left == right);

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="vals"></param>
        public Eigen(double3[] vals)
        {
            var centeroid = M.CalculateCentroid(vals);
            var covarianceMatrix = M.CreateCovarianceMatrix(centeroid, vals);

            Values = new double[3];
            Vectors = new double3[3];
            CalculateVectorsAndValues(covarianceMatrix);
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="vals"></param>
        public Eigen(float3[] vals)
        {
            var valsD = vals.Select(val => (double3)val).ToArray();
            var centeroid = M.CalculateCentroid(valsD);
            var covarianceMatrix = M.CreateCovarianceMatrix(centeroid, valsD);

            Values = new double[3];
            Vectors = new double3[3];
            CalculateVectorsAndValues(covarianceMatrix);
        }

        /// <summary>
        /// Diagonalizes a given covariance matrix with double precision.
        /// <para>
        /// Credits to: https://github.com/melax/sandbox
        /// http://melax.github.io/diag.html
        /// A must be a symmetric matrix.
        /// returns quaternion q such that its corresponding matrix Q
        /// can be used to Diagonalize A
        /// Diagonal matrix D = transpose(Q) * A * (Q); thus A == Q * D * QT
        /// The directions of Q (cols of Q) are the eigenvectors D's diagonal is the eigenvalues.
        /// As per 'col' convention if float3x3 Q = qgetmatrix(q); then Q*v = q*v*conj(q).
        /// </para>
        /// </summary>
        /// <param name="A">The covariance matrix</param>
        /// <returns></returns>
        private void CalculateVectorsAndValues(double4x4 A)
        {
            const int maxsteps = 512; // certainly wont need that many.

            var q = new QuaternionD(0, 0, 0, 1);
            var D = double4x4.Identity;
            var Q = double4x4.Identity;

            //DIAGONALIZE
            for (var i = 0; i < maxsteps; i++)
            {
                // Q = float4x4.CreateRotation(q); // v*Q == q*v*conj(q)
                Q = q.ToRotationMatrix(); // v*Q == q*v*conj(q)
                D = Q.Transpose() * A * Q;  // A = Q^T*D*Q
                var offDiagonal = new double3(D.M23, D.M13, D.M12); // elements not on the diagonal
                var om = new double3(System.Math.Abs(offDiagonal.x), System.Math.Abs(offDiagonal.y), System.Math.Abs(offDiagonal.z)); // mag of each offdiag elem
                var k = (om.x > om.y && om.x > om.z) ? 0 : (om.y > om.z) ? 1 : 2; // index of largest element of offdiag
                var k1 = (k + 1) % 3;
                var k2 = (k + 2) % 3;

                if (offDiagonal[k].Equals(0.0)) break;  // diagonal already

                var theta = (D[k2, k2] - D[k1, k1]) / (2.0 * offDiagonal[k]);
                var sgn = (theta > 0.0) ? 1.0 : -1.0;
                theta *= sgn; // make it positive
                var t = sgn / (theta + ((theta < 1.0e+16) ? System.Math.Sqrt(theta * theta + 1.0) : theta)); // sign(T)/(|T|+sqrt(T^2+1))
                var c = 1.0 / System.Math.Sqrt(t * t + 1.0); //  c= 1/(t^2+1) , t=s/c

                if (c.Equals(1.0)) break;  // no room for improvement - reached machine precision.

                var jr = new QuaternionD(0, 0, 0, 0) // jacobi rotation for this iteration.
                {
                    [k] = (sgn * System.Math.Sqrt((1.0 - c) / 2.0))
                };

                // using 1/2 angle identity sin(a/2) = sqrt((1-cos(a))/2)
                jr[k] *= -1.0; // note we want a final result semantic that takes D to A, not A to D
                jr.w = System.Math.Sqrt(1.0 - (jr[k] * jr[k]));
                if (jr.w.Equals(1.0)) break; // reached limits of floating point precision
                q *= jr;
                q.Normalize();
            }

            Values[0] = D.M11;
            Values[1] = D.M22;
            Values[2] = D.M33;

            Vectors[0] = Q.Row1.xyz;
            Vectors[1] = Q.Row2.xyz;
            Vectors[2] = Q.Row3.xyz;
        }
    }
}