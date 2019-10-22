using AutoMapper;
using EducNotes.API.Data;
using EducNotes.API.Dtos;
using EducNotes.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EducNotes.API.Controllers
{
      [Route("api/[controller]")]
    [ApiController]

    public class TresoController: ControllerBase
    {
         private readonly DataContext _context;
        private readonly IEducNotesRepository _repo;
        private readonly IMapper _mapper;

    
        public TresoController(DataContext context, IEducNotesRepository repo,IMapper mapper)
        {
            _context = context;
            _repo = repo;
            _mapper = mapper;
        }

         [HttpGet("GetProductTypes")]
         public async Task<IActionResult> GetProductTypes()
         {
             var producTypes= await _context.ProductTypes.OrderBy(p => p.Name).ToListAsync();
             return Ok(producTypes);
         }

         [HttpGet("GetDeadLines")]
         public async Task<IActionResult> GetDeadLines()
         {
             var deadLines= await _context.DeadLines.OrderBy(p => p.DueDate).ToListAsync();
             return Ok(deadLines);
         }

         [HttpGet("GetProducts")]
         public async Task<IActionResult> GetProducts()
         {
             var products= await _context.Products.OrderBy(p => p.Name).ToListAsync();
             return Ok(products);
         }

         [HttpGet("GetDeadLine/{deadLineId}")]
         public async Task<IActionResult> GetDeadLine(int deadLineId)
         {
             var deadLine= await _context.DeadLines.FirstOrDefaultAsync(p => p.Id == deadLineId);
             return Ok(deadLine);
         }

         [HttpGet("GetProduct/{productId}")]
         public async Task<IActionResult> GetProduct(int productId)
         {
             var product= await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
             return Ok(product);
         }

         [HttpGet("GetProducts/{productTypeId}")]
         public async Task<IActionResult> GetProducts(int productTypeId)
         {
             var producTypes= await _context.Products.
             Where(p => p.ProductTypeId ==  productTypeId)
             .OrderBy(p => p.Name)
             .ToListAsync();
             return Ok(producTypes);
         }

         [HttpGet("GetLvlServices/{classLevelId}")]
         public async Task<IActionResult> GetLvlServices(int classLevelId)
         {
             var services= await _context.ClassLevelProducts
                                        .Include(p =>p.Product)
                                        .Where(p => p.ClasssLevelId ==  classLevelId)
                                        .OrderBy(p => p.Product.Name)
                                        .ToListAsync();
             return Ok(services);
         }

         [HttpPost("CreateProduct")]
         public async Task<IActionResult> CreateProduct(ProductDto productToCreate)
         {
            var produit = _mapper.Map<Product>(productToCreate);
             _context.Products.Add(produit);
             if(await _repo.SaveAll())
             return Ok();

             return BadRequest("impossible de creer ce produit");
         }

          [HttpPost("CreateClassLevelProduct")]
         public async Task<IActionResult> CreateClassLevelProduct(ClasslevelProductDto lvlprodToCreate)
         {
             decimal amount = lvlprodToCreate.Amount;
             int productId = lvlprodToCreate.ProductId;

             foreach (var levelid in lvlprodToCreate.ClassLevelIds)
             {
               var lvlprod = await _context.ClassLevelProducts.FirstOrDefaultAsync(a => a.ClasssLevelId == levelid && a.ProductId == productId);   
               if(lvlprod == null){
                 var lp = new ClassLevelProduct{ProductId = productId, ClasssLevelId = levelid , Amount = amount};
                 _repo.Add(lp);
               }
             }

             if(await _repo.SaveAll())
             return Ok();

             return BadRequest("impossible de faire l'enregistrement...");
            
         }

         [HttpPost("CreateDeadLine")]
         public async Task<IActionResult> CreateDeadLine(DeadLineDto dtToCreate)
         {
           var dealdline = _mapper.Map<DeadLine>(dtToCreate);
           _repo.Add(dealdline);
           if(await _repo.SaveAll())
          return Ok();

          return BadRequest("impossible de terminer l'enregistrement");
            
         } 

         [HttpPost("EditDeadLine/{deadLineId}")]
         public async Task<IActionResult> EditDeadLine(int deadLineId,DeadLineDto dtToCreate)
         {
           var dl = await _context.DeadLines.FirstOrDefaultAsync(d => d.Id == deadLineId);
           if(dtToCreate != null)
           {
             dl.Name = dtToCreate.Name;
             dl.Comment = dtToCreate.Comment;
             dl.DueDate = dtToCreate.DueDate;
             dl.Percentage = dtToCreate.Percentage;
             _repo.Update(dl);
             if(await _repo.SaveAll())
             return Ok();

            return BadRequest("impossible de terminer l'enregistrement");
           }
           return NotFound();
         }


         [HttpPost("EditProduct/{productId}")]
         public async Task<IActionResult> EditProduct(int productId,ProductDto prodToCreate)
         {
           var product = await _context.Products.FirstOrDefaultAsync(d => d.Id == productId);
           if(product != null)
           {
             product.Name = prodToCreate.Name;
             product.Comment = prodToCreate.Comment;
               _repo.Update(product);
             if(await _repo.SaveAll())
             return Ok();

            return BadRequest("impossible de terminer l'enregistrement");
           }
           return NotFound();
         }

    }
}