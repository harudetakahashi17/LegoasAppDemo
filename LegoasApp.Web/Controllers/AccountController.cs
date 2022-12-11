using LegoasApp.Core.Interfaces;
using LegoasApp.Core.Services;
using LegoasApp.Infrastructure.Models;
using LegoasApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

namespace LegoasApp.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private IAccountService _accountService;
        private IBranchService _branchService;
        private IRoleService _roleService;
        private IMenuScreenService _menuService;
        private IUserService _userService;
        private IAccountRoleService _accountRoleService;
        private IUserBranchService _userBranchService;
        private IRoleMenuService _roleMenuService;

        public AccountController(
            ILogger<AccountController> logger, 
            IAccountService accountService, 
            IBranchService branchService, 
            IRoleService roleService, 
            IMenuScreenService menuService,
            IUserService userService,
            IAccountRoleService accountRoleService,
            IUserBranchService userBranchService,
            IRoleMenuService roleMenuService
            )
        {
            _logger = logger;
            _accountService = accountService;
            _branchService = branchService;
            _roleService = roleService;
            _menuService = menuService;
            _userService = userService;
            _accountRoleService = accountRoleService;
            _userBranchService = userBranchService;
            _roleMenuService = roleMenuService;
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

        public async Task<IActionResult> Register()
        {
            var listBranch = await _branchService.GetAll();
            var listRole = await _roleService.GetAll();
            var listMenu = await _menuService.GetAll();

            AccountRegisterInputViewModel initView = new AccountRegisterInputViewModel();
            initView.Address = string.Empty;
            initView.Password = string.Empty;
            initView.Province = string.Empty;
            initView.BranchList = new MultiSelectList(listBranch, "Id", "BranchName", listBranch.Take(1).Select(x => x.Id.ToString()));
            initView.RoleList = new MultiSelectList(listRole, "Id", "RoleName", listRole.Take(1).Select(x => x.Id.ToString()));
            initView.MenuAccessList = new MultiSelectList(listMenu, "Id", "ScreenName", listMenu.Take(1).Select(x => x.Id.ToString()));
            initView.AccountName = string.Empty;
            initView.UserName = string.Empty;
            initView.PostalCode = string.Empty;

            return View(initView);
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
                var listBranch = await _branchService.GetAll();
                var listRole = await _roleService.GetAll();
                var listMenu = await _menuService.GetAll();

                request.BranchList = new MultiSelectList(listBranch, "Id", "BranchName", listBranch.Take(1).Select(x => x.Id.ToString()));
                request.RoleList = new MultiSelectList(listRole, "Id", "RoleName", listRole.Take(1).Select(x => x.Id.ToString()));
                request.MenuAccessList = new MultiSelectList(listMenu, "Id", "ScreenName", listMenu.Take(1).Select(x => x.Id.ToString()));

                return View(request);
            }

            var userLogin = string.IsNullOrEmpty(HttpContext.Session.GetString("AccountName")) ? "Guest" : HttpContext.Session.GetString("AccountName");

            if(request.Roles == null)
            {
                ModelState.AddModelError("Roles", "Role is required");
                return View(request);
            }

            if (request.MenuAccess == null)
            {
                ModelState.AddModelError("MenuAccess", "MenuAccess is required");
                return View(request);
            }

            if (request.Branches == null)
            {
                ModelState.AddModelError("Branches", "Branches is required");
                return View(request);
            }

            var branchList = await _branchService.GetAll();
            var selectedBranch = branchList.Where(x => request.Branches.Contains(x.Id)).ToList();

            var roleList = await _roleService.GetAll();
            var selectedRole = roleList.Where(x => request.Roles.Contains(x.Id)).ToList();

            var menuList = await _menuService.GetAll();
            var selectedMenu = menuList.Where(x => request.MenuAccess.Contains(x.Id)).ToList();

            try
            {
                // Add Account
                var newAccount = new Account()
                {
                    AccountName = request.AccountName,
                    Password = request.Password,
                    CreatedBy = userLogin,
                    CreatedDate = DateTime.Now,
                    RowStatus = true
                };

                newAccount = _accountService.Add(newAccount);

                // Add User
                var newUser = new User()
                {
                    UserName = request.UserName ?? string.Empty,
                    Address = request.Address ?? string.Empty,
                    PostalCode = request.PostalCode ?? string.Empty,
                    Province = request.Province ?? string.Empty,
                    BranchName = string.Join(",", selectedBranch.Select(x => x.BranchName)),
                    CreatedBy = userLogin,
                    CreatedDate = DateTime.Now,
                    RowStatus = true,
                    AccountId = newAccount.Id
                };

                newUser = _userService.Add(newUser);

                // Add Account Role
                List<AccountRole> assignedRoles = new List<AccountRole>();
                foreach(var role in selectedRole)
                {
                    assignedRoles.Add(new AccountRole
                    {
                        AccountId = newAccount.Id,
                        RoleId = role.Id,
                        CreatedBy = userLogin,
                        CreatedDate = DateTime.Now,
                        RowStatus = true,
                    });
                }
                _accountRoleService.Add(assignedRoles);

                // Add User Branch
                List<UserBranch> assignedBranches = new List<UserBranch>();
                foreach(var branch in selectedBranch)
                {
                    assignedBranches.Add(new UserBranch
                    {
                        UserId = newUser.Id,
                        BranchId = branch.Id,
                        CreatedBy = userLogin,
                        CreatedDate = DateTime.Now,
                        RowStatus = true,
                    });
                }
                _userBranchService.Add(assignedBranches);

                // Add Role Menu
                List<RoleMenu> assignedMenu = new List<RoleMenu>();
                foreach(var role in selectedRole)
                {
                    foreach(var menu in selectedMenu)
                    {
                        assignedMenu.Add(new RoleMenu
                        {
                            RoleId = role.Id,
                            MenuScreenId = menu.Id,
                            CreatedBy = userLogin,
                            CreatedDate = DateTime.Now,
                            RowStatus = true,
                        });
                    }
                }
                _roleMenuService.Add(assignedMenu);

                return RedirectToAction(nameof(Index));
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
