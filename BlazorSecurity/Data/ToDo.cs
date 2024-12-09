using System.ComponentModel.DataAnnotations;

namespace BlazorSecurity.Data;

public class ToDo
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; }

    [Required]
    public string ToDoItem { get; set; }
}
