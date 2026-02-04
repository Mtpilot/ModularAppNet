using System.Reflection;

namespace Modules.MaintenanceOperations.PublicApi;

public static class AssemblyReference
{
	public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
