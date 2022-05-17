namespace UserService.GraphQL
{
    public record UserInput
    (
        int? id,
        string FullName,
        string Email,
        string UserName,
        string Password
    );
}
