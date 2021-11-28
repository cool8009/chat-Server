using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgammonChat.Handlers;
using ChatService.Models;
using ChatService.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ChatService.Controllers
{
    //allowing anonymous users access to certain controller actions
    [AllowAnonymous]
    public class MessagesController : BaseApiController
    {
        private readonly DataContext _context;

        public MessagesController(DataContext dataContext)
        {
            _context = dataContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<Message>>> GetMessages() =>
            
            await Mediator.Send(new List.Query());

    }
}