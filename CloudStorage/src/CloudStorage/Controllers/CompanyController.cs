﻿using System;
using CloudStorage.ViewModels;
using Core.Constants;
using Core.Entities;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CloudStorage.Controllers
{
    [Authorize]
    public class CompanyController : Controller
    {
        private readonly ICompanyData _companyData;

        public CompanyController(ICompanyData companyData)
        {
            _companyData = companyData;
        }

        public IActionResult Index()
        {
            return View(_companyData.GetAll());
        }

        public IActionResult Details(Guid id)
        {
            var company = _companyData.Get(id);
            return View(company);
        }

        [HttpGet]
        [Authorize(Roles = AuthConstants.AdminRole)]
        public IActionResult Edit(Guid id)
        {
            var company = _companyData.Get(id);
            if (company == null)
                return RedirectToAction(nameof(Index));
            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AuthConstants.AdminRole)]
        public IActionResult Edit(Guid id, CreateCompanyViewModel model)
        {
            var company = _companyData.Get(id);
            if (ModelState.IsValid)
            {
                company.ContactEmail = model.ContactEmail;
                company.Name = model.Name;
                company.MainAddress = model.MainAddress;
                company.ContactPhoneNumber = model.ContactPhoneNumber;

                _companyData.Commit();

                return RedirectToAction(nameof(Details), new {id = company.Id});
            }

            return View(company);
        }

        [HttpGet]
        [Authorize(Policy = AuthConstants.AdministratorClaimPolicy)]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = AuthConstants.AdministratorClaimPolicy)]
        public IActionResult Create(CreateCompanyViewModel model)
        {
            if (ModelState.IsValid)
            {
                var company = new Company
                {
                    ContactEmail = model.ContactEmail,
                    ContactPhoneNumber = model.ContactPhoneNumber,
                    MainAddress = model.MainAddress,
                    Name = model.Name
                };

                _companyData.Add(company);
                _companyData.Commit();

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }
    }
}