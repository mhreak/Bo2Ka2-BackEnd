using Microsoft.AspNetCore.Mvc;
using Bodokado.API.Constants;
using Bodokado.API.Helpers;
using Bodokado.Application.Common.File.DTOs;
using Bodokado.Application.Common.File.Interfaces;
using Bodokado.Application.Common.Localization;
using Bodokado.Domain.Entities;

namespace Bodokado.API.Areas.Admin.Controllers;

[Route(ApiRoutes.Admin.Files)]
[Tags("Admin Files")]
public class AdminFileController : AdminBaseController
{
    private readonly IFileService _fileService;
    private readonly IResponseLocalizer _responseLocalizer;

    public AdminFileController(IFileService fileService, IResponseLocalizer responseLocalizer)
    {
        _fileService = fileService;
        _responseLocalizer = responseLocalizer;
    }

    [HttpPost("upload")]
    [RequestSizeLimit(4 * 1024 * 1024)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload([FromForm] GenericUploadFileRequest request)
    {
        if (request.File == null || request.File.Length == 0)
            return BadRequest(ApiResult.BadRequest(await _responseLocalizer.LocalizeAsync(MessageKeys.FileInvalid)));

        var userId = GetCurrentUserId();
        var file = await _fileService.UploadForUserAsync(request.File, userId, "Admin", request.FileType);
        return Ok(ApiResult.Success(MapToResponse(file), await _responseLocalizer.LocalizeAsync(MessageKeys.FileUploaded)));
    }

    [HttpGet("user-files/{userId:guid}")]
    public async Task<IActionResult> GetUserFiles(Guid userId)
    {
        var files = await _fileService.GetByUploaderIdAsync(userId);
        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.FilesRetrieved);
        return Ok(ApiResult.Success(files.Select(MapToResponse), message));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetFileById(Guid id)
    {
        var file = await _fileService.GetByIdAsync(id);
        if (file == null)
            return NotFound(ApiResult.NotFound(await _responseLocalizer.LocalizeAsync(MessageKeys.FileNotFound)));

        var message = await _responseLocalizer.LocalizeAsync(MessageKeys.FilesRetrieved);
        return Ok(ApiResult.Success(MapToResponse(file), message));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _fileService.DeleteAsync(id, Guid.Empty, isAdmin: true);
        if (!success)
            return NotFound(ApiResult.NotFound(await _responseLocalizer.LocalizeAsync(MessageKeys.FileNotFound)));

        return Ok(ApiResult.Success(await _responseLocalizer.LocalizeAsync(MessageKeys.FileDeletedByAdmin)));
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
