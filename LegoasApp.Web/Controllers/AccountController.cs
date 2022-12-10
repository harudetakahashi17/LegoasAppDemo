using LegoasApp.Core.Interfaces;
using LegoasApp.Core.Services;
using LegoasApp.Infrastructure.Models;
using LegoasApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Diagnostics;

namespace LegoasApp.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IAccountService _accountService;
        private IBranchService _branchService;
        private IRoleService _roleService;
        private IMenuScreenService _menuService;

        public AccountController(
            ILogger<HomeController> logger, 
            IAccountService accountService, 
            IBranchService branchService, 
            IRoleService roleService, 
            IMenuScreenService menuService
            )
        {
            _logger = logger;
            _accountService = accountService;
            _branchService = branchService;
            _roleService = roleService;
            _menuService = menuService;
        }

        #region Views
        public IActionResult Index() // Login View
        {
            int accId = HttpContext.Session.GetInt32("AccountId").HasValue ? HttpContext.Session.GetInt32("AccountId").Value : 0;
            if ( accId == 0)
            {
                HttpContext.Session.SetInt32("AccountId", 0);
                HttpContext.Session.SetString("AccountName", string.Empty);
                ViewBag.AccountId = 0;
                ViewBag.AccountName = string.Empty;
            }
            else
            {
                ViewBag.AccountId = HttpContext.Session.GetInt32("AccountId").Value;
                ViewBag.AccountName = HttpContext.Session.GetString("AccountName");
            }
            
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("AccountId");
            HttpContext.Session.Remove("AccountName");

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        public async Task<IActionResult> AccountList()
        {
            var listAccount = await _accountService.GetAll();
            var result = listAccount.Select(x => new AccountViewModel
            {
                Id = x.Id,
                AccountName = x.AccountName,
                Password = x.Password,
                CreatedDate = x.CreatedDate.ToString("dd-mm-yyyy")
            }).ToList();
            return View(result);
        }
        #endregion

        #region Input
        [HttpPost]
        public async Task<IActionResult> Register(AccountRegisterInputViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            var userLogin = HttpContext.Session.GetString("AccountName") ?? "Guest";

            if(request.Roles == null)
            {
                ModelState.AddModelError("Roles", "Role is required");
                return View(request);
            }

            if (request.MenuAccess == null)
            {
                ModelState.AddModelError("MenuAccess", "Role is required");
                return View(request);
            }

            if (request.Branches == null)
            {
                ModelState.AddModelError("Branches", "Role is required");
                return View(request);
            }

            var branchList = await _branchService.GetAll();
            var selectedBranch = branchList.Where(x => request.Branches.Contains(x.Id)).ToList();

            var newAccount = new Account()
            {
                AccountName = request.AccountName,
                Password = request.Password,
                CreatedBy = userLogin,
                CreatedDate = DateTime.Now,
                RowStatus = true
            };

            var newUser = new User()
            {
                UserName = request.UserName ?? string.Empty,
                Address = request.Address ?? string.Empty,
                PostalCode = request.PostalCode ?? string.Empty,
                Province = request.Province ?? string.Empty,
                BranchName = string.Join(",", selectedBranch.Select(x => x.BranchName)),
                CreatedBy = userLogin,
                CreatedDate = DateTime.Now,
                RowStatus = true
            };

            try
            {
                _accountService.Add(newAccount);
                // Add user
                // Add accountrole
                // Add userbranch
                // Add rolemenu
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create account");
                return Error();
            }

            return RedirectToAction(nameof(AccountList));
        }

        [HttpPost]
        public async Task<IActionResult> Index(AccountInputViewModel request)
        {
            if(!ModelState.IsValid)
            {
                return View(request);
            }

            var acc = await _accountService.Login(request.AccountName, request.Password);
            if(acc == null)
            {
                ViewBag.AccountMsg = "Invalid user";
                return View(request);
            }

            HttpContext.Session.SetInt32("AccountId", acc.Id);
            HttpContext.Session.SetString("AccountName", acc.AccountName);

            return RedirectToAction(nameof(Index));
        }
        #endregion

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
