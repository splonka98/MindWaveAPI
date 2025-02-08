using Microsoft.EntityFrameworkCore;
using MindWaveApi.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindWaveApi.Infrastructure.DbContexts
{
    public class UserEntitieContext : DbContext
    {
        public UserEntitieContext(DbContextOptions<UserEntitieContext> options) : base(options)
        {
        }

        
    }
}