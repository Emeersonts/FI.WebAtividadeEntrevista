using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FI.AtividadeEntrevista.DML
{
    public class Beneficiario
    {
        public long Id { get; set; }
        public long Idcliente { get; set; }
        public string Nome { get; set; }
        public string CPF { get; set; }
    }
}
