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
}