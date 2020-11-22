using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Text.RegularExpressions;

namespace ServicoLeituraMoedas
{
    public partial class SrvLeituraMoedas : ServiceBase
    {
        public SrvLeituraMoedas()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            EventLog.WriteEntry("Iniciando o servico de leitura de moedas", EventLogEntryType.Warning);
            ThreadStart Start = new ThreadStart(Leitura);
            Thread thread = new Thread(Start);
            thread.Start();

        }

        protected override void OnStop()
        {
            EventLog.WriteEntry("Finalizado o serviço de leitura de moedas", EventLogEntryType.Warning);
        }

        public void Leitura()
        {
            try
            {
                int vTempoLeitura = 120;

                while (true)
                {
                    DateTime startTime = DateTime.Now;
                    double interval = 0;

                    while (true)
                    {
                        //Pesquisa a cada 120 segundos
                        interval = (DateTime.Now - startTime).Seconds;

                        if (interval >= vTempoLeitura)
                        { 
                            GetAllMoedas();

                            startTime = DateTime.Now;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private async void GetAllMoedas()
        {
            string URI = "";

            URI = "http://localhost:51055/api/DadosMoedas";
            List<Moedas> listaMoedas = new List<Moedas>();

            if (listaMoedas != null)
            { 
                using (var client = new HttpClient())
                {
                    using (var response = await client.GetAsync(URI))
                    {
                        var MoedaJsonString = await response.Content.ReadAsStringAsync();
                        listaMoedas = JsonConvert.DeserializeObject<Moedas[]>(MoedaJsonString).ToList();

                        string[] linhaOK = null;
                        string linha = "";
                        StreamReader rd = new StreamReader("C:\\Falcari\\_Particular\\Wirpro\\Projetos\\DadosMoeda.CSV");

                        List<Moedas> ListaSelecao = new List<Moedas>();

                        foreach (var item in listaMoedas)
                        {
                            linha = rd.ReadLine();
                            if (linha == null)
                            {
                                break;
                            }
                            linhaOK = linha.Split(';');

                            var vI = linhaOK[1];
                            var vF = linhaOK[2];

                            if (DateTime.Parse(vI) >= item.Data_Inicio && DateTime.Parse(vF) <= item.Data_Fim)
                            {
                                ListaSelecao.Add(item);
                            }
                        }

                        //Iniciando o CSV
                        string dataAtual = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
                        string horaAtual = DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();

                        StreamWriter wt = new StreamWriter("C:\\Wirpro\\Resultado" + dataAtual + "_" + horaAtual + ".csv");

                        wt.WriteLine("ID_MOEDA;DATA_REF;VL_COTACAO");
                        

                        //ListaSelecao com dados da DadosMoeda.csv, contendo moedas que estao no período
                        foreach (var item in ListaSelecao)
                        {
                            string moedaPesquisa = string.Empty;

                            //Abaixo alguns exemplos da lista de moedas
                            switch (item.Moeda)
                            {
                                case "USD":
                                    moedaPesquisa = "Dólar dos Estados Unidos (USD)";
                                    break;
                                case "AFN":
                                    moedaPesquisa = "Afegane (AFN)";
                                    break;
                                case "MGA":
                                    moedaPesquisa = "Ariary (MGA)";
                                    break;
                                case "PAB":
                                    moedaPesquisa = "Balboa (PAB)";
                                    break;
                                case "THB":
                                    moedaPesquisa = "Baht (THB)";
                                    break;
                                case "EUR":
                                    moedaPesquisa = "Euro (EUR)";
                                    break;
                                case "CHF":
                                    moedaPesquisa = "Franco suíço (CHF)";
                                    break;
                                default:
                                    moedaPesquisa = "Dólar dos Estados Unidos (USD)";
                                    break;
                            }

                            string converterde = "1,00";

                            ChromeOptions opt = new ChromeOptions();
                            opt.AddArgument("--disable-notifications");
                            opt.AddArgument("headless");

                            opt.AddAdditionalCapability("useAutomationExtension", false);

                            OpenQA.Selenium.IWebDriver driver = new ChromeDriver(@"C:\Users\Public", opt);
                            driver.Navigate().GoToUrl("https://www.bcb.gov.br/conversao");
                            System.Threading.Thread.Sleep(4000);
                            //driver.FindElement(By.Name("inputDate")).SendKeys("11/20/2020");
                            driver.FindElement(By.Name("inputDate")).SendKeys(Convert.ToString(item.Data_Inicio));
                            driver.FindElement(By.Name("valorBRL")).SendKeys(converterde);
                            driver.FindElement(By.Id("button-converter-para")).Click();
                            driver.FindElement(By.XPath("//*[contains(text(),'" + moedaPesquisa + "')]")).Click();
                            System.Threading.Thread.Sleep(4000);
                            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                            string innerHtml = (string)js.ExecuteScript("return document.documentElement.innerHTML;");
                            string resultadodaconversao = Regex.Match(innerHtml, @"(Para: )(.*?)(?=<\/div>)", RegexOptions.Singleline).Value;
                            resultadodaconversao = Regex.Match(resultadodaconversao, @"(?<=ão:<\/strong>)(.*?)($)", RegexOptions.Singleline).Value.Trim();

                            //Inserindo a linha no CSV
                            wt.WriteLine(item.Moeda + ";" + item.Data_Inicio + ";" + resultadodaconversao);


                        }


                    }
                }
            }   
        }

    }
}
