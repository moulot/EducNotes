using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EducNotes.API.data;
using EducNotes.API.Data;
using EducNotes.API.Dtos;
using EducNotes.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EducNotes.API.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class ProductsController : ControllerBase
  {
    private readonly DataContext _context;
    private readonly IEducNotesRepository _repo;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;
    private readonly UserManager<User> _userManager;
    CultureInfo frC = new CultureInfo("fr-FR");
    public ICacheRepository _cache { get; }
    int tuitionId, serviceTypeId, schoolingTypeId;

    public ProductsController(DataContext context, UserManager<User> userManager, IConfiguration config,
      IEducNotesRepository repo, IMapper mapper, ICacheRepository cache)
    {
      _cache = cache;
      _userManager = userManager;
      _config = config;
      _context = context;
      _repo = repo;
      _mapper = mapper;
      tuitionId = _config.GetValue<int>("AppSettings:tuitionId");
      serviceTypeId = _config.GetValue<int>("AppSettings:serviceTypeId");
      schoolingTypeId = _config.GetValue<int>("AppSettings:schoolingTypeId");
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
      List<Product> productsCached = await _cache.GetProducts();
      List<ProductDeadLine> productDueDates = await _cache.GetProductDeadLines();

      List<ProductForConfigDto> products = _mapper.Map<List<ProductForConfigDto>>(productsCached);
      foreach (var product in products)
      {
        List<ProductDeadLine> dueDates = productDueDates.OrderBy(o => o.DueDate).Where(d => d.ProductId == product.Id).ToList();
        product.DueDates = _mapper.Map<List<ProductDeadlineDto>>(dueDates);
      }

      return Ok(products);
    }

    [HttpGet("Services")]
    public async Task<IActionResult> GetServices()
    {
      List<Product> productsCached = await _cache.GetProducts();
      List<ProductDeadLine> productDueDates = await _cache.GetProductDeadLines();
      List<ProductZone> productZones = await _cache.GetProductZones();

      List<Product> services = productsCached.Where(p => p.ProductTypeId == serviceTypeId).ToList();
      List<ProductForConfigDto> products = _mapper.Map<List<ProductForConfigDto>>(services);
      foreach(var product in products)
      {
        List<ProductDeadLine> dueDates = productDueDates.OrderBy(o => o.DueDate).Where(d => d.ProductId == product.Id).ToList();
        product.DueDates = _mapper.Map<List<ProductDeadlineDto>>(dueDates);

        List<ProductZone> zones = productZones.OrderBy(o => o.Zone.Name).Where(p => p.ProductId == product.Id).ToList();
        product.Zones = _mapper.Map<List<ProductZoneDto>>(zones);
      }

      return Ok(products);
    }

    [HttpGet("{id}", Name = "GetProduct")]
    public async Task<IActionResult> GetProduct(int id)
    {
      List<Product> productsCached = await _cache.GetProducts();
      List<ProductDeadLine> productDueDates = await _cache.GetProductDeadLines();
      List<ClassLevelProduct> classLevelProducts = await _cache.GetClassLevelProducts();

      Product productFromDB = productsCached.First(p => p.Id == id);
      ProductForConfigDto product = _mapper.Map<ProductForConfigDto>(productFromDB);
      List<ProductDeadLine> dueDates = productDueDates.OrderBy(o => o.DueDate).Where(d => d.ProductId == product.Id).ToList();
      product.DueDates = _mapper.Map<List<ProductDeadlineDto>>(dueDates);
      List<ClassLevelProduct> levelPrices = classLevelProducts.Where(c => c.ProductId == product.Id).ToList();
      product.LevelPrices = _mapper.Map<List<ClasslevelProductDto>>(levelPrices);

      return Ok(product);
    }

    [HttpGet("ProductTypes")]
    public async Task<IActionResult> GetProductTypes()
    {
      List<ProductType> productTypes = await _cache.GetProductTypes();
      return Ok(productTypes);
    }

    [HttpGet("GetDeadLines")]
    public async Task<IActionResult> GetDeadLines()
    {
      var deadLines = await _context.DeadLines.OrderBy(p => p.DueDate).ToListAsync();
      var res = _mapper.Map<IEnumerable<DealLineDetailsDto>>(deadLines);
      return Ok(res);
    }

    [HttpGet("Periodicities")]
    public async Task<IActionResult> GetPeriodicicities()
    {
      List<Periodicity> periodicities = await _repo.GetPeriodicities();
      return Ok(periodicities);
    }

    [HttpGet("PayableAts")]
    public async Task<IActionResult> GetPayableAts()
    {
      var payableAts = await _repo.GetPayableAts();
      return Ok(payableAts);
    }

    [HttpGet("ProductData")]
    public async Task<IActionResult> GetProductData()
    {
      List<Periodicity> periodicities = await _repo.GetPeriodicities();
      List<PayableAt> payableAts = await _repo.GetPayableAts();

      return Ok(new {
        periodicities,
        payableAts
      });
    }

    [HttpGet("GetProducts/{productTypeId}")]
    public async Task<IActionResult> GetProductsByTypeId(int productTypeId)
    {
      var productsByType = await _context.Products.Where(p => p.ProductTypeId == productTypeId)
                                                  .OrderBy(p => p.Name)
                                                  .ToListAsync();
      return Ok(productsByType);
    }

    [HttpGet("GetLvlServices/{classLevelId}")]
    public async Task<IActionResult> GetLvlServices(int classLevelId)
    {
      var services = await _context.ClassLevelProducts.Include(p => p.Product)
                                                      .Where(p => p.ClassLevelId == classLevelId)
                                                      .OrderBy(p => p.Product.Name)
                                                      .ToListAsync();
      return Ok(services);
    }

    [HttpGet("Getperiodicity/{periodicityId}")]
    public async Task<IActionResult> Getperiodicity(int periodicityId)
    {
      var pp = await _context.Periodicities.FirstOrDefaultAsync(p => p.Id == periodicityId);
      return Ok(pp);
    }

    [HttpGet("GetpayableAt/{payableAtid}")]
    public async Task<IActionResult> GetpayableAt(int payableAtid)
    {
      var pp = await _context.PayableAts.FirstOrDefaultAsync(p => p.Id == payableAtid);
      return Ok(pp);
    }

    [HttpGet("ClassLevelProducts")]
    public async Task<IActionResult> GetClassLevelProducts()
    {
      var levelProdFromDB = await _context.ClassLevelProducts.Include(i => i.Product)
                                                             .Include(i => i.ClassLevel)
                                                             .OrderBy(o => o.ClassLevel.DsplSeq)
                                                             .ToListAsync();

      var levelproducts = _mapper.Map<IEnumerable<ClasslevelProductDto>>(levelProdFromDB);
      return Ok(levelproducts);
    }

    [HttpPost("CreateProduct")]
    public async Task<IActionResult> CreateProduct (ProductDto productToCreate) {
      var produit = _mapper.Map<Product> (productToCreate);
      _context.Products.Add (produit);

      if (!produit.IsPeriodic) {
        // des échéances sont sélectionnées     
        foreach (var deadline in productToCreate.Deadlines) {
          if (deadline.DeadLineId != null && deadline.Percentage != null)
            _repo.Add (new ProductDeadLine {
              // DeadLineId = Convert.ToInt32 (deadline.DeadLineId),
                ProductId = produit.Id,
                Percentage = Convert.ToDecimal (deadline.Percentage)
            });
        }
      }

      if (productToCreate.IsByLevel) {
        // les montants sont définis par niveau
        foreach (var level in productToCreate.Levels) {
          if (level.Price != null)
            _repo.Add (new ClassLevelProduct { ProductId = produit.Id, ClassLevelId = level.id, Price = Convert.ToDecimal (level.Price) });
        }

      }

      if (await _repo.SaveAll())
        return Ok();

      return BadRequest("impossible de creer ce produit");
    }

    [HttpPost("CreatePayableAt")]
    public async Task<IActionResult> CreatePayableAt(PayableDto payableToCreate) {
      var p = _mapper.Map<PayableAt>(payableToCreate);
      _context.PayableAts.Add(p);
      if (await _repo.SaveAll())
        return Ok();

      return BadRequest("impossible de creer ce produit");
    }

    [HttpPost("SaveClassLevelProducts")]
    public async Task<IActionResult> SaveClassLevelProducts(List<ClasslevelProductDto> levelProducts)
    {
      foreach (var clp in levelProducts)
      {
        if(clp.Id == 0)
        {
          ClassLevelProduct classlevelProd = new ClassLevelProduct {
            ProductId = clp.ProductId,
            ClassLevelId = clp.ClassLevelId,
            Price = clp.Price
          };
          _repo.Add(classlevelProd);
        }
        else
        {
          ClassLevelProduct classlevelProd = await _context.ClassLevelProducts.FirstAsync(c => c.Id == clp.Id);
          classlevelProd.ProductId = clp.ProductId;
          classlevelProd.ClassLevelId = clp.ClassLevelId;
          classlevelProd.Price = clp.Price;
          _repo.Update(classlevelProd);
        }
      }

      if(await _repo.SaveAll())
        return NoContent();

      return BadRequest("problème pour enregistrer les données");
    }

    [HttpPost("CreateClassLevelProduct")]
    public async Task<IActionResult> CreateClassLevelProduct(LevelListProductDto lvlprodToCreate)
    {
      decimal amount = lvlprodToCreate.Amount;
      int productId = lvlprodToCreate.ProductId;

      foreach(var levelid in lvlprodToCreate.ClassLevelIds) {
        var lvlprod = await _context.ClassLevelProducts
                            .FirstOrDefaultAsync(a => a.ClassLevelId == levelid && a.ProductId == productId);
        if(lvlprod == null) {
          var lp = new ClassLevelProduct {
            ProductId = productId,
            ClassLevelId = levelid,
            Price = amount
          };
          _repo.Add(lp);
        }
      }

      if (await _repo.SaveAll())
        return Ok();

      return BadRequest("impossible de faire l'enregistrement...");
    }

    [HttpPost("CreateDeadLine")]
    public async Task<IActionResult> CreateDeadLine (DeadLineDto dtToCreate) {
      var dealdline = _mapper.Map<DeadLine> (dtToCreate);
      _repo.Add (dealdline);
      if (await _repo.SaveAll ())
        return Ok();

      return BadRequest ("impossible de terminer l'enregistrement");

    }

    [HttpPost("CreatePeriodicity/{periodictyName}")]
    public async Task<IActionResult> CreatePeriodicity (string periodictyName) {

      if (!string.IsNullOrEmpty (periodictyName)) {
        _repo.Add (new Periodicity { Name = periodictyName });
        if (await _repo.SaveAll ())
          return Ok();

        return BadRequest ("impossible de faire l'ajout");
      }
      return BadRequest ();
    }

    [HttpPost("EditDeadLine/{deadLineId}")]
    public async Task<IActionResult> EditDeadLine (int deadLineId, DeadLineDto dtToCreate) {
      var dl = await _context.DeadLines.FirstOrDefaultAsync (d => d.Id == deadLineId);

      CultureInfo frC = new CultureInfo ("fr-FR");

      if (dtToCreate != null) {
        dl.Name = dtToCreate.Name;
        dl.Comment = dtToCreate.Comment;
        dl.DueDate = DateTime.ParseExact (dtToCreate.DueDate, "dd/MM/yyyy", frC);
        // dl.Amount = dtToCreate.Amount;
        _repo.Update (dl);
        if (await _repo.SaveAll ())
          return Ok();

        return BadRequest ("impossible de terminer l'enregistrement");
      }
      return NotFound ();
    }

    [HttpPost("EditProduct/{productId}")]
    public async Task<IActionResult> EditProduct (int productId, ProductDto prodToCreate) {
      var product = await _context.Products.FirstOrDefaultAsync (d => d.Id == productId);
      if (product != null) {
        product.Name = prodToCreate.Name;
        product.Comment = prodToCreate.Comment;
        _repo.Update (product);
        if (await _repo.SaveAll ())
          return Ok();

        return BadRequest ("impossible de terminer l'enregistrement");
      }
      return NotFound ();
    }

    [HttpPost("EditPeriodicity/{periodicityid}/{periodicityName}")]
    public async Task<IActionResult> EditPeriodicity (int periodicityid, string periodicityName) {
      var p = await _context.Periodicities.FirstOrDefaultAsync (a => a.Id == periodicityid);
      if (p != null) {
        p.Name = periodicityName;
        _repo.Update (p);
        if (await _repo.SaveAll ())
          return Ok();

        return BadRequest ("impossible de faier la mise à jour");
      }
      return NotFound ();
    }

    [HttpPost("EditPayableAt/{payableAtId}")]
    public async Task<IActionResult> EditPeriodicity (int payableAtId, PayableDto payableToUpdate) {
      var p = await _context.PayableAts.FirstOrDefaultAsync (a => a.Id == payableAtId);
      if (p != null) {
        p.Name = payableToUpdate.Name;
        p.DayCount = payableToUpdate.DayCount;
        _repo.Update (p);
        if (await _repo.SaveAll ())
          return Ok();

        return BadRequest ("impossible de faier la mise à jour");
      }
      return NotFound ();
    }

    [HttpGet("LevelPrices/{productId}")]
    public async Task<IActionResult> GetClassLevelPrices(int productId)
    {
      List<ClassLevelProduct> classLevelProducts = await _cache.GetClassLevelProducts();
      List<ClassLevel> levels = await _repo.GetActiveClassLevels();

      List<ClasslevelProductDto> levelPrices = new List<ClasslevelProductDto>();
      List<ClassLevelProduct> productPrices = classLevelProducts.Where(p => p.ProductId == productId).ToList();
      if(productPrices.Count() > 0)
      {
        foreach (ClassLevel level in levels)
        {
          ClasslevelProductDto levelProduct = new ClasslevelProductDto();
          ClassLevelProduct levelPrice = productPrices.First(p => p.ClassLevelId == level.Id);
          levelProduct.Id = levelPrice.Id;
          levelProduct.ProductId = levelPrice.ProductId;
          levelProduct.ProductName = levelPrice.Product.Name;
          levelProduct.ClassLevelId = levelPrice.ClassLevelId;
          levelProduct.LevelName = levelPrice.ClassLevel.NameAbbrev;
          levelProduct.Price = levelPrice.Price;
          levelPrices.Add(levelProduct);
        }
      }
      else
      {
        List<Product> products = await _cache.GetProducts();
        Product product = products.First(p => p.Id == productId);

        foreach (ClassLevel level in levels)
        {
          ClasslevelProductDto levelProduct = new ClasslevelProductDto();
          levelProduct.Id = 0;
          levelProduct.ProductId = productId;
          levelProduct.ProductName = product.Name;
          levelProduct.ClassLevelId = level.Id;
          levelProduct.LevelName = level.NameAbbrev;
          levelProduct.Price = 0;
          levelPrices.Add(levelProduct);
        }
      }

      return Ok(levelPrices);
    }

    [HttpGet("ZonePrices/{productId}")]
    public async Task<IActionResult> GetZonePrices(int productId)
    {
      List<ProductZone> zoneProducts = await _cache.GetProductZones();
      List<Zone> zones = await _cache.GetZones();

      List<ProductZoneDto> zonePrices = new List<ProductZoneDto>();
      List<ProductZone> productPrices = zoneProducts.Where(p => p.ProductId == productId).ToList();
      if(productPrices.Count() > 0)
      {
        foreach (Zone zone in zones)
        {
          ProductZoneDto zoneProduct = new ProductZoneDto();
          ProductZone zonePrice = productPrices.First(p => p.ZoneId == zone.Id);
          zoneProduct.Id = zonePrice.Id;
          zoneProduct.ProductId = zonePrice.ProductId;
          zoneProduct.ProductName = zonePrice.Product.Name;
          zoneProduct.ZoneId = zonePrice.ZoneId;
          zoneProduct.ZoneName = zonePrice.Zone.Name;
          zoneProduct.Price = zonePrice.Price;
          zonePrices.Add(zoneProduct);
        }
      }
      else
      {
        List<Product> products = await _cache.GetProducts();
        Product product = products.First(p => p.Id == productId);

        foreach (Zone zone in zones)
        {
          ProductZoneDto zoneProduct = new ProductZoneDto();
          zoneProduct.Id = 0;
          zoneProduct.ProductId = productId;
          zoneProduct.ProductName = product.Name;
          zoneProduct.ZoneId = zone.Id;
          zoneProduct.ZoneName = zone.Name;
          zoneProduct.Price = 0;
          zonePrices.Add(zoneProduct);
        }
      }

      return Ok(zonePrices);
    }

  }
}