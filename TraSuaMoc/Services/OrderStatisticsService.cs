using System.Data;
using System.Data.SqlClient;

namespace TraSuaMoc.Services
{
    // ADO.NET dùng truy vấn tham số hóa cho tổng hợp; EF dùng cho các thao tác nghiệp vụ.
    public static class OrderStatisticsService
    {
        public static decimal TotalSpent(string connectionString, int customerId)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand("SELECT COALESCE(SUM(Total), 0) FROM dbo.Orders WHERE CustomerId = @CustomerId", connection))
            {
                command.Parameters.Add("@CustomerId", SqlDbType.Int).Value = customerId;
                connection.Open();
                return (decimal)command.ExecuteScalar();
            }
        }
    }
}
