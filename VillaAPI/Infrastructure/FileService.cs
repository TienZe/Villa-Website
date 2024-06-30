namespace VillaAPI.Infrastructure;

public class FileService
{
    private readonly IWebHostEnvironment environment;
    public FileService(IWebHostEnvironment env)
    {
        environment = env;
    }

    // Upload file vào đường dẫn tương ứng
    // Trả về relative path of uploaded file nếu upload thành công, ngược lại ném Exception chứa thông báo lỗi
    public string UploadFile(IFormFile file, string relativeFolderPath)
    {
        try
        {
            string wwwrootPath = environment.WebRootPath;
            string folderPath = Path.Combine(wwwrootPath, relativeFolderPath);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string absoluteFilePath = Path.Combine(folderPath, uniqueFileName);
            using (var fileStream = new FileStream(absoluteFilePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            string relativeFilePath = relativeFolderPath + "/" + uniqueFileName;
            return relativeFilePath;
        }
        catch (Exception)
        {
            throw new Exception("Cannot upload this file name " + file.FileName
                + ", something went wrong!");
        }
    }

    // Thực hiện upload file với extension tương ứng
    // Ném exception nếu file extension ko đúng hoặc upload file lỗi
    public string UploadFile(IFormFile file, string relativeFolderPath, string[] acceptExtensions)
    {
        // Validate file extension
        string fileExtension = Path.GetExtension(file.FileName);
        if (!acceptExtensions.Contains(fileExtension))
        {
            throw new Exception("Can only upload files with the extension "
                + string.Join(", ", acceptExtensions));
        }
        string filePath = UploadFile(file, relativeFolderPath);
        return filePath;
    }

    // Thực hiện upload file AUDIO
    // Ném exception nếu file extension ko đúng hoặc upload file lỗi
    public string UploadAudio(IFormFile audio, string relativeFolderPath)
    {
        return UploadFile(audio, relativeFolderPath, new string[] { ".mp3", ".wav" });
    }

    // Thực hiện upload file IMAGE
    // Ném exception nếu file extension ko đúng hoặc upload file lỗi
    public string UploadImage(IFormFile image, string relativeFolderPath)
    {
        return UploadFile(image, relativeFolderPath, new string[] { ".jpg", ".png", ".jpeg" });
    }


    // Thực hiện xóa file có đương dẫn tương ứng
    // Trả về true nếu xóa file thành công
    // Trả về false nếu xóa file thất bại hoặc file không tồn tại
    public bool DeleteFile(string relativeFilePath)
    {
        try
        {
            string wwwrootPath = environment.WebRootPath;
            string absolutePath = Path.Combine(wwwrootPath, relativeFilePath);
            if (File.Exists(absolutePath))
            {
                File.Delete(absolutePath);
                return true;
            }
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }
}