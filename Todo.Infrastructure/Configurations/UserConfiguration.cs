using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Todo.Infrastructure.Persistence.Entities;

namespace Todo.Infrastructure.Configurations
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(x => x.Email).IsRequired()
                 .HasMaxLength(256);

            builder.Property(x => x.FullName).IsRequired()
                .HasMaxLength(200);


        }
    }
}
