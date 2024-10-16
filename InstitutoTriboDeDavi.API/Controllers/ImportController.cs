//using InstitutoTriboDeDavi.System.Factory;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using System.IO;
//using System.Threading.Tasks;

//namespace InstitutoTriboDeDavi.API.Controllers
//{
//    public class ImportController : Controller
//    {
//        private readonly FactoryPlanilhaDB _excelImporter;

//        // Injeta a dependência de ExcelImporter no controller
//        public ImportController(FactoryPlanilhaDB excelImporter)
//        {
//            _excelImporter = excelImporter;
//        }

//        // Método para fazer o upload do arquivo e processá-lo
//        [HttpPost]
//        [Route("import/upload")]
//        public async Task<IActionResult> UploadFile(IFormFile file)
//        {
//            if (file == null || file.Length == 0)
//            {
//                return BadRequest("Nenhum arquivo enviado.");
//            }

//            // Define um caminho temporário para salvar o arquivo
//            var filePath = Path.Combine(Path.GetTempPath(), file.FileName);

//            // Salva o arquivo no servidor temporariamente
//            using (var stream = new FileStream(filePath, FileMode.Create))
//            {
//                await file.CopyToAsync(stream);
//            }

//            // Chama o método de importação passando o caminho do arquivo
//            try
//            {
//                _excelImporter.ImportData(filePath);
//            }
//            catch (Exception ex)
//            {
//                // Em caso de erro, você pode logar o erro ou retornar uma mensagem
//                return StatusCode(500, $"Erro ao processar o arquivo: {ex.Message}");
//            }

//            // Retorna sucesso
//            return Ok("Arquivo processado com sucesso.");
//        }
//    }

//}

