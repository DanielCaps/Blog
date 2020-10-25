using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Blog.Models;
using Blog.Data.Repository;
using Blog.Data.FileManager;
using Blog.ViewModels;
using System.Collections.Generic;
using Blog.Models.Comments;
using System;

namespace Blog.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository _repo;
        private readonly IFileManager _fileManager;
        public HomeController(IRepository repo, IFileManager fileManager)
        {
            _repo = repo;
            _fileManager = fileManager;
        }


        public IActionResult Index(int pageNumber, string category, string search)
        {
            if (pageNumber < 1)
                return RedirectToAction("Index", new { pageNumber = 1, category });

            var vm = _repo.GetAllPosts(pageNumber, category, search);

            return View(vm);
        }
        public IActionResult Post(int id)
        {
            var post = _repo.GetPost(id);

            return View(post);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Post());
        }
        [HttpPost]
        public async Task<IActionResult> Create(Post post)
        {
            _repo.AddPost(post);
            if (await _repo.SaveChangesAsync())
                return RedirectToAction("Index");
            else
                return View(post);
        }

        [HttpGet("/Image/{image}")]
        [ResponseCache(CacheProfileName = "Monthly")]
        public IActionResult Image(string image)
        {
            var mime = image.Substring(image.LastIndexOf('.') + 1);
            return new FileStreamResult(_fileManager.ImageStream(image), $"image/{mime}");
        }

        [HttpPost]
        public async Task<IActionResult> Comment(CommentViewModel vm)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Post", new { id = vm.PostId });

            var post = _repo.GetPost(vm.PostId);
            if (vm.MainCommentId == 0)
            {
                post.MainComments = post.MainComments ?? new List<MainComment>();

                post.MainComments.Add(new MainComment
                {
                    Message = vm.Message,
                    Created = DateTime.Now,

                });
                _repo.UpdatePost(post);
            }
            else
            {
                var comment = new SubComment
                {
                    MainCommentID = vm.MainCommentId,
                    Message = vm.Message,
                    Created = DateTime.Now,
                };
                _repo.AddSubComment(comment);

            }

            await _repo.SaveChangesAsync();
            return RedirectToAction("Post", new { id = vm.PostId });
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
