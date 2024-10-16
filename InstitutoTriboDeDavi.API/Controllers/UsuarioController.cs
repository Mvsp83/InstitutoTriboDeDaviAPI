using AutoMapper;
using InstitutoTriboDeDavi.API.Utilities;
using InstitutoTriboDeDavi.API.ViewModels.Result;
using InstitutoTriboDeDavi.API.ViewModels.Usuario;
using InstitutoTriboDeDavi.System.Core.Exceptions;
using InstitutoTriboDeDavi.System.DTO;
using InstitutoTriboDeDavi.System.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InstitutoTriboDeDavi.API.Controllers
{
    [ApiController]    
    public class UsuarioController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IMapper mapper, IUsuarioService usuarioService)
        {
            _mapper = mapper;
            _usuarioService = usuarioService;
        }

        [HttpPost]
        [Route("/usuario/create")]
        public async Task<IActionResult> Create([FromBody] UsuarioViewModel usuarioViewModel)
        {
            try
            {
                var usuarioDTO = _mapper.Map<UsuarioDTO>(usuarioViewModel);
                var usuarioCreated = await _usuarioService.Create(usuarioDTO);

                return Ok(new ResultViewModel {
                    Message = "Usuário criado com sucesso!",
                    Success = true,
                    Data = usuarioCreated
                });
            }
            catch (DomainException ex)
            {
                return BadRequest(Responses.DomainErrorMessage(ex.Message, ex.Errors));
            }
            catch (Exception)
            {
                return StatusCode(500, Responses.ApplicationErrorMessage());
            }
        }

        [HttpPut]
        [Route("/usuario/update")]
        public async Task<IActionResult> Update([FromBody] UsuarioDTO usuarioDTO)
        {
            try
            {
                var usuarioUpdated = await _usuarioService.Update(usuarioDTO);

                return Ok(new ResultViewModel
                {
                    Message = "Usuário atualizado com sucesso!",
                    Success = true,
                    Data = usuarioUpdated
                });
            }
            catch (DomainException ex)
            {
                return BadRequest(Responses.DomainErrorMessage(ex.Message, ex.Errors));
            }
            catch (Exception)
            {
                return StatusCode(500, Responses.ApplicationErrorMessage());
            }
        }

        [HttpDelete]
        [Route("/usuario/delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var usuario = await _usuarioService.Get(id);

                if (usuario == null)
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Usuário foi encontrado com o ID informado!",
                        Success = true,
                        Data = usuario
                    });
                }

                await _usuarioService.Delete(id);

                return Ok(new ResultViewModel
                {
                    Message = "Usuário removido com sucesso!",
                    Success = true,
                    Data = null
                });
            }
            catch (DomainException ex)
            {
                return BadRequest(Responses.DomainErrorMessage(ex.Message, ex.Errors));
            }
            catch (Exception)
            {
                return StatusCode(500, Responses.ApplicationErrorMessage());
            }
        }

        [HttpGet]
        [Route("/usuario/get/{id}")]
        public async Task<IActionResult> Get(long id)
        {
            try
            {
                var usuario = await _usuarioService.Get(id);

                if (usuario == null)
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Usuário foi encontrado com o ID informado!",
                        Success = true,
                        Data = usuario
                    });
                }

                return Ok(new ResultViewModel
                {
                    Message = "Usuário encontrado com sucesso!",
                    Success = true,
                    Data = usuario
                });
            }
            catch (DomainException ex)
            {
                return BadRequest(Responses.DomainErrorMessage(ex.Message, ex.Errors));
            }
            catch (Exception)
            {
                return StatusCode(500, Responses.ApplicationErrorMessage());
            }
        }

        [HttpGet]
        [Route("/usuario/get-all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var allUsuarios = await _usuarioService.GetAll();

                return Ok(new ResultViewModel
                {
                    Message = "Usuários encontrados com sucesso!",
                    Success = true,
                    Data = allUsuarios
                });
            }
            catch (DomainException ex)
            {
                return BadRequest(Responses.DomainErrorMessage(ex.Message, ex.Errors));
            }
            catch (Exception)
            {
                return StatusCode(500, Responses.ApplicationErrorMessage());
            }
        }

        [HttpGet]
        [Route("/usuario/get-by-email")]
        public async Task<IActionResult> GetByEmail([FromQuery] string email)
        {
            try
            {
                var usuario = await _usuarioService.GetByEmail(email);

                if (usuario == null)
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Usuário foi encontrado com o Email informado!",
                        Success = true,
                        Data = usuario
                    });
                }

                return Ok(new ResultViewModel
                {
                    Message = "Usuário encontrado com sucesso!",
                    Success = true,
                    Data = usuario
                });
            }
            catch (DomainException ex)
            {
                return BadRequest(Responses.DomainErrorMessage(ex.Message, ex.Errors));
            }
            catch (Exception)
            {
                return StatusCode(500, Responses.ApplicationErrorMessage());
            }
        }

        [HttpGet]
        [Route("/usuario/search-by-email")]
        public async Task<IActionResult> SearchByEmail([FromQuery] string email)
        {
            try
            {
                var allUsuarios = await _usuarioService.SearchByEmail(email);

                if (allUsuarios.Count() == 0)
                {
                    return Ok(new ResultViewModel
                    {
                        Message = "Nenhum Usuário foi encontrado com o Email informado!",
                        Success = true,
                        Data = null
                    });
                }

                return Ok(new ResultViewModel
                {
                    Message = "Usuários encontrados com sucesso!",
                    Success = true,
                    Data = allUsuarios
                });
            }
            catch (DomainException ex)
            {
                return BadRequest(Responses.DomainErrorMessage(ex.Message, ex.Errors));
            }
            catch (Exception)
            {
                return StatusCode(500, Responses.ApplicationErrorMessage());
            }
        }
    }
}
