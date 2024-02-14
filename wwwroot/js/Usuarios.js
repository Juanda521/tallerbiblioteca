function MostrarMensajeUsuarios(icono,Mensaje) {

    Mensaje = Mensaje.replace(/^"(.*)"$/, '$1'); // Elimina las comillas al principio y al final
    Swal.fire({
        icon: icono,
        
        // title: 'BookWare Dice',
        toast : true,
        text: Mensaje,
     
        
        
        // footer: '<a>!BookWare!</a>'
    });
}

document.addEventListener('DOMContentLoaded', function() {
    const passwordInput = document.getElementById('Contraseña');
    const togglePasswordButton = document.getElementById('togglePassword');

    if (passwordInput && togglePasswordButton) {
        togglePasswordButton.addEventListener('click', function() {
            if (passwordInput.type === 'password') {
                passwordInput.type = 'text';
                togglePasswordButton.querySelector('i').classList.remove('fa-eye');
                togglePasswordButton.querySelector('i').classList.add('fa-eye-slash');
            } else {
                passwordInput.type = 'password';
                togglePasswordButton.querySelector('i').classList.remove('fa-eye-slash');
                togglePasswordButton.querySelector('i').classList.add('fa-eye');
            }
        });
    }

   
});

function base64ToDataURL(base64String) {
    return "data:image/png;base64," + base64String;
}

$(document).ready(function () {
    // Escucha el evento click en los botones con la clase 'libros-relacionados-button'
    $('.libros-relacionados-button').click(function () {
        // Obtén el ID del libro desde el atributo 'data-id'
        var idLibro = $(this).data('id');
       
        // Llama a la función para obtener libros relacionados
        obtenerLibrosRelacionados(idLibro);
    });

    // Función para obtener y mostrar los libros relacionados en el modal
    function obtenerLibrosRelacionados(idLibro) {
        
        $.ajax({
            url: '/Libros/LibrosRelacionadosPorGenero',
            type: 'GET',
            data: { idLibro: idLibro },
            success: function (data) {
                var librosRelacionadosContent = $('.relaciones-' + idLibro);
                librosRelacionadosContent.empty();
               
                for (var i = 0; i < data.length; i++) {
                    var libro = data[i];
                    librosRelacionadosContent.append('<p>' + libro.nombre + '</p>');

                        // Agregar la imagen debajo del nombre del libro
                    if (libro.imagenLibro) {
                        var urlDatos = base64ToDataURL(libro.imagenLibro);
                        
                        librosRelacionadosContent.append('<img class="imagen-libro-relacionado" src="' + urlDatos + '" alt="Imagen del libro relacionado" data-libro-id="' + libro.id + '">');                    }else{
                  
                    }
                }
               

                console.log(data)
            },
            error: function () {
                console.log('Hubo un error al obtener los libros relacionados.');
            }
        });
    }
});

$(document).on('click', '.imagen-libro-relacionado', function () {
    var libroId = $(this).data('libro-id');
    console.log('#info_' + libroId);
    $('#info_' + libroId).modal('show');
});



