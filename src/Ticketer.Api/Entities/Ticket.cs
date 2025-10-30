// ===================================THIS FILE WAS AUTO GENERATED===================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ticketer.Api.Entities;

/// <summary>
/// Work item tracked in the system.
/// </summary>
[Table("Ticket")]
public class Ticket
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TicketID { get; set; }

    /// <summary>
    /// Ticket title
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string? Title { get; set; } 

    /// <summary>
    /// Detailed description
    /// </summary>
    [Required]
    [MaxLength(1000)]
    public string? Description { get; set; } 

    /// <summary>
    /// Workflow status
    /// </summary>
    [Required]
    public string? Status { get; set; } = "Pending";

    /// <summary>
    /// Creator full name
    /// </summary>
    [Required]
    public string? CreatedBy { get; set; } 

    /// <summary>
    /// Creation date
    /// </summary>
    public DateTime? CreatedAt { get; set; } 

    /// <summary>
    /// Due date
    /// </summary>
    [Required]
    public DateTime? DueDate { get; set; } 

    /// <summary>
    /// Completion date
    /// </summary>
    public DateTime? CompletedAt { get; set; } 

    /// <summary>
    /// Assigned user ID
    /// </summary>
    [Required]
    public int? UserID { get; set; } 

}
