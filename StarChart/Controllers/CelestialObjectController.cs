using System.Diagnostics;
using System.Linq;

using Microsoft.AspNetCore.Mvc;

using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            Debugger.Break();
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
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
            var celestialObjects = _context.CelestialObjects.Where(c => c.Name == name).Select(c => c);
            if (!celestialObjects.Any())
            {
                return new NotFoundResult();
            }

            foreach (var celestialObject in celestialObjects)
            {
                var satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId.HasValue && c.OrbitedObjectId == celestialObject.Id).Select(c => c).ToList();
                celestialObject.Satellites = satellites;
            }

            return Ok(celestialObjects);
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

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();
            return CreatedAtRoute("GetById", routeValues: new { id = celestialObject.Id }, value: celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject updatedCelestialObject)
        {
            var celestialObject = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);
            if (celestialObject == null)
            {
                return NotFound();
            }

            celestialObject.Name = updatedCelestialObject.Name;
            celestialObject.OrbitedObjectId = updatedCelestialObject.OrbitedObjectId;
            celestialObject.OrbitalPeriod = updatedCelestialObject.OrbitalPeriod;

            _context.CelestialObjects.Update(celestialObject);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var celestialObject = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);
            if (celestialObject == null)
            {
                return NotFound();
            }

            celestialObject.Name = name;

            _context.CelestialObjects.Update(celestialObject);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestialObjects = _context.CelestialObjects.Where(c => c.Id == id || (c.OrbitedObjectId.HasValue && c.OrbitedObjectId == id)).Select(c => c);

            if (!celestialObjects.Any())
            {
                return new NotFoundResult();
            }

            _context.CelestialObjects.RemoveRange(celestialObjects);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
