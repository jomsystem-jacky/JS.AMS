﻿using System.Data;
using JS.AMS.Data.Entity.CompanyModule;
using JS.AMSWeb.Areas.CompanyModule.ViewModels.CompanyProfile;
using JS.AMSWeb.Data;
using JS.AMSWeb.Areas.CompanyModule.ViewModels.CompanyProfile;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using JS.AMS.Data;
using Humanizer;
using DocumentFormat.OpenXml.Wordprocessing;

namespace JS.AMSWeb.Areas.CompanyModule
{
    [Area("CompanyModule")]
    public class CompanyProfileController : Controller
    {
        private readonly AMSDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CompanyProfileController(AMSDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index(int? page)
        {
            //var pagination = new PaginationDTO();
            //pagination.CurrentPage = dto.Page;

            var companyProfiles = _db.CompanyProfiles
                .Where(x => x.Active);

            var listVm = new List<CompanyProfileViewModel>();

            var companyProfileList = companyProfiles.ToList();
            foreach (var a in companyProfileList)
            {
                var vm = new CompanyProfileViewModel();
                vm.Id = a.Id;
                vm.Name = a.Name;
                vm.BRN = a.BRN;
                vm.FullAddress = a.FullAddress;
                vm.ContactPersonName = a.ContactPersonName;
                vm.ContactPersonPhoneNumber = a.ContactPersonPhoneNumber;
                listVm.Add(vm);
            }

            int pageSize = 10;
            int pageNumber = page ?? 1;

            var listing = listVm.ToPagedList(pageNumber, pageSize);

            var result = new CompanyProfilePageViewModel();
            result.Listing = listing;
            result.AddCompanyProfileDTO = new AddCompanyProfileViewModel();

            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddCompanyProfileViewModel dto)
        {
            var companyProfile = new CompanyProfile();
            companyProfile.Active = true;
            companyProfile.BRN = dto.BRN;
            companyProfile.ContactPersonName = dto.ContactPersonName;
            companyProfile.ContactPersonPhoneNumber = dto.ContactPersonPhoneNumber;
            companyProfile.Name = dto.Name;
            companyProfile.FullAddress = dto.FullAddress;

            _db.CompanyProfiles.Add(companyProfile);
            _db.SaveChanges("system");

            return RedirectToAction("Index");
        }



        public IActionResult Edit(Guid id)
        {
            var companyProfile = _db.CompanyProfiles
                .FirstOrDefault(x => x.Id == id);
            if (companyProfile == null)
            {
                return BadRequest("Company profile not found");
            }

            var vm = new ManageCompanyProfileViewModel();
            vm.Id = companyProfile.Id;
            vm.Name = companyProfile.Name;
            vm.BRN = companyProfile.BRN;
            vm.FullAddress = companyProfile.FullAddress;
            vm.ContactPersonName = companyProfile.ContactPersonName;
            vm.ContactPersonPhoneNumber = companyProfile.ContactPersonPhoneNumber;

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ManageCompanyProfileViewModel dto)
        {
            try
            {
                var companyProfile = _db.CompanyProfiles
                    .FirstOrDefault(x => x.Id == dto.Id);
                if (companyProfile == null)
                {
                    return BadRequest("Company profile not found");
                }

                companyProfile.Name = dto.Name;
                companyProfile.BRN = dto.BRN;
                companyProfile.FullAddress = dto.FullAddress;
                companyProfile.ContactPersonName = dto.ContactPersonName;
                companyProfile.ContactPersonPhoneNumber = dto.ContactPersonPhoneNumber;

                _db.CompanyProfiles.Update(companyProfile);
                await _db.SaveChangesAsync("system");

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public IActionResult Delete(Guid id)
        {
            var companyProfile = _db.CompanyProfiles
                .FirstOrDefault(x => x.Id == id);
            if (companyProfile == null)
            {
                return BadRequest("Company profile not found");
            }

            var vm = new ManageCompanyProfileViewModel();
            vm.Id = companyProfile.Id;
            vm.Name = companyProfile.Name;
            vm.BRN = companyProfile.BRN;
            vm.FullAddress = companyProfile.FullAddress;
            vm.ContactPersonName = companyProfile.ContactPersonName;
            vm.ContactPersonPhoneNumber = companyProfile.ContactPersonPhoneNumber;

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(ManageCompanyProfileViewModel dto)
        {
            try
            {
                var companyProfile = _db.CompanyProfiles
                    .FirstOrDefault(x => x.Id == dto.Id);
                if (companyProfile == null)
                {
                    return BadRequest("Company profile not found");
                }

                companyProfile.Active = false;

                _db.CompanyProfiles.Update(companyProfile);
                await _db.SaveChangesAsync("system");

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}