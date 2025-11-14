using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using MiWebApp.Models;
using SistemaVentas.Web.ViewModels;


namespace MiWebApp.Controllers;

public class ProductosController : Controller
{


    private ProductoRepositorio producto;
    public ProductosController()
    {
        producto = new ProductoRepositorio();
    }


    [HttpGet]
    public IActionResult Index()
    {
        List<Productos> productos = producto.GetAll();
        return View(productos);

    }



    [HttpGet]

    public IActionResult Details()
    {
        return View(new Productos());
    }


    [HttpPost]

    public IActionResult Details(string descripcion)
    {
        if (!ModelState.IsValid)
        {
            return View(producto);
        }

        var aux = producto.ObtenerProductoPorNombre(descripcion);
        
        return View(aux);

    }

    //razor no distingue entre mayus y  minus

    [HttpGet]

    public IActionResult Create()
    {

            return View(new Productos());
        
        
    }


    [HttpPost]

    public IActionResult Create(ProductoViewModel productoVM)
    {
        // 1. CHEQUEO DE SEGURIDAD DEL SERVIDOR
        if (!ModelState.IsValid)//preguntar
        {
            // Si falla: Devolvemos el ViewModel con los datos y errores a la Vista
            return View(new Productos());
        }
        // 2. SI ES VÁLIDO: Mapeo Manual de VM a Modelo de Dominio
        var nuevoProducto = new Productos
        {
            Descripcion = productoVM.Descripcion,
            Precio = productoVM.Precio

        };
        // 3. Llamada al Repositorio
        producto.InsertarProducto(nuevoProducto);
        return RedirectToAction(nameof(Index));

    }




    [HttpGet]
    public IActionResult Edit()
    {

        return View(new Productos());
    }



    [HttpPost]
    public IActionResult Edit(int id, ProductoViewModel productoVM)
    {
        
        if(id != productoVM.idProducto) return NotFound();

        if (!ModelState.IsValid)
        {
            return View(productoVM);
        }

        var productoAEditar = new Productos
        {
            IdProducto = productoVM.idProducto,
            Descripcion = productoVM.Descripcion,
            Precio = productoVM.Precio
        };


        producto.ActualizarProducto(id, productoAEditar);
        return RedirectToAction(nameof(Index));
    }

    
    [HttpGet]
    public IActionResult Delete()
    {

        return View(new Productos());
    }



    [HttpPost]
    public IActionResult Delete(Productos produc)
    {
        producto.borrarProducto(produc.IdProducto);
        return RedirectToAction("Index");
    }







    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}