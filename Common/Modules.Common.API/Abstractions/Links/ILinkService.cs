using System;
using System.Collections.Generic;
using System.Text;


namespace Modules.Common.API.Abstractions.Links;

public interface ILinkService
{
	Link Generate(string endPointName, object? routeValues, string rel, string method);
}
