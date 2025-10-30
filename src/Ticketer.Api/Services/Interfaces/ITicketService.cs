// ===================================THIS FILE WAS AUTO GENERATED===================================

using Ticketer.Api.DTOs;
using Ticketer.Api.Models;

namespace Ticketer.Api.Services.Interfaces
{
    public interface ITicketService
    {
        /// <summary>
        /// Retrieves all tickets with optional paging support.
        /// </summary>
        /// <param name="paging"></param>
        /// <returns></returns>
        Task<PagedResult<TicketDto>> GetAll(Paging paging);

        /// <summary>
        /// Retrieves a ticket by its ID.
        /// </summary>
        /// <param name="paging"></param>
        /// <returns></returns>
        Task<TicketDto> GetID(int ID);

        /// <summary>
        /// Creates a new ticket entry.
        /// </summary>
        /// <param name="paging"></param>
        /// <returns></returns>
        Task<TicketDto> CreateTicket(TicketDto ticketDto);

        /// <summary>
        /// Updates an existing ticket entry by its ID.
        /// </summary>
        /// <param name="paging"></param>
        /// <returns></returns>
        Task<TicketDto> UpdateTicket(int id, TicketDto ticketDto);

        /// <summary>
        /// Deletes a ticket entry by its ID.
        /// </summary>
        /// <param name="paging"></param>
        /// <returns></returns>
        Task DeleteTicket(int ID);

        /// <summary>
        /// Updates the status of a ticket by its ID.
        /// </summary>
        /// <remarks>
        /// Allows a user or admin to update the workflow status of a ticket to any value deemed appropriate.
        /// Common status values include "In Progress", "Pending", and "Completed".
        /// If the status is set to "Completed", the <c>CompletedAt</c> property is updated to the current date and time.
        /// For other statuses, <c>CompletedAt</c> is cleared.
        /// <para>
        /// Example request: <c>GET /api/ticket/UpdateTicketStatus/2/Completed</c>
        /// </para>
        /// <para>
        /// Only users with the "User" or "Admin" role are authorized to perform this action.
        /// </para>
        /// </remarks>
        /// <param name="ticketService">The ticket service used to retrieve and update the ticket.</param>
        /// <param name="id">The unique identifier of the ticket to update.</param>
        /// <param name="status">The new status to assign to the ticket. The user can update this value as deemed fit.</param>
        /// <returns>
        /// Returns <see cref="TicketDto"/> with the updated ticket if successful.
        /// Returns <see cref="NotFoundObjectResult"/> if the ticket with the specified ID does not exist.
        /// </returns>
        Task<TicketDto> UpdateTicketStatus(int id, string status);
    }
}
