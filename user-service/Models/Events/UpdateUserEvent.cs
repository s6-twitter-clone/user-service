namespace user_service.Models.Events;

public class UpdateUserEvent
{
    public string Id { get; set; } = "";
    public string DisplayName { get; set; } = "";

    public string Bio { get; set; } = "";
}
