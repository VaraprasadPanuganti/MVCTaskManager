using MVCTaskManager.Models;
using System.Data;

namespace MVCTaskManager.Repositories.Interfaces
{
    public interface IProjectRepository
    {
        public Task<DataTable> GetAllProjects();
        public Task AddProject(Project project);
        public Task<int> UpdateProject(Project project);
        public Task<int> DeleteProject(int projectId);
        public Task<DataTable> SearchProject(ProjectSearchRequest projectSearchRequest);
    }
}
