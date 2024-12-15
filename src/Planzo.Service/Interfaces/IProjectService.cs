using Planzo.Data.Dtos;
using Planzo.Data.Dtos.Project;

namespace Planzo.Service.Interfaces;

public interface IProjectService
{
    Task<ResponseDto<ProjectDto>> CreateProjectAsync(CreateProjectDto createProjectDto, string userId);
    Task<ResponseDto<ProjectDto>> UpdateProjectAsync(int projectId, CreateProjectDto createProjectDto, string userId);
    Task<ResponseDto<ProjectDto>> DeleteProjectAsync(int projectId, string userId);
    Task<ResponseDto<ProjectDto>> GetProjectByIdAsync(int projectId, string userId);
    Task<ResponseDto<List<ProjectDto>>> GetAllProjectsAsync(string userId);    
}
