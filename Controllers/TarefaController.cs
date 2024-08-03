using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrilhaApiDesafio.Context;
using TrilhaApiDesafio.Models;

namespace TrilhaApiDesafio.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TarefaController : ControllerBase
    {
        private readonly OrganizadorContext _context;

        public TarefaController(OrganizadorContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public IActionResult ObterPorId(int id)
        {
            // Busca o Id no banco utilizando o EF
            var tarefa = _context.Tarefas.Find(id);

            // Valida o tipo de retorno. Se não encontrar a tarefa, retorna NotFound,
            if (tarefa == null)
                return NotFound();

            // caso contrário retorna OK com a tarefa encontrada
            return Ok(tarefa);
        }

        [HttpGet("ObterTodos")]
        public IActionResult ObterTodos()
        {
            // Busca todas as tarefas no banco utilizando o EF
            var tarefas = _context.Tarefas.ToList();

            return Ok(tarefas);
        }

        [HttpGet("ObterPorTitulo")]
        public IActionResult ObterPorTitulo(string titulo)
        {
            if (string.IsNullOrEmpty(titulo))
                return BadRequest(new { Erro = "O título não pode ser vazio" });

            // Busca  as tarefas no banco utilizando o EF, que contenha o titulo recebido por parâmetro
            var tarefas = _context.Tarefas.Where(x => EF.Functions.Collate(x.Titulo, "SQL_Latin1_General_CP1_CI_AI").Contains(EF.Functions.Collate(titulo, "SQL_Latin1_General_CP1_CI_AI"))).ToList();

            if (!tarefas.Any())
                return NotFound();

            return Ok(tarefas);
        }

        [HttpGet("ObterPorData")]
        public IActionResult ObterPorData(DateTime data)
        {
            if (data == DateTime.MinValue)
                return BadRequest(new { Erro = "A data não pode ser vazia." });

            var tarefas = _context.Tarefas
                .Where(x => x.Data.Date == data.Date).ToList();

            if (!tarefas.Any())
                return NotFound();

            return Ok(tarefas);
        }

        [HttpGet("ObterPorStatus")]
        public IActionResult ObterPorStatus(EnumStatusTarefa status)
        {
            // Busca  as tarefas no banco utilizando o EF, que contenha o status recebido por parâmetro
            var tarefas = _context.Tarefas.Where(x => x.Status == status).ToList();

            return Ok(tarefas);
        }

        [HttpPost]
        public IActionResult Criar(Tarefa tarefa)
        {
            if (tarefa.Data == DateTime.MinValue)
                return BadRequest(new { Erro = "A data da tarefa não pode ser vazia" });

            // Adiciona a tarefa recebida no EF e salva as mudanças
            _context.Tarefas.Add(tarefa);
            _context.SaveChanges();

            return CreatedAtAction(nameof(ObterPorId), new { id = tarefa.Id }, tarefa);
        }

        [HttpPut("{id}")]
        public IActionResult Atualizar(int id, Tarefa tarefa)
        {
            var tarefaBanco = _context.Tarefas.Find(id);

            if (tarefaBanco == null)
                return NotFound();

            if (tarefa.Data == DateTime.MinValue)
                return BadRequest(new { Erro = "A data da tarefa não pode ser vazia" });

            // Atualiza as informações da variável tarefaBanco com a tarefa recebida via parâmetro
            tarefaBanco.Titulo = tarefa.Titulo;
            tarefaBanco.Descricao = tarefa.Descricao;
            tarefaBanco.Data = tarefa.Data;
            tarefaBanco.Status = tarefa.Status;

            // Atualiza a variável tarefaBanco no EF e salva as mudanças
            _context.Tarefas.Update(tarefaBanco);
            _context.SaveChanges();

            return Ok(tarefaBanco);
        }

        [HttpDelete("{id}")]
        public IActionResult Deletar(int id)
        {
            var tarefaBanco = _context.Tarefas.Find(id);

            if (tarefaBanco == null)
                return NotFound();

            // Remove a tarefa encontrada através do EF e salva as mudanças
            _context.Tarefas.Remove(tarefaBanco);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
