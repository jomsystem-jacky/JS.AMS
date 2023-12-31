﻿using System.Data;
using JS.AMS.Data.Entity.CompanyModule;
using JS.AMSWeb.Areas.CompanyModule.ViewModels.CompanyProfile;
using JS.AMSWeb.Areas.CompanyModule.ViewModels.CompanyBranch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using JS.AMS.Data;
using JS.AMSWeb.DTO.Identity;
using JS.AMSWeb.Utils;
using JS.AMSWeb.DTO.Shared;

namespace JS.AMSWeb.Areas.CompanyModule
{
    [Area("CompanyModule")]
    public class CompanyBranchController : Controller
    {
        private readonly AMSDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CompanyBranchController(AMSDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index(int? page, string searchName)
        {
            var pagination = new PaginationDTO();
            pagination.CurrentPage = page ?? 1;
            var sessionData = HttpContext.Session?.GetObjectFromJson<UserSessionDTO>("UserSession") ?? null;
            if (sessionData == null)
            {
                return Redirect("/");
            }

            var companyBranch = _db.CompanyBranches
                .Include(m => m.CompanyProfile)
                .Include(m => m.LocationTag)
                .Where(x => x.Active);

            if (!string.IsNullOrWhiteSpace(searchName))
            {
                companyBranch = companyBranch.Where(x => x.Name.ToLower().Replace(" ", "").Contains(searchName.ToLower().Replace(" ", "")));
            }

            var listVm = new List<CompanyBranchViewModel>();

            var companyBranchList = companyBranch.ToList();
            foreach (var a in companyBranchList)
            {
                var vm = new CompanyBranchViewModel();
                vm.Id = a.Id;
                vm.Name = a.Name;
                vm.FullAddress = a.FullAddress;
                vm.BranchContactPersonName = a.BranchContactPersonName;
                vm.BranchContactPersonPhoneNumber = a.BranchContactPersonPhoneNumber;
                vm.CompanyProfileId = a.CompanyProfileId;
                vm.CompanyProfileName = a.CompanyProfile.Name;
                vm.LocationTagId = a.LocationTagId;
                //vm.LocationTagName = a.LocationTag.Name;
                
                listVm.Add(vm);
            }

            int pageSize = 10;
            int pageNumber = page ?? 1;

            ViewData["SearchName"] = searchName;

            var listing = listVm.ToPagedList(pageNumber, pageSize);

            var result = new CompanyBranchPageViewModel();
            result.Listing = listing;
            result.AddCompanyBranchDTO = new AddCompanyBranchViewModel();

            ViewBag.CompanyProfiles = new SelectList(_db.CompanyProfiles.Where(x => x.Active), "Id", "Name");
            ViewBag.LocationTags = new SelectList(_db.LocationTags.Where(x => x.Active), "Id", "Name");

            return View(result);
        }

        [HttpPost]
        public IActionResult Search(string? searchName, int? page)
        {
            var sessionData = HttpContext.Session?.GetObjectFromJson<UserSessionDTO>("UserSession") ?? null;
            if (sessionData == null)
            {
                return Redirect("/");
            }

            if (page == 0 || page == null)
            {
                page = 1;
            }

            return RedirectToAction("Index", new { page = page, searchName = searchName });
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddCompanyBranchViewModel dto)
        {
            var sessionData = HttpContext.Session?.GetObjectFromJson<UserSessionDTO>("UserSession") ?? null;
            if (sessionData == null)
            {
                return Redirect("/");
            }

            try
            {
                var companyProfile = _db.CompanyProfiles
                    .FirstOrDefault(x => x.Id == dto.CompanyProfileId);
                if (companyProfile == null)
                {
                    return BadRequest("Company Profile not found");
                }

                var locationTag = new LocationTag();
                locationTag.Active = true;
                locationTag.Name = dto.Name;
                locationTag.Code = dto.Name;
                locationTag.CompanyProfile = companyProfile;

                _db.LocationTags.Add(locationTag);
                _db.SaveChanges("system");

                var companyBranch = new CompanyBranch();
                companyBranch.Active = true;
                companyBranch.BranchContactPersonName = dto.BranchContactPersonName;
                companyBranch.BranchContactPersonPhoneNumber = dto.BranchContactPersonPhoneNumber;
                companyBranch.Name = dto.Name;
                companyBranch.FullAddress = dto.FullAddress;
                companyBranch.CompanyProfile = companyProfile;
                companyBranch.LocationTag = locationTag;

                _db.CompanyBranches.Add(companyBranch);
                _db.SaveChanges("system");

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return Ok(ex);
            }
        }

        public IActionResult Edit(Guid id)
        {
            var sessionData = HttpContext.Session?.GetObjectFromJson<UserSessionDTO>("UserSession") ?? null;
            if (sessionData == null)
            {
                return Redirect("/");
            }
            var companyBranch = _db.CompanyBranches
                .FirstOrDefault(x => x.Id == id);
            if (companyBranch == null)
            {
                return BadRequest("Company Branch not found");
            }

            var vm = new ManageCompanyBranchViewModel();
            vm.Id = companyBranch.Id;
            vm.Name = companyBranch.Name;
            vm.FullAddress = companyBranch.FullAddress;
            vm.BranchContactPersonName = companyBranch.BranchContactPersonName;
            vm.BranchContactPersonPhoneNumber = companyBranch.BranchContactPersonPhoneNumber;
            vm.CompanyProfileId = companyBranch.CompanyProfileId;
            vm.LocationTagId = companyBranch.LocationTagId;

            ViewBag.CompanyProfiles = new SelectList(_db.CompanyProfiles.Where(x => x.Active), "Id", "Name");
            ViewBag.LocationTags = new SelectList(_db.LocationTags.Where(x => x.Active), "Id", "Name");

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ManageCompanyBranchViewModel dto)
        {
            var sessionData = HttpContext.Session?.GetObjectFromJson<UserSessionDTO>("UserSession") ?? null;
            if (sessionData == null)
            {
                return Redirect("/");
            }

            try
            {
                var companyBranch = _db.CompanyBranches
                    .FirstOrDefault(x => x.Id == dto.Id);
                if (companyBranch == null)
                {
                    return BadRequest("Company Branch not found");
                }

                companyBranch.Name = dto.Name;
                companyBranch.FullAddress = dto.FullAddress;
                companyBranch.FullAddress = dto.FullAddress;
                companyBranch.BranchContactPersonName = dto.BranchContactPersonName;
                companyBranch.BranchContactPersonPhoneNumber = dto.BranchContactPersonPhoneNumber;
                companyBranch.CompanyProfileId = dto.CompanyProfileId;
                companyBranch.LocationTagId = dto.LocationTagId;

                _db.CompanyBranches.Update(companyBranch);
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
            var sessionData = HttpContext.Session?.GetObjectFromJson<UserSessionDTO>("UserSession") ?? null;
            if (sessionData == null)
            {
                return Redirect("/");
            }

            var CompanyBranch = _db.CompanyBranches
                .FirstOrDefault(x => x.Id == id);
            if (CompanyBranch == null)
            {
                return BadRequest("Company Branch not found");
            }

            var vm = new ManageCompanyBranchViewModel();
            vm.Id = CompanyBranch.Id;
            vm.Name = CompanyBranch.Name;
            vm.CompanyProfileId = CompanyBranch.CompanyProfileId;
            vm.LocationTagId = CompanyBranch.LocationTagId;
            vm.BranchContactPersonName = CompanyBranch.BranchContactPersonName;
            vm.FullAddress = CompanyBranch.FullAddress;
            vm.BranchContactPersonPhoneNumber = CompanyBranch.BranchContactPersonPhoneNumber;

            ViewBag.CompanyProfiles = new SelectList(_db.CompanyProfiles.Where(x => x.Active), "Id", "Name");
            ViewBag.LocationTags = new SelectList(_db.LocationTags.Where(x => x.Active), "Id", "Name");

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(ManageCompanyBranchViewModel dto)
        {
            var sessionData = HttpContext.Session?.GetObjectFromJson<UserSessionDTO>("UserSession") ?? null;
            if (sessionData == null)
            {
                return Redirect("/");
            }

            try
            {
                var CompanyBranch = _db.CompanyBranches
                    .FirstOrDefault(x => x.Id == dto.Id);
                if (CompanyBranch == null)
                {
                    return BadRequest("Company Branch not found");
                }

                CompanyBranch.Active = false;

                _db.CompanyBranches.Update(CompanyBranch);
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
