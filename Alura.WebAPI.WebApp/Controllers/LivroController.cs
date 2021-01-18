using System.Linq;
using Alura.ListaLeitura.Persistencia;
using Alura.ListaLeitura.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Alura.ListaLeitura.HttpClients;

namespace Alura.ListaLeitura.WebApp.Controllers
{
    [Authorize]
    public class LivroController : Controller
    {
        private readonly IRepository<Livro> _repo;
        private readonly LivroApiClient _api;

        public LivroController(IRepository<Livro> repository, LivroApiClient api)
        {
            _repo = repository;
            _api = api;
        }

        [HttpGet]
        public IActionResult Novo()
        {
            return View(new LivroUpload());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Novo(LivroUpload model)
        {
            if (ModelState.IsValid)
            {
                await _api.PostLivroAsync(model);
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ImagemCapa(int id)
        {

            byte[] img = await _api.GetCapaLivroAsync(id);
            if (img != null)
            {
                return File(img, "image/png");
            }
            return File("~/images/capas/capa-vazia.png", "image/png");
        }

        [HttpGet]
        public async Task<IActionResult> Detalhes(int id)
        {
            var model = await _api.GetLivroAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model.ToUpload());
        }

        [HttpGet]
        public IActionResult DetalhesSemHTML(int id)
        {
            var model = _repo.Find(id);
            if (model == null)
            {
                return NotFound();
            }
            //return View(model.ToModel());
            //XML
            //JSON
            model.ImagemCapa = null;
            return Json(model);
        }

        [HttpGet]
        public Livro LivroJson(int id)
        {
            return _repo.Find(id);
        }

        [HttpGet]
        public ActionResult<Livro> LivroHttp(int id)
        {
            var livro = _repo.Find(id);
            if (livro == null)
            {
                return NotFound();
            }
            return livro;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Detalhes(LivroUpload model)
        {
            if (ModelState.IsValid)
            {
                await _api.PutLivroAsync(model);
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remover(int id)
        {
            var model = await _api.GetLivroAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            await _api.DeleteLivroAsync(id);
            return RedirectToAction("Index", "Home");
        }
    }
}