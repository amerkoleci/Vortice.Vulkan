// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Vortice.Mathematics;

namespace Vortice.Vulkan
{
    /// <summary>
    /// Structure specifying a 3x4 affine transformation matrix
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public partial struct VkTransformMatrixKHR
    {
        public Matrix3x4 matrix;

        /// <summary>
        /// Initializes a new instance of the <see cref="VkTransformMatrixKHR"/> struct.
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix3x4"/> to init from.</param>
        public VkTransformMatrixKHR(in Matrix3x4 matrix)
        {
            this.matrix = matrix;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VkTransformMatrixKHR"/> struct.
        /// </summary>
        /// <param name="m11">The value to assign at row 1 column 1 of the matrix.</param>
        /// <param name="m12">The value to assign at row 1 column 2 of the matrix.</param>
        /// <param name="m13">The value to assign at row 1 column 3 of the matrix.</param>
        /// <param name="m14">The value to assign at row 1 column 4 of the matrix.</param>
        /// <param name="m21">The value to assign at row 2 column 1 of the matrix.</param>
        /// <param name="m22">The value to assign at row 2 column 2 of the matrix.</param>
        /// <param name="m23">The value to assign at row 2 column 3 of the matrix.</param>
        /// <param name="m24">The value to assign at row 2 column 4 of the matrix.</param>
        /// <param name="m31">The value to assign at row 3 column 1 of the matrix.</param>
        /// <param name="m32">The value to assign at row 3 column 2 of the matrix.</param>
        /// <param name="m33">The value to assign at row 3 column 3 of the matrix.</param>
        /// <param name="m34">The value to assign at row 3 column 4 of the matrix.</param>
        public VkTransformMatrixKHR(float m11, float m12, float m13, float m14,
            float m21, float m22, float m23, float m24,
            float m31, float m32, float m33, float m34)
        {
            matrix = new Matrix3x4(m11, m12, m13, m14, m21, m22, m23, m24, m31, m32, m33, m34);
        }
    }
}
