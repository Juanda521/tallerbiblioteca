
document.addEventListener('DOMContentLoaded', function() {
  var calendarEl = document.getElementById('calendar');
  var allEvents = []; // Array para almacenar todos los eventos

  

  function handleDatesRender(info) {
    console.log('viewType:', info.view.type);
  }

      // Función para abrir el modal Bootstrap
    function openBootstrapModal(eventstr,userstr,librostr,idUser,fechafin) {
      // Obtener el elemento del modal de fecha
     
      const modalEvent = document.getElementById('modal-event');
      const modalUser  = document.getElementById('modal-user');
      const modalLibro = document.getElementById('modal-libro');
      const modalCc = document.getElementById('modal-id-usuario');
      const modal_fecha_fin = document.getElementById('modal-fecha-fin');

      // Establecer el texto del modal de fecha
      
      modalEvent.textContent = eventstr;
      modalUser.textContent  = userstr;
      modalLibro.textContent = librostr;
      modalCc.textContent = idUser;
      modal_fecha_fin.textContent = fechafin;
      
      // Abrir el modal Bootstrap
      $('#myModal').modal('show');
    }

    // Función para abrir el modal Bootstrap con carrusel
    function openBootstrapModalWithCarousel(items) {
      // Limpiar el contenido actual del carrusel
      $("#carouselExample .carousel-inner").empty();
  
      // Agregar cada item al carrusel
      items.forEach((item, index) => {

        console.log("Item completo:", item); // Agrega esta línea para ver la estructura del objeto
        const activeClass = index === 0 ? "active" : "";
        const carouselItem = `
          <div class="carousel-item ${activeClass}">
            <span class="text-success" id="modal-event">${item.peticion.motivo}</span> <p class="fw-bold">Evento</p>
            <span class="text-success" id="modal-id-usuario">${item.peticion.usuario.numero_documento}</span> <p class="fw-bold">Identificación Usuario</p>
            <span class="text-success" id="modal-user">${item.peticion.nombreUsuario}</span> <p class="fw-bold">Nombre usuario</p>
            <span class="text-success" id="modal-libro">${item.peticion.nombreLibro}</span> <p class="fw-bold">Nombre Libro</p>
            <span class="text-danger" id="modal-fecha-fin">${item.fecha_fin}</span> <p class="fw-bold">Fecha Devolución</p>
          </div>
        `;
  
        $("#carouselExample .carousel-inner").append(carouselItem);
      });

      $("#carouselExample .carousel-item:first").addClass("active");
  
      // Abrir el modal Bootstrap
      $("#myModal").modal("show");
    }





  fetch("/api/Calendario")
    .then(response => response.json()) // Suponiendo que los datos son un JSON
    .then(data => {
        
      // Guardar los eventos de la petición en la matriz
      allEvents = data.map(function(event) {
        // console.log(data)
        // const user  = event.peticion.usuario.Name
        // const evento  = event.peticion.Motivo
        return {
          title: event.peticion.nombreUsuario,
          start: event.fecha_inicio,
          // timeEnd: event.fecha_fin,
          // eventEndTime: event.fecha_fin,
          // end: event.fecha_fin,
          id: event.id,
          color: 'green',
          user: event.peticion.nombreUsuario,
          libro: event.peticion.nombreLibro,
          document:event.peticion.usuario.numero_documento,
          fecha_fin : event.fecha_fin
          // Puedes agregar más propiedades aquí si es necesario
        };
      });
      console.log("Todos los eventos:", allEvents);

      const datePicker = document.getElementById('datePicker');


  flatpickr(datePicker, {
    dateFormat: 'Y-m-d',  // Formato de fecha
    onChange: function(selectedDates, dateStr, instance) {
      // Lógica que se ejecuta cuando se selecciona una fecha
      // Puedes agregar aquí la lógica de filtrado por fecha
      console.log("Fecha seleccionada:", dateStr);
    },
  });
      // Obtén el elemento de entrada y el calendario
  const searchInput = document.getElementById('searchInput');

  

  // Maneja el evento de entrada para filtrar los eventos del calendario
  searchInput.addEventListener('input', function() {
    const searchText = searchInput.value.toLowerCase();
    const selectedDate = datePicker.value;
    console.log(searchText);
    console.log(selectedDate);

    const filteredEvents = allEvents.filter(event => {
      const documentoVisible = event.document.toString().includes(searchText);

      // Convierte la fecha del evento al formato 'Y-m-d' para comparar con la fecha seleccionada
    const eventoFechaYMD = event.start.split('T')[0];

    // Verifica si la fecha del evento coincide con la fecha seleccionada
    const fechaVisible = selectedDate ? eventoFechaYMD === selectedDate : true;

      return documentoVisible && fechaVisible;
    });
  
    // Escribe los eventos filtrados en la consola
    console.log("Eventos filtrados por número de documento:", filteredEvents);

    // Actualiza el array 'allEvents' con los eventos filtrados
  calendar.removeAllEvents();
  calendar.addEventSource(filteredEvents);
  calendar.refetchEvents();




  });

      // Crear una variable para almacenar el elemento del calendario
      var calendar = new FullCalendar.Calendar(calendarEl, {
        
        initialView: 'dayGridMonth',
        headerToolbar: {
          // left: 'prev,next today',
          // center: 'title',
          // right: 'dayGridMonth,timeGridWeek,timeGridDay,listWeek'
        },datesRender: handleDatesRender,

        eventColor: 'green',
        display: 'background',
        events: allEvents, // Usar la matriz de eventos combinados
        dateClick: function(info) {
        
          // // Buscar un evento que coincida con la fecha del calendario
          // const clickedEvents = data.filter(event => event.fecha_inicio.split('T')[0] == info.dateStr);
          // console.log("Todos los eventos:", clickedEvents);
          // if (clickedEvents.length > 0) {
          //   console.log("hay varios prestamos");
          //   // Si hay eventos, abrir el modal Bootstrap con carrusel
          //   openBootstrapModalWithCarousel(clickedEvents);
          // } else {
          //   console.log("no esta encontrando los prestamos");
            
          //   // Si no hay eventos, abrir el modal Bootstrap sin información adicional
          //   openBootstrapModal(info.dateStr, "", "", "", "");
          // }

          // if (clickedEvent) {
          //   const motivo = clickedEvent.peticion.motivo;
          //   const user = clickedEvent.peticion.usuario.name + "  " + clickedEvent.peticion.usuario.apellido;
          //   const libro = clickedEvent.peticion.ejemplar.libro.nombre;
          //   const idUser = clickedEvent.peticion.usuario.numero_documento;
          //   const fechaDevolucion = clickedEvent.fecha_fin;
          //   console.log("hablalo desde el if")
            
          //   console.log(clickedEvent)
          //   // Abrir el modal Bootstrap y pasar las variables motivo y user
          //   openBootstrapModal(motivo, user,libro,idUser,fechaDevolucion);
          // } else {
          //   // Abrir el modal Bootstrap sin información adicional
          //   openBootstrapModal(info.dateStr, "", "","","");
          //   console.log("hablalo desde el else ")
          // }
              },
              eventClick: function(info) {
                // Obtén el evento específico que se hizo clic
                const clickedEvent = info.event;
                console.log(clickedEvent);
            
                // Obtén la información del evento
                const motivo = clickedEvent.extendedProps.title;
                const user = clickedEvent.extendedProps.user; // Ajusta esto según la estructura real de tu objeto de evento
                const libro = clickedEvent.extendedProps.libro; // Ajusta esto según la estructura real de tu objeto de evento
                const documento = clickedEvent.extendedProps.document; // Ajusta esto según la estructura real de tu objeto de evento
                const fechaDevolucion = clickedEvent.extendedProps.fecha_fin;
            
                // Abrir el modal Bootstrap y pasar las variables motivo y user
                openBootstrapModal(motivo, user, libro, documento, fechaDevolucion);
              },
        datesSet: handleDatesRender,
      
        locale: 'es', 
      });
  calendar.updateSize() // Forza al calendario a ajustar su tamaño inmediatamente

      calendar.render();

      
    });
});