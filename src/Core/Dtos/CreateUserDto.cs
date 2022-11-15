namespace Goals.Core.Dtos;

public class CreateUserDto
{
    public string Name { get; init; } = "";
    public string Email { get; init; } = "";
    public string Password { get; init; } = "";

    public override string ToString() => $"Name: {Name}, Email: {Email}, Password: {Password}";
}
