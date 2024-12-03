using System.Diagnostics;
using System.Reflection;

using StarmyKnife.Contracts.Services;

namespace StarmyKnife.Services;

public class ApplicationInfoService : IApplicationInfoService
{
    public static readonly string UnknownAppVersion = "0.0.0.0";

    public ApplicationInfoService()
    {
    }

    public Version GetVersion()
    {
        // Set the app version in StarmyKnife > Properties > Package > PackageVersion
        var assembly = Assembly.GetExecutingAssembly();
        var versionAttribute = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
        return new Version(versionAttribute?.Version ?? UnknownAppVersion);
    }
}
