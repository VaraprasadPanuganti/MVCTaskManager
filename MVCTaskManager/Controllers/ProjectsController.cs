using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MVCTaskManager.Common;
using MVCTaskManager.Models;
using MVCTaskManager.Services.Interfaces;
using System.Data;

namespace MVCTaskManager.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : Controller
    {

        private readonly ILogger<ProjectsController> _logger;
        private readonly IProjectService projectService;
        public ProjectsController(ILogger<ProjectsController> logger, IProjectService _projectService)
        {
            _logger = logger;
            projectService = _projectService;
        }

        [HttpGet("/")]
        [HttpGet("getProjects")]
        public async Task<IActionResult> GetProjects()
        {
            try
            {
                List<Project> projects = await projectService.GetAllProjects();
                return StatusCode(200, new { message = "Suucessfully Completed", data = projects });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "GetProjects Error Message");
                _logger.LogError(ex.StackTrace, "GetProjects Error StackTrace");
                return StatusCode(500, new { message = "Internal server error while inserting project." });
            }

        }

        [HttpPost("insertProject")]
        public async Task<IActionResult> AddProject([FromBody] Project project)
        {
            try
            {
                if (project == null || project.ProjectId <= 0 || string.IsNullOrWhiteSpace(project.ProjectName) || project.TeamSize <= 0 || project.DateOfStart == DateTime.MinValue)
                {
                    return BadRequest("One of the Project parameter is null or empty.");
                }
                await projectService.AddProject(project);
                return StatusCode(201, new { message = "Successfully added." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "AddProject Error Message");
                _logger.LogError(ex.StackTrace, "AddProject Error Stacktrace");
                return StatusCode(500, new { message = "Internal server error while adding project." });
            }
        }

        [HttpPost("updateProject")]
        public async Task<IActionResult> UpdateProject([FromBody] Project project)
        {
            try
            {
                if (project.ProjectId <= 0 || string.IsNullOrWhiteSpace(project.ProjectName) || project.TeamSize <= 0 || project.DateOfStart == DateTime.MinValue)
                {
                    return BadRequest("One of the Project parameter is null or empty.");
                }
                int result = await projectService.UpdateProject(project);
                if (result == 1)
                {
                    return StatusCode(200, new { message = "Project Updated Successfully." });
                }
                else
                {
                    return StatusCode(404, new { message = "Project not found in db" });
                }


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "UpdateProject Error Message");
                _logger.LogError(ex.StackTrace, "UpdateProject Error SatckTrace");
                return StatusCode(500, new { message = "Internal server error while updating project request." });

            }
        }

        [HttpGet("deleteProject/{projectId}")]
        public async Task<IActionResult> deleteProject(int projectId)
        {
            try
            {
                if (projectId <= 0)
                {
                    return BadRequest("Invalid Project ID.");
                }
                int result = await projectService.DeleteProject(projectId);
                if (result == 1)
                {
                    return StatusCode(200, new { message = "project deleted Successfully." });
                }
                else if (result == 0)
                {
                    return StatusCode(404, new { message = "no record found in db." });
                }
                else
                {
                    return StatusCode(500, new { message = "Internal server error while updating project request." });
                }


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "deleteProject Error Message");
                _logger.LogError(ex.StackTrace, "deleteProject Error stackTrace");
                return StatusCode(500, new { message = "Internal server error while updating project request." });
            }
        }

        [HttpPost("searchProjects")]
        public async Task<IActionResult> searchProjects([FromBody] ProjectSearchRequest projectSearchRequest)
        {
            try
            {
                List<Project> searchResults = await projectService.SearchProject(projectSearchRequest);
                return StatusCode(200, new { message = "suucessfully completed", data = searchResults });
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "searchProjects Error Message");
                _logger.LogError(ex.StackTrace, "searchProjects Error StackTrace");
                return StatusCode(500, new { message = "Internal server error while searching project request.", data = new List<Project>() });
            }
        }

    }
}
