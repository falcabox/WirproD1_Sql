using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WirproWebApi_D1.Models
{
    interface IMoedasRepositorio
    {
        IEnumerable<DadosMoeda> GetAll();
        DadosMoeda Get(string moeda);
        bool Add(DadosMoeda dadosMoeda);
        void Remove(string moeda, DateTime dtMoeda);
        bool Update(DadosMoeda dadosMoeda);
       
    }
}
