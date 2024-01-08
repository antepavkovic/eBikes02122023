using Bikes.Models;
using Bikes.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace Bikes.Controllers
{
    public class UserController:Controller
    {
        
        
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            
            _context = context;
            _userManager = userManager;

        }


        public IActionResult Index()
        {
            var users = _context.Users.ToList();
            var userRoles = _context.UserRoles.ToList();
            var roles = _context.Roles.ToList();
            foreach (var user in users)
            {
                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;

            }
            return View(users);
        }

        public IActionResult RoleManagment(string id)
        { 
            

            string RoleID =_context.UserRoles.FirstOrDefault(u => u.UserId ==id).RoleId;
           
            RoleManagmentVM Roleman = new RoleManagmentVM()
            {
                ApplicationUser = _context.Users.FirstOrDefault(u => u.Id == id),
                RoleList = _context.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                })
            };
            
            Roleman.ApplicationUser.Role = _context.Roles.FirstOrDefault(u => u.Id == RoleID).Name;
          

            return View(Roleman);
        }
        [HttpPost]

        public IActionResult RoleManagment(RoleManagmentVM roleManagmentVM)
        {
            string RoleID = _context.UserRoles.FirstOrDefault(u => u.UserId == roleManagmentVM.ApplicationUser.Id).RoleId;
            string oldRole = _context.Roles.FirstOrDefault(u => u.Id == RoleID).Name;
            if (!(roleManagmentVM.ApplicationUser.Role == oldRole))
            {
                ApplicationUser applicationUser = _context.Users.FirstOrDefault(u => u.Id == roleManagmentVM.ApplicationUser.Id);
                _context.SaveChanges();
                _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(applicationUser, roleManagmentVM.ApplicationUser.Role).GetAwaiter().GetResult();
            }
            
            return RedirectToAction("Index");
        }
        public IActionResult Delete(string id)
        {
          
            var user =   _context.Users.FirstOrDefault(m => m.Id == id);
            if (user != null)
            {
                 _context.Remove(user);
            

            }
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        
    }
}
