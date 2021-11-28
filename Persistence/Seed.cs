using System.Threading.Tasks;
using ChatService.Persistence;
using ChatService.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq; 
using System.Collections.Generic;

namespace ChatService
{
    public class Seed
    {
        public static async Task SeedData(DataContext context, UserManager<AppUser> userManager)
        {   
            if(!userManager.Users.Any())
            {
                var users = new List<AppUser>
                {
                    new AppUser{FirstName= "Nadav", LastName="Shitrit", UserName="Nadav"},
                    new AppUser{FirstName= "Yoni", LastName="Shmerlich", UserName="Yoni"},
                    new AppUser{FirstName= "Yarin", LastName="Tall", UserName="Yarin"},
                    new AppUser{FirstName= "Ori", LastName="kjsdfhkjsdhfkdsjhf", UserName="Ori"}
                };

                foreach(var user in users)
                {
                    await userManager.CreateAsync(user, "Pass12");
                }
            }
        }      
    }    
}