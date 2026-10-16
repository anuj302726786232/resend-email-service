using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Resend;

namespace EmailService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IResend _resend;

        public EmailController(IResend resend)
        {
            _resend = resend;
        }

        [HttpPost("send-email")]
        public async Task<IActionResult> SendTest()
        {
            var message = new EmailMessage
            {
                From = "onboarding@resend.dev",
                Subject = "My first .NET email",
                TextBody = "Email sending from my .NET API works!"
            };

            message.To.Add("anujyadav3027@gmail.com");

            var response = await _resend.EmailSendAsync(message);

            return Ok(new
            {
                message = "Email accepted by the provider.",
                emailId = response.Content
            });
        }

        [HttpPost("send-file")]
        [RequestSizeLimit(6 * 1024 * 1024)]
        public async Task<IActionResult> SendFile(IFormFile file, [FromForm] string recipient)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Select a file.");

            if (file.Length > 5 * 1024 * 1024)
                return BadRequest("Maximum file size is 5 MB.");

            /*if (!string.Equals(
                    Path.GetExtension(file.FileName),
                    ".pdf",
                    StringComparison.OrdinalIgnoreCase))
                return BadRequest("Only PDF files are allowed.");*/

            // Read the uploaded file stream.
            await using var stream = file.OpenReadStream();
            using var memory = new MemoryStream();

            await stream.CopyToAsync(memory);

            var message = new EmailMessage
            {
                From = "onboarding@resend.dev",
                Subject = "Your attached document",
                TextBody = "Please find your document attached.",
                Attachments = new List<EmailAttachment>()
            };

            message.To.Add("anujyadav3027@gmail.com");

            message.Attachments.Add(new EmailAttachment
            {
                Filename = Path.GetFileName(file.FileName),
                Content = Convert.ToBase64String(memory.ToArray())
            });

            var response = await _resend.EmailSendAsync(message);

            return Ok(new
            {
                message = "Email submitted.",
                emailId = response.Content
            });
        }

        [HttpGet("test-service")]
        public async Task<IActionResult> CheckApi()
        {
            return Ok("Email Service Running");
        }
    }
}
