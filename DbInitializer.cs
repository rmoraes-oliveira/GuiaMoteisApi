using Microsoft.EntityFrameworkCore;
using Bogus;
using System.ComponentModel;

public static class DbInitializer
{
    public static void Seed(IServiceProvider serviceProvider)
    {
        var configuration = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json")
                                .Build();

        if (configuration["Environment"] == "Development")
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var faker = new Faker("pt_BR");

                context.Database.Migrate();

                if (context.Motels.Any() == false){
                    var motels = new List<Motel>();

                    for (int i = 1; i <= 100; i++)
                    {
                        motels.Add(new Motel
                        {
                            Name = faker.Company.CatchPhrase(),
                            Address = faker.Address.StreetAddress(),
                            Phone = faker.Phone.PhoneNumber("(##) #####-####")
                        });
                    }

                    context.Motels.AddRange(motels);
                    context.SaveChanges();

                    context.TypeSuites.AddRange(
                        new TypeSuite { Description = "Master", Price = 99.00m },
                        new TypeSuite { Description = "Premium", Price = 199.00m },
                        new TypeSuite { Description = "Premium Plus", Price = 299.00m }
                    );
                    context.SaveChanges();


                    motels = context.Motels.ToList();
                    var typeSuites = context.TypeSuites.ToList();
                    var suites = new List<Suite>();

                    foreach (var motel in motels)
                    {
                        foreach (var typeSuite in typeSuites)
                        {
                            suites.Add(new Suite
                            {
                                Name = $"{typeSuite.Description} - {motel.Name}",
                                Capacity = new Random().Next(2, 6),
                                Price = typeSuite.Price,
                                Motel = motel,
                                TypeSuite = typeSuite
                            });
                        }
                    }

                    context.Suites.AddRange(suites);
                    context.SaveChanges();

                    var customers = new List<Customer>();

                    for (int i = 0; i < 100; i++)
                    {
                        customers.Add(new Customer
                        {
                            Name = faker.Name.FullName(),
                            Phone = faker.Phone.PhoneNumber("(##) #####-####"),
                            Email = faker.Internet.Email()
                        });
                    }

                    context.Customers.AddRange(customers);
                    context.SaveChanges();

                    var random = new Random();
                    customers = context.Customers.ToList();
                    var reservations = new List<Reservation>();

                    foreach (var motel in motels)
                    {
                        var motelSuites = context.Suites.Where(s => s.MotelId == motel.Id).ToList();

                        for (int i = 0; i < 10; i++)
                        {
                            var suite = motelSuites[random.Next(motelSuites.Count)];
                            var customer = customers[random.Next(customers.Count)];
                            var checkIn = DateTime.Now.AddDays(random.Next(1, 30));
                            var nights = random.Next(1, 5);
                            var checkOut = checkIn.AddDays(nights);
                            var total = suite.Price * nights;

                            reservations.Add(new Reservation
                            {
                                CheckIn = checkIn,
                                Checkout = checkOut,
                                Total = total,
                                Customer = customer,
                                Suite = suite
                            });
                        }
                    }

                    context.Reservations.AddRange(reservations);
                    context.SaveChanges();
                }
            }
        }
    }
}
