namespace Ontap.Util
{
    public class SqlConnectionOptions
    {
        public enum ConnectionTypes
        {
            Mysql,
            Sqlserver
        }

        public ConnectionTypes ConnectionType { get; set; }
        public string ConnectionString { get; set; }
    }
}