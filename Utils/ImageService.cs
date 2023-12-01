using OnlineShoppingCart.Data.Entities;

namespace OnlineShoppingCart.Utils
{
    public class ImageService
    {
        private readonly IWebHostEnvironment _env;
        public ImageService(IWebHostEnvironment env)
        {
            _env = env;
        }
        public string UpLoadImage(IFormFile file)
        {
            var fileName = string.Empty;
            if (file != null)
            {
                string uploadFolder = Path.Combine(_env.WebRootPath, "assets/img");
                fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(uploadFolder, fileName);
                using var Stream = new FileStream(filePath, FileMode.Create);
                file.CopyTo(Stream);
            }
            return fileName;
        }

        public void DeleteImage(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                string imageFile = Path.Combine(_env.WebRootPath, "assets/img", fileName);
                if (System.IO.File.Exists(imageFile))
                {
                    System.IO.File.Delete(imageFile);
                }
            }
        }

        public bool IsExist(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                string imageFile = Path.Combine(_env.WebRootPath, "assets/img", fileName);
                if (System.IO.File.Exists(imageFile))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
