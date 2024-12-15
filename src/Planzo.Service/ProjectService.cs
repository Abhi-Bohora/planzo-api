using AutoMapper;
using Planzo.Data.Dtos;
using Planzo.Data.Dtos.Project;
using Planzo.Data.Models;
using Planzo.Data.UnitOfWork;
using Planzo.Service.Interfaces;

namespace Planzo.Service;

public class ProjectService: IProjectService
{
    private readonly IUnitOfWork _unitOfWork;   
    private readonly IMapper _mapper;

    public ProjectService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ResponseDto<ProjectDto>> CreateProjectAsync(CreateProjectDto createProjectDto, string userId)
    {
        try
        {
            var project = _mapper.Map<Project>(createProjectDto);
            project.UserId = userId;
            project.CreatedAt = DateTime.UtcNow;
            
            var projectRepo = _unitOfWork.Repository<Project>();
            await projectRepo.AddAsync(project);
            await _unitOfWork.SaveChangesAsync();
            var createdProjectDto = _mapper.Map<ProjectDto>(project);
            return new ResponseDto<ProjectDto>
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "Project Created Successfully!",
                Data = createdProjectDto
            };
        }
        catch (Exception e)
        {
            return new ResponseDto<ProjectDto>
            {
                IsSuccess = false,
                StatusCode = 500,
                Message = "An error occurred while creating the project.",
                Data = null
            };
        }
    }
    
    public async Task<ResponseDto<ProjectDto>> UpdateProjectAsync(int projectId, CreateProjectDto createProjectDto, string userId)
    {
        try
        {
            var projectRepo = _unitOfWork.Repository<Project>();
            var existingProject = await projectRepo.GetAsync(p => p.Id == projectId && p.UserId == userId);

            if (existingProject == null)
            {
                return new ResponseDto<ProjectDto>
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    Message = "Project not found.",
                    Data = null
                };
            }
            _mapper.Map(createProjectDto, existingProject);
            existingProject.UpdatedAt = DateTime.UtcNow;

            projectRepo.Update(existingProject);
            await _unitOfWork.SaveChangesAsync();

            var updatedProjectDto = _mapper.Map<ProjectDto>(existingProject);
            return new ResponseDto<ProjectDto>
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "Project Updated Successfully!",
                Data = updatedProjectDto
            };
        }
        catch (Exception e)
        {
            return new ResponseDto<ProjectDto>
            {
                IsSuccess = false,
                StatusCode = 500,
                Message = $"An error occurred while updating the project: {e.Message}",
                Data = null
            };
        }
    }

    public async Task<ResponseDto<ProjectDto>> DeleteProjectAsync(int projectId, string userId)
    {
        try
        {
            var projectRepo = _unitOfWork.Repository<Project>();
            var existingProject = await projectRepo.GetAsync(p => p.Id == projectId && p.UserId == userId);

            if (existingProject == null)
            {
                return new ResponseDto<ProjectDto>
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    Message = "Project not found.",
                    Data = null
                };
            }

            await projectRepo.DeleteAsync(existingProject);
            await _unitOfWork.SaveChangesAsync();

            return new ResponseDto<ProjectDto>
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "Project Deleted Successfully!",
                Data = null
            };
        }
        catch (Exception e)
        {
            return new ResponseDto<ProjectDto>
            {
                IsSuccess = false,
                StatusCode = 500,
                Message = $"An error occurred while deleting the project: {e.Message}",
                Data = null
            };
        }
    }
    
     public async Task<ResponseDto<ProjectDto>> GetProjectByIdAsync(int projectId, string userId)
    {
        try
        {
            var projectRepo = _unitOfWork.Repository<Project>();
            var project = await projectRepo.GetAsync(p => p.Id == projectId && p.UserId == userId);

            if (project == null)
            {
                return new ResponseDto<ProjectDto>
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    Message = "Project not found.",
                    Data = null
                };
            }

            var projectDto = _mapper.Map<ProjectDto>(project);
            return new ResponseDto<ProjectDto>
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "Project Retrieved Successfully!",
                Data = projectDto
            };
        }
        catch (Exception e)
        {
            return new ResponseDto<ProjectDto>
            {
                IsSuccess = false,
                StatusCode = 500,
                Message = $"An error occurred while retrieving the project: {e.Message}",
                Data = null
            };
        }
    }

    public async Task<ResponseDto<List<ProjectDto>>> GetAllProjectsAsync(string userId)
    {
        try
        {
            var projectRepo = _unitOfWork.Repository<Project>();
            var projects = await projectRepo.GetAllAsync(p => p.UserId == userId);

            var projectDtos = _mapper.Map<List<ProjectDto>>(projects);
            return new ResponseDto<List<ProjectDto>>
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "Projects Retrieved Successfully!",
                Data = projectDtos
            };
        }
        catch (Exception e)
        {
            return new ResponseDto<List<ProjectDto>>
            {
                IsSuccess = false,
                StatusCode = 500,
                Message = $"An error occurred while retrieving projects: {e.Message}",
                Data = null
            };
        }
    }
}