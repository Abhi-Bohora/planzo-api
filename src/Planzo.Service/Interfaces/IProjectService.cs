using Planzo.Data.Dtos.Auth;
using Planzo.Data.Dtos.Project;

namespace Planzo.Service.Interfaces;

public interface IProjectService
{
    Task<ResponseDto> CreateProjectAsync(CreateProjectDto createProjectDto);
    Task<ResponseDto> UpdateProjectAsync(int projectId, CreateProjectDto createProjectDto, string userId);
    Task<ResponseDto> DeleteProjectAsync(int projectId, string userId);
    Task<ResponseDto> GetProjectByIdAsync(int projectId, string userId);
    Task<ResponseDto> GetAllProjectsAsync(string userId);   
}
