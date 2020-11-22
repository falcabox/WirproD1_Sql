using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WirproWebApi_D1.Models
{
    public class DadosMoeda
    {
        public string Moeda { get; set; }
        public DateTime Data_Inicio { get; set; }
        public DateTime Data_Fim { get; set; }
    }
}