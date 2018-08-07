using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Forum020.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Forum020.Service.Interfaces;
using Forum020.Admin.ViewModels;

namespace Forum020.Admin.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new ReportsViewModel()
            {
                Reports = await _reportService.GetReports()
            };
            return View(model);
        }
    }
}
