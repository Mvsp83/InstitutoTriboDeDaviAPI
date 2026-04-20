using InstitutoTriboDeDavi.System.Domain.Enums;
using InstitutoTriboDeDavi.System.Factory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoTriboDeDavi.API.Controllers
{
    public class ImportController : BaseController
    {
        private readonly FactoryPlanilhaDB _excelImporter;
        private ILogger<ImportController> logger;

        public ImportController(FactoryPlanilhaDB excelImporter, ILogger<ImportController> logger) : base(logger as ILogger<Controller>)
        {
            _excelImporter = excelImporter;
            this.logger = logger;
        }

        [HttpPost]
        [Authorize]
        [Route("import/upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            return await ExecuteAsync(async () =>
            {
                ValidateUserRole(UserRole.Administrador);

                if (file == null || file.Length == 0)
                {
                    return BadRequest("Nenhum arquivo enviado.");
                }

                var filePath = Path.Combine(Path.GetTempPath(), file.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                _excelImporter.ImportDataAlunos(filePath);

                return Ok("Arquivo processado com sucesso.");
            });          
        }

        [HttpPost]
        [Authorize]
        [Route("import/upload2")]
        public async Task<IActionResult> UploadFile2(IFormFile file)
        {
            return await ExecuteAsync(async () =>
            {
                ValidateUserRole(UserRole.Administrador);

                if (file == null || file.Length == 0)
                {
                    return BadRequest("Nenhum arquivo enviado.");
                }

                var filePath = Path.Combine(Path.GetTempPath(), file.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                _excelImporter.ImportDataPolos(filePath);

                return Ok("Arquivo processado com sucesso.");
            });
        }
    }
}

