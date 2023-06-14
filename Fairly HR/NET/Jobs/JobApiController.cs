using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Sabio.Services;
using Sabio.Web.Models.Responses;
using System.Collections.Generic;
using System;
using Sabio.Web.Controllers;
using Sabio.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Sabio.Models;
using Microsoft.Build.Utilities;
using Sabio.Models.Requests.Jobs;
using Stripe;
using Sabio.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using Sabio.Models.Domain.Jobs;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/jobs")]
    [ApiController]
    public class JobApiController : BaseApiController
    {
        private IJobService _jobService = null;
        private IAuthenticationService<int> _WebAuthenticationService = null;

        public JobApiController(IJobService jobService,
            ILogger<JobApiController> logger,
            IAuthenticationService<int> WebAuthenticationService) : base(logger)
        {
            _jobService = jobService;
            _WebAuthenticationService = WebAuthenticationService;
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public ActionResult<ItemResponse<Job>> GetById(int id)
        {
            int iCode = 200;
            BaseResponse response = null;

            try
            {

                Job job = _jobService.GetById(id);


                if (job == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("Job not found.");
                    return NotFound404(response);
                }

                else
                {
                    response = new ItemResponse<Job> { Item = job };
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

        [HttpPost]
        public ActionResult<ItemResponse<int>> Create(JobAddRequest model)
        {
            ObjectResult result = null;

            try
            {
                int userId = _WebAuthenticationService.GetCurrentUserId();
                int id = _jobService.Add(model, userId);
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

        [HttpGet("paginate")]
        public ActionResult<ItemResponse<Paged<Job>>> GetOrgIdPaginated(int pageIndex, int pageSize, int organizationId)
        {
            int code = 200;
            BaseResponse response = null;//do not declare an instance.

            try
            {
                Paged<Job> page = _jobService.GetOrgIdPaginated(pageIndex, pageSize, organizationId);

                if (page == null)
                {
                    code = 404;
                    response = new ErrorResponse("Page not found.");
                }
                else
                {
                    response = new ItemResponse<Paged<Job>> { Item = page };
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

        [HttpPut("{id:int}")]
        public ActionResult<ItemResponse<int>> Update(JobUpdateRequest model)
        {
            int code = 200;
            BaseResponse response = null; //DO NOT declare new instance.

            try
            {
                int userId = _WebAuthenticationService.GetCurrentUserId();
                _jobService.Update(model, userId);

                response = new SuccessResponse();
            }
            catch (Exception ex)
            {

                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }

        [HttpPut("status/{id:int}")]
        public ActionResult<ItemResponse<int>> UpdateJobStatus(JobUpdateStatusRequest model)
        {
            int code = 200;
            BaseResponse response = null; //DO NOT declare new instance.

            try
            {

                _jobService.UpdateJobStatus(model);

                response = new SuccessResponse();
            }
            catch (Exception ex)
            {

                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }

        [HttpPost("locations")]
        public ActionResult<SuccessResponse> JobLocCreate(JobLocAddRequest model)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                int userId = _WebAuthenticationService.GetCurrentUserId();

                _jobService.AddJobLoc(model, userId);

                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }

        [HttpGet("{jobId:int}/locations")]
        public ActionResult JobLocGetById(int jobId)
        {
            int iCode = 200;
            BaseResponse response = null;
            try
            {
                JobLocations jobLoc = _jobService.GetJobLoc(jobId);

                if (jobLoc == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("Application Resource not found.");
                }
                else
                {
                    response = new ItemResponse<JobLocations> { Item = jobLoc };
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

        [HttpDelete("{jobId:int}/locations/{locationId:int}")]
        public ActionResult<SuccessResponse> JobLocDeleteById(int jobId, int locationId)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                _jobService.DeleteJobLoc(jobId, locationId);

                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }

        [HttpGet("locations/paginate")]
        public ActionResult<ItemResponse<Paged<JobLocations>>> JobLocGetPage(int pageIndex, int pageSize, int locationId)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                Paged<Job> page = _jobService.GetJobLocPage(pageIndex, pageSize, locationId);

                if (page == null)
                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found.");
                }
                else
                {
                    response = new ItemResponse<Paged<Job>> { Item = page };
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

        [HttpGet("")]
        [AllowAnonymous]
        public ActionResult<ItemResponse<Paged<Job>>> GetAllPaginated(int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Paged<Job> page = _jobService.GetAllPaginated(pageIndex, pageSize);

                if (page == null)
                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found.");
                }
                else
                {
                    response = new ItemResponse<Paged<Job>> { Item = page };
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

        [HttpGet("search")]
        [AllowAnonymous]
        public ActionResult<ItemResponse<Paged<Job>>> SearchPaginated(int pageIndex, int pageSize, string query)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Paged<Job> page = _jobService.SearchPaginated(pageIndex, pageSize, query);

                if (page == null)
                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found.");
                }
                else
                {
                    response = new ItemResponse<Paged<Job>> { Item = page };
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

        [HttpGet("geolocation")]
        [AllowAnonymous]
        public ActionResult<ItemResponse<Paged<Job>>> GetByLocation(int pageIndex, int pageSize, double latitude, double longitude, int radius)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Paged<Job> page = _jobService.GetByLocation(pageIndex, pageSize, latitude, longitude, radius);

                if (page == null)
                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found.");
                }
                else
                {
                    response = new ItemResponse<Paged<Job>> { Item = page };
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

