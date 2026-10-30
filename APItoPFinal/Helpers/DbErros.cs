using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace APItoPFinal.Helpers
{
    public static class DbErros
    {
        // 547 = violação de chave estrangeira no SQL Server
        public static bool ViolacaoDeFk(DbUpdateException ex)
        {
            return ex.InnerException is SqlException { Number: 547 };
        }
    }
}
