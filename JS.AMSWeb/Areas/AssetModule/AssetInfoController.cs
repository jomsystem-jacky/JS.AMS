﻿using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using JS.AMS.Data;
using JS.AMSWeb.Areas.AssetModule.ViewModels.AssetInfo;
using JS.AMS.Data.Entity.AssetModule;
using JS.AMSWeb.Areas.AssetModule.ViewModels.AssetLocationHistory;
using JS.AMSWeb.DTO.Identity;
using JS.AMSWeb.Utils;
using JS.AMSWeb.DTO.Shared;

namespace JS.AMSWeb.Areas.AssetModule
{
    [Area("AssetModule")]
    public class AssetInfoController : Controller
    {
        private readonly AMSDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AssetInfoController(AMSDbContext db, IWebHostEnvironment webHostEnvironment)
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

            var assetInfo = _db.AssetInfos
                .Include(m => m.CompanyProfile)
                .Include(m => m.AssetType)
                .Where(x => x.Active);

            if (!string.IsNullOrWhiteSpace(searchName))
            {
                assetInfo = assetInfo.Where(x => x.Name.ToLower().Replace(" ", "").Contains(searchName.ToLower().Replace(" ", "")));
            }

            var listVm = new List<AssetInfoViewModel>();

            var assetInfoList = assetInfo.ToList();
            foreach (var a in assetInfoList)
            {
                var vm = new AssetInfoViewModel();
                vm.Id = a.Id;
                vm.Name = a.Name;
                vm.Code = a.Code;
                vm.Remark = a.Remark;
                vm.CompanyProfileId = a.CompanyProfileId;
                vm.CompanyProfileName = a.CompanyProfile.Name;
                vm.AssetTypeId = a.AssetTypeId;
                vm.AssetTypeName = a.AssetType.Name;

                listVm.Add(vm);
            }

            int pageSize = 10;
            int pageNumber = page ?? 1;

            ViewData["SearchName"] = searchName;

            var listing = listVm.ToPagedList(pageNumber, pageSize);

            var result = new AssetInfoPageViewModel();
            result.Listing = listing;
            result.AddAssetInfoDTO = new AddAssetInfoViewModel();

            ViewBag.CompanyProfiles = new SelectList(_db.CompanyProfiles.Where(x => x.Active), "Id", "Name");
            ViewBag.AssetTypes = new SelectList(_db.AssetTypes.Where(x => x.Active), "Id", "Name");
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
        public async Task<IActionResult> Create(AddAssetInfoViewModel dto)
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

                //var assetTypes = _db.AssetTypes
                //    .FirstOrDefault(x => x.Id == dto.AssetTypeId);
                var assetTypes = new AssetType();
                assetTypes.Name = dto.Name;
                assetTypes.Code = dto.Code;
                assetTypes.Remark = dto.Remark;

                _db.AssetTypes.Add(assetTypes);
                _db.SaveChanges("system");

                if (companyProfile == null)
                {
                    return BadRequest("Company Profile not found");
                }
                var assetInfo = new AssetInfo();
                assetInfo.Active = true;
                assetInfo.Name = dto.Name;
                assetInfo.Code = dto.Code;
                assetInfo.Remark = dto.Remark;
                assetInfo.CompanyProfile = companyProfile;
                assetInfo.AssetType = assetTypes;

                _db.AssetInfos.Add(assetInfo);
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

            var assetInfo = _db.AssetInfos
                .FirstOrDefault(x => x.Id == id);
            if (assetInfo == null)
            {
                return BadRequest("Asset Type not found");
            }

            var vm = new ManageAssetInfoViewModel();
            vm.Id = assetInfo.Id;
            vm.Name = assetInfo.Name;
            vm.Code = assetInfo.Code;
            vm.Remark = assetInfo.Remark;
            vm.CompanyProfileId = assetInfo.CompanyProfileId;
            vm.AssetTypeId = assetInfo.AssetTypeId;

            ViewBag.CompanyProfiles = new SelectList(_db.CompanyProfiles.Where(x => x.Active), "Id", "Name");
            ViewBag.AssetTypes = new SelectList(_db.AssetTypes.Where(x => x.Active), "Id", "Name");

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ManageAssetInfoViewModel dto)
        {
            var sessionData = HttpContext.Session?.GetObjectFromJson<UserSessionDTO>("UserSession") ?? null;
            if (sessionData == null)
            {
                return Redirect("/");
            }

            try
            {
                var assetInfo = _db.AssetInfos
                    .FirstOrDefault(x => x.Id == dto.Id);
                if (assetInfo == null)
                {
                    return BadRequest("Asset Type not found");
                }

                assetInfo.Name = dto.Name;
                assetInfo.Code = dto.Code;
                assetInfo.Remark = dto.Remark;
                assetInfo.CompanyProfileId = dto.CompanyProfileId;
                assetInfo.AssetTypeId = dto.AssetTypeId;

                _db.AssetInfos.Update(assetInfo);
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
            var assetInfo = _db.AssetInfos
                .FirstOrDefault(x => x.Id == id);
            if (assetInfo == null)
            {
                return BadRequest("Specification Type not found");
            }

            var vm = new ManageAssetInfoViewModel();
            vm.Id = assetInfo.Id;
            vm.Name = assetInfo.Name;
            vm.Code = assetInfo.Code;
            vm.Remark = assetInfo.Remark;
            vm.CompanyProfileId = assetInfo.CompanyProfileId;
            vm.AssetTypeId = assetInfo.AssetTypeId;

            ViewBag.CompanyProfiles = new SelectList(_db.CompanyProfiles.Where(x => x.Active), "Id", "Name");
            ViewBag.AssetTypes = new SelectList(_db.AssetTypes.Where(x => x.Active), "Id", "Name");


            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(ManageAssetInfoViewModel dto)
        {
            var sessionData = HttpContext.Session?.GetObjectFromJson<UserSessionDTO>("UserSession") ?? null;
            if (sessionData == null)
            {
                return Redirect("/");
            }

            try
            {
                var assetInfo = _db.AssetInfos
                    .FirstOrDefault(x => x.Id == dto.Id);
                if (assetInfo == null)
                {
                    return BadRequest("Asset Type not found");
                }

                assetInfo.Active = false;

                _db.AssetInfos.Update(assetInfo);
                await _db.SaveChangesAsync("system");

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public IActionResult Assign(Guid assetInfoId)
        {
            var sessionData = HttpContext.Session?.GetObjectFromJson<UserSessionDTO>("UserSession") ?? null;
            if (sessionData == null)
            {
                return Redirect("/");
            }

            var assetInfo = _db.AssetInfos
                .FirstOrDefault(x => x.Id == assetInfoId);
            if (assetInfo == null)
            {
                return BadRequest("Asset Info not found");
            }

            var vm = new AssignAssetInfoViewModel();
            vm.AssetInfoId = assetInfo.Id;
            vm.AssetInfoName = assetInfo.Name;
            vm.AssignDate = DateTime.Now;

            ViewBag.LocationTags = new SelectList(_db.LocationTags.Where(x => x.Active), "Id", "Name");
            ViewBag.Staff = new SelectList(_db.Staff.Where(x => x.Active), "Id", "Name");

            return View(vm);
        }

        [HttpPost]
        public IActionResult Assign(AssignAssetInfoViewModel dto)
        {
            var sessionData = HttpContext.Session?.GetObjectFromJson<UserSessionDTO>("UserSession") ?? null;
            if (sessionData == null)
            {
                return Redirect("/");
            }

            var assetInfo = _db.AssetInfos
                .FirstOrDefault(x => x.Id == dto.AssetInfoId);
            var LocationTag = _db.LocationTags
                .FirstOrDefault(x => x.Id == dto.LocationTagId);
            var staff = _db.Staff
                .FirstOrDefault(x => x.Id == dto.AssignByStaffId);

            if (assetInfo == null)
            {
                return BadRequest("Asset Info not found");
            }

            var assignmentHistory = new AssetLocationHistory();
            assignmentHistory.Active = true;
            assignmentHistory.AssetInfo = assetInfo;
            assignmentHistory.LocationTag = LocationTag;
            assignmentHistory.AssignedDate = dto.AssignDate;
            assignmentHistory.AssignedByStaff = staff;

            _db.AssetLocationHistories.Add(assignmentHistory);
            _db.SaveChanges("system");

            return RedirectToAction("Index");
        }

        public IActionResult History(int? page, Guid assetInfoId)
        {
            var sessionData = HttpContext.Session?.GetObjectFromJson<UserSessionDTO>("UserSession") ?? null;
            if (sessionData == null)
            {
                return Redirect("/");
            }

            var assetInfo = _db.AssetLocationHistories
                .Include(m => m.AssetInfo)
                .Include(m => m.LocationTag)
                .Include(m => m.AssignedByStaff)
                .Where(x => x.Active);

            var locationHistories = _db.AssetLocationHistories
                .Include(alh => alh.AssetInfo)
                .Include(alh => alh.LocationTag)
                .Include(alh => alh.AssignedByStaff)
                .OrderByDescending(alh => alh.AssignedDate)
                .Where(alh => alh.Active && alh.AssetInfoId == assetInfoId)
                .ToList();

            var listVm = new List<AssetLocationHistoryViewModel>();

            var locationHistoryList = locationHistories.ToList();
            foreach (var a in locationHistoryList)
            {
                var vm = new AssetLocationHistoryViewModel();
                vm.AssignedDate = a.AssignedDate;
                vm.AssetInfoName = a.AssetInfo.Name;
                vm.LocationTagName = a.LocationTag.Name;
                //vm.AssignedByStaffName = a.AssignedByStaff.Name;

                listVm.Add(vm);
            }

            int pageSize = 10;
            int pageNumber = page ?? 1;

            var listing = listVm.ToPagedList(pageNumber, pageSize);

            var result = new AssetLocationHistoryPageViewModel();
            result.Listing = listing;

            return View(result);
        }
    }
}
