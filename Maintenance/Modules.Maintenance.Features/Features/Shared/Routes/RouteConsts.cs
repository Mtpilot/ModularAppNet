using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Maintenance.Features.Features.Shared.Routes;

internal static class RouteConsts
{
	internal const string BaseRoute = "/api/maintenance";
	internal const string CreateOperationForActRoute = $"{BaseRoute}/{{actId}}/operations";
}
