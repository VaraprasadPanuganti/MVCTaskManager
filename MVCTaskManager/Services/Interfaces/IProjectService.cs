using MVCTaskManager.Models;
using System.Data;

namespace MVCTaskManager.Services.Interfaces
{
    public interface IProjectService
    {
        public Task<List<Project>> GetAllProjects();
        public Task AddProject(Project project);
        public Task<int> UpdateProject(Project project);
        public Task<int> DeleteProject(int projectId);
        public Task<List<Project>> SearchProject(ProjectSearchRequest projectSearchRequest);
    }
}
