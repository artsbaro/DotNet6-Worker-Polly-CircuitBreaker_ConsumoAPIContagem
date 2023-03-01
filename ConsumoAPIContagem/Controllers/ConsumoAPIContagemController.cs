using ConsumoAPIContagem.Model;
using Microsoft.AspNetCore.Mvc;
using Polly.CircuitBreaker;

namespace ConsumoAPIContagem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConsumoAPIContagemController : ControllerBase
    {

        private readonly ILogger<ConsumoAPIContagemController> _logger;
        private readonly AsyncCircuitBreakerPolicy _circuitBreaker;

        public ConsumoAPIContagemController(ILogger<ConsumoAPIContagemController> logger, 
            AsyncCircuitBreakerPolicy circuitBreaker)
        {
            _logger = logger;
            _circuitBreaker = circuitBreaker;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IActionResult> Get()
        {
            var httpClient = new HttpClient();
            var urlApiContagem = "http://localhost:5235/Contador";


            try
            {
                var resultado = await _circuitBreaker.ExecuteAsync<ResultadoContador>(() =>
                {
                    return httpClient
                        .GetFromJsonAsync<ResultadoContador>(urlApiContagem)!;
                });

                _logger.LogInformation($"* {DateTime.Now:HH:mm:ss} * " +
                    $"Circuito = {_circuitBreaker.CircuitState} | " +
                    $"Contador = {resultado.ValorAtual} | " +
                    $"Mensagem = {resultado.Mensagem}");

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError($"# {DateTime.Now:HH:mm:ss} # " +
                    $"Circuito = {_circuitBreaker.CircuitState} | " +
                    $"Falha ao invocar a API: {ex.GetType().FullName} | {ex.Message}");

                // Pode-se pegar os valores de contingência de outro lugar
                // Ex: Base de dados, Cache, AppSettings.json, Parameters Store
                var objContingencia = new ResultadoContador { ValorAtual = 10, Mensagem = "Teste de contingencia" };
                return  Ok(objContingencia);
            }
        }
    }
}