using System.ComponentModel.DataAnnotations;

namespace APItoPFinal.Helpers
{
    public static class Cpf
    {
        public static string Normalizar(string? cpf)
        {
            return new string((cpf ?? string.Empty).Where(Char.IsAsciiDigit).ToArray());
        }

        public static bool EhValido(string? cpf)
        {
            var d = Normalizar(cpf);

            if(d.Length != 11 || d.Distinct().Count() == 1)
            {
                return false;
            }

            return DigitoConfere(d, 9) && DigitoConfere(d, 10);
        }

        private static bool DigitoConfere(string d, int posicao)
        {
            var soma = 0;
            for (var i = 0; i < posicao; i++)
            {
                soma += (d[i] - '0') * (posicao + 1 - i);
            }

            var resto = (soma * 10) % 11;
            if (resto == 10) resto = 0;

            return resto == d[posicao] - '0';
        }
    }

    public class CpfValidationAttribute : ValidationAttribute
    {
        public CpfValidationAttribute() : base("CPF inválido.") { }

        public override bool IsValid(object? value)
        {
            //Vazio fica por conta do [Required]
            if(value is not string texto || string.IsNullOrWhiteSpace(texto))
            {
                return true;
            }

            return Cpf.EhValido(texto);
        }
    }
}
