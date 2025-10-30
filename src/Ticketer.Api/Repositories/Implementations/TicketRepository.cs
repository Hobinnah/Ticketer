// ===================================THIS FILE WAS AUTO GENERATED===================================

using Ticketer.Api.Entities;
using Ticketer.Api.Repositories.Interfaces;

namespace Ticketer.Api.Repositories.Implementations
{
    public class TicketRepository : Repository<Ticket>, ITicketRepository, IDisposable
    {
        private readonly TicketerDbContext context;

        public TicketRepository(TicketerDbContext context) : base(context)
        {
            this.context = context;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && this.context != null)
            {
                  this.context.Dispose();
            }
        }
    }
}
