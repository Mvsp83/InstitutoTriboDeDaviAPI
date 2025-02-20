using InstitutoTriboDeDavi.System.Factory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoTriboDeDavi.API.Controllers
{
    public class ImportController : BaseController
    {
        private readonly FactoryPlanilhaDB _excelImporter;

        public ImportController(FactoryPlanilhaDB excelImporter)
        {
            _excelImporter = excelImporter;
        }

        [HttpPost]
        [Authorize]
        [Route("import/upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                if (UsuarioAutenticado.Role == System.Domain.Enums.UserRole.Administrador)
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

                throw new UnauthorizedAccessException("Usuário não tem permissão para acessar esta informação.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Authorize]
        [Route("import/upload2")]
        public async Task<IActionResult> UploadFile2(IFormFile file)
        {
            try
            {
                if (UsuarioAutenticado.Role == System.Domain.Enums.UserRole.Administrador)
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

                throw new UnauthorizedAccessException("Usuário não tem permissão para acessar esta informação.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

