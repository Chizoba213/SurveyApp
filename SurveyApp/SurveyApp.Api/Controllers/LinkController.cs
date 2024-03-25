﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyApp.Application.Abstraction.IServices;
using SurveyApp.Application.DTOs;

namespace SurveyApp.Api.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [Authorize]
    public class LinkController : ControllerBase
    {
        private readonly ILinkService _emailService;
        private readonly ISurveyService _surveyService; 
        private readonly ILogger<LinkController> _logger;  

        public LinkController(ILinkService emailService, ISurveyService surveyService, ILogger<LinkController> logger)
        {
            _emailService = emailService;
            _surveyService = surveyService;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateSurveyAndSendLinkAsync([FromBody] CreateLinkDTO request)
        {
            try
            {
                var userIdentity = HttpContext.User;
                string email = userIdentity.Claims.FirstOrDefault(c => c.Type == "Name")?.Value?.ToString(); 

                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest("Email not found");
                }

                string surveyLink = await _emailService.GenerateAndSendSurveyLinkOnCreationAsync(request.SurveyId, request.ParticipantEmail);

                return Ok(new { SurveyLink = surveyLink });
            }
            catch (Exception ex)
            {
                // If an error occurs, log the error and return a 500 Internal Server Error response
                _logger.LogError($"An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }
    }
}