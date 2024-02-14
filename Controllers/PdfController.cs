using Microsoft.AspNetCore.Mvc;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using tallerbiblioteca.Context;
using tallerbiblioteca.Services;
using PdfSharp.Fonts;
using System.Reflection;
using QuestPDF;
using QuestPDF.Fluent;
using PdfSharp.Pdf.IO;
using QuestPDF.Helpers;
using System.Drawing;

namespace tallerbiblioteca.Controllers
{
    public class PdfController : Controller
    {
        private readonly BibliotecaDbContext _context;
        private readonly UsuariosServices _usuariosServices;
        private readonly PeticionesServices _peticionesServices;
        private readonly ReservasServices _reservasServices;
        private readonly PrestamosServices _prestamosServices;
        //private readonly PdfServices _pdfServices;

        public PdfController(BibliotecaDbContext context, UsuariosServices usuariosServices, PeticionesServices peticionesServices, ReservasServices reservasServices, PrestamosServices prestamosServices)
        {
            _context = context;
            _usuariosServices = usuariosServices;
            _peticionesServices = peticionesServices;
            _reservasServices = reservasServices;
            _prestamosServices = prestamosServices;
        }
        public async Task<IActionResult> GenerarPdfUsuarios()
        {
            var usuarios = await _usuariosServices.ObtenerUsuariosPdf();

            var pdf = Document.Create(document =>
            {
                document.Page(page =>
                {
                    page.Margin(30);
                    page.Header().ShowOnce().Row(row =>
                    {
                        var rutaImagen = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Escudo.png");

                        row.ConstantItem(95).Background(Colors.White).Image(rutaImagen);
                        row.RelativeItem()
                        .Column(col =>
                        {
                            col.Item().AlignCenter().Text("Bookware").Bold().FontSize(14);
                            col.Item().AlignCenter().Text("Institución Educativa San Lorenzo de Aburrá").Bold().FontSize(14);
                            col.Item().AlignCenter().Text("Dirección: Cr 39 #80-34").FontSize(10);
                        });

                        row.RelativeItem()
                        .Column(col =>
                        {
                            col.Item().AlignCenter().Text("Usuarios registrados").FontSize(12);
                            col.Item().AlignCenter().Text("Correo: BookWare2024").FontSize(10);
                        });
                    });
                    // Cuerpo con tabla personalizada
                    page.Content().PaddingVertical(20).Column(col1 =>
                    {
                        col1.Item().AlignCenter().Text("Usuarios Registrados").Bold().FontSize(18).FontColor("#1e6042");

                        col1.Item().PaddingVertical(15).Table(tabla =>
                        {
                            tabla.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(10);
                                columns.RelativeColumn(10);
                                columns.RelativeColumn(15);
                                columns.RelativeColumn(15);
                                columns.RelativeColumn(10);
                                columns.RelativeColumn(13);
                            });

                            // Encabezado
                            tabla.Header(header =>
                            {
                                header.Cell().Background("#1e6042").Border(0.5f).Padding(4).AlignCenter().Text("Nombre").Bold().FontColor("#ffffff");
                                header.Cell().Background("#1e6042").Border(0.5f).Padding(4).AlignCenter().Text("Apellido").Bold().FontColor("#ffffff");
                                header.Cell().Background("#1e6042").Border(0.5f).Padding(4).AlignCenter().Text("Correo").Bold().FontColor("#ffffff");
                                header.Cell().Background("#1e6042").Border(0.5f).Padding(4).AlignCenter().Text("Rol").Bold().FontColor("#ffffff");
                                header.Cell().Background("#1e6042").Border(0.5f).Padding(4).AlignCenter().Text("Documento").Bold().FontColor("#ffffff");
                                header.Cell().Background("#1e6042").Border(0.5f).Padding(4).AlignCenter().Text("Estado").Bold().FontColor("#ffffff");
                            });

                            // Datos de usuarios
                            foreach (var usuario in usuarios)
                            {
                                tabla.Cell().Border(0.5f).Padding(2).AlignCenter().Text(usuario.Name).FontSize(12);
                                tabla.Cell().Border(0.5f).Padding(2).AlignCenter().Text(usuario.Apellido).FontSize(12);
                                tabla.Cell().Border(0.5f).Padding(2).AlignCenter().Text(usuario.Correo);
                                tabla.Cell().Border(0.5f).Padding(2).AlignCenter().Text(usuario.Rol.Nombre.ToString()).FontSize(12);
                                tabla.Cell().Border(0.5f).Padding(2).AlignCenter().Text(usuario.Numero_documento.ToString()).FontSize(10);
                                tabla.Cell().Border(0.5f).Padding(2).AlignCenter().Text(usuario.Estado).FontSize(12);
                            }
                        });
                    });
                    // Pie de página
                    page.Footer().Height(65)
                        .AlignCenter()
                        .Text(txt =>
                        {
                            txt.Span("Página ").FontSize(10);
                            txt.CurrentPageNumber().FontSize(10);
                            txt.Span(" de ").FontSize(10);
                            txt.TotalPages().FontSize(10);
                        });
                });
            })
            .GeneratePdf();

            Stream stream = new MemoryStream(pdf);

            return File(stream, "application/pdf", "Usuarios.pdf");
        }
        public async Task<IActionResult> GenerarPdfPeticiones()
        {
            var Peticiones = await _peticionesServices.ObtenerpeticionesPdf();

            var pdf = Document.Create(document =>
            {
                document.Page(page =>
                {
                    // Cabecera con logo y texto personalizado
                    page.Margin(30);
                    page.Header().ShowOnce().Row(row =>
                    {
                        var rutaImagen = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Escudo.png");

                        row.ConstantItem(95).Background(Colors.White).Image(rutaImagen);
                        row.RelativeItem()
                        .Column(col =>
                        {
                            col.Item().AlignCenter().Text("Bookware").Bold().FontSize(14);
                            col.Item().AlignCenter().Text("Institución Educativa San Lorenzo de Aburrá").Bold().FontSize(14);
                            col.Item().AlignCenter().Text("Dirección: Cr 39 #80-34").FontSize(10);
                        });

                        row.RelativeItem()
                        .Column(col =>
                        {
                            col.Item().AlignCenter().Text("Peticiones").Bold().FontSize(14);
                            col.Item().AlignCenter().Text("Correo: bookware2024@gmail.com").Bold().FontSize(10);
                        });
                    });
                    // Cuerpo con tabla personalizada
                    page.Content().PaddingVertical(20).Column(col1 =>
                    {
                        col1.Item().AlignCenter().Text("Peticiones Registradas").Bold().FontSize(18).FontColor("#1e6042");

                        col1.Item().PaddingVertical(15).Table(tabla =>
                        {
                            tabla.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(10);
                                columns.RelativeColumn(10);
                                columns.RelativeColumn(15);
                                columns.RelativeColumn(15);
                                columns.RelativeColumn(10);
                                columns.RelativeColumn(13);
                            });

                            // Encabezado
                            tabla.Header(header =>
                            {
                                header.Cell().Background("#1e6042").Border(0.5f).Padding(4).AlignCenter().Text("IdEjemplar").Bold().FontColor("#ffffff");
                                header.Cell().Background("#1e6042").Border(0.5f).Padding(4).AlignCenter().Text("Libro").Bold().FontColor("#ffffff");
                                header.Cell().Background("#1e6042").Border(0.5f).Padding(4).AlignCenter().Text("Nombre").Bold().FontColor("#ffffff");
                                header.Cell().Background("#1e6042").Border(0.5f).Padding(4).AlignCenter().Text("Apellido").Bold().FontColor("#ffffff");
                                header.Cell().Background("#1e6042").Border(0.5f).Padding(4).AlignCenter().Text("Fecha").Bold().FontColor("#ffffff");
                                header.Cell().Background("#1e6042").Border(0.5f).Padding(4).AlignCenter().Text("Estado").Bold().FontColor("#ffffff");
                            });

                            // Datos de usuarios
                            foreach (var peticiones in Peticiones)
                            {
                                tabla.Cell().Border(0.5f).Padding(2).AlignCenter().Text(peticiones.Id_ejemplar.ToString()).FontSize(12);
                                tabla.Cell().Border(0.5f).Padding(2).AlignCenter().Text(peticiones.Ejemplar.Libro.Nombre).FontSize(12);
                                tabla.Cell().Border(0.5f).Padding(2).AlignCenter().Text(peticiones.Usuario.Name);
                                tabla.Cell().Border(0.5f).Padding(2).AlignCenter().Text(peticiones.Usuario.Apellido).FontSize(12);
                                tabla.Cell().Border(0.5f).Padding(2).AlignCenter().Text(peticiones.FechaPeticion.ToString()).FontSize(10);
                                tabla.Cell().Border(0.5f).Padding(2).AlignCenter().Text(peticiones.Estado).FontSize(12);
                            }
                        });
                    });
                    // Pie de página
                    page.Footer().Height(65)
                        .AlignCenter()
                        .Text(txt =>
                        {
                            txt.Span("Página ").FontSize(10);
                            txt.CurrentPageNumber().FontSize(10);
                            txt.Span(" de ").FontSize(10);
                            txt.TotalPages().FontSize(10);
                        });
                });
            })
            .GeneratePdf();

            Stream stream = new MemoryStream(pdf);

            return File(stream, "application/pdf", "Peticiones.pdf");
        }
        public async Task<IActionResult> GenerarPdfReservas()
        {
            var reservas = await _reservasServices.ObtenerReservasPdf();

            var pdf = Document.Create(document =>
            {
                document.Page(page =>
                {

                   
                    page.Margin(30);
                    page.Header().ShowOnce().Row(row =>
                    {
                        var rutaImagen = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Escudo.png");

                        row.ConstantItem(95).Background(Colors.White).Image(rutaImagen);
                        row.RelativeItem()
                        .Column(col =>
                        {
                            col.Item().AlignCenter().Text("Bookware").Bold().FontSize(14);
                            col.Item().AlignCenter().Text("Institución Educativa San Lorenzo de Aburrá").Bold().FontSize(14);
                            col.Item().AlignCenter().Text("Dirección: Cr 39 #80-34").FontSize(10);
                        });

                        row.RelativeItem()
                        .Column(col =>
                        {
                            col.Item().AlignCenter().Text("Reservas").Bold().FontSize(14);
                            col.Item().AlignCenter().Text("Correo: bookware2024@gmail.com").Bold().FontSize(10);
                        });
                    });
                    // Cuerpo con tabla personalizada
                    page.Content().PaddingVertical(20).Column(col1 =>
                    {
                        col1.Item().AlignCenter().Text("Reservas Registradas").Bold().FontSize(18).FontColor("#1e6042");

                        col1.Item().PaddingVertical(15).Table(tabla =>
                        {
                            tabla.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(10);
                                columns.RelativeColumn(10);
                                columns.RelativeColumn(15);
                                columns.RelativeColumn(15);
                                columns.RelativeColumn(10);
                                
                            });

                            // Encabezado
                            tabla.Header(header =>
                            {
                                header.Cell().Background("#1e6042").Border(0.5f).Padding(4).AlignCenter().Text("Ejemplar").Bold().FontColor("#ffffff");
                                header.Cell().Background("#1e6042").Border(0.5f).Padding(4).AlignCenter().Text("Nombre").Bold().FontColor("#ffffff");
                                header.Cell().Background("#1e6042").Border(0.5f).Padding(4).AlignCenter().Text("Apellido").Bold().FontColor("#ffffff");
                                header.Cell().Background("#1e6042").Border(0.5f).Padding(4).AlignCenter().Text("Fecha").Bold().FontColor("#ffffff");
                                header.Cell().Background("#1e6042").Border(0.5f).Padding(4).AlignCenter().Text("Estado").Bold().FontColor("#ffffff");
                            });

                            // Datos de usuarios
                            foreach (var reserva in reservas)
                            {
                                
                                tabla.Cell().Border(0.5f).Padding(2).AlignCenter().Text(reserva.Ejemplar.Libro.Nombre).FontSize(12);
                                tabla.Cell().Border(0.5f).Padding(2).AlignCenter().Text(reserva.Usuario.Name);
                                tabla.Cell().Border(0.5f).Padding(2).AlignCenter().Text(reserva.Usuario.Apellido).FontSize(12);
                                tabla.Cell().Border(0.5f).Padding(2).AlignCenter().Text(reserva.FechaReserva.ToString()).FontSize(10);
                                tabla.Cell().Border(0.5f).Padding(2).AlignCenter().Text(reserva.Estado).FontSize(12);
                            }
                        });
                    });
                    // Pie de página
                    page.Footer().Height(65)
                        .AlignCenter()
                        .Text(txt =>
                        {
                            txt.Span("Página ").FontSize(10);
                            txt.CurrentPageNumber().FontSize(10);
                            txt.Span(" de ").FontSize(10);
                            txt.TotalPages().FontSize(10);
                        });
                });
            })
            .GeneratePdf();

            Stream stream = new MemoryStream(pdf);

            return File(stream, "application/pdf", "Reservas.pdf");
        }
        public async Task<IActionResult> GenerarPdfPrestamos()
        {
            var prestamos = await _prestamosServices.ObtenerPrestamos();
            
            var pdf = Document.Create(document =>
            {
                document.Page(page =>
                {

                    page.Margin(30);
                    page.Header().ShowOnce().Row(row =>
                    {
                        var rutaImagen = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Escudo.png");

                        row.ConstantItem(95).Background(Colors.White).Image(rutaImagen);
                        row.RelativeItem()
                        .Column(col =>
                        {
                            col.Item().AlignCenter().Text("Bookware").Bold().FontSize(14);
                            col.Item().AlignCenter().Text("Institución Educativa San Lorenzo de Aburrá").Bold().FontSize(14);
                            col.Item().AlignCenter().Text("Dirección: Cr 39 #80-34").FontSize(10);
                        });

                        row.RelativeItem()
                        .Column(col =>
                        {
                            col.Item().AlignCenter().Text("Préstamos").Bold().FontSize(14);
                            col.Item().AlignCenter().Text("Correo: bookware2024@gmail.com").Bold().FontSize(10);
                        });
                    });
                    // Cuerpo con tabla personalizada
                    page.Content().PaddingVertical(20).Column(col1 =>
                    {
                        col1.Item().AlignCenter().Text("Préstamos Registradas").Bold().FontSize(18).FontColor("#1e6042");

                        col1.Item().PaddingVertical(15).Table(tabla =>
                        {
                            tabla.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(10);
                                columns.RelativeColumn(10);
                                columns.RelativeColumn(15);
                                columns.RelativeColumn(10);
                                columns.RelativeColumn(10);
                                columns.RelativeColumn(10);


                            });
                            tabla.Header(header =>
                            {
                                header.Cell().Background("#1e6042").Border(0.5f).Padding(4).AlignCenter().Text("Id ejemplar").Bold().FontColor("#ffffff");
                                header.Cell().Background("#1e6042").Border(0.5f).Padding(4).AlignCenter().Text("Libro").Bold().FontColor("#ffffff");
                                header.Cell().Background("#1e6042").Border(0.5f).Padding(4).AlignCenter().Text("Nombre").Bold().FontColor("#ffffff");
                                header.Cell().Background("#1e6042").Border(0.5f).Padding(4).AlignCenter().Text("Prestado").Bold().FontColor("#ffffff");
                                header.Cell().Background("#1e6042").Border(0.5f).Padding(4).AlignCenter().Text("Devolución").Bold().FontColor("#ffffff");
                                header.Cell().Background("#1e6042").Border(0.5f).Padding(4).AlignCenter().Text("Estado").Bold().FontColor("#ffffff");

                            });

                            
                            foreach (var prestamo in prestamos)
                            {
                                var nombreCompleto = string.Concat(prestamo.Peticion.NombreUsuario, " ", prestamo.Peticion.Usuario.Apellido);
                                tabla.Cell().Border(0.5f).Padding(2).AlignCenter().Text(prestamo.Peticion.Id_ejemplar.ToString()).FontSize(12);
                                tabla.Cell().Border(0.5f).Padding(2).AlignCenter().Text(prestamo.Peticion.NombreLibro);
                                tabla.Cell().Border(0.5f).Padding(2).AlignCenter().Text(nombreCompleto).FontSize(12);
                                tabla.Cell().Border(0.5f).Padding(2).AlignCenter().Text(prestamo.Fecha_inicio.ToString()).FontSize(10);
                                tabla.Cell().Border(0.5f).Padding(2).AlignCenter().Text(prestamo.Fecha_fin.ToString()).FontSize(12);
                                tabla.Cell().Border(0.5f).Padding(2).AlignCenter().Text(prestamo.Estado).FontSize(12);

                            }
                        });
                    });
                    // Pie de página
                    page.Footer().Height(65)
                        .AlignCenter()
                        .Text(txt =>
                        {
                            txt.Span("Página ").FontSize(10);
                            txt.CurrentPageNumber().FontSize(10);
                            txt.Span(" de ").FontSize(10);
                            txt.TotalPages().FontSize(10);
                        });
                });
            })
            .GeneratePdf();

            Stream stream = new MemoryStream(pdf);

            return File(stream, "application/pdf", "Préstamos.pdf");
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}