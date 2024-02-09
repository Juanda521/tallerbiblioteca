namespace tallerbiblioteca.Models
{
    public class Encrypt{


        public string Encryptar( string cadena_encriptar){
            
            
            string resultado;
            byte[] encrypt =  System.Text.Encoding.Unicode.GetBytes(cadena_encriptar);

            resultado = Convert.ToBase64String(encrypt);

            return resultado;
        }

        public string Desencryptar(string cadena_desencriptar){

            string resultado;
            byte[] decryted = Convert.FromBase64String(cadena_desencriptar);
            resultado = System.Text.Encoding.Unicode.GetString(decryted);
            return resultado;

        }
    }
}