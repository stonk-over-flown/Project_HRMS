﻿using AutoMapper;
using HumanResourceApi.DTO.Experience;
using HumanResourceApi.DTO.Job;
using HumanResourceApi.Models;
using HumanResourceApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace HumanResourceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        public readonly IMapper _mapper;
        public readonly JobRepo _job;
        public Regex jobIdRegex = new Regex(@"^JB\d{6}");

        public JobController(IMapper mapper, JobRepo job)
        {
            _mapper = mapper;
            _job = job;
        }


        [Authorize]
        [HttpGet("jobs")]
        public IActionResult GetJobs()
        {
            try
            {
                //var jobList = _mapper.Map<List<JobDto>>(_job.GetAll());

                var jobList = _mapper.Map<List<JobDto>>(_job.GetAll());

                return Ok(jobList);
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong: " + ex.Message);
            }
        }

        [Authorize]
        [HttpGet("get/job/{jobId}")]
        public IActionResult GetJob(string jobId)
        {
            try
            {
                if (!jobIdRegex.IsMatch(jobId))
                {
                    return BadRequest("Wrong jobId Format.");
                }
                var job = _mapper.Map<JobDto>(_job.GetAll().Where(j => j.JobId == jobId).FirstOrDefault());
                if (job == null)
                {
                    return BadRequest("Job ID = " + jobId + " doesn't seem to be found.");
                }
                return Ok(job);
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong: " + ex.Message);
            }
        }

        [Authorize]
        [HttpPost("create")]
        public IActionResult Create([FromBody] JobDto job)
        {
            try
            {
                if (job == null)
                {
                    return BadRequest("Some input information is null");
                }
                if (!jobIdRegex.IsMatch(job.JobId))
                {
                    return BadRequest("Wrong jobId Format.");
                }
                bool validJob = _job.GetAll().Any(j => j.JobId == job.JobId);
                if (validJob)
                {
                    return BadRequest("Job ID = " + job.JobId + " existed");
                }
                var temp = _mapper.Map<Job>(job);
                _job.Add(temp);
                return Ok(_mapper.Map<JobDto>(temp));
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong: " + ex.Message);
            }
        }

        [Authorize]
        [HttpPut("update/job/{jobId}")]
        public IActionResult UpdateJob(string jobId, [FromBody] UpdateJobDto job)
        {
            try
            {
                if (job == null)
                {
                    return BadRequest("Some input information is null");
                }
                if (!jobIdRegex.IsMatch(jobId))
                {
                    return BadRequest("Wrong jobId Format.");
                }
                var validJob = _job.GetAll().Where(j => j.JobId == jobId).FirstOrDefault();
                if (validJob == null)
                {
                    return BadRequest();
                }
                _mapper.Map(job, validJob);
                validJob.JobId = jobId;

                _job.Update(validJob);
                return Ok(_mapper.Map<JobDto>(validJob));
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong: " + ex.Message);
            }
        }

        //[Authorize]
        //[HttpPost("delete")]
        //public IActionResult DeleteJob([FromQuery] string id)
        //{
        //    var job = _mapper.Map<JobDto>(_job.GetAll().Where(j => j.JobId == id).FirstOrDefault());
        //    if (job == null)
        //    {
        //        return BadRequest();
        //    }
        //    var validJob = _job.GetAll().Where(j => j.JobId == id).FirstOrDefault();
        //    _mapper.Map(job, validJob);
        //    validJob.JobId = id;
        //    validJob.Status = "Disable";

        //    _job.Update(validJob);
        //    return Ok(validJob);
        //}
    }
}

