using System.Reflection;
using System.Runtime.CompilerServices;


namespace Modules.MaintenanceOperations.Infrastructure;

public static class AssemblyReference
{
	public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
