using System.Reflection;
using System.Runtime.Loader;

namespace Web.Utilities.Extensiones;

public class CustomAssemblyLoadContext:AssemblyLoadContext
{
    //permite trabajar con extesniones o librerias externas en el proyecto
    public IntPtr LoadUnmanagedLibrary(string absolutePath)
    {
        return LoadUnmanagedDll(absolutePath);
    }
    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        return LoadUnmanagedDllFromPath(unmanagedDllName);
    }
    protected override Assembly Load(AssemblyName assemblyName)
    {
        throw new NotImplementedException();
    }
}
