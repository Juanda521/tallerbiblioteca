
document.addEventListener('DOMContentLoaded', function() {
  var calendarEl = document.getElementById('calendar');
  var allEvents = []; // Array para almacenar todos los eventos


  //para realizar la grafica
  fetch("/api/Graficas")
    .then(response => response.json()) // Suponiendo que los datos son un JSON
    .then(prestamos => {
      console.log("estamos con los datos");
      console.log(prestamos);
      var ctx = document.getElementById('graficaLibros').getContext('2d');

      var colors = ['#1e6042', 'rgb(0, 0, 0)','#82B440'];

      var data = {
        labels: prestamos.map(prestamo=>prestamo.libro),
        datasets: [{
          label: 'Prestamos por Libros',
          data: prestamos.map(prestamo=>prestamo.cantidad),
          backgroundColor: colors,
        }]
      };

      var options = {
        responsive: true,
        maintainAspectRatio: false,
        cutout: '70%', // Controla el tamaño del agujero en el centro del gráfico (70% en este caso)
      };


      var bookChart = new Chart(ctx, {
          type: 'doughnut',
          data: data,
          options: options
      });
  
    }).catch(error => {
      console.error('Error al procesar la solicitud:', error);
      // Aquí puedes manejar el error, como mostrar un mensaje al usuario o registrar el error
    });
    

  function handleDatesRender(info) {
    console.log('viewType:', info.view.type);
  }

      // Función para abrir el modal Bootstrap
    function openBootstrapModal(eventstr,userstr,librostr,idUser,fechafin) {
      // Obtener el elemento del modal de fecha
      try{
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
      } catch (error) {
        console.error('Error al abrir el modal:', error);
        // Aquí puedes manejar el error, como mostrar un mensaje al usuario o registrar el error
      }
    }

    // Función para abrir el modal Bootstrap con carrusel
    function openBootstrapModalWithCarousel(items) {
      // Limpiar el contenido actual del carrusel
      try {
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
      } catch (error) {
        console.error('Error al abrir el modal:', error);
        
      }
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

