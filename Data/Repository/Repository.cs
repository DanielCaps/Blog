using Blog.Helpers;
using Blog.Models;
using Blog.Models.Comments;
using Blog.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Data.Repository
{
    public class Repository : IRepository
    {
        private readonly BlogDbContext _ctx;

        public Repository(BlogDbContext ctx)
        {
            _ctx = ctx;
        }

        public void AddPost(Post post)
        {
            _ctx.Add(post);
        }
        public List<Post> GetAllPosts()
        {
            return _ctx.Posts.ToList();
        }

        public IndexViewModel GetAllPosts(int pageNumber, string category, string search)
        {
            var query = _ctx.Posts.AsNoTracking().AsQueryable();

            int pageSize = 3;
            int skipAmount = pageSize * (pageNumber - 1);

            if (!String.IsNullOrEmpty(category))
            {
                query = query.Where(x => x.Category == category);
            }
            if (!String.IsNullOrEmpty(search))
                query = query.Where(x => EF.Functions.Like(x.Title, $"%{search}%")
                                    || EF.Functions.Like(x.Body, $"%{search}%")
                                    || EF.Functions.Like(x.Description, $"%{search}%"));


            int postsCount = query.Count();
            int pageCount = (int)Math.Ceiling(postsCount * 1.0 / pageSize);

            return new IndexViewModel
            {
                PageNumber = pageNumber,
                PageCount = pageCount,
                NextPage = postsCount > skipAmount + pageSize,
                Pages = PageHelper.PageNumbers(pageNumber, pageCount).ToList(),
                Category = category,
                Search = search,
                Posts = query
                    .Skip(skipAmount)
                    .Take(pageSize)
                    .ToList()
            };
        }

        public Post GetPost(int id)
        {
            return _ctx.Posts.Include(p => p.MainComments).ThenInclude(mc => mc.SubComments).FirstOrDefault(p => p.Post_Id == id);
        }

        public void RemovePost(int id)
        {
            _ctx.Remove(GetPost(id));
        }

        public void UpdatePost(Post post)
        {
            _ctx.Posts.Update(post);
        }

        public async Task<bool> SaveChangesAsync()
        {
            if (await _ctx.SaveChangesAsync() > 0)
            {
                return true;
            }
            return false;
        }

        public void AddSubComment(SubComment comment)
        {
            _ctx.SubComments.Add(comment);
        }
    }
}
