

using ClickHouse.Ado;
using ClickHouse.Net;
using System.Linq;

namespace HangfireService.tasks
{
    public class TestConnection
    {

        public void ExecJob()
        {
            using (var cnn = GetConnection())
            {
                var reader = cnn.CreateCommand("SELECT 1 ").ExecuteReader();
            }
        }

        private ClickHouseConnection GetConnection(string cstr = "Compress=True;CheckCompressedHash=False;Host=192.168.1.168;Port=8123;Database=default;User=ts;Password=tspanda")
        {
            var settings = new ClickHouseConnectionSettings(cstr);
            var cnn = new ClickHouseConnection(settings);
            cnn.Open();
            return cnn;
        }
       

    }
}
