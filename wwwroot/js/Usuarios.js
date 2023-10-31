function MostrarMensajeUsuarios(icono,Mensaje) {
    console.log(icono);
    Swal.fire({
        icon: icono,
        title: 'BookWare Dice',
        text: Mensaje,
        footer: '<a>!BookWare!</a>'
    });
}

document.addEventListener('DOMContentLoaded', function() {
    const passwordInput = document.getElementById('Contraseña');
    const togglePasswordButton = document.getElementById('togglePassword');

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
});


