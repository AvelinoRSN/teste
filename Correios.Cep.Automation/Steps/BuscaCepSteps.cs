using Correios.Cep.Automation.Pages;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace Correios.Cep.Automation.Steps;

[Binding]
public sealed class BuscaCepSteps
{
    private readonly CorreiosBuscaCepPage _buscaCepPage;
    private string _ultimoCepPesquisado = string.Empty;

    public BuscaCepSteps(ScenarioContext scenarioContext)
    {
        if (!scenarioContext.TryGetValue("WEB_DRIVER", out IWebDriver? driver) || driver is null)
        {
            throw new InvalidOperationException("WebDriver nao foi inicializado no ScenarioContext.");
        }

        _buscaCepPage = new CorreiosBuscaCepPage(driver);
    }

    [Given(@"que eu acesso o site de busca de CEP dos Correios")]
    public void DadoQueEuAcessoOSiteDeBuscaDeCepDosCorreios()
    {
        _buscaCepPage.AcessarPagina();
    }

    [When(@"eu pesquiso pelo CEP ""(.*)""")]
    public void QuandoEuPesquisoPeloCep(string cep)
    {
        _ultimoCepPesquisado = cep;
        _buscaCepPage.PesquisarCep(cep);
    }

    [When(@"eu resolvo o captcha manualmente e continuo")]
    public void QuandoEuResolvoOCaptchaManualmenteEContinuo()
    {
        _buscaCepPage.AguardarResolucaoCaptcha();
    }

    [Then(@"devo visualizar a mensagem de que o CEP nao existe")]
    public void EntaoDevoVisualizarAMensagemDeQueOCepNaoExiste()
    {
        Assert.That(_buscaCepPage.ExibeMensagemCepInexistente(), Is.True,
            "Era esperado encontrar a mensagem de CEP inexistente apos a busca.");
    }

    [When(@"eu volto para a tela inicial de busca")]
    public void QuandoEuVoltoParaATelaInicialDeBusca()
    {
        _buscaCepPage.VoltarParaTelaInicial();
    }

    [Then(@"devo visualizar um resultado de endereco para o CEP pesquisado")]
    public void EntaoDevoVisualizarUmResultadoDeEnderecoParaOCepPesquisado()
    {
        Assert.That(_buscaCepPage.ExibeResultadoDeEndereco(_ultimoCepPesquisado), Is.True,
            "Era esperado encontrar um resultado de endereco para o CEP informado.");
    }

    [When(@"eu acesso a pagina de rastreamento dos Correios")]
    public void QuandoEuAcessoAPaginaDeRastreamentoDosCorreios()
    {
        _buscaCepPage.AcessarPaginaRastreamento();
    }

    [When(@"eu pesquiso no rastreamento o codigo ""(.*)""")]
    public void QuandoEuPesquisoNoRastreamentoOCodigo(string codigo)
    {
        _buscaCepPage.PesquisarCodigoRastreamento(codigo);
    }

    [Then(@"devo visualizar a mensagem de que o codigo de rastreamento nao esta correto")]
    public void EntaoDevoVisualizarAMensagemDeQueOCodigoDeRastreamentoNaoEstaCorreto()
    {
        Assert.That(_buscaCepPage.ExibeMensagemCodigoRastreamentoIncorreto(), Is.True,
            "Era esperado encontrar uma mensagem informando codigo de rastreamento invalido/incorreto.");
    }

    [Then(@"eu fecho o navegador")]
    public void EntaoEuFechoONavegador()
    {
        _buscaCepPage.FecharNavegador();
    }
}
