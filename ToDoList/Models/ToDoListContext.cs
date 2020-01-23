using Microsoft.EntityFrameworkCore;

namespace ToDoList.Models
{
  public class ToDoListContext : DbContext
  {
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Item> Items { get; set; }   // virtual here?
    
    public ToDoListContext(DbContextOptions options) : base(options) { }
  }
}