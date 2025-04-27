using LeadManagerPro.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyServices
{
    public class ImportResult
    {
        public int Status { get; set; } // 0: OK, 1: Company exists + new lead, 2: Both exist, 3: Error
        public string CompanyName { get; set; }
        public string Description { get; set; }
        public string ErrorMessage { get; set; }
    }

    public interface IImportService
    {
        Task<IEnumerable<ImportResult>> ImportCompaniesAsync(IEnumerable<CompanyDto> companies);
    }
}
