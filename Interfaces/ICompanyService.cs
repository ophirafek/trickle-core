﻿using System.Collections.Generic;
using System.Threading.Tasks;
using ACIA.DTOs;
using ACIA.Models;

namespace ACIA.Services
{
    public interface ICompanyService
    {
        Task<IEnumerable<CompanyDto>> GetCompaniesAsync();
        Task<CompanyDto> GetCompanyByIdAsync(int id);
        Task<CompanyDto> CreateCompanyAsync(CompanyDto companyDto);
        Task UpdateCompanyAsync(int id, CompanyDto companyDto);
        Task DeleteCompanyAsync(int id);
        Task<bool> CompanyExistsAsync(int id);
        Task<NoteDto> AddNoteAsync(int companyId, NoteCreateDto noteDto);
    }
}