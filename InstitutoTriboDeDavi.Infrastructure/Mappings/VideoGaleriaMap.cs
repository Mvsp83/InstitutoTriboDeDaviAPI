using InstitutoTriboDeDavi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoTriboDeDavi.Infrastructure.Mappings
{
    public class VideoGaleriaMap : IEntityTypeConfiguration<VideoGaleria>
    {
        public void Configure(EntityTypeBuilder<VideoGaleria> builder)
        {
            builder.ToTable("VIDEO_GALERIA");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn().HasColumnType("BIGINT");

            builder.Property(x => x.Titulo).IsRequired().HasMaxLength(150);
            builder.Property(x => x.YoutubeId).IsRequired().HasMaxLength(20);
            builder.Property(x => x.Url).HasMaxLength(400).IsRequired(false);
            builder.Property(x => x.Descricao).HasMaxLength(500).IsRequired(false);
            builder.Property(x => x.CriadoEm).IsRequired();
        }
    }
}
