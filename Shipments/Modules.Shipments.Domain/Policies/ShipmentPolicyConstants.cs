namespace Modules.Shipments.Domain.Policies;

public static class ShipmentPolicyConsts //SESZH: файл называется Constants, а класс - Consts, confuses.
{
    public const string ReadPolicy = "shipments:read";
    public const string CreatePolicy = "shipments:create";
    public const string UpdatePolicy = "shipments:update";
    public const string DeletePolicy = "shipments:delete";
}
