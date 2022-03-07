using Dapper;
using Discount.API.Entities;
using Npgsql;

namespace Discount.API.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        IConfiguration _configuration;

        public DiscountRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            using var connection = new NpgsqlConnection(
                _configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

            string sql = "INSERT INTO Coupon (ProductName, Description, Amount) VALUES " +
                "(@ProductName, @Description, @Amount)";

            var affected = await connection.ExecuteAsync(sql,
                new { 
                    coupon.ProductName,
                    coupon.Description,
                    coupon.Amount
                });

            if (affected == 0)
                return false;

            return true;
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            using var connection = new NpgsqlConnection(
                _configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

            string sql = "DELETE FROM Coupon " +
                "WHERE ProductName=@ProductName";

            var affected = await connection.ExecuteAsync(sql,
                new
                {
                    productName
                });

            if (affected == 0)
                return false;

            return true;
        }

        public async Task<Coupon> GetDiscount(string productName)
        {
            using var connection = new NpgsqlConnection(
                _configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

            var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>(
                "SELECT * FROM Coupon WHERE ProductName = @ProductName",
                new { productName });

            if (coupon == null)
            {
                return new Coupon { ProductName = "No Discount", Amount = 0, Description = "No Discount"};
            }
            return coupon;
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            using var connection = new NpgsqlConnection(
                _configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

            string sql = "UPDATE Coupon SET ProductName=@ProductName, " +
                "Description=@Description, Amount=@Amount " +
                "WHERE Id=@Id";

            var affected = await connection.ExecuteAsync(sql,
                new
                {
                    coupon.ProductName,
                    coupon.Description,
                    coupon.Amount,
                    coupon.Id
                });

            if (affected == 0)
                return false;

            return true;
        }
    }
}
