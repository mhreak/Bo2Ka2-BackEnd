using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bodokado.API.Constants;
using Bodokado.API.Helpers;
using Bodokado.Application.Common.File.DTOs;
using Bodokado.Application.Common.File.Interfaces;
using Bodokado.Application.Common.Localization;
using Bodokado.Domain.Entities;

namespace Bodokado.API.Areas.Customer.Controllers;

[Route(ApiRoutes.Customer.Files)]
[Authorize(Roles = "User,Shop")]
[ApiController]
[Tags("Files")]
public class FileController : ControllerBase
{
    private readonly IFileService _fileService;
    private readonly IResponseLocalizer _responseLocalizer;

    public FileController(IFileService fileService, IResponseLocalizer responseLocalizer)
    {
        _fileService = fileService;
        _responseLocalizer = responseLocalizer;
    }

    [RequestSizeLimit(4 * 1024 * 1024)]
    [Consumes("multipart/form-data")]
    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] GenericUploadFileRequest request)
    {
        if (request.File == null || request.File.Length == 0)
            return BadRequest(ApiResult.BadRequest(await _responseLocalizer.LocalizeAsync(MessageKeys.FileInvalid)));

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var role = User.IsInRole("Shop") ? "Shop" : "User";
        var file = await _fileService.UploadForUserAsync(request.File, userId, role, request.FileType);
        return Ok(ApiResult.Success(MapToResponse(file), await _responseLocalizer.LocalizeAsync(MessageKeys.FileUploaded)));
    }

    [HttpGet("my-files")]
    public async Task<IActionResult> GetMyFiles()
    {
        var uploaderId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var files = await _fileService.GetByUploaderIdAsync(uploaderId);
        var response = files.Select(MapToResponse);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.FilesRetrieved);
        return Ok(ApiResult.Success(response, message));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var uploaderId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var success = await _fileService.DeleteAsync(id, uploaderId, isAdmin: false);
        if (!success)
        {
            var notFoundMsg = await _responseLocalizer.LocalizeAsync(MessageKeys.FileNotFound);
            return NotFound(ApiResult.NotFound(notFoundMsg));
        }
        var deletedMsg = await _responseLocalizer.LocalizeAsync(MessageKeys.FileDeleted);
        return Ok(ApiResult.Success(deletedMsg));
    }

    private static FileUploadResponse MapToResponse(FileAsset file) => new()
    {
        Id = file.Id,
        Path = file.Path,
        FileName = file.FileName,
        Extension = file.Extension,
        Size = file.Size
    };
}