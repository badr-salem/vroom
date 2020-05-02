using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vroom.AppDbContext;
using vroom.Models;
using vroom.Models.ViewModels;
using System.IO;
using Microsoft.AspNetCore.Hosting.Internal;
using cloudscribe.Pagination.Models;
using vroom.Helpers;

namespace vroom.Controllers
{
    [Authorize(Roles = Roles.Admin + ","+Roles.Executive)]
    public class BikeController : Controller
    {
       
        private readonly VroomDbContext _db;
        private readonly HostingEnvironment _hostingEnvironment;

        [BindProperty]
        public BikeViewModel BikeVM { get; set; }

        public BikeController(VroomDbContext db , HostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _db = db;
            BikeVM = new BikeViewModel()
            {
                Makes = _db.Makes.ToList(),
                Models = _db.Models.ToList(),
                Bike = new Models.Bike()
            };
        }

        [AllowAnonymous]
        public IActionResult Index( string SearchString,string SortOrder ,int PageNumber = 1 , int PageSize = 2)
        {
            ViewBag.CurrentFilter = SearchString;
            ViewBag.CurrentSortOrder = SortOrder;
            ViewBag.PriceSortParam =String.IsNullOrEmpty(SortOrder)? "Price_Desc" : "";
            int ExcludeRecords = (PageSize * PageNumber) - PageSize;
            var Bikes = from b in _db.Bikes.Include(m => m.Make).Include(m => m.Model) select b;
            var BikeCount = Bikes.Count();
            if (!String.IsNullOrEmpty(SearchString))
            {
                Bikes = Bikes.Where(b => b.Make.Name.Contains(SearchString));
                BikeCount = Bikes.Count();
            }

            //sorting Logic
            switch (SortOrder)
            {
                case "Price_Desc":
                    Bikes = Bikes.OrderByDescending(b => b.Price);
                    break;

                default:
                    Bikes = Bikes.OrderBy(b => b.Price);
                    break;
            }
            
            Bikes = Bikes.Skip(ExcludeRecords).Take(PageSize);

            var Result = new PagedResult<Bike>
            {
                Data = Bikes.AsNoTracking().ToList(),
                TotalItems = BikeCount,
                PageNumber = PageNumber,
                PageSize = PageSize
            };
            return View(Result);
        }

        public IActionResult Create()
        {
            return View(BikeVM);
        }

        [HttpPost, ActionName("Create")]
        public IActionResult CreatePost()
        {
            if (!ModelState.IsValid)
            {
                BikeVM.Makes = _db.Makes.ToList();
                BikeVM.Models = _db.Models.ToList();
                return View(BikeVM);
            }

                _db.Bikes.Add(BikeVM.Bike);
                UploadImageIfAvailable();

                _db.SaveChanges();
                        
            return RedirectToAction(nameof(Index));
        }



        public void UploadImageIfAvailable()
        {

            //////////////
            ///save logic image bike/////
            /////////////

            //Get bike Id  we have save in database
            var BikeID = BikeVM.Bike.Id;

            //Get wwwrootpath to save the file on server
            string wwwrootpath = _hostingEnvironment.WebRootPath;

            //Get the upload file
            var files = HttpContext.Request.Form.Files;

            //Get the refrence of DBSet for the bike we just saved in the database
            var SavedBike = _db.Bikes.Find(BikeID);

            //Upload the files on the server and save image path of user have uploaded any files
            if (files.Count > 0)
            {
                var ImagePath = @"images\bikes\";
                var Extension = Path.GetExtension(files[0].FileName);
                var RelativeImagePath = ImagePath + BikeID + Extension;
                var AbsImagePath = Path.Combine(wwwrootpath, RelativeImagePath);

                //upload the file on server
                using (var filestream = new FileStream(AbsImagePath, FileMode.Create))
                {
                    files[0].CopyTo(filestream);
                }

                //set the image path on database
                SavedBike.ImagePath = RelativeImagePath;
            }
        }


        //public IActionResult Edit(int id)
        //{
        //    ModelVM.Model = _db.Models.Include(m => m.Make).SingleOrDefault(m => m.Id == id);
        //    if (ModelVM.Model == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(ModelVM);
        //}

        //[HttpPost, ActionName("Edit")]
        //public IActionResult EditPost()
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(ModelVM);
        //    }

        //    _db.Update(ModelVM.Model);
        //    _db.SaveChanges();
        //    return RedirectToAction(nameof(Index));
        //}


            public IActionResult Edit(int Id)
        {
            BikeVM.Bike = _db.Bikes.SingleOrDefault(b => b.Id == Id);

            //filter the models in dropdowm list with assioted make
            BikeVM.Models = _db.Models.Where(m => m.MakeId == BikeVM.Bike.MakeID);

            if(BikeVM.Bike == null)
            {
                return NotFound();
            }

            return View(BikeVM);    
        }


        [AllowAnonymous]
        public IActionResult View(int Id)
        {
            BikeVM.Bike = _db.Bikes.SingleOrDefault(b => b.Id == Id);

            if (BikeVM.Bike == null)
            {
                return NotFound();
            }

            return View(BikeVM);
        }


        [HttpPost, ActionName("Edit")]
        public IActionResult EditPost()
        {
            if (!ModelState.IsValid)
            {
                BikeVM.Makes = _db.Makes.ToList();
                BikeVM.Models = _db.Models.ToList();
                return View(BikeVM);
            }

            _db.Bikes.Update(BikeVM.Bike);
            UploadImageIfAvailable();

            _db.SaveChanges();

            return RedirectToAction(nameof(Index));
        }



        [HttpPost]
        public IActionResult Delete(int id)
        {
            Bike bike = _db.Bikes.Find(id);
            if (bike == null)
            {
                return NotFound();
            }

            _db.Bikes.Remove(bike);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}