using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniWalletSystemApi.Models.Payloads.Responses;
using MiniWalletSystemApi.Services.Interfaces;

namespace MiniWalletSystemApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;
        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }
        
        [HttpGet("overview")]
        public async Task<IActionResult> GetAllOverview()
        {
            var result = await _walletService.GetAll(); 

            if (!result.Any())

                return Ok(result ?? new List<WalletResponse>());

            return Ok(result);
        }
        
    }
}
