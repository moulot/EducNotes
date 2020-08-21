using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EducNotes.API.Data;
using EducNotes.API.Dtos;
using EducNotes.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EducNotes.API.Controllers {
  [Route ("api/[controller]")]
  [ApiController]
  public class TresoController : ControllerBase {
    private readonly DataContext _context;
    private readonly IEducNotesRepository _repo;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;

    public TresoController (DataContext context, IEducNotesRepository repo, IMapper mapper, IConfiguration config) {
      _context = context;
      _repo = repo;
      _config = config;
      _mapper = mapper;
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////ZONES DES GET LISTES/////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    [HttpGet ("GetProductTypes")]
    public async Task<IActionResult> GetProductTypes () {
      var producTypes = await _context.ProductTypes.OrderBy (p => p.Name).ToListAsync ();
      return Ok (producTypes);
    }

    [HttpGet ("GetDeadLines")]
    public async Task<IActionResult> GetDeadLines () {
      var deadLines = await _context.DeadLines.OrderBy (p => p.DueDate).ToListAsync ();
      var res = _mapper.Map<IEnumerable<DealLineDetailsDto>> (deadLines);

      return Ok (res);
    }

    [HttpGet ("GetProducts")]
    public async Task<IActionResult> GetProducts () {
      // var products = await _context.Products.OrderBy (p => p.Name).ToListAsync ();
      // return Ok (products);
      int schoolServiceTypeId = _config.GetValue<int> ("AppSettings:schoolServiceTypeId");
      var prods = await _context.Products
        .Include (a => a.Periodicity)
        .Include (a => a.PayableAt)
        .Where (p => p.ProductTypeId == schoolServiceTypeId)
        .OrderBy (a => a.Name)
        .ToListAsync ();

      var produits = new List<SchoolServicesDto> ();
      foreach (var prod in prods) {
        var produit = new SchoolServicesDto {
          id = prod.Id,
          Name = prod.Name,
          Comment = prod.Comment,
          Price = prod.Price,
          IsByLevel = prod.IsByLevel,
          IsPeriodic = prod.IsPeriodic,
          Periodicity = prod.Periodicity,
          PayableAt = prod.PayableAt
        };

        if (!prod.IsPeriodic) {
          produit.ProductDeadLines = await _context.ProductDeadLines
            // .Include (d => d.DeadLine)
            .Where (p => p.ProductId == prod.Id)
            .ToListAsync ();
        }

        if (prod.IsByLevel) {
          produit.ClassLevelProducts = await _context.ClassLevelProducts
            .Include (d => d.ClassLevel)
            .Where (p => p.ProductId == prod.Id)
            .ToListAsync ();
        }
        produits.Add(produit);
      }

      return Ok(produits);

    }

    [HttpGet ("GetPeriodicities")]
    public async Task<IActionResult> GetPeriodicicities () {
      var periodicities = await _context.Periodicities.OrderBy (p => p.Name).ToListAsync ();
      return Ok (periodicities);
    }

    [HttpGet ("GetPayableAts")]
    public async Task<IActionResult> GetPayableAts () {
      var ats = await _context.PayableAts.OrderBy (p => p.Name).ToListAsync ();
      return Ok (ats);
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////ZONE DES GET BY ID/////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    [HttpGet ("GetDeadLine/{deadLineId}")]
    public async Task<IActionResult> GetDeadLine (int deadLineId) {
      var deadLine = await _context.DeadLines.FirstOrDefaultAsync (p => p.Id == deadLineId);
      deadLine.DueDate = deadLine.DueDate.Date;
      var res = _mapper.Map<DealLineDetailsDto> (deadLine);
      return Ok (res);
    }

    [HttpGet ("GetProduct/{productId}")]
    public async Task<IActionResult> GetProduct (int productId) {
      var product = await _context.Products.FirstOrDefaultAsync (p => p.Id == productId);
      return Ok (product);
    }

    [HttpGet ("GetProducts/{productTypeId}")]
    public async Task<IActionResult> GetProducts (int productTypeId) {
      var producTypes = await _context.Products
                              .Where (p => p.ProductTypeId == productTypeId)
                              .OrderBy (p => p.Name)
                              .ToListAsync ();
      return Ok (producTypes);
    }

    [HttpGet ("GetLvlServices/{classLevelId}")]
    public async Task<IActionResult> GetLvlServices (int classLevelId) {
      var services = await _context.ClassLevelProducts
        .Include (p => p.Product)
        .Where (p => p.ClassLevelId == classLevelId)
        .OrderBy (p => p.Product.Name)
        .ToListAsync ();
      return Ok (services);
    }

    [HttpGet ("Getperiodicity/{periodicityId}")]
    public async Task<IActionResult> Getperiodicity (int periodicityId) {
      var pp = await _context.Periodicities.FirstOrDefaultAsync (p => p.Id == periodicityId);
      return Ok (pp);
    }

    [HttpGet ("GetpayableAt/{payableAtid}")]
    public async Task<IActionResult> GetpayableAt (int payableAtid) {
      var pp = await _context.PayableAts.FirstOrDefaultAsync (p => p.Id == payableAtid);
      return Ok (pp);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////ZONE DES POST///////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    [HttpPost ("CreateProduct")]
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

      if (await _repo.SaveAll ())
        return Ok ();

      return BadRequest ("impossible de creer ce produit");
    }

    [HttpPost ("CreatePayableAt")]
    public async Task<IActionResult> CreatePayableAt (PayableDto payableToCreate) {
      var p = _mapper.Map<PayableAt> (payableToCreate);
      _context.PayableAts.Add (p);
      if (await _repo.SaveAll ())
        return Ok ();

      return BadRequest ("impossible de creer ce produit");
    }

    [HttpPost ("CreateClassLevelProduct")]
    public async Task<IActionResult> CreateClassLevelProduct (ClasslevelProductDto lvlprodToCreate) {
      decimal amount = lvlprodToCreate.Amount;
      int productId = lvlprodToCreate.ProductId;

      foreach (var levelid in lvlprodToCreate.ClassLevelIds) {
        var lvlprod = await _context.ClassLevelProducts.FirstOrDefaultAsync (a => a.ClassLevelId == levelid && a.ProductId == productId);
        if (lvlprod == null) {
          var lp = new ClassLevelProduct { ProductId = productId, ClassLevelId = levelid, Price = amount };
          _repo.Add (lp);
        }
      }

      if (await _repo.SaveAll ())
        return Ok ();

      return BadRequest ("impossible de faire l'enregistrement...");

    }

    [HttpPost ("CreateDeadLine")]
    public async Task<IActionResult> CreateDeadLine (DeadLineDto dtToCreate) {
      var dealdline = _mapper.Map<DeadLine> (dtToCreate);
      _repo.Add (dealdline);
      if (await _repo.SaveAll ())
        return Ok ();

      return BadRequest ("impossible de terminer l'enregistrement");

    }

    [HttpPost ("CreatePeriodicity/{periodictyName}")]
    public async Task<IActionResult> CreatePeriodicity (string periodictyName) {

      if (!string.IsNullOrEmpty (periodictyName)) {
        _repo.Add (new Periodicity { Name = periodictyName });
        if (await _repo.SaveAll ())
          return Ok ();

        return BadRequest ("impossible de faire l'ajout");
      }
      return BadRequest ();
    }

    [HttpPost ("EditDeadLine/{deadLineId}")]
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
          return Ok ();

        return BadRequest ("impossible de terminer l'enregistrement");
      }
      return NotFound ();
    }

    [HttpPost ("EditProduct/{productId}")]
    public async Task<IActionResult> EditProduct (int productId, ProductDto prodToCreate) {
      var product = await _context.Products.FirstOrDefaultAsync (d => d.Id == productId);
      if (product != null) {
        product.Name = prodToCreate.Name;
        product.Comment = prodToCreate.Comment;
        _repo.Update (product);
        if (await _repo.SaveAll ())
          return Ok ();

        return BadRequest ("impossible de terminer l'enregistrement");
      }
      return NotFound ();
    }

    [HttpPost ("EditPeriodicity/{periodicityid}/{periodicityName}")]
    public async Task<IActionResult> EditPeriodicity (int periodicityid, string periodicityName) {
      var p = await _context.Periodicities.FirstOrDefaultAsync (a => a.Id == periodicityid);
      if (p != null) {
        p.Name = periodicityName;
        _repo.Update (p);
        if (await _repo.SaveAll ())
          return Ok ();

        return BadRequest ("impossible de faier la mise à jour");
      }
      return NotFound ();
    }

    [HttpPost ("EditPayableAt/{payableAtId}")]
    public async Task<IActionResult> EditPeriodicity (int payableAtId, PayableDto payableToUpdate) {
      var p = await _context.PayableAts.FirstOrDefaultAsync (a => a.Id == payableAtId);
      if (p != null) {
        p.Name = payableToUpdate.Name;
        p.DayCount = payableToUpdate.DayCount;
        _repo.Update (p);
        if (await _repo.SaveAll ())
          return Ok ();

        return BadRequest ("impossible de faier la mise à jour");
      }
      return NotFound ();
    }

  }
}