using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.UI;
using System.Text;

namespace WirproWebApi_D1.Models
{
    public class DadosMoedaRepositorio : IMoedasRepositorio
    {
        private List<DadosMoeda> dadosMoedas = new List<DadosMoeda>();
        //private int _nextid = 1;




        public DadosMoedaRepositorio()
        {
            try
            {
                string linha = "";
                string[] linhaOK = null;
                int nuLinha = 0;
                StreamReader rd = new StreamReader("C:\\Falcari\\_Particular\\Wirpro\\Projetos\\DadosMoeda.CSV");
                while (true)
                {
                    nuLinha++;
                    linha = rd.ReadLine();

                    if (nuLinha == 1)
                    {
                        continue;
                    }

                    if (linha == null)
                    {
                        break;
                    }
                    linhaOK = linha.Split(';');

                    var v1 = linhaOK[0];
                    var v2 = linhaOK[1];

                    Add(new DadosMoeda { Moeda = v1, Data_Inicio = Convert.ToDateTime(v2), Data_Fim = DateTime.MaxValue });


                }
            }
            catch (Exception ex)
            {

                throw ex;
            }






            //Add(new DadosMoeda { Moeda = "US$", Data_Inicio = Convert.ToDateTime("30/12/2015"), Data_Fim = DateTime.MaxValue });
            //Add(new DadosMoeda { Moeda = "ARS", Data_Inicio = Convert.ToDateTime("25/04/2019"), Data_Fim = Convert.ToDateTime("30/04/2019") });
            //Add(new DadosMoeda { Moeda = "LSL", Data_Inicio = Convert.ToDateTime("24/12/2019"), Data_Fim = Convert.ToDateTime("31/12/2019") });


        }


        public bool Add(DadosMoeda dadosMoeda)
        {
            bool addResult = false;
            if (dadosMoedas == null)
            {
                return addResult;
            }

            int index = dadosMoedas.FindIndex(m => m.Moeda == dadosMoeda.Moeda);
            if (index == -1)
            {
                dadosMoedas.Add(dadosMoeda);
                addResult = true;
                return addResult;
            }
            else
            {
                return addResult;
            }


        }


        public DadosMoeda Get(string moeda)
        {
            return dadosMoedas.Find(m => m.Moeda == moeda);
        }




      

        public IEnumerable<DadosMoeda> GetAll()
        {
            return dadosMoedas;
        }


        
        public void Remove(string moeda, DateTime dtMoeda)
        {
            dadosMoedas.RemoveAll(m => m.Moeda == moeda && m.Data_Inicio == dtMoeda);
        }



        public bool Update(DadosMoeda dadosMoeda)
        {
            if (dadosMoeda == null)
            {
                throw new ArgumentNullException("dadosmoeda");
            }
            int index = dadosMoedas.FindIndex(m => m.Moeda == dadosMoeda.Moeda);

            if (index == -1)
            {
                return false;
            }
            dadosMoedas.RemoveAt(index);
            dadosMoedas.Add(dadosMoeda);
            return true;
        }
    }
}