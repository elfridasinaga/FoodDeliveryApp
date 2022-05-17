namespace UserService.GraphQL
{
    public record UserInput
    (
        int? id,
        string UserName,
        string Password
    );
}
