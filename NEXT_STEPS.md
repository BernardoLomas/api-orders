# API Orders — Backlog de Desafios

Fase atual: pré-MVP, concluindo a primeira fatia vertical de pedidos de compra.

Os cards abaixo substituem a lista anterior. Eles estão ordenados por dependência e devem ser concluídos sequencialmente. Cada card descreve comportamentos observáveis sem prescrever uma estrutura de implementação.

## Diretrizes arquiteturais

- Preserve as fronteiras atuais entre `Api`, `Application`, `Domain` e `Infrastructure`.
- Mantenha as invariantes de negócio no domínio e a orquestração dos casos de uso na aplicação.
- Mantenha questões HTTP na API e detalhes de persistência na infraestrutura.
- Prefira contratos pequenos e explícitos a frameworks ou abstrações genéricas.
- Não introduza banco de dados, mediator, event bus, framework CQRS ou biblioteca de mapeamento sem uma necessidade demonstrada.
- Preserve a propagação de `CancellationToken` entre fronteiras assíncronas.

---

# TAREFA 1 — Tornar confiável o total do pedido

## Contexto

Os itens calculam seus totais, mas o pedido atualmente informa zero porque o total do agregado não acompanha seus itens. Isso torna financeiramente incorretas as respostas de criação e consulta.

## Onde está o problema

- Em `PurchaseOrder`, a propriedade `Total` possui setter privado, mas nunca recebe nem recalcula um valor.
- O método `PurchaseOrder.AddItem` adiciona um novo `PurchaseOrderItem` ou chama `IncreaseQuantity`, porém não atualiza o total do pedido em nenhum desses caminhos.
- `PurchaseOrderItem.Total` já calcula corretamente `Quantity * UnitPrice`; atualmente esse valor não é consolidado pelo agregado.
- `CreatePurchaseOrderHandler.HandleAsync` copia `purchaseOrder.Total` para `CreatePurchaseOrderResult`, propagando o zero incorreto para a API.
- `GetPurchaseOrderHandler.HandleAsync` devolve o mesmo agregado, portanto a consulta também apresenta o valor incorreto.

## Objetivo

Garantir que um pedido sempre exponha a soma dos totais de seus itens, inclusive após adicionar um produto ou aumentar a quantidade de um produto existente.

## Regras de negócio

- O total do pedido é a soma de `quantidade × preço unitário` de todos os itens.
- Adicionar o mesmo produto pelo mesmo preço aumenta sua quantidade, sem criar uma linha duplicada.
- Adicionar o mesmo produto com preço diferente continua sendo inválido.
- Todos os itens devem usar a moeda do pedido.
- Operações rejeitadas não podem deixar o agregado parcialmente alterado.

## Restrições técnicas

- O modelo de domínio permanece como fonte da verdade.
- Chamadores não devem calcular nem atribuir o total.
- Evite manter representações independentemente mutáveis do mesmo valor.
- Não altere o contrato HTTP de entrada nesta tarefa.

## Critérios de aceite

- Um pedido novo sem itens informa total zero.
- Um item gera o total correto.
- Vários itens diferentes geram a soma correta.
- Adicionar duas vezes o mesmo produto pelo mesmo preço atualiza quantidade e total.
- Preços decimais são calculados sem conversão para ponto flutuante.
- As respostas de criação e consulta apresentam o total correto.

## Casos de borda

- Várias unidades do mesmo produto.
- Preços unitários decimais.
- Quantidade, preço ou moeda rejeitados.
- Segunda adição inválida de um produto existente.

## Definição de pronto

- O agregado atende a todos os critérios de aceite.
- Testes de domínio cobrem total e produto repetido.
- Os comportamentos de status e moeda permanecem intactos.
- Toda a suíte de testes passa.

---

# TAREFA 2 — Testar o caso de uso de criação de pedido

## Contexto

O handler de criação coordena quatro repositórios e várias pré-condições de negócio, mas não possui cobertura automatizada. Regressões de elegibilidade, relacionamento comercial ou persistência podem passar despercebidas.

## Onde está o problema

- `CreatePurchaseOrderHandler.HandleAsync` concentra toda a orquestração da criação e não possui testes.
- As dependências `ISupplierRepository`, `IProductRepository`, `ISupplierProductRepository` e `IPurchaseOrderRepository` participam do fluxo, mas nenhum teste verifica como o handler reage aos resultados delas.
- A pasta de testes contém apenas `SupplierTests`; não existem testes da camada de aplicação nem do agregado `PurchaseOrder`.
- `SupplierTests.Deactivate_WhenActive_ShouldDeactivateSupplier` afirma que `UpdatedAt` permanece igual após uma mudança real de estado, contrariando o comportamento implementado em `Supplier.Deactivate`.
- Comparações diretas entre instantes produzidos por `DateTime.UtcNow` também podem tornar testes de atualização temporal instáveis.

## Objetivo

Adicionar testes da camada de aplicação que verifiquem a criação completa do pedido independentemente de HTTP e da infraestrutura concreta.

## Regras de negócio

- O fornecedor deve existir e estar ativo.
- Todo produto deve existir e estar ativo.
- O fornecedor selecionado deve vender ativamente cada produto solicitado.
- Preço e moeda vêm da relação entre fornecedor e produto, nunca da entrada do cliente.
- Um pedido deve conter ao menos um item e toda quantidade deve ser positiva.
- Uma solicitação inválida não deve persistir um pedido.

## Restrições técnicas

- Teste a fronteira da aplicação por meio dos contratos de repositório.
- Mantenha os testes determinísticos e independentes dos dados de seed.
- Não transforme o projeto de testes em uma segunda implementação de infraestrutura.
- Verifique resultados e interações importantes, não detalhes privados.

## Critérios de aceite

- O caminho feliz verifica ID, status de rascunho, moeda, total e persistência.
- Há testes para fornecedores inexistentes e inativos.
- Há testes para produtos inexistentes e inativos.
- Há testes para relações fornecedor-produto ausentes e inativas.
- Há testes para lista vazia, IDs inválidos e quantidades não positivas.
- Os testes provam que nenhuma ordem é salva quando a validação falha.

## Casos de borda

- Produtos repetidos no mesmo comando.
- Produtos de fornecedor com moedas incompatíveis.
- Cancelamento solicitado durante acesso aos repositórios.

## Definição de pronto

- A criação está coberta sem iniciar a aplicação web.
- Os nomes dos testes comunicam cenário e resultado esperado.
- Os testes falham por violações reais, não por suposições temporais.
- Toda a suíte passa consistentemente.

---

# TAREFA 3 — Expor o catálogo necessário para criar um pedido

## Contexto

Clientes conseguem listar fornecedores ativos, mas não descobrir quais produtos cada fornecedor oferece. IDs, preços e moedas aparecem apenas no console durante o seed, portanto o fluxo HTTP não é autossuficiente.

## Onde está o problema

- `SuppliersController.GetActive` retorna somente os dados definidos em `SupplierResult`: ID, nome e descrição.
- Não existe controller ou caso de uso de leitura para produtos oferecidos por um fornecedor.
- `ISupplierProductRepository` permite apenas buscar uma relação por fornecedor e produto já conhecidos; ele não permite listar as ofertas de um fornecedor.
- `IProductRepository` permite apenas buscar produto por ID; não oferece a leitura necessária para compor um catálogo.
- `InMemorySupplierProductRepository` mantém todas as relações necessárias internamente, mas não expõe uma operação de consulta compatível com esse caso de uso.
- `InMemoryDataSeeder.Seed` imprime os IDs no console porque atualmente esse é o único meio de o usuário descobrir os produtos criados.

## Objetivo

Disponibilizar uma leitura dos produtos ativos oferecidos pelo fornecedor selecionado, com informações suficientes para montar uma solicitação válida de criação.

## Funcionalidade

- O cliente seleciona um fornecedor ativo.
- Consulta o catálogo desse fornecedor.
- A resposta identifica os produtos e apresenta informações comerciais relevantes.
- O cliente envia somente fornecedor, produtos e quantidades na criação do pedido.

## Regras de negócio

- Fornecedores inativos não podem ser usados.
- Produtos inativos não são retornados.
- Relações fornecedor-produto inativas não são retornadas.
- Preço e moeda podem ser exibidos, mas são novamente resolvidos pelo servidor na criação.
- Um catálogo válido e vazio retorna sucesso com coleção vazia.

## Restrições técnicas

- Não exponha coleções da infraestrutura diretamente.
- Retorne um contrato específico de leitura, não entidades de domínio.
- Mantenha a semântica de filtragem atrás dos contratos da aplicação.
- Não implemente administração de produtos ou busca genérica nesta tarefa.

## Critérios de aceite

- O cliente descobre via HTTP todos os identificadores necessários para criar um pedido.
- Um fornecedor válido retorna somente ofertas ativas.
- Um fornecedor válido sem ofertas retorna coleção vazia.
- Um fornecedor inexistente produz a resposta de recurso não encontrado acordada.
- Campos e códigos de resposta aparecem no Swagger.

## Casos de borda

- Fornecedor existente, porém inativo.
- Produto ativo com relação comercial inativa.
- Produto inativo com relação comercial ativa.
- Vários produtos com preços ou moedas diferentes.

## Definição de pronto

- O caso de uso cruza API, Aplicação e Infraestrutura respeitando as fronteiras.
- Os contratos de repositório suportam a leitura sem vazar detalhes em memória.
- Testes cobrem sucesso, vazio, filtragem e fornecedor inexistente.
- O fluxo pode ser concluído sem consultar o console.

---

# TAREFA 4 — Definir contratos estáveis de leitura do pedido

## Contexto

O endpoint de consulta serializa diretamente a entidade de domínio. Isso acopla a API externa ao formato interno e transforma mudanças de domínio em mudanças acidentais de contrato.

## Onde está o problema

- `GetPurchaseOrderHandler.HandleAsync` retorna `PurchaseOrder?`, expondo a entidade de domínio na saída da camada de aplicação.
- `PurchaseOrdersController.GetById` recebe essa entidade e a entrega diretamente em `Ok(purchaseOrder)`.
- Não existe um resultado específico para a consulta, equivalente ao `CreatePurchaseOrderResult` usado na criação.
- Como consequência, propriedades públicas adicionadas ou alteradas em `PurchaseOrder` e `PurchaseOrderItem` podem mudar o JSON da API sem uma decisão explícita sobre o contrato.
- O controller também conhece `PurchaseOrder`, criando uma dependência desnecessária da borda HTTP com a representação interna do agregado.

## Objetivo

Retornar uma representação explícita de leitura contendo o resumo do pedido e os detalhes dos itens.

## Regras de negócio

- A resposta reflete o retrato comercial persistido: produto, quantidade, preço unitário, moeda e total da linha.
- O total do pedido deve corresponder à soma das linhas.
- Pedido inexistente retorna recurso não encontrado.
- Comportamentos e métodos internos do domínio não pertencem ao contrato HTTP.

## Restrições técnicas

- O mapeamento pertence à fronteira Aplicação/API, não à Infraestrutura.
- Não adicione biblioteca de mapeamento para esse contrato pequeno.
- Não redesenhe o agregado nem armazene nome do produto sem requisito explícito.
- Preserve a rota atual salvo decisão deliberada de versionamento.

## Critérios de aceite

- `GET /purchase-orders/{id}` deixa de declarar entidade de domínio como resultado.
- A resposta contém IDs do pedido e fornecedor, status, moeda, total, datas e itens.
- Totais das linhas e do pedido estão corretos.
- Pedido inexistente retorna `404` com o formato padronizado.

## Casos de borda

- Pedido em rascunho vazio, caso possa existir internamente.
- Produtos repetidos consolidados em uma linha.
- Serialização de enum consistente e documentada.

## Definição de pronto

- O contrato HTTP é explícito e testado.
- Refatorações do domínio não alteram automaticamente a resposta.
- O Swagger descreve corretamente sucesso e recurso não encontrado.

---

# TAREFA 5 — Padronizar validações e respostas de erro

## Contexto

Falhas da aplicação e do domínio escapam como exceções gerais. Sem uma fronteira de tradução, erros previsíveis do cliente podem virar respostas `500` inconsistentes.

## Onde está o problema

- `CreatePurchaseOrderHandler.HandleAsync` usa `ArgumentException`, `ArgumentOutOfRangeException`, `KeyNotFoundException` e `InvalidOperationException` para comunicar categorias diferentes de falha.
- `PurchaseOrder`, `PurchaseOrderItem`, `Supplier`, `Product` e `SupplierProduct` também lançam exceções de domínio que podem atravessar a aplicação.
- `PurchaseOrdersController.Create` não traduz nenhuma dessas falhas para respostas HTTP.
- `PurchaseOrdersController.GetById` trata somente o retorno nulo; uma entrada inválida rejeitada por `GetPurchaseOrderHandler` não recebe tratamento explícito.
- `Program.cs` não registra middleware, exception handler ou configuração central de `ProblemDetails` para essa tradução.
- Os controllers não documentam os possíveis contratos de erro para o Swagger.

## Objetivo

Introduzir um mecanismo centralizado na API que traduza categorias conhecidas de falha em códigos HTTP e respostas Problem Details estáveis.

## Regras de negócio

- Entrada malformada ou inválida é erro do cliente.
- Fornecedores, produtos ou pedidos inexistentes são recursos não encontrados.
- Conflitos com o estado atual são distintos de entradas malformadas.
- Falhas inesperadas permanecem erros do servidor sem expor detalhes internos.

## Restrições técnicas

- Decisões HTTP permanecem na API.
- Domínio e Aplicação não dependem de tipos do ASP.NET Core.
- Use as convenções Problem Details da plataforma.
- Mantenha o modelo de falhas tão pequeno quanto os casos de uso atuais exigem.

## Critérios de aceite

- Falhas conhecidas de validação retornam `400` consistente.
- Recursos ausentes retornam `404` consistente.
- Operações incompatíveis com o estado retornam consistentemente `409` ou `422`, conforme decisão explícita.
- Exceções inesperadas retornam `500` sanitizado.
- Falhas equivalentes usam o mesmo tipo de conteúdo e campos principais.

## Casos de borda

- JSON inválido ou campos ausentes tratados pelo model binding.
- GUID vazio em rota ou entrada.
- Cancelamento iniciado pelo cliente.
- Exceções de domínio após validações da aplicação.

## Definição de pronto

- A tradução de erros é centralizada, sem repetição nos controllers.
- Testes de integração verificam código e formato de cada categoria.
- O Swagger documenta respostas de falha relevantes.
- Logs permitem diagnóstico sem vazar detalhes ao cliente.

---

# TAREFA 6 — Comprovar a primeira fatia vertical via HTTP

## Contexto

Testes unitários e de aplicação não provam que roteamento, model binding, injeção, serialização, seed e tratamento de erros funcionam em conjunto.

## Onde está o problema

- `PurchaseOrderApi.Tests` referencia Domain e Application, mas não referencia a API; atualmente ele não consegue hospedar o pipeline HTTP do projeto.
- Não existem testes para `PurchaseOrdersController`, `SuppliersController` ou para a aplicação iniciada por `Program.cs`.
- O registro manual dos repositórios e handlers em `Program.cs` não é verificado por nenhum teste.
- Os repositórios em memória são singletons e guardam estado em `Dictionary`; testes compartilhando a mesma aplicação podem interferir entre si.
- `InMemoryDataSeeder` gera novos GUIDs durante a inicialização, impedindo que testes dependam de identificadores fixos.
- O projeto ainda não prova automaticamente que o header `Location` gerado pelo POST conduz ao GET do mesmo pedido.

## Objetivo

Adicionar testes de integração focados no fluxo fornecedor → catálogo → criação → consulta do pedido.

## Regras de negócio

- Neste nível, verifique apenas o comportamento HTTP público.
- O pedido criado deve ser consultável com os mesmos valores comerciais.
- Testes não dependem de IDs impressos no console nem da ordem de execução.

## Restrições técnicas

- Exercite o pipeline real do ASP.NET Core com dados controlados.
- Isole o estado para execuções repetíveis e independentes.
- Mantenha a suíte focada; combinações detalhadas pertencem a testes inferiores.
- Não exija serviços externos para o MVP em memória.

## Critérios de aceite

- Um teste descobre fornecedor e ofertas via HTTP.
- Um teste cria pedido e recebe `201` com header `Location` utilizável.
- Seguir a localização retorna o pedido criado.
- Itens, moeda, status e total consultados correspondem à criação.
- Solicitações inválidas, ausentes e conflitantes verificam o contrato de erro.

## Casos de borda

- Ausência de fornecedores ou ofertas nos dados controlados.
- Várias solicitações na mesma instância da aplicação.
- Testes paralelos compartilhando repositórios singleton em memória.

## Definição de pronto

- A jornada principal está protegida por testes determinísticos.
- Os testes executam repetidamente e em paralelo sem vazamento de estado.
- Toda a solução compila e todos os testes passam.

---

# TAREFA 7 — Atualizar a documentação da API

## Contexto

O README contém apenas o nome do repositório e o arquivo HTTP ainda aponta para o endpoint removido do template. Um desenvolvedor não consegue executar o produto sem inspecionar o código.

## Onde está o problema

- `README.md` contém somente o título do repositório.
- `PurchaseOrderApi.Api.http` ainda chama `/weatherforecast`, rota que não existe no projeto atual.
- As rotas presentes em `PurchaseOrdersController` e `SuppliersController` não possuem exemplos manuais de uso.
- O fluxo depende dos IDs gerados por `InMemoryDataSeeder`, mas essa limitação e a perda de dados ao reiniciar não estão documentadas.
- A separação e a direção das dependências entre os quatro projetos só podem ser descobertas lendo os arquivos `.csproj` e o código.
- Não há instruções documentadas para compilar, executar ou testar a solução.

## Objetivo

Documentar a arquitetura atual, execução local, jornada da API e limitações da persistência em memória.

## Funcionalidade

- Explique a responsabilidade de cada projeto.
- Mostre como executar e testar a solução.
- Forneça exemplos para descobrir fornecedores, consultar catálogo, criar e consultar pedidos.
- Explique que os dados e IDs do seed mudam após reiniciar.

## Restrições técnicas

- Documente comportamentos implementados, não planejados.
- Não trate GUIDs gerados pelo seed como estáveis.
- Mantenha exemplos alinhados às rotas e contratos reais.
- Mantenha a documentação arquitetural concisa e atual.

## Critérios de aceite

- Um novo desenvolvedor identifica a direção das dependências.
- Um novo desenvolvedor consegue executar API e testes com os comandos documentados.
- O arquivo HTTP não contém a requisição WeatherForecast do template.
- A sequência documentada exercita todo o fluxo do MVP.
- Limitações e decisões adiadas estão explícitas.

## Casos de borda

- Perfis locais HTTP e HTTPS.
- Identificadores dinâmicos do seed.
- Erros esperados e seu formato Problem Details.

## Definição de pronto

- README e exemplos HTTP correspondem à API implementada.
- Comandos e exemplos foram verificados manualmente.
- Nenhuma funcionalidade futura é apresentada como disponível.

---

## Decisões adiadas — não são tarefas atuais

Os itens abaixo ficam fora do backlog do MVP até surgir um requisito concreto:

- Escolha de banco relacional e ORM.
- Modelo de autenticação e autorização.
- Endpoints de alteração, confirmação e cancelamento de pedidos.
- Administração de fornecedores e produtos.
- Paginação e filtros genéricos.
- Conversão de moedas.
- Mensageria, eventos de domínio ou event bus.
- CQRS ou frameworks mediator.
- Implantação, contêineres e infraestrutura em nuvem.

Quando algum deles virar requisito real, crie um card separado com motivação de negócio e critérios de aceite antes de alterar a arquitetura.
