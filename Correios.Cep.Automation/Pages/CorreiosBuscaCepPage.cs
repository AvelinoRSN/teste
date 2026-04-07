using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Correios.Cep.Automation.Pages;

public class CorreiosBuscaCepPage
{
    private const string UrlBuscaCep = "https://buscacepinter.correios.com.br/app/endereco/index.php";
    private const string UrlRastreamento = "https://rastreamento.correios.com.br/app/index.php";
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;

    public CorreiosBuscaCepPage(IWebDriver driver)
    {
        _driver = driver;
        _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
    }

    public void AcessarPagina()
    {
        NavegarComRetry(UrlBuscaCep);
        _wait.Until(d => d.FindElement(By.CssSelector("input[name='endereco'], #endereco")).Displayed);
    }

    public void PesquisarCep(string cep)
    {
        var campoEndereco = _wait.Until(d => d.FindElement(By.CssSelector("input[name='endereco'], #endereco")));
        campoEndereco.Clear();
        campoEndereco.SendKeys(cep);

        Thread.Sleep(TimeSpan.FromSeconds(10));

        var botaoBuscar = _driver.FindElements(By.CssSelector("button[type='submit'], input[type='submit'], #btn_pesquisar"))
            .First(e => e.Displayed && e.Enabled);
        botaoBuscar.Click();
    }

    public void AguardarResolucaoCaptcha(int timeoutSegundos = 120)
    {
        var waitCaptcha = new WebDriverWait(_driver, TimeSpan.FromSeconds(timeoutSegundos));
        waitCaptcha.Until(d =>
        {
            var textoPagina = d.PageSource.ToLowerInvariant();
            var apareceuResultado = d.Url.Contains("app/endereco", StringComparison.OrdinalIgnoreCase)
                                    && (textoPagina.Contains("dados não encontrado")
                                        || textoPagina.Contains("dados nao encontrado")
                                        || textoPagina.Contains("dados nao encontrados")
                                        || textoPagina.Contains("não há dados")
                                        || textoPagina.Contains("nao ha dados")
                                        || d.FindElements(By.CssSelector("table tbody tr, #resultado-DNEC tbody tr")).Count > 0);
            return apareceuResultado;
        });
    }

    public bool ExibeMensagemCepInexistente()
    {
        var texto = _driver.PageSource.ToLowerInvariant();
        var contemMensagem = texto.Contains("dados não encontrado")
                             || texto.Contains("dados nao encontrado")
                             || texto.Contains("dados nao encontrados")
                             || texto.Contains("não há dados")
                             || texto.Contains("nao ha dados")
                             || texto.Contains("nenhum registro encontrado")
                             || texto.Contains("nenhum resultado");

        var semLinhasNaTabela = _driver.FindElements(By.CssSelector("#resultado-DNEC tbody tr, table tbody tr")).Count == 0;

        return contemMensagem || semLinhasNaTabela;
    }

    public void VoltarParaTelaInicial()
    {
        var botoesVoltar = _driver.FindElements(By.CssSelector("a[href*='index.php'], button, input[type='button'], input[type='submit']"))
            .Where(e => e.Displayed)
            .ToList();

        var botao = botoesVoltar.FirstOrDefault(e =>
            e.Text.Contains("Nova Busca", StringComparison.OrdinalIgnoreCase)
            || e.Text.Contains("Voltar", StringComparison.OrdinalIgnoreCase)
            || e.GetAttribute("value")?.Contains("Nova Busca", StringComparison.OrdinalIgnoreCase) == true
            || e.GetAttribute("value")?.Contains("Voltar", StringComparison.OrdinalIgnoreCase) == true);

        if (botao is not null)
        {
            botao.Click();
        }
        else
        {
            _driver.Navigate().GoToUrl(UrlBuscaCep);
        }

        _wait.Until(d => d.FindElement(By.CssSelector("input[name='endereco'], #endereco")).Displayed);
    }

    public bool ExibeResultadoDeEndereco(string cep)
    {
        var texto = _driver.PageSource.ToLowerInvariant();
        var linhasResultado = _driver.FindElements(By.CssSelector("#resultado-DNEC tbody tr, table tbody tr"));
        var cepNormalizado = cep.Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase);
        var cepComHifen = cep.Length == 8 ? $"{cep[..5]}-{cep[5..]}" : cep;

        return linhasResultado.Any()
               && !texto.Contains("dados não encontrado")
               && !texto.Contains("dados nao encontrado")
               && !texto.Contains("dados nao encontrados")
               && (texto.Contains(cepComHifen.ToLowerInvariant()) || texto.Contains(cepNormalizado.ToLowerInvariant()));
    }

    public void AcessarPaginaRastreamento()
    {
        NavegarComRetry(UrlRastreamento);
        _wait.Until(d => d.FindElements(By.CssSelector("input#objeto, input#objetos, input[name='objeto'], textarea#objetos")).Any());
    }

    public void PesquisarCodigoRastreamento(string codigoRastreamento)
    {
        var camposCodigo = _wait.Until(d =>
        {
            var elementos = d.FindElements(By.CssSelector("input#objeto, input#objetos, input[name='objeto'], textarea#objetos"));
            return elementos.Any() ? elementos : null;
        });

        var campoCodigo = camposCodigo!.FirstOrDefault(e => e.Displayed && e.Enabled) ?? camposCodigo.First();
        campoCodigo.Clear();
        campoCodigo.SendKeys(codigoRastreamento);

        Thread.Sleep(TimeSpan.FromSeconds(10));

        var botaoConsultar = _wait.Until(d =>
        {
            var candidatos = d.FindElements(By.CssSelector("button, input[type='submit'], input[type='button']"));
            foreach (var e in candidatos)
            {
                if (!e.Displayed || !e.Enabled)
                {
                    continue;
                }

                if (ElementoRepresentaBotaoConsultar(e))
                {
                    return e;
                }
            }

            return null;
        });

        botaoConsultar!.Click();
    }

    private static bool ElementoRepresentaBotaoConsultar(IWebElement elemento)
    {
        var texto = elemento.Text?.Trim() ?? string.Empty;
        if (texto.Equals("Consultar", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var valor = elemento.GetAttribute("value")?.Trim() ?? string.Empty;
        return valor.Equals("Consultar", StringComparison.OrdinalIgnoreCase);
    }

    public bool ExibeMensagemCodigoRastreamentoIncorreto()
    {
        var waitResultado = new WebDriverWait(_driver, TimeSpan.FromSeconds(30))
        {
            PollingInterval = TimeSpan.FromMilliseconds(400)
        };

        try
        {
            waitResultado.Until(d => ContemIndicativoDeErroOuObjetoNaoEncontradoNoRastreamento(d.PageSource));
        }
        catch (WebDriverTimeoutException)
        {
        }

        return ContemIndicativoDeErroOuObjetoNaoEncontradoNoRastreamento(_driver.PageSource);
    }

    private static bool ContemIndicativoDeErroOuObjetoNaoEncontradoNoRastreamento(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return false;
        }

        var t = html.ToLowerInvariant();
        var entidadeNao = "n&atilde;o";

        if (t.Contains("sro-"))
        {
            return true;
        }

        var padroesTexto = new[]
        {
            "objeto não encontrado",
            "objeto nao encontrado",
            $"objeto {entidadeNao} encontrado",
            "não encontrado na base",
            "nao encontrado na base",
            $"{entidadeNao} encontrado na base",
            "não localizado",
            "nao localizado",
            "código incorreto",
            "codigo incorreto",
            "código inválido",
            "codigo invalido",
            "formato inválido",
            "formato invalido",
            "não foi possível",
            "nao foi possivel",
            "informação não disponível",
            "informacao nao disponivel",
            "nenhum objeto",
            "verifique o código",
            "verifique o codigo",
            "verifique se o código",
            "código inexistente",
            "codigo inexistente",
            "dados não encontrados",
            "dados nao encontrados",
            "sem informações",
            "sem informacoes",
            "não há dados",
            "nao ha dados",
            "objeto inválido",
            "objeto invalido"
        };

        foreach (var p in padroesTexto)
        {
            if (t.Contains(p.ToLowerInvariant()))
            {
                return true;
            }
        }

        return false;
    }

    public void FecharNavegador()
    {
        try
        {
            _driver.Quit();
        }
        catch (WebDriverException)
        {
        }
    }

    private void NavegarComRetry(string url, int tentativas = 5)
    {
        WebDriverException? ultimaExcecao = null;

        for (var tentativa = 1; tentativa <= tentativas; tentativa++)
        {
            try
            {
                _driver.Navigate().GoToUrl(url);
                return;
            }
            catch (WebDriverException ex)
            {
                ultimaExcecao = ex;
                if (tentativa >= tentativas)
                {
                    break;
                }

                var segundosEspera = Math.Min(2 * tentativa, 10);
                Thread.Sleep(TimeSpan.FromSeconds(segundosEspera));
            }
        }

        throw new WebDriverException(
            $"Nao foi possivel abrir a URL apos {tentativas} tentativas: {url}. " +
            "Verifique rede, VPN, firewall ou proxy. Erro original:",
            ultimaExcecao);
    }
}
