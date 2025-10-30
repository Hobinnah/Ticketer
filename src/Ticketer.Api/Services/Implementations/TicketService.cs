// ===================================THIS FILE WAS AUTO GENERATED===================================

using AutoMapper;
using Ticketer.Api.DTOs;
using Ticketer.Api.Models;
using Ticketer.Api.Enums;
using Ticketer.Api.Entities;
using Ticketer.Api.Repositories.Interfaces;
using Ticketer.Api.Services.Interfaces;
using Ticketer.Api.Configurations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ticketer.Api.Services.Implementations
{
    public class TicketService : ITicketService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<TicketService> logger;
        private readonly ITicketRepository ticketRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public TicketService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<TicketService> logger, ITicketRepository ticketRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.ticketRepository = ticketRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc />
        public async Task<PagedResult<TicketDto>> GetAll(Paging paging)
        {
            IEnumerable<Ticket> entities;
            try
            {
                entities = cache.Get<IEnumerable<Ticket>>(Cache.TICKETS.ToString()) ?? new List<Ticket>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.ticketRepository.GetAll())?.Where(x => x != null).Cast<Ticket>().ToList() ?? new List<Ticket>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<Ticket>>(Cache.TICKETS.ToString(), entities, DateTime.Now.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Ticket by ID. Timestamp: {Timestamp}", DateTime.Now);
                return new PagedResult<TicketDto> { Data = new List<TicketDto>(), TotalCount = 0 };
            }

            IEnumerable<Ticket> filteredEntities = entities;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities.Where(x => !string.IsNullOrEmpty(x.Title) && x.Title.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }
            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.TicketID).Skip((paging.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<TicketDto>>(pagedEntities);
            return new PagedResult<TicketDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc />
        public async Task<TicketDto> GetID(int ID)
        {
            TicketDto dto;
            Ticket entity;
            try
            {
                IEnumerable<Ticket> entities = cache.Get<IEnumerable<Ticket>>(Cache.TICKETS.ToString()) ?? new List<Ticket>();
                if (entities == null || !entities.Any())
                {
                    entity = await this.ticketRepository.GetByID(ID) ?? new Ticket();
                    return this.mapper.Map<TicketDto>(entity);
                }
                dto = this.mapper.Map<TicketDto>(entities.FirstOrDefault(x => x.TicketID == ID));
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Ticket by ID. Timestamp: {Timestamp}", DateTime.Now);
                return new TicketDto();
            }
            return dto;
        }

        /// <inheritdoc />
        public async Task<TicketDto> CreateTicket(TicketDto ticketDto)
        {
            Ticket ticket = new Ticket();
            IEnumerable<Ticket?> checkEntity;
            try
            {
                checkEntity = await this.ticketRepository.Find(x => x.Title!.ToLower().Trim() == ticketDto.Title!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    ticket = this.mapper.Map<Ticket>(ticketDto);
                    ticket.CreatedAt = DateTime.Now;
                    ticket = await ticketRepository.Create(ticket) ?? new Ticket();
                    await ticketRepository.Save();
                    cache.Remove(Cache.TICKETS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating Ticket. Timestamp: {Timestamp}", DateTime.Now);
                return new TicketDto();
            }
            return this.mapper.Map<TicketDto>(ticket);
        }

        /// <inheritdoc />
        public async Task<TicketDto> UpdateTicket(int id, TicketDto ticketDto)
        {
            try
            {
                Ticket ticket = this.mapper.Map<Ticket>(ticketDto);
                ticket = await ticketRepository.Update(ticket) ?? new Ticket();
                await ticketRepository.Save();
                cache.Remove(Cache.TICKETS.ToString());
                ticketDto = this.mapper.Map<TicketDto>(ticket);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating Ticket. Timestamp: {Timestamp}", DateTime.Now);
                return new TicketDto();
            }
            return ticketDto;
        }

        /// <inheritdoc />
        public async Task DeleteTicket(int ID)
        {
            try
            {
                Ticket ticket = await this.ticketRepository.GetByID(ID) ?? new Ticket();
                await ticketRepository.Delete(ticket);
                await ticketRepository.Save();
                cache.Remove(Cache.TICKETS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting Ticket . Timestamp: {Timestamp}", DateTime.Now);
            }
        }

        /// <inheritdoc />
        public async Task<TicketDto> UpdateTicketStatus(int id, string status)
        {
            var ticket = await ticketRepository.GetByID(id);
            if (ticket == null)
                return null!;

            if (status.Equals("In Progress", StringComparison.OrdinalIgnoreCase) || status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
            {
                ticket.Status = status;
                ticket.CompletedAt = null;
            }
            else if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                ticket.Status = "Pending";
                ticket.CompletedAt = null;
            }
            else if (status.Equals("Completed", StringComparison.OrdinalIgnoreCase))
            {
                ticket.Status = status;
                ticket.CompletedAt = DateTime.Now;
            }

            var ticketDto = await UpdateTicket(id, this.mapper.Map<TicketDto>(ticket));

            return ticketDto;
        }
    }
}
