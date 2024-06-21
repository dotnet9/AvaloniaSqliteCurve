using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AvaloniaSqliteCurve.Services
{
    public interface IFileChooserService
    {
        void SetHostWindow(TopLevel window);

        Task<List<string>?> OpenFilePickerAsync(string title = "选择文件",
            IReadOnlyList<FilePickerFileType>? fileTypeFilter = null,
            bool allowMultiple = false);

        Task<List<string>?> OpenFolderPickerAsync(string title = "选择目录", bool allowMultiple = false);

        Task OpenFolderAsync(string folder);
    }
}