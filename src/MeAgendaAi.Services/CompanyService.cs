﻿using MeAgendaAi.Application.Notification;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.Interfaces.Repositories;
using MeAgendaAi.Domains.Interfaces.Services;
using MeAgendaAi.Domains.RequestAndResponse;
using MeAgendaAi.Infra.Cryptography;
using MeAgendaAi.Infra.Extension;
using MeAgendaAi.Services.CSVMaps;
using Microsoft.Extensions.Logging;

namespace MeAgendaAi.Services
{
    public class CompanyService : Service<Company>, ICompanyService
    {
        private readonly IUserService _userService;
        private readonly NotificationContext _notificationContext;
        private readonly IReport _report;
        private readonly ILogger<CompanyService> _logger;

        private const string ActionType = "CompanyService";

        public CompanyService(
            IUserService userService,
            ICompanyRepository companyRepository,
            NotificationContext notificationContext,
            IReport report, ILogger<CompanyService> logger) : base(companyRepository)
        {
            _userService = userService;
            _notificationContext = notificationContext;
            _report = report;
            _logger = logger;
        }

        public async Task<Guid> AddAsync(AddCompanyRequest request)
        {
            if (await _userService.HasUser(request.Email))
            {
                _notificationContext.AddNotification("Email", "Email já cadastrado");
                _logger.LogError("[{ActionType}/AddAsync] A registered user for {Email} already exists.", ActionType, request.Email);
                return Guid.Empty;
            }

            if (_userService.NotSamePassword(request.Password, request.ConfirmPassword))
            {
                _notificationContext.AddNotification("ConfirmPassword", "Senha de confirmação não é igual a senha");
                _logger.LogError("[{ActionType}/AddAsync] Confirmation password is not the same as password.", ActionType);
                return Guid.Empty;
            }

            var company = new Company(request.Email, request.Password, request.Name, request.CNPJ, request.Description, request.LimitCancelHours);
            if (company.Invalid)
            {
                _notificationContext.AddNotifications(company.ValidationResult);
                _logger.LogError("[{ActionType}/AddAsync] Invalid information {Errors}", ActionType, company.ValidationResult.Errors);
                return Guid.Empty;
            }

            company.Encript(Encrypt.EncryptString(company.Password, company.Id.ToString()));

            var companyId = await AddAsync(company);
            _logger.LogInformation("[{ActionType}/AddAsync] User {companyId} registered successfully.", ActionType, companyId);
            return companyId;
        }

        public async Task<byte[]?> ReportAsync()
        {
            var companies = await GetAllAsync();
            if (companies.IsEmpty())
                return null;

            _logger.LogInformation("[{ActionType}/ReportAsync] Report generated successfully.", ActionType);
            return _report.Generate<Company, CompanyMap>(companies);
        }
    }
}