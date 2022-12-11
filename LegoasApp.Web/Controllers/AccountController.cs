using LegoasApp.Core.Common;
using LegoasApp.Core.Interfaces;
using LegoasApp.Core.Services;
using LegoasApp.Infrastructure.Models;
using LegoasApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Tls;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Principal;

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
            IConfiguration config,
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
                ViewBag.AccountId = accId;
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
            int accId = HttpContext.Session.GetInt32("AccountId").HasValue ? HttpContext.Session.GetInt32("AccountId").Value : 0;
            if (accId == 0)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.AccountId = accId;
                ViewBag.AccountName = HttpContext.Session.GetString("AccountName");
            }

            var listAccount = await _accountService.GetAllList();
            var result = listAccount.Select(x => new AccountViewModel
            {
                Id = x.Id,
                AccountName = x.AccountName,
                CreatedDate = x.CreatedDate,
                BranchNames = x.BranchNames,
                RoleName = x.RoleName,
                UserName = x.UserName,
            }).ToList();
            return View(result);
        }

        public async Task<IActionResult> AccountUserBranchSetting()
        {
            int accId = HttpContext.Session.GetInt32("AccountId").HasValue ? HttpContext.Session.GetInt32("AccountId").Value : 0;
            if (accId == 0)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.AccountId = accId;
                ViewBag.AccountName = HttpContext.Session.GetString("AccountName");
            }

            var listBranch = await _branchService.GetAll();
            var accSet = await _accountService.GetAccountUser(accId);

            AccountSettingViewModel acvm = new AccountSettingViewModel
            {
                AccountId = accSet.AccountId,
                AccountName = accSet.AccountName,
                Address = accSet.Address,
                BranchList = new MultiSelectList(listBranch, "Id", "BranchName", accSet.Branches),
                Branches = accSet.Branches,
                Password = accSet.Password,
                PostalCode = accSet.PostalCode,
                Province = accSet.Province,
                UserId = accSet.UserId,
                UserName = accSet.UserName,
            };

            return View(acvm);
        }

        public async Task<IActionResult> AccountRoleMenuSetting()
        {
            int accId = HttpContext.Session.GetInt32("AccountId").HasValue ? HttpContext.Session.GetInt32("AccountId").Value : 0;
            if (accId == 0)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.AccountId = accId;
                ViewBag.AccountName = HttpContext.Session.GetString("AccountName");
            }
            var listRole = await _roleService.GetAll();
            var listMenu = await _menuService.GetAll();
            var accSet = await _accountService.GetAccountUser(accId);

            AccountSettingViewModel acvm = new AccountSettingViewModel
            {
                AccountId = accSet.AccountId,
                UserId = accSet.UserId,
                Roles = accSet.Roles,
                MenuAccess = accSet.MenuAccess,
                RoleList = new MultiSelectList(listRole, "Id", "RoleName", accSet.Roles),
                MenuAccessList = new MultiSelectList(listMenu, "Id", "ScreenName", accSet.MenuAccess),
            };
            return View(acvm);
        }
        #endregion

        #region Input
        [HttpPost]
        public async Task<IActionResult> Register(AccountRegisterInputViewModel request)
        {
            #region Re-initialize view
            ViewBag.RegErrMsg = string.Empty;
            var listBranch = await _branchService.GetAll();
            var listRole = await _roleService.GetAll();
            var listMenu = await _menuService.GetAll();

            request.BranchList = new MultiSelectList(listBranch, "Id", "BranchName", listBranch.Take(1).Select(x => x.Id.ToString()));
            request.RoleList = new MultiSelectList(listRole, "Id", "RoleName", listRole.Take(1).Select(x => x.Id.ToString()));
            request.MenuAccessList = new MultiSelectList(listMenu, "Id", "ScreenName", listMenu.Take(1).Select(x => x.Id.ToString()));
            #endregion

            if (!ModelState.IsValid)
            {
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

            var selectedBranch = listBranch.Where(x => request.Branches.Contains(x.Id)).ToList();

            var selectedRole = listRole.Where(x => request.Roles.Contains(x.Id)).ToList();

            var selectedMenu = listMenu.Where(x => request.MenuAccess.Contains(x.Id)).ToList();

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

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create account");
                ViewBag.RegErrMsg = "Failed to create account";
                return View(request);
            }
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

            return RedirectToAction(nameof(AccountUserBranchSetting), "Account");
        }

        [HttpPost]
        public async Task<IActionResult> AccountUserBranchSetting(AccountSettingViewModel request)
        {
            #region Re-initialize view
            ViewBag.AccSetErrMsg = string.Empty;
            var listBranch = await _branchService.GetAll();

            request.BranchList = new MultiSelectList(listBranch, "Id", "BranchName", listBranch.Take(1).Select(x => x.Id.ToString()));
            #endregion

            int accId = HttpContext.Session.GetInt32("AccountId").HasValue ? HttpContext.Session.GetInt32("AccountId").Value : 0;
            if (accId == 0)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.AccountId = accId;
                ViewBag.AccountName = HttpContext.Session.GetString("AccountName");
            }

            try
            {
                string userLogin = ViewBag.AccountName;

                if (request.Branches == null)
                {
                    ModelState.AddModelError("Branches", "Branches is required");
                    return View(request);
                }

                var selectedBranch = listBranch.Where(x => request.Branches.Contains(x.Id)).ToList();
                request.BranchList = new MultiSelectList(listBranch, "Id", "BranchName", selectedBranch.Select(x => x.Id));

                Account updAccount = _accountService.GetById(accId);
                if(request.AccountName != null)
                    updAccount.AccountName = request.AccountName;
                if(request.Password != null)
                    updAccount.Password = request.Password;

                updAccount.ModifiedBy = userLogin;
                updAccount.ModifiedDate = DateTime.Now;

                _accountService.Update(accId, updAccount);

                User updUser = await _userService.GetById(request.UserId.Value);
                updUser.Address = request.Address;
                updUser.UserName = request.UserName;
                updUser.BranchName = string.Join(",", selectedBranch.Select(x => x.BranchName));
                updUser.PostalCode = request.PostalCode;
                updUser.Province = request.Province;

                _userService.Update(updUser.Id, updUser, userLogin);

                List<UserBranch> assignedBranches = new List<UserBranch>();
                foreach (var branch in selectedBranch)
                {
                    assignedBranches.Add(new UserBranch
                    {
                        UserId = updUser.Id,
                        BranchId = branch.Id,
                        CreatedBy = userLogin,
                        CreatedDate = DateTime.Now,
                        RowStatus = true,
                    });
                }

                _userBranchService.Update(assignedBranches);

                ViewBag.AccSetMsg = "Successfully updated";

                return View(request);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to update account");
                ViewBag.AccSetErrMsg = "Failed to update account";
                return View(request);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AccountRoleMenuSetting(AccountSettingViewModel request)
        {
            #region Re-initialize view
            ViewBag.AccSetErrMsg = string.Empty;
            var listRole = await _roleService.GetAll();
            var listMenu = await _menuService.GetAll();

            request.RoleList = new MultiSelectList(listRole, "Id", "RoleName", listRole.Take(1).Select(x => x.Id.ToString()));
            request.MenuAccessList = new MultiSelectList(listMenu, "Id", "ScreenName", listMenu.Take(1).Select(x => x.Id.ToString()));
            #endregion

            int accId = HttpContext.Session.GetInt32("AccountId").HasValue ? HttpContext.Session.GetInt32("AccountId").Value : 0;
            if (accId == 0)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.AccountId = accId;
                ViewBag.AccountName = HttpContext.Session.GetString("AccountName");
            }

            try
            {
                string userLogin = ViewBag.AccountName;

                if (request.Roles == null)
                {
                    ModelState.AddModelError("Roles", "Role is required");
                    return View(request);
                }

                if (request.MenuAccess == null)
                {
                    ModelState.AddModelError("MenuAccess", "MenuAccess is required");
                    return View(request);
                }

                var selectedRole = listRole.Where(x => request.Roles.Contains(x.Id)).ToList();
                var selectedMenu = listMenu.Where(x => request.MenuAccess.Contains(x.Id)).ToList();

                request.RoleList = new MultiSelectList(listRole, "Id", "RoleName", selectedRole.Select(x => x.Id));
                request.MenuAccessList = new MultiSelectList(listMenu, "Id", "ScreenName", selectedMenu.Select(x => x.Id));

                List<AccountRole> assignedRoles = new List<AccountRole>();
                foreach (var role in selectedRole)
                {
                    assignedRoles.Add(new AccountRole
                    {
                        AccountId = accId,
                        RoleId = role.Id,
                        CreatedBy = userLogin,
                        CreatedDate = DateTime.Now,
                        RowStatus = true,
                    });
                }

                _accountRoleService.Update(assignedRoles);

                List<RoleMenu> assignedMenu = new List<RoleMenu>();
                foreach (var role in selectedRole)
                {
                    foreach (var menu in selectedMenu)
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

                _roleMenuService.Update(assignedMenu);

                ViewBag.AccSetMsg = "Successfully updated";

                return View(request);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to update account role");
                ViewBag.AccSetErrMsg = "Failed to update account role";
                return View(request);
            }
        }
        #endregion

    }
}
