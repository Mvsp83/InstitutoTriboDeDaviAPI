using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Application.Services
{
    public class VideoGaleriaService : IVideoGaleriaService
    {
        private readonly IMapper _mapper;
        private readonly IVideoGaleriaRepository _repository;

        // Extrai o id do vídeo das formas comuns de URL do YouTube.
        private static readonly Regex RegexYoutube = new(
            @"(?:youtu\.be/|youtube\.com/(?:watch\?v=|embed/|shorts/|live/|v/))([A-Za-z0-9_-]{11})",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public VideoGaleriaService(IMapper mapper, IVideoGaleriaRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<List<VideoGaleriaDTO>> Listar()
        {
            var videos = await _repository.ListarAsync();
            return _mapper.Map<List<VideoGaleriaDTO>>(videos);
        }

        public async Task<VideoGaleriaDTO> Salvar(VideoGaleriaDTO dto)
        {
            var video = _mapper.Map<VideoGaleria>(dto);
            video.Titulo = (dto.Titulo ?? string.Empty).Trim();
            video.Descricao = (dto.Descricao ?? string.Empty).Trim();
            video.Url = (dto.Url ?? string.Empty).Trim();
            video.YoutubeId = ExtrairYoutubeId(dto.YoutubeId, video.Url);

            video.Validate();

            if (video.Id == 0)
                video.CriadoEm = DateTime.Now;

            var salvo = await _repository.SalvarAsync(video);
            if (salvo == null)
                throw new DomainException("Não existe um vídeo com o ID informado!");

            return _mapper.Map<VideoGaleriaDTO>(salvo);
        }

        public Task Excluir(long id) => _repository.ExcluirAsync(id);

        // Usa o id já informado (11 chars) ou extrai da URL.
        private static string ExtrairYoutubeId(string idInformado, string url)
        {
            if (!string.IsNullOrWhiteSpace(idInformado) &&
                Regex.IsMatch(idInformado.Trim(), "^[A-Za-z0-9_-]{11}$"))
                return idInformado.Trim();

            if (!string.IsNullOrWhiteSpace(url))
            {
                var m = RegexYoutube.Match(url);
                if (m.Success) return m.Groups[1].Value;
            }

            return string.Empty; // Validate() acusa o erro amigável.
        }
    }
}
