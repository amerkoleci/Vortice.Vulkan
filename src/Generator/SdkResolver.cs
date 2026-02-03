// https://github.com/SharpGenTools/SharpGenTools

using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using CppAst;
using Microsoft.Win32;

namespace Generator;

public static class SdkResolver
{
    private static readonly string[] WindowsSdkIncludes = { "shared", "um", "ucrt", "winrt" };
    private static readonly char[] ComponentSeparator = { ';' };

    public static IEnumerable<string> ResolveStdLib(string? version = default)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            foreach (var vsInstallDir in GetVSInstallPath())
            {
                string actualVersion;
                if (version is not null)
                {
                    actualVersion = version;
                }
                else
                {
                    var defaultVersion = Path.Combine(
                        vsInstallDir, "VC", "Auxiliary", "Build", "Microsoft.VCToolsVersion.default.txt"
                    );

                    if (!File.Exists(defaultVersion))
                        continue;

                    actualVersion = File.ReadAllText(defaultVersion);
                }

                var path = Path.Combine(
                    vsInstallDir, "VC", "Tools", "MSVC", actualVersion.Trim(), "include"
                );

                if (!Directory.Exists(path))
                    continue;

                yield return path;
                yield break;
            }
        }
        else
        {
            if (version is not null)
            {
                var path = Path.Combine("/usr", "include", "c++", version);
                if (Directory.Exists(path))
                    yield return path;
            }
            else
            {
                DirectoryInfo cpp = new(Path.Combine("/usr", "include", "c++"));

                if (!cpp.Exists)
                    yield break;

                // TODO: pick latest if not specified
                var path = cpp.EnumerateDirectories().FirstOrDefault()?.FullName;
                if (path is not null)
                    yield return path;
            }
        }
    }

    private static List<string?> GetVSInstallPath()
    {
        List<string?> paths = [];
        try
        {
            SetupConfiguration query = new();
            IEnumSetupInstances enumInstances = query.EnumInstances();

            int fetched;
            var instances = new ISetupInstance[1];
            do
            {
                enumInstances.Next(1, instances, out fetched);
                if (fetched <= 0)
                    continue;

                var instance2 = (ISetupInstance2)instances[0];
                InstanceState state = instance2.GetState();
                if ((state & InstanceState.Registered) != InstanceState.Registered)
                    continue;

                if (instance2.GetPackages().Any(Predicate))
                    paths.Add(instance2.GetInstallationPath());
            } while (fetched > 0);

            static bool Predicate(ISetupPackageReference pkg) =>
                pkg.GetId() == "Microsoft.VisualStudio.Component.VC.Tools.x86.x64";
        }
        catch (Exception e)
        {
            //ogger.LogRawMessage(
            //   LogLevel.Warning, LoggingCodes.VisualStudioDiscoveryError,
            //   "Visual Studio installation discovery has thrown an exception", e
            //;
        }

        //if (paths.Count == 0)
        //    Logger.Fatal("Unable to find a Visual Studio installation that has the Visual C++ Toolchain installed.");

        return paths;
    }

    public static List<string> ResolveWindowsSdk(string version, string? components = default)
    {
        string[] componentList = components is not null
                                ? [.. components.Split(ComponentSeparator, StringSplitOptions.RemoveEmptyEntries).Select(static x => x.Trim())]
                                : WindowsSdkIncludes;

        List<string> result = [];

        if (Environment.GetEnvironmentVariable("SHARPGEN_SDK_OVERRIDE") is { Length: > 0 } sdkPathOverride)
        {
            foreach (var include in componentList)
            {
                result.Add(Path.Combine(sdkPathOverride, "Include", version, include));
            }
        }

        const string prefix =
            @"=HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows Kits\Installed Roots\KitsRoot10;Include\";

        foreach (var include in componentList)
        {
            string fullRegistryPath = prefix + version + '\\' + include;
            var registryPath = fullRegistryPath.Substring(1);
            var indexOfSubPath = fullRegistryPath.IndexOf(";");
            var subPath = "";
            if (indexOfSubPath >= 0)
            {
                subPath = registryPath.Substring(indexOfSubPath);
                registryPath = registryPath.Substring(0, indexOfSubPath - 1);
            }

            var (registryPathPortion, success) = ResolveRegistryDirectory(registryPath);

            if (!success)
                continue;

            string resolvedPath = Path.Combine(registryPathPortion, subPath);

            if (Directory.Exists(resolvedPath))
            {
                result.Add(resolvedPath);
            }
        }

        return result;
    }


    [SupportedOSPlatform("windows")]
    private static (string path, bool success) ResolveRegistryDirectory(string registryPath)
    {
        string path = null;
        var success = true;
        var indexOfKey = registryPath.LastIndexOf("\\");
        var subKeyStr = registryPath.Substring(indexOfKey + 1);
        registryPath = registryPath.Substring(0, indexOfKey);

        var indexOfHive = registryPath.IndexOf("\\");
        var hiveStr = registryPath.Substring(0, indexOfHive).ToUpper();
        registryPath = registryPath.Substring(indexOfHive + 1);

        try
        {
            var hive = RegistryHive.LocalMachine;
            switch (hiveStr)
            {
                case "HKEY_LOCAL_MACHINE":
                    hive = RegistryHive.LocalMachine;
                    break;
                case "HKEY_CURRENT_USER":
                    hive = RegistryHive.CurrentUser;
                    break;
                case "HKEY_CURRENT_CONFIG":
                    hive = RegistryHive.CurrentConfig;
                    break;
            }

            using (var rootKey = RegistryKey.OpenBaseKey(hive, RegistryView.Registry32))
            using (var subKey = rootKey.OpenSubKey(registryPath))
            {
                if (subKey == null)
                {
                    //Logger.Error(LoggingCodes.RegistryKeyNotFound, "Unable to locate key [{0}] in registry",
                    //             registryPath);
                    success = false;
                }

                path = subKey.GetValue(subKeyStr).ToString();
                //Logger.Message($"Resolved registry path {registryPath} to {path}");
            }
        }
        catch (Exception)
        {
            //Logger.Error(LoggingCodes.RegistryKeyNotFound, "Unable to locate key [{0}] in registry", registryPath);
            success = false;
        }

        return (path, success);
    }

    #region VisualStudioSetup
    [Flags]
    internal enum InstanceState : uint
    {
        None = 0,
        Local = 1,
        Registered = 2,
        NoRebootRequired = 4,
        NoErrors = 8,
        Complete = unchecked((uint)-1),
    }

    [Guid("DA8D8A16-B2B6-4487-A2F1-594CCCCD6BF5")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    internal interface ISetupPackageReference
    {
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetId();

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetVersion();

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetChip();

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetLanguage();

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetBranch();

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetType();

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetUniqueId();

        [return: MarshalAs(UnmanagedType.VariantBool)]
        bool GetIsExtension();
    }

    [Guid("6380BCFF-41D3-4B2E-8B2E-BF8A6810C848")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    internal interface IEnumSetupInstances
    {
        void Next([MarshalAs(UnmanagedType.U4), In] int celt, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Interface), Out] ISetupInstance[] rgelt, [MarshalAs(UnmanagedType.U4)] out int pceltFetched);

        void Skip([MarshalAs(UnmanagedType.U4), In] int celt);

        void Reset();

        [return: MarshalAs(UnmanagedType.Interface)]
        IEnumSetupInstances Clone();
    }

    [Guid("B41463C3-8866-43B5-BC33-2B0676F7F42E")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    internal interface ISetupInstance
    {
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetInstanceId();

        /*FILETIME*/
        long GetInstallDate();

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetInstallationName();

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetInstallationPath();

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetInstallationVersion();

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetDisplayName([MarshalAs(UnmanagedType.U4), In] int lcid = 0);

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetDescription([MarshalAs(UnmanagedType.U4), In] int lcid = 0);

        [return: MarshalAs(UnmanagedType.BStr)]
        string ResolvePath([MarshalAs(UnmanagedType.LPWStr), In] string pwszRelativePath = null);
    }

    [Guid("c601c175-a3be-44bc-91f6-4568d230fc83")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    internal interface ISetupPropertyStore
    {
        [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)]
        string[] GetNames();

        object GetValue([MarshalAs(UnmanagedType.LPWStr), In] string pwszName);
    }

    [Guid("89143C9A-05AF-49B0-B717-72E218A2185C")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    internal interface ISetupInstance2 : ISetupInstance
    {
        [return: MarshalAs(UnmanagedType.BStr)]
        new string GetInstanceId();

        new /*FILETIME*/long GetInstallDate();

        [return: MarshalAs(UnmanagedType.BStr)]
        new string GetInstallationName();

        [return: MarshalAs(UnmanagedType.BStr)]
        new string GetInstallationPath();

        [return: MarshalAs(UnmanagedType.BStr)]
        new string GetInstallationVersion();

        [return: MarshalAs(UnmanagedType.BStr)]
        new string GetDisplayName([MarshalAs(UnmanagedType.U4), In] int lcid = 0);

        [return: MarshalAs(UnmanagedType.BStr)]
        new string GetDescription([MarshalAs(UnmanagedType.U4), In] int lcid = 0);

        [return: MarshalAs(UnmanagedType.BStr)]
        new string ResolvePath([MarshalAs(UnmanagedType.LPWStr), In] string pwszRelativePath = null);

        [return: MarshalAs(UnmanagedType.U4)]
        InstanceState GetState();

        [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UNKNOWN)]
        ISetupPackageReference[] GetPackages();

        ISetupPackageReference GetProduct();

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetProductPath();

        ISetupErrorState GetErrors();

        [return: MarshalAs(UnmanagedType.VariantBool)]
        bool IsLaunchable();

        [return: MarshalAs(UnmanagedType.VariantBool)]
        bool IsComplete();

        ISetupPropertyStore GetProperties();

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetEnginePath();
    }

    [Guid("E73559CD-7003-4022-B134-27DC650B280F")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    internal interface ISetupFailedPackageReference : ISetupPackageReference
    {
        [return: MarshalAs(UnmanagedType.BStr)]
        new string GetId();

        [return: MarshalAs(UnmanagedType.BStr)]
        new string GetVersion();

        [return: MarshalAs(UnmanagedType.BStr)]
        new string GetChip();

        [return: MarshalAs(UnmanagedType.BStr)]
        new string GetLanguage();

        [return: MarshalAs(UnmanagedType.BStr)]
        new string GetBranch();

        [return: MarshalAs(UnmanagedType.BStr)]
        new string GetType();

        [return: MarshalAs(UnmanagedType.BStr)]
        new string GetUniqueId();

        [return: MarshalAs(UnmanagedType.VariantBool)]
        new bool GetIsExtension();
    }

    [Guid("46DCCD94-A287-476A-851E-DFBC2FFDBC20")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    internal interface ISetupErrorState
    {
        [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UNKNOWN)]
        ISetupFailedPackageReference[] GetFailedPackages();

        [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UNKNOWN)]
        ISetupPackageReference[] GetSkippedPackages();
    }

    [Guid("42843719-DB4C-46C2-8E7C-64F1816EFD5B")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    internal interface ISetupConfiguration
    {
        [return: MarshalAs(UnmanagedType.Interface)]
        IEnumSetupInstances EnumInstances();

        [return: MarshalAs(UnmanagedType.Interface)]
        ISetupInstance GetInstanceForCurrentProcess();

        [return: MarshalAs(UnmanagedType.Interface)]
        ISetupInstance GetInstanceForPath([MarshalAs(UnmanagedType.LPWStr), In] string path);
    }

    [Guid("26AAB78C-4A60-49D6-AF3B-3C35BC93365D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    internal interface ISetupConfiguration2 : ISetupConfiguration
    {
        [return: MarshalAs(UnmanagedType.Interface)]
        new IEnumSetupInstances EnumInstances();

        [return: MarshalAs(UnmanagedType.Interface)]
        new ISetupInstance GetInstanceForCurrentProcess();

        [return: MarshalAs(UnmanagedType.Interface)]
        new ISetupInstance GetInstanceForPath([MarshalAs(UnmanagedType.LPWStr), In] string path);

        [return: MarshalAs(UnmanagedType.Interface)]
        IEnumSetupInstances EnumAllInstances();
    }

    [Guid("177F0C4A-1CD3-4DE7-A32C-71DBBB9FA36D")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComImport]
    internal class SetupConfigurationClass
    {
    }

    [Guid("42843719-DB4C-46C2-8E7C-64F1816EFD5B")]
    [CoClass(typeof(SetupConfigurationClass))]
    [ComImport]
    internal interface SetupConfiguration : ISetupConfiguration2, ISetupConfiguration
    {
    }
    #endregion
}
