using Microsoft.AspNetCore.Mvc;
using OnlineShoppingCart.Data.Entities;

namespace OnlineShoppingCart.Components;

[ViewComponent]
public class CategorySidebar : ViewComponent
{
    public class CategorySidebarData
    {
        public List<Category> Categories { set; get; }
        public int Level { set; get; }
        public string SlugCategory { set; get; }
    }

    public const string COMPONENTNAME = "CategorySidebar";
    public CategorySidebar() { }
    public IViewComponentResult Invoke(CategorySidebarData data)
    {
        return View(data);
    }
}
