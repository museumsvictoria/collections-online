using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionsOnline.Import.Importers
{
    public class ItemMigration : IItemMigration
    {
        public void Run()
        {
            using (var connection = new SqlConnection(ConfigurationManager.AppSettings["SqlImportConnectionString"]))
            {
                
            }
        }
    }
}