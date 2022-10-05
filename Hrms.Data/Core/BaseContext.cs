using Microsoft.EntityFrameworkCore;

namespace Hrms.Data.Core
{
    public class BaseContext : DbContext
    {
        public BaseContext(DbContextOptions<BaseContext> options)
            : base(options)
        {
        }
    }
}
