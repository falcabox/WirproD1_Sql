using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WirproWebApi_D1.Models;

namespace WirproWebApi_D1.Controllers
{
    public class DadosMoedasController : ApiController
    {
        static readonly IMoedasRepositorio dadosMoedasRepositorio = new DadosMoedaRepositorio();

        // Postman = http://localhost:51055/Api/DadosMoedas
        public HttpResponseMessage GetItemFila()
        {
            DadosMoeda dadosMoeda = dadosMoedasRepositorio.GetAll().LastOrDefault();

            if (dadosMoeda != null)
            {
                dadosMoeda.Data_Fim = dadosMoeda.Data_Inicio.AddDays(30);
                DeleteDadosMoeda(dadosMoeda.Moeda, dadosMoeda.Data_Inicio);
                return Request.CreateResponse<DadosMoeda>(HttpStatusCode.OK, dadosMoeda);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Não há registros para a leitura.");
            }
        }

        //Postman = http://localhost:51055/Api/DadosMoedas?moeda=US$
        public HttpResponseMessage GetDadoMoeda(string moeda)
        {
            DadosMoeda dadosMoeda = dadosMoedasRepositorio.Get(moeda);
            if (moeda == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Moeda não foi encontrada");
            }
            else
            {
                return Request.CreateResponse<DadosMoeda>(HttpStatusCode.OK, dadosMoeda);
            }
        }




       

        //http://localhost:51055/Api/DadosMoedas?moeda=AAA&dataMoeda=12/12/2002
        [HttpPost]
        public HttpResponseMessage PostDadosMoedas(string moeda, DateTime dataMoeda)
        {
            DadosMoeda dadosMoeda = new DadosMoeda();
            dadosMoeda.Moeda = moeda;
            dadosMoeda.Data_Inicio = dataMoeda;

            bool result = dadosMoedasRepositorio.Add(dadosMoeda);

            if (result)
            {
                var response = Request.CreateResponse<DadosMoeda>(HttpStatusCode.Created, dadosMoeda);
                string uri = Url.Link("DefaultApi", new { moeda = dadosMoeda.Moeda });
                response.Headers.Location = new Uri(uri);
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Moeda não foi incluida");
            }

        }

        public HttpResponseMessage AddItemFila(string moeda, DateTime dataMoeda)
        {
            DadosMoeda dadosMoeda = new DadosMoeda();
            dadosMoeda.Moeda = moeda;
            dadosMoeda.Data_Inicio = dataMoeda;

            if (!dadosMoedasRepositorio.Update(dadosMoeda))
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Moeda nao atualizada na base");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }
        }



        public HttpResponseMessage DeleteDadosMoeda(string moeda, DateTime dtMoeda)
        {
            dadosMoedasRepositorio.Remove(moeda, dtMoeda);
            //dadosMoedasRepositorio.Remove(moeda);
            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }


    }
}
