
document.addEventListener('DOMContentLoaded', function() {
  var calendarEl = document.getElementById('calendar');
  var allEvents = []; // Array para almacenar todos los eventos

  function handleDatesRender(info) {
    console.log('viewType:', info.view.type);
  }

      // Función para abrir el modal Bootstrap
    function openBootstrapModal(dateStr,eventstr,userstr) {
      // Obtener el elemento del modal de fecha
      const modalDate = document.getElementById('modal-date');
      const modalEvent = document.getElementById('modal-event');
      const modalUser  = document.getElementById('modal-user');

      // Establecer el texto del modal de fecha
      modalDate.textContent = dateStr;
      modalEvent.textContent = eventstr;
      modalUser.textContent  = userstr;
      // Abrir el modal Bootstrap
      $('#myModal').modal('show');
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
          title: event.estado,
          start: event.fecha_inicio,
          timeEnd: event.fecha_fin,
          // eventEndTime: event.fecha_fin,
          // end: event.fecha_fin,
          id: event.id,
          color: 'green'
          // Puedes agregar más propiedades aquí si es necesario
        };
      });

      // Crear una variable para almacenar el elemento del calendario
      var calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'dayGridMonth',
        headerToolbar: {
          left: 'prev,next today',
          center: 'title',
          right: 'dayGridMonth,timeGridWeek,timeGridDay,listWeek'
        },datesRender: handleDatesRender,

        eventColor: 'green',
        events: allEvents, // Usar la matriz de eventos combinados
        dateClick: function(info) {
        
          // Buscar un evento que coincida con la fecha del calendario
          const clickedEvent = data.find(event => event.fecha_inicio.split('T')[0] == info.dateStr);
          // data.forEach(x => {
          //   let fecha =  x.fecha_inicio.split('T')[0];
          //   console.log(fecha)
          // });
          // console.log(info.dateStr)

          if (clickedEvent) {
            const motivo = clickedEvent.peticion.motivo;
            const user = clickedEvent.peticion.usuario.name;
            console.log("hablalo desde el if")
            console.log(clickedEvent)
            // Abrir el modal Bootstrap y pasar las variables motivo y user
            openBootstrapModal(info.dateStr, motivo, user);
          } else {
            // Abrir el modal Bootstrap sin información adicional
            openBootstrapModal(info.dateStr, "", "");
            console.log("hablalo desde el else ")
          }
              },
        datesSet: handleDatesRender,
        locale: 'es', 
      });

      calendar.render();
    });
});