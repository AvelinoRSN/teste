#language: pt
Funcionalidade: Busca de CEP no site dos Correios

  Cenario: Validar CEP inexistente e pesquisar CEP valido
    Dado que eu acesso o site de busca de CEP dos Correios
    Quando eu pesquiso pelo CEP "80700000"
    Entao devo visualizar a mensagem de que o CEP nao existe
    Quando eu volto para a tela inicial de busca
    E eu pesquiso pelo CEP "01013-001"
    Entao devo visualizar um resultado de endereco para o CEP pesquisado
    Quando eu volto para a tela inicial de busca
    Quando eu acesso a pagina de rastreamento dos Correios
    E eu pesquiso no rastreamento o codigo "SS987654321BR"
    Entao devo visualizar a mensagem de que o codigo de rastreamento nao esta correto
    E eu fecho o navegador
