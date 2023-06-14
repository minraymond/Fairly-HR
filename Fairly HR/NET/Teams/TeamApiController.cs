using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sabio.Services.Interfaces;
using Sabio.Services;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using System;
using Sabio.Models.Domain.Blogs;
using System.Collections.Generic;
using Sabio.Models.Domain.Teams;
using Sabio.Models.Domain.JobSkills;
using Sabio.Models;
using Sabio.Models.Requests;
using Sabio.Models.Requests.Teams;
using Sabio.Models.Domain;
using SendGrid;
using static Google.Apis.Requests.BatchRequest;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/teams")]
    [ApiController]
    public class TeamApiController : BaseApiController
    {
        private ITeamService _service = null;
        private IAuthenticationService<int> _authService = null;
        public TeamApiController(ITeamService service, IAuthenticationService<int> authService, ILogger<TeamApiController> logger) : base(logger)
        {
            _service = service;
            _authService = authService;
        }

        [HttpDelete("members/del/{userId:int}/{teamId:int}")]
        public ActionResult<SuccessResponse> DelTeamMembers(int userId, int teamId)
        {
            int code = 201;
            BaseResponse response = null;

            try
            {
                int createdBy = _authService.GetCurrentUserId();

                _service.DelTeamMembers(userId, teamId, createdBy);
                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                code = 500;
                response = new ErrorResponse(ex.Message);

            }

            return StatusCode(code, response);

        }

        [HttpPost("members/new")]
        public ActionResult<SuccessResponse> AddTeamMembers(TeamMembersAddRequest model)
        {
            int code = 201;
            BaseResponse response = null;

            try
            {
                int createdBy = _authService.GetCurrentUserId();

                _service.AddTeamMembers(model, createdBy);
                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                code = 500;
                response = new ErrorResponse(ex.Message);

            }

            return StatusCode(code, response);

        }

        [HttpPost] 
        public ActionResult<ItemResponse<int>> Add(TeamAddRequest model)
        {
            ObjectResult result = null;

            try
            {
                int createdBy = _authService.GetCurrentUserId();

                int id = _service.Add(model, createdBy);
                ItemResponse<int> response = new ItemResponse<int>() { Item = id };
                

                result = Created201(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);
                result = StatusCode(500, response);

            }

            return result;
        }

        [HttpGet("{id:int}")] 
        public ActionResult<ItemResponse<Team>> Get(int id)
        {
            int iCode = 200;
            BaseResponse response = null;

            try
            {
                Team team = _service.Get(id);

                if (team == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("Application Resource not found!");
                }
                else
                {
                    response = new ItemResponse<Team> { Item = team };
                }
            }
            catch (Exception ex)
            {
                iCode = 500;
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse($"Generic Error: {ex.Message}");
            }

            return StatusCode(iCode, response);
        }

        [HttpPut("{id:int}")] 
        public ActionResult<SuccessResponse> Update(TeamUpdateRequest model)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                _service.Update(model);

                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);

        }

        [HttpDelete("{id:int}")] 
        public ActionResult<SuccessResponse> Delete(int id)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {

                _service.Delete(id);

                response = new SuccessResponse();

            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());

            }

            return StatusCode(code, response);
        }

        [HttpGet("paginate")] 
        public ActionResult<ItemResponse<Paged<Team>>> GetOrgId(int orgId, int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Paged<Team> aTeam = _service.GetOrgId(orgId, pageIndex, pageSize);

                if (aTeam == null)
                {
                    code = 404;
                    response = new ErrorResponse("Team not found.");
                }
                else
                {
                    response = new ItemResponse<Paged<Team>> { Item = aTeam };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }

        [HttpGet("org/{orgId:int}")] 
        public ActionResult<ItemsResponse<Team>> GetAll(int orgId)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                List<LookUp> list = _service.GetTeamNamesByOrganizationId(orgId);

                if (list == null)
                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found!");
                }
                else
                {
                    response = new ItemsResponse<LookUp> { Items = list };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }

            return StatusCode(code, response);
        }
    }
}
      