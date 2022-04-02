using Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

UserStore s = new UserStore();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();


app.MapPost("/auth", (AuthBody u) =>s.Authorizate(u.Login, u.Password));
app.MapPost("/registercustomer", ((Customer u) => s.RegisterCustomer(u.Login,u.Password, u.Discount, u.Role)));
app.MapPost("/registerperformer", ((Performer u) => s.RegisterPerformer(u.Login,u.Password,u.Salary, u.Role)));
app.MapGet(pattern: "/allusers", handler: () => s.GetUserLogin());

app.UseStaticFiles();
app.Run();