using MVCTaskManager.Common;
using MVCTaskManager.Models;
using MVCTaskManager.Repositories.Interfaces;
using MVCTaskManager.Services.Interfaces;
using System.Data;

namespace MVCTaskManager.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository projectRepository;
        public ProjectService(IProjectRepository _projectRepository)
        {
            projectRepository = _projectRepository;
        }

        public Task<int> DeleteProject(int projectId)
        {
            return projectRepository.DeleteProject(projectId);
        }

        public async Task<List<Project>> GetAllProjects()
        {
            DataTable projectsDataTable = await projectRepository.GetAllProjects();
            // Convert DataTable to List<User>
            List<Project> projects = await CommonHelper.ConvertDataTableToList<Project>(projectsDataTable);
            return projects;
        }

        public async Task AddProject(Project project)
        {
            await projectRepository.AddProject(project);
        }

        public async Task<List<Project>> SearchProject(ProjectSearchRequest projectSearchRequest)
        {
            DataTable projectsSearchDataTable =  await projectRepository.SearchProject(projectSearchRequest);
            // Convert DataTable to List<User>
            List<Project> searchResulst = await CommonHelper.ConvertDataTableToList<Project>(projectsSearchDataTable);
            return searchResulst;
        }

        public Task<int> UpdateProject(Project project)
        {
            return projectRepository.UpdateProject(project);
        }
    }
}
