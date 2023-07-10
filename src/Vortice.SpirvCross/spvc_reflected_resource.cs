// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using static Vortice.SpirvCross.Utils;

namespace Vortice.SpirvCross;

partial struct spvc_reflected_resource
{
    public unsafe string GetName()
    {
        return GetUtf8Span(name).GetString()!;
    }
}
