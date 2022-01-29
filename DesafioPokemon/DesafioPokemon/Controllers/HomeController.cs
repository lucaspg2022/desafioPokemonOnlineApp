using DesafioPokemon.Models;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using static DesafioPokemon.Models.HomeModel;

namespace DesafioPokemon.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var model = new HomeModel();

            return View(model);
        }

        public async Task<ActionResult> Filtrar(string cidade)
        {
            HttpClient client = new HttpClient();

            string retornoClima = await client.GetStringAsync("https://api.openweathermap.org/data/2.5/weather?q=" + cidade + "&appid=8ed3ffcc12385a93aeebe881e2b84847");

            ResponseClima objClima = new JavaScriptSerializer().Deserialize<HomeModel.ResponseClima>(retornoClima);

            double temperatura = objClima.main.temp - 273;

            bool chovendo = objClima.weather.FirstOrDefault().main.Contains("rain");

            string tipo = ObterTipo(temperatura, chovendo);

            string retornoPokemon = await client.GetStringAsync("https://pokeapi.co/api/v2/type/" + tipo + "");

            ResponsePokemon pokemon = new JavaScriptSerializer().Deserialize<HomeModel.ResponsePokemon>(retornoPokemon);

            int pokemonEscolhido = new Random().Next(0, pokemon.pokemon.Length - 1);

            HomeModel model = new HomeModel();

            model.Temperatura = temperatura;
            model.Nome = pokemon.pokemon[pokemonEscolhido].pokemon.name.ToUpper();
            model.Cidade = cidade;
            model.Chovendo = chovendo ? "SIM" : "NÃO";

            return View("~/Views/Home/Index.cshtml", model);
        }

        private static string ObterTipo(double temperatura, bool chovendo)
        {
            string tipo;

            if (chovendo)
            {
                tipo = "electric";
            }
            else if (temperatura < 5)
            {
                tipo = "ice";
            }
            else if (temperatura >= 5 && temperatura < 10)
            {
                tipo = "water";
            }
            else if (temperatura >= 12 && temperatura < 15)
            {
                tipo = "grass";
            }
            else if (temperatura >= 15 && temperatura < 21)
            {
                tipo = "ground";
            }
            else if (temperatura >= 23 && temperatura < 27)
            {
                tipo = "bug";
            }
            else if (temperatura >= 27 || temperatura <= 33)
            {
                tipo = "rock";
            }
            else if (temperatura > 33)
            {
                tipo = "fire";
            }
            else
            {
                tipo = "normal";
            }

            return tipo;
        }
    }
}