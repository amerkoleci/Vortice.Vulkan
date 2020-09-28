// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace Vortice.Vulkan
{
    /// <summary>
    /// Structure specifying a 3x4 affine transformation matrix
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public partial struct VkTransformMatrixKHR
    {
        [FieldOffset(0)]
        public unsafe fixed float matrix[3 * 4];

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
        public unsafe VkTransformMatrixKHR(
            float m11, float m12, float m13, float m14,
            float m21, float m22, float m23, float m24,
            float m31, float m32, float m33, float m34)
        {
            matrix[0] = m11;
            matrix[1] = m12;
            matrix[2] = m13;
            matrix[3] = m14;

            matrix[4] = m21;
            matrix[5] = m22;
            matrix[6] = m23;
            matrix[7] = m24;

            matrix[8] = m31;
            matrix[9] = m32;
            matrix[10] = m33;
            matrix[11] = m34;
        }
    }
}
