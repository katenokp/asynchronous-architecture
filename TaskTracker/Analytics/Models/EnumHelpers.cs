namespace Analytics.Models;

public static class EnumHelpers
{
    public static UserRole ParseUserRole(string role)
    {
        return role switch
               {
                   "Popug" => UserRole.User,
                   _ => UserRole.Manager
               };
    }
}