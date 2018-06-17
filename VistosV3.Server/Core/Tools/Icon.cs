using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Tools
{
    public static class Icon
    {
        public static string GetIconByExtension(String extension)
        {
            string icon = "far fa-file";
            switch (extension.ToLower().Replace(".", ""))
            {
                case "zip":
                case "rar":
                case "tar":
                case "7z":
                    {
                        icon = "far fa-file-archive";
                        break;
                    }
                case "wav":
                case "mp3":
                case "aac":
                case "weba":
                    {
                        icon = "far fa-file-audio";
                        break;
                    }
                case "cs":
                    {
                        icon = "far fa-file-code";
                        break;
                    }
                case "ods":
                case "xls":
                case "xlsx":
                case "csv":
                    {
                        icon = "far fa-file-excel";
                        break;
                    }
                case "bmp":
                case "png":
                case "jpg":
                case "jpeg":
                case "gif":
                case "webp":
                    {
                        icon = "far fa-file-image";
                        break;
                    }
                case "pdf":
                    {
                        icon = "far fa-file-pdf";
                        break;
                    }
                case "ppt":
                case "pptx":
                case "odp":
                    {
                        icon = "far fa-file-powerpoint";
                        break;
                    }
                case "txt":
                case "text":
                    {
                        icon = "far fa-file";
                        break;
                    }
                case "avi":
                case "mkv":
                case "mov":
                case "mpeg":
                case "webm":
                    {
                        icon = "far fa-file-video";
                        break;
                    }
                case "doc":
                case "docx":
                case "odt":
                case "rtf":
                    {
                        icon = "far fa-file-word";
                        break;
                    }
            }
            return icon;
        }
    }
}
