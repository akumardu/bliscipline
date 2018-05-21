using Bliscipline.Data.Helpers;
using Bliscipline.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bliscipline.Api.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IUserRepository userRepository;

        public UsersController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(string username, string password)
        {
            var user = await userRepository.GetAsync(username,
                HashHelper.Sha512(password + username));

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

    }
}
