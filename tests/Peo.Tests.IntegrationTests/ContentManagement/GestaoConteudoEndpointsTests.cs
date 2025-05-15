using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Peo.ContentManagement.Application.Dtos;
using Peo.ContentManagement.Application.UseCases.Aula.Cadastrar;
using Peo.ContentManagement.Application.UseCases.Aula.ObterTodos;
using Peo.ContentManagement.Application.UseCases.Curso.Cadastrar;
using Peo.ContentManagement.Application.UseCases.Curso.ObterTodos;
using Peo.ContentManagement.Domain.Entities;
using Peo.Identity.Application.Endpoints.Requests;
using Peo.Identity.Application.Endpoints.Responses;
using Peo.Tests.IntegrationTests.Setup;
using System.Net;
using System.Net.Http.Json;
using CursoCadastrar = Peo.ContentManagement.Application.UseCases.Curso.Cadastrar;
using CursoObterTodos = Peo.ContentManagement.Application.UseCases.Curso.ObterTodos;
using AulaCadastrar = Peo.ContentManagement.Application.UseCases.Aula.Cadastrar;
using AulaObterTodos = Peo.ContentManagement.Application.UseCases.Aula.ObterTodos;

namespace Peo.Tests.IntegrationTests.ContentManagement;

public class GestaoConteudoEndpointsTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly TestDatabaseSetup _testDb;
    private Guid _testUserId = Guid.CreateVersion7();
    private Curso? _cursoTeste;

    public GestaoConteudoEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _testDb = new TestDatabaseSetup(_factory.Services);
    }

    public async Task InitializeAsync()
    {
        await _testDb.CriarUsuarioAdmin(_testUserId);

        // Create test course
        _cursoTeste = await _testDb.CriarCursoTesteAsync(instrutorId: _testUserId);
        await LoginAsync();
    }

    private async Task LoginAsync()
    {
        // Arrange
        var request = new LoginRequest(_testDb.EmailUsuarioTeste, _testDb.SenhaUsuarioTeste);

        // Act
        var response = await _client.PostAsJsonAsync("/v1/identity/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrEmpty();

        _testUserId = result!.UserId;

        // Set the token in the client
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.Token);
    }

    public async Task DisposeAsync()
    {
        await _testDb.LimparAsync();
    }

    [Fact]
    public async Task CadastrarCurso_ComRequisicaoValida_DeveCadastrarCurso()
    {
        // Arrange
        var comando = new CursoCadastrar.Command(
            "Curso Teste",
            "Descrição Teste",
            _testUserId,
            null,
            99.99m,
            new List<string> { "teste", "integração" }
        );

        // Act
        var resposta = await _client.PostAsJsonAsync("/v1/conteudo/curso/", comando);

        // Assert
        resposta.StatusCode.Should().Be(HttpStatusCode.OK);
        var resultado = await resposta.Content.ReadFromJsonAsync<Peo.ContentManagement.Application.UseCases.Curso.Cadastrar.Response>();
        resultado.Should().NotBeNull();
        resultado!.CursoId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task CadastrarCurso_ComRequisicaoInvalida_DeveRetornarErroValidacao()
    {
        // Arrange
        var comando = new CursoCadastrar.Command(
            "", // Título inválido
            "Descrição Teste",
            _testUserId,
            null,
            99.99m,
            new List<string> { "teste", "integração" }
        );

        // Act
        var resposta = await _client.PostAsJsonAsync("/v1/conteudo/curso/", comando);

        // Assert
        resposta.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var resultado = await resposta.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        resultado.Should().NotBeNull();
        resultado.Errors.Any(e => e.Value.Any(x => x.Contains("The Titulo field is required."))).Should().BeTrue();
    }

    [Fact]
    public async Task ObterCursoPorId_ComIdValido_DeveRetornarCurso()
    {
        // Arrange
        var cursoId = _cursoTeste!.Id;

        // Act
        var resposta = await _client.GetAsync($"/v1/conteudo/curso/{cursoId}");

        // Assert
        resposta.StatusCode.Should().Be(HttpStatusCode.OK);
        var resultado = await resposta.Content.ReadFromJsonAsync<CursoResponse>();
        resultado.Should().NotBeNull();
        resultado!.Should().NotBeNull();
        resultado.Id.Should().Be(cursoId);
        resultado.Titulo.Should().Be(_cursoTeste.Titulo);
        resultado.Descricao.Should().Be(_cursoTeste.Descricao);
        resultado.Preco.Should().Be(_cursoTeste.Preco);
        resultado.EstaPublicado.Should().Be(_cursoTeste.EstaPublicado);
    }

    [Fact]
    public async Task ObterCursoPorId_ComIdInvalido_DeveRetornarNotFound()
    {
        // Arrange
        var cursoId = Guid.CreateVersion7();

        // Act
        var resposta = await _client.GetAsync($"/v1/conteudo/curso/{cursoId}");

        // Assert
        resposta.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ObterTodosCursos_DeveRetornarCursos()
    {
        // Act
        var resposta = await _client.GetAsync("/v1/conteudo/curso/");

        // Assert
        resposta.StatusCode.Should().Be(HttpStatusCode.OK);
        var resultado = await resposta.Content.ReadFromJsonAsync<Peo.ContentManagement.Application.UseCases.Curso.ObterTodos.Response>();
        resultado.Should().NotBeNull();
        resultado!.Cursos.Should().NotBeEmpty();
        resultado.Cursos.Should().Contain(c => c.Id == _cursoTeste!.Id);
    }

    [Fact]
    public async Task CadastrarAula_ComRequisicaoValida_DeveCadastrarAula()
    {
        // Arrange
        var comando = new AulaCadastrar.Command
        {
            Titulo = "Aula Teste",
            Descricao = "Descrição da Aula Teste",
            UrlVideo = "https://example.com/video",
            Duracao = TimeSpan.FromMinutes(30),
            Arquivos = new List<Peo.ContentManagement.Application.Dtos.ArquivoAulaRequest>()
        };

        // Act
        var resposta = await _client.PostAsJsonAsync($"/v1/conteudo/curso/{_cursoTeste!.Id}/aula", comando);

        // Assert
        resposta.StatusCode.Should().Be(HttpStatusCode.OK);
        var resultado = await resposta.Content.ReadFromJsonAsync<Peo.ContentManagement.Application.UseCases.Aula.Cadastrar.Response>();
        resultado.Should().NotBeNull();
        resultado!.AulaId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task CadastrarAula_ComRequisicaoInvalida_DeveRetornarErroValidacao()
    {
        // Arrange
        var comando = new AulaCadastrar.Command
        {
            Titulo = "", // Título inválido
            Descricao = "Descrição da Aula Teste",
            UrlVideo = "https://example.com/video",
            Duracao = TimeSpan.FromMinutes(30),
            Arquivos = new List<Peo.ContentManagement.Application.Dtos.ArquivoAulaRequest>()
        };

        // Act
        var resposta = await _client.PostAsJsonAsync($"/v1/conteudo/curso/{_cursoTeste!.Id}/aula", comando);

        // Assert
        resposta.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var resultado = await resposta.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        resultado.Should().NotBeNull();
        resultado.Errors.Any(e => e.Value.Any(x => x.Contains("The Titulo field is required."))).Should().BeTrue();
    }

    [Fact]
    public async Task ObterTodasAulas_DeveRetornarAulas()
    {
        // Act
        var resposta = await _client.GetAsync($"/v1/conteudo/curso/{_cursoTeste!.Id}/aula");

        // Assert
        resposta.StatusCode.Should().Be(HttpStatusCode.OK);
        var resultado = await resposta.Content.ReadFromJsonAsync<AulaObterTodos.Response>();
        resultado.Should().NotBeNull();
        resultado!.Aulas.Should().NotBeEmpty();
        resultado.Aulas.Should().AllSatisfy(a => a.Id.Should().NotBe(Guid.Empty));
    }
}