using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    public class VideoGaleriaRepository : IVideoGaleriaRepository
    {
        private readonly TriboDeDaviContext _context;

        public VideoGaleriaRepository(TriboDeDaviContext context)
        {
            _context = context;
        }

        public async Task<List<VideoGaleria>> ListarAsync()
        {
            return await _context.VideosGaleria
                .AsNoTracking()
                .OrderByDescending(v => v.CriadoEm)
                .ToListAsync();
        }

        public async Task<VideoGaleria> SalvarAsync(VideoGaleria video)
        {
            if (video.Id > 0)
            {
                var existente = await _context.VideosGaleria.FirstOrDefaultAsync(v => v.Id == video.Id);
                if (existente == null) return null;

                existente.Titulo = video.Titulo;
                existente.YoutubeId = video.YoutubeId;
                existente.Url = video.Url;
                existente.Descricao = video.Descricao;

                await _context.SaveChangesAsync();
                return existente;
            }

            _context.VideosGaleria.Add(video);
            await _context.SaveChangesAsync();
            return video;
        }

        public async Task ExcluirAsync(long id)
        {
            var video = await _context.VideosGaleria.FirstOrDefaultAsync(v => v.Id == id);
            if (video != null)
            {
                _context.VideosGaleria.Remove(video);
                await _context.SaveChangesAsync();
            }
        }
    }
}
