using InstitutoTriboDeDavi.System.Factory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace InstitutoTriboDeDavi.API.Controllers
{
    public class ImportController : Controller
    {
        private readonly FactoryPlanilhaDB _excelImporter;

        public ImportController(FactoryPlanilhaDB excelImporter)
        {
            _excelImporter = excelImporter;
        }

        [HttpPost]
        [Route("import/upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Nenhum arquivo enviado.");
            }

            var filePath = Path.Combine(Path.GetTempPath(), file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            try
            {
                _excelImporter.ImportDataAlunos(filePath);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao processar o arquivo: {ex.Message}");
            }

            return Ok("Arquivo processado com sucesso.");
        }

        [HttpPost]
        [Route("import/upload2")]
        public async Task<IActionResult> UploadFile2(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Nenhum arquivo enviado.");
            }

            var filePath = Path.Combine(Path.GetTempPath(), file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            try
            {
                _excelImporter.ImportDataPolos(filePath);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao processar o arquivo: {ex.Message}");
            }

            return Ok("Arquivo processado com sucesso.");
        }
    }

}

