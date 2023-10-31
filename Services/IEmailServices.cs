
using tallerbiblioteca.Models;

namespace tallerbiblioteca.Services
{
    public interface IEmailServices
    {
        void SendEmail(SendEmailDTO request);

       SendEmailDTO EmailRegisterUser(string destinatario);

       SendEmailDTO EmailPrestamo(Prestamo prestamo);
    }

}