namespace WebStore.Models.ViewModels;

public class CreateAccountViewModel
{
    public string Name { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }


    public bool IsValidEmail()
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(Email);
            return addr.Address == Email;
        }
        catch
        {
            return false;
        }
    }

    public User CreateUser(string hashedPassword, string salt)
        => new User
        {
            Name = Name,
            UserName = Username,
            Email = Email,
            IsAdmin = false,
            HashPw = hashedPassword,
            Salt = salt
        };
}
