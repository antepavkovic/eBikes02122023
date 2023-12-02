using Bikes.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bikes.ViewModels
{
    public class RoleManagmentVM
    {
        public ApplicationUser ApplicationUser { get; set; }
        public IEnumerable<SelectListItem> RoleList { get; set; }


    }
}
