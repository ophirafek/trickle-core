using Interfaces;
using ACIA.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Interfaces.IIdPoolService;

namespace ACIA.Services
{
    public class IdPoolService : IIdPoolService
    {
        private ApplicationDbContext _dbContext;

        public IdPoolService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<int> GetNextIdAsync(IdPoolType idType)
        {
                        // Create a parameter for the return value
            var returnParameter = new Microsoft.Data.SqlClient.SqlParameter
            {
                ParameterName = "@ReturnVal",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            };

            // Create the ID type parameter
            var idTypeParameter = new Microsoft.Data.SqlClient.SqlParameter
            {
                ParameterName = "@IdType",
                SqlDbType = SqlDbType.Int,
                Value = idType
            };


            // Execute the stored procedure
            await _dbContext.Database.ExecuteSqlRawAsync(
                "DECLARE @ReturnValue INT; " +
                "EXEC @ReturnValue = GetNextId @IdType; " +
                "SET @ReturnVal = @ReturnValue",
                idTypeParameter, returnParameter);

            // Get the return value
            return (int)returnParameter.Value;

        }
    }
}
