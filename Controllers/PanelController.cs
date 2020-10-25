using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Data.FileManager;
using Blog.Data.Repository;
using Blog.Models;
using Blog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PanelController : Controller
    {
        private readonly IRepository _repo;
        private readonly IFileManager _fileManager;
        public PanelController(IRepository repo, IFileManager fileManager)
        {
            _repo = repo;
            _fileManager = fileManager;
        }

        public IActionResult Index()
        {
            var posts = _repo.GetAllPosts();
            return View(posts);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return View(new PostViewModel());
            }
            else
            {
                var post = _repo.GetPost((int)id);
                return View(new PostViewModel { 
                    Post_Id = post.Post_Id,
                    Title =  post.Title,
                    Body = post.Body,
                    CurrentImage = post.Image,
                    Description = post.Description,
                    Tags = post.Tags,
                    Category = post.Category
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit(PostViewModel vm)
        {
            var post = new Post
            {
                Post_Id = vm.Post_Id,
                Title = vm.Title,
                Body = vm.Body,
                Description = vm.Description,
                Tags = vm.Tags,
                Category = vm.Category
            };

            if (vm.CurrentImage == null && vm.Image == null)
            {
                ModelState.AddModelError(string.Empty, "Image Required");
                return View(vm);
            }

            if (vm.Image == null)
                post.Image = vm.CurrentImage;
            else
                if (!string.IsNullOrEmpty(vm.CurrentImage))
                {
                    _fileManager.RemoveImage(vm.CurrentImage);
                    post.Image = await _fileManager.SaveImage(vm.Image);
                }
            else
                if(vm.Image != null && vm.CurrentImage == null)
                {
                    post.Image = await _fileManager.SaveImage(vm.Image);
                }


            if (post.Post_Id > 0)
                _repo.UpdatePost(post);
            else
                _repo.AddPost(post);

            if (await _repo.SaveChangesAsync())
                return RedirectToAction("Index");
            else
                return View(post);
        }

        public async Task<IActionResult> Delete(int id)
        {
            _repo.RemovePost(id);
            await _repo.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
