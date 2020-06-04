using Volo.Abp.Reflection;

namespace Uncmd.Blogging.Permissions
{
    public class BloggingPermissions
    {
        public const string GroupName = "Blogging";

        public static string[] GetAll()
        {
            return ReflectionHelper.GetPublicConstantsRecursively(typeof(BloggingPermissions));
        }
    }
}