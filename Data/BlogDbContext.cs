using Blog.Models;
using Blog.Models.Comments;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Blog.Data
{
    public class BlogDbContext : IdentityDbContext
    {
        public BlogDbContext(DbContextOptions options) : base(options){ }

        public DbSet<Post> Posts { get; set; }
        public DbSet<SubComment> SubComments { get; set; }
        public DbSet<MainComment> MainComments{ get; set; }

    }
}
