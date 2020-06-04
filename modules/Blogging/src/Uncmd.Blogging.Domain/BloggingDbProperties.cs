namespace Uncmd.Blogging
{
    public static class BloggingDbProperties
    {
        public static string DbTablePrefix { get; set; } = "Blogging";

        public static string DbSchema { get; set; } = null;

        public const string ConnectionStringName = "Blogging";
    }
}
