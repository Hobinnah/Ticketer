// ===================================THIS FILE WAS AUTO GENERATED===================================

using AutoMapper;
using Ticketer.Api.Entities;
using Ticketer.Api.DTOs;

namespace Ticketer.Api.DTOs.DtoProfiles
{
    public class TicketProfile : Profile
    {
        public TicketProfile()
        {
            CreateMap<Ticket, TicketDto>().ReverseMap();
        }
    }
}
