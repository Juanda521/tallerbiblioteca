using System;
using System.Data.SqlClient;

using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace tallerbiblioteca.Services
{

    public class BackupService
    {
        private readonly IConfiguration _configuration;

        public BackupService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> GenerateDatabaseBackupAsync()
        {

            Console.WriteLine("estamos en los servicios del backup");
            string backupFolderPath = _configuration["BackupFolderPath"];
            
            string backupFileName = "BibliotecaFinalBookware.bak";
            string backupPath = Path.Combine(backupFolderPath, backupFileName);
            Console.WriteLine($"esta es la ruta donde guardaremos el archivo: {backupFolderPath}");

            using (var connection = new SqlConnection(_configuration.GetConnectionString("Connection")))
            {
                await connection.OpenAsync();
                Console.WriteLine("abrimos la conexion");


                var commandText = $"BACKUP DATABASE BibliotecaFinal TO DISK = '{backupPath}'";
                using (var command = new SqlCommand(commandText, connection))
                {
                    await command.ExecuteNonQueryAsync();
                    Console.WriteLine("realizamos la copia");
                }
            }
            return backupFileName;
        }

        public async Task<string> ObtenerRuta(){
           return _configuration["BackupFolderPath"];
        }

        public void RestoreDatabaseFromBackup(string backupFilePath)
        {
            try
            {
                // Crear un objeto ServerConnection usando la cadena de conexión
                ServerConnection serverConnection = new ServerConnection(_configuration.GetConnectionString("Connection"));

                // Crear un objeto Server usando la conexión
                Server server = new Server(serverConnection);

                // Crear un objeto Restore y establecer propiedades
                Restore restore = new Restore();
                restore.Database = server.Databases["bibliotecaFinal"].Name;
                restore.Action = RestoreActionType.Database;
                restore.Devices.AddDevice(backupFilePath, DeviceType.File);

                // Ejecutar la operación de restauración
                restore.SqlRestore(server);
                
                Console.WriteLine("La restauración se ha completado con éxito.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al restaurar la base de datos: " + ex.Message);
            }
        }
    }
}
