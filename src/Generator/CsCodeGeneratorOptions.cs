// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Generator;

public sealed class CsCodeGeneratorOptions
{
    public string OutputPath { get; set; } = null!;
    public string ClassName { get; set; } = null!;
    public string? Namespace { get; set; }
    public bool PublicVisiblity { get; set; } = true;
    public bool GenerateFunctionPointers { get; set; } = false;
    public bool ReadOnlySpanForStrings { get; set; } = false;

    public bool IsVulkan { get; set; } = false;
    public List<string> ExtraUsings { get; } = [];
}
