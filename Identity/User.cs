using System.Collections;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using System.Data.Entity;

namespace Identity;

public class SampleContext : DbContext
{
    // Имя будущей базы данных можно указать через
    // вызов конструктора базового класса
    public SampleContext() : base("RepairOfEquipment")
    { }

    // Отражение таблиц базы данных на свойства с типом DbSet
    public DbSet<Customer> Customer { get; set; }
    public DbSet<Performer> Performer { get; set; }

    public List<User> GetUsers()
    {
        var users = Customer.Select(c => c as User).ToList();
        users.AddRange(Performer.Select(c => c as User));
        return users;
    }
}

public static class Hash
{
    public static string getHashSha256(string text)
    {
        byte[] bytes = Encoding.Unicode.GetBytes(text);
        SHA256Managed hashstring = new SHA256Managed();
        byte[] hash = hashstring.ComputeHash(bytes);
        string hashString = string.Empty;
        foreach (byte x in hash)
        {
            hashString += String.Format("{0:x2}", x);
        }
        return hashString;
    }
}
public class User
{
    [Required]
    [MaxLength(150)]
    public string Login { get; set; }
    
    [MaxLength(150)]
    public string Password { get; set; }
    public Role Role { get; set; }
}

public class UserStore
{
    private SampleContext context;
    public UserStore()
    {
        context = new SampleContext();
    }

    public void RegisterCustomer(string login, string password, string discount, Role role) {
        if (this.context.Customer.Any(user => user.Login == login) == false)
            this.context.Customer.Add(new Customer() 
        {
            Login=login,
            Password=Hash.getHashSha256(password),
            Discount=discount,
            Role=role
        });
        context.SaveChanges(); 
    }

    public void RegisterPerformer(string login, string password, string salary, Role role) {
        if (this.context.Performer.Any(user => user.Login == login) == false)
            this.context.Performer.Add(new Performer()
        {
            Login=login,
            Password=Hash.getHashSha256(password),
            Salary=salary,
            Role=role
        });
        context.SaveChanges();
    }

    public List<string> GetUserLogin() {
        var result = new List<string>();
        foreach (User u in context.GetUsers()) {
            result.Add(u.Login);
        }
        return result;
    }
    public User Authorizate(string login,string password) {
        var user = context.GetUsers().Find(user => user.Login == login);
        if (user!=null&&user.Password==Hash.getHashSha256(password)) return user;
        return null;
    }
}
public class AuthBody{
    public string Login { get; set; }
    public string Password { get; set; }
    
}
public class Performer : User {
    public string Salary { get; set; }
}

public class Customer : User {
    public string Discount { get; set; }
}

public enum Role{
    Administrator,Customer,Support
}