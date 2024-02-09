
//validacion del formulario de registrar libros (gracias a las facilidades de asp, en este script solo se encuentra la validacion de generos y autores)

function validarYEnviarFormulario() {
    // Obtén los valores seleccionados en los selectores de género y autor
    var generoIds = document.getElementById('GeneroIds').selectedOptions;
    var autorIds = document.getElementById('AutorIds').selectedOptions;

    console.log("hablalo desde la validacion");

    // Verifica si al menos un género y un autor han sido seleccionados
    if (generoIds.length === 0 || autorIds.length === 0) {
        // Muestra un mensaje de error
        console.log("no hay generos ni autores");
        var errorSpan = document.getElementById('errorMensaje');
        if (!errorSpan) {
            console.log("vamos a crear el span");
            errorSpan = document.createElement('span');
            errorSpan.id = 'errorMensaje';
            errorSpan.style.color = 'red';
            errorSpan.innerHTML = 'Debes seleccionar al menos 1 género y 1 autor.';
            document.querySelector('#errorGenero').appendChild(errorSpan);
        }
        // Evita que el formulario se envíe si no se cumplen los requisitos
        return false;
    }

    // Si se cumplen los requisitos, obténemos el formulario y lo enviamos
    var formulario = document.getElementById('formularioLibros');
    formulario.submit();

    // Si llegamos a este punto, eliminamos el mensaje de error si existe
    var errorSpan = document.getElementById('errorMensaje');
    if (errorSpan) {
        errorSpan.parentNode.removeChild(errorSpan);
    }
}


    $(document).ready(function() {
        $('.chkCambiarCampo').change(function() {
            var libroId = $(this).data('libro-id');
            $('#libroId').val(libroId);
            console.log("ID del libro: " + libroId);
            $('#formCambiarCampo').submit();
            console.log("le dimos click al boton y enviamos el formulario");
        });
    });



// $(document).ready(function() {
//   $(".selectpicker").selectpicker({
    
//     size: 2, // Establecer el número de opciones visibles
//     width: '50%' // Establecer el ancho del select
//     // Puedes agregar más opciones según la documentación de Bootstrap-Select
//   });
// });



    

// Agrega un event listener al botón del formulario
document.getElementById('botonRegistrar').addEventListener('click', function () {
    validarYEnviarFormulario();
});