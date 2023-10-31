// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


document.addEventListener("DOMContentLoaded", function () {
    // Hacer una solicitud a la API para obtener los datos
    fetch("/api/notificaciones")
        .then(response => response.json()) // Suponiendo que los datos son un JSON
        .then(data => {
         

            // Actualizar el contenido de la campana con los datos obtenidos
            document.querySelector("#valorCampana").textContent = data.length;

            // Actualizar el dropdown con las notificaciones
            const notificacionesContainer = document.querySelector("#notificacionesContainer");

            if (data.length > 0) {
                data.forEach(item => {
                    const notificacion = document.createElement("li");
                    if (item.usuario && item.usuario.name) {
                        notificacion.textContent = `${item.usuario.name} ha solicitado. ${item.motivo}`;
                    } else {
                        notificacion.textContent = `Alguien ha solicitado. ${item.motivo}`;
                    }
                    notificacionesContainer.appendChild(notificacion);
                });
                // Cambiar el color del ícono
                document.querySelector("#valorCampana").style.color = "white";
                document.querySelector("#valorCampana").style.background = "black";
            } else {
                // No hay notificaciones nuevas, puedes ocultar el contenedor o mostrar un mensaje predeterminado
                notificacionesContainer.innerHTML = "<li>No hay notificaciones nuevas.</li>";
            }

            // Hacer algo con los datos recibidos si es necesario
        })
        .catch(error => console.error(error));
});

