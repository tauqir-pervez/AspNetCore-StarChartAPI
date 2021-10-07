using System.Linq;

using Microsoft.AspNetCore.Mvc;

using StarChart.Data;

namespace StarChart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);
            if (celestialObject == null)
            {
                return new NotFoundResult();
            }

            var satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId.HasValue && c.OrbitedObjectId == id).Select(c => c).ToList();
            celestialObject.Satellites = satellites;
            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObject = _context.CelestialObjects.FirstOrDefault(c => c.Name == name);
            if (celestialObject == null)
            {
                return new NotFoundResult();
            }

            var satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId.HasValue && c.OrbitedObjectId == celestialObject.Id).Select(c => c).ToList();
            celestialObject.Satellites = satellites;
            return Ok(celestialObject);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects.ToList();
            foreach (var celestialObject in celestialObjects)
            {
                var satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId.HasValue && c.OrbitedObjectId == celestialObject.Id).Select(c => c).ToList();
                celestialObject.Satellites = satellites;
            }
            return Ok(celestialObjects);
        }
    }
}
