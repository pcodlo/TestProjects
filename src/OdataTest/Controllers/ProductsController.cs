using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;
using OdataTest.Models;

namespace OdataTest.Controllers
{
    public class ProductsController : ODataController
    {
        private readonly ProductsContext _db = new ProductsContext();

        private bool ProductExists(int key)
        {
            return _db.Products.Any(p => p.Id == key);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

        //[EnableQuery]
        //public IQueryable<Product> Get()
        //{
        //    return _db.Products;
        //}

        public IQueryable<Product> Get(ODataQueryOptions opts)
        {
            var settings = new ODataValidationSettings()
            {
                // Initialize settings as needed.
                AllowedFunctions = AllowedFunctions.AllFunctions
            };

            opts.Validate(settings);

            IQueryable results = opts.ApplyTo(_db.Products.AsQueryable());
            return results as IQueryable<Product>;
        }

        [EnableQuery]
        public SingleResult<Product> Get([FromODataUri] string key)
        {
            var result = int.TryParse(key, out int intKey) ? _db.Products.Where(p => p.Id == intKey) : _db.Products.Where(p => p.Name == key);
            return SingleResult.Create(result);
        }

        public async Task<IHttpActionResult> Post(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            return Created(product);
        }

        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Product> product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var entity = await _db.Products.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }
            product.Patch(entity);
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Updated(entity);
        }

        public async Task<IHttpActionResult> Put([FromODataUri] int key, Product update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (key != update.Id)
            {
                return BadRequest();
            }
            _db.Entry(update).State = EntityState.Modified;
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Updated(update);
        }

        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            var product = await _db.Products.FindAsync(key);
            if (product == null)
            {
                return NotFound();
            }
            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}