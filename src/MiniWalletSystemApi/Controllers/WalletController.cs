using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniWalletSystemApi.Interfaces.Services;
using MiniWalletSystemApi.Payloads.Responses;

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
        
        [HttpGet("test-error")]
        public IActionResult TestError()
        {
            throw new InvalidOperationException("This is a test error to verify Serilog error logging.");
        }
        
        [HttpGet("test-ci")]
        public IActionResult TestCI()
        {
            return Ok("Hello, successful test ci!");
        }
        
        
    }
}
