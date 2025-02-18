public interface IRoleService
{
    public  Task<List<UserDto>> GetUsersAsync();
    public Task<bool> AssignRoleAsync(int userId, string role);


}