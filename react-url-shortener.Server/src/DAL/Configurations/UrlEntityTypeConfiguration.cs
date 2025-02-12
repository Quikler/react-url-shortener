using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Configurations;

public class UrlEntityTypeConfiguration : IEntityTypeConfiguration<UrlEntity>
{
    public void Configure(EntityTypeBuilder<UrlEntity> builder)
    {
        
    }
}