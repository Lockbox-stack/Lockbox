namespace Lockbox.Api
{
    public class FeatureSettings
    {
        public int EntrySizeBytesLimit { get; set; }
        public int EntriesPerBoxLimit { get; set; }
        public int UsersPerBoxLimit { get; set; }
        public int BoxesPerUserLimit { get; set; }
        public bool RequireAdminToCreateUser { get; set; }
    }
}