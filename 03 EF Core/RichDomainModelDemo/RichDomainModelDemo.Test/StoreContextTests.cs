using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using RichDomainModelDemo.Application.Infrastructure;

namespace RichDomainModelDemo.Test
{
    public class StoreContextTests : DatabaseTest
    {
        [Fact]
        public void CreateDatabaseTest()
        {
            _db.Database.EnsureCreated();
        }
    }
}
