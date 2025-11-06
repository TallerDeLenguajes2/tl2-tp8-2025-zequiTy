using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MiWebApp.Models;

namespace MiWebApp.Controllers;

public class ProductosController : Controller
{
    //private readonly ILogger<ProductosController> _logger;
    private ProductoRepositorio productoRepository;
    public ProductosController()//ILogger<ProductosController> logger
    {
        // _logger = logger;
        productoRepository = new ProductoRepositorio();
    }

    /*public IActionResult Index()
    {
        return View();
    }
    */
    
    [HttpGet] 
    public IActionResult Index() 
    { 
        List<Productos> productos = productoRepository.GetAll(); 
        return View(productos); 
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
