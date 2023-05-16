using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using net_il_mio_fotoalbum.Models;

namespace net_il_mio_fotoalbum.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        [HttpGet]
        public IActionResult Photos(string? search)
        {
            Context db = new();
            List<Photo> photos = new();
            if (string.IsNullOrEmpty(search))
            {
                photos = db.Photos.Include(p => p.Category).Where(p => p.Visibility == true).ToList();
                return Ok(photos);
            }
            else
            {
                photos = db.Photos.Include(p => p.Category).Where(p => p.Visibility == true && p.Title.Contains(search)).ToList();
                return Ok(photos);
            }
        }
    }
}
