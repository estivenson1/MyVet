
using MyVet.Web.Data.Entities;
using MyVet.Web.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyVet.Web.Data
{
    public class SeedDb
    {
        private readonly DataContext _dataContext;
        private readonly IUserHelper _userHelper;

        public SeedDb(
            DataContext context,
            IUserHelper userHelper)
        {
            _dataContext = context;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await _dataContext.Database.EnsureCreatedAsync();
            await CheckRoles();
            //var manager = await CheckUserAsync("1010", "Juan", "Zuluaga", "jzuluaga55@gmail.com", "350 634 2747", "Calle Luna Calle Sol", "Admin");
            var manager2 = await CheckUserAsync("1011", "Estivenson", "Ortega Villar", "estivenson@vet.com", "3017915843", "Manga 23#75, Cartagena", "Admin");
            //var manager3 = await CheckUserAsync("1012", "Guillermo", "Coronel", "guillermo@vet.com", "350 634 2747", "Calle Luna Calle Sol 2", "Admin");
            //var manager4 = await CheckUserAsync("1013", "Alba", "Ponto", "alba@vet.com", "350 634 2747", "Calle Luna Calle Sol 3", "Admin");

            //var customer2 = await CheckUserAsync("2020", "Juan", "Zuluaga", "jzuluaga55@hotmail.com", "350 634 2747", "Calle Luna Calle Sol", "Customer");
            var customer = await CheckUserAsync("2021", "Ever", "Valdes", "ever@vet.com", "300 634 2747", "Centro, Cartagena", "Customer");
            var customer3 = await CheckUserAsync("2022", "Fabian", "Blanco", "fabian@vet.com", "315 634 2747", "Crespo, Cartagena", "Customer");
            var customer4 = await CheckUserAsync("2023", "Jose", "Coronel2", "jose@vet.com", "310 634 2747", "Turbaco, Bolivar", "Customer");

            await CheckPetTypesAsync();
            await CheckServiceTypesAsync();

            await CheckOwnerAsync(customer);
            //await CheckOwnerAsync(customer2);
            await CheckOwnerAsync(customer3);
            await CheckOwnerAsync(customer4);

            //await CheckManagerAsync(manager);
            await CheckManagerAsync(manager2);
            //await CheckManagerAsync(manager3);
            //await CheckManagerAsync(manager4);

            await CheckPetsAsync();
            await CheckAgendasAsync();
        }

        private async Task CheckRoles()
        {
            await _userHelper.CheckRoleAsync("Admin");
            await _userHelper.CheckRoleAsync("Customer");
        }

        private async Task<User> CheckUserAsync(
            string document,
            string firstName,
            string lastName,
            string email,
            string phone,
            string address,
            string role)
        {
            var user = await _userHelper.GetUserByEmailAsync(email);
            if (user == null)
            {
                user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phone,
                    Address = address,
                    Document = document
                };

                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, role);
            }

            return user;
        }

        private async Task CheckPetsAsync()
        {
            if (!_dataContext.Pets.Any())
            {
                var owner = _dataContext.Owners.FirstOrDefault();
                var petType = _dataContext.PetTypes.FirstOrDefault();
                AddPet("Otto", owner, petType, "Shih tzu");
                AddPet("Killer", owner, petType, "Dobermann");
                await _dataContext.SaveChangesAsync();
            }
        }

        private async Task CheckServiceTypesAsync()
        {
            if (!_dataContext.ServiceTypes.Any())
            {
                _dataContext.ServiceTypes.Add(new ServiceType { Name = "Consulta" });
                _dataContext.ServiceTypes.Add(new ServiceType { Name = "Urgencia" });
                _dataContext.ServiceTypes.Add(new ServiceType { Name = "Vacunación" });
                await _dataContext.SaveChangesAsync();
            }
        }

        private async Task CheckPetTypesAsync()
        {
            if (!_dataContext.PetTypes.Any())
            {
                _dataContext.PetTypes.Add(new PetType { Name = "Perro" });
                _dataContext.PetTypes.Add(new PetType { Name = "Gato" });
                await _dataContext.SaveChangesAsync();
            }
        }

        private async Task CheckOwnerAsync(User user)
        {
            if (!_dataContext.Owners.Any())
            {
                _dataContext.Owners.Add(new Owner { User = user });
                await _dataContext.SaveChangesAsync();
            }
        }

        private async Task CheckManagerAsync(User user)
        {
            if (!_dataContext.Managers.Any())
            {
                _dataContext.Managers.Add(new Manager { User = user });
                await _dataContext.SaveChangesAsync();
            }
        }

        private void AddPet(string name, Owner owner, PetType petType, string race)
        {
            _dataContext.Pets.Add(new Pet
            {
                Born = DateTime.Now.AddYears(-2),
                Name = name,
                Owner = owner,
                PetType = petType,
                Race = race
            });
        }

        private async Task CheckAgendasAsync()
        {
            if (!_dataContext.Agendas.Any())
            {
                var initialDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 0, 0);
                var finalDate = initialDate.AddYears(1);
                while (initialDate < finalDate)
                {
                    if (initialDate.DayOfWeek != DayOfWeek.Sunday)
                    {
                        var finalDate2 = initialDate.AddHours(10);
                        while (initialDate < finalDate2)
                        {
                            _dataContext.Agendas.Add(new Agenda
                            {
                                Date = initialDate,
                                IsAvailable = true
                            });

                            initialDate = initialDate.AddMinutes(30);
                        }

                        initialDate = initialDate.AddHours(14);
                    }
                    else
                    {
                        initialDate = initialDate.AddDays(1);
                    }
                }
            }

            await _dataContext.SaveChangesAsync();
        }
    }
}