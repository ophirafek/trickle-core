using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IIdPoolService
    {
        /// <summary>
        /// Gets the next available ID for the specified ID type
        /// </summary>
        /// <param name="idType">The type of ID to retrieve</param>
        /// <returns>The next available ID</returns>
        Task<int> GetNextIdAsync(IdPoolType idType);

        public enum IdPoolType
        {
            Company = 1,
            Contact = 2,
            Lead = 3,
            Task = 5,
            Note = 6,
            User = 8,
            Role = 9,
            Permission = 10,
            Assignment = 11,
            InsuredCompany = 12
        }

    }
}
