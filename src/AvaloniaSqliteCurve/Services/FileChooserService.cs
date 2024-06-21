using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaSqliteCurve.Services
{
    internal class FileChooserService : IFileChooserService
    {
        private IStorageProvider? _storageProvider;

        public void SetHostWindow(TopLevel window)
        {
            var topLevel = TopLevel.GetTopLevel(window);
            _storageProvider = topLevel?.StorageProvider;
        }

        public async Task<List<string>?> OpenFolderPickerAsync(string title, bool allowMultiple)
        {
            if (_storageProvider is null)
                throw new ArgumentNullException(nameof(_storageProvider));
            var result = await _storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
            {
                Title = title,
                AllowMultiple = allowMultiple
            });
            return result.Any() ? result.Select(file => file.Path.AbsolutePath).ToList() : default;
        }

        public async Task<List<string>?> OpenFilePickerAsync(string title,
            IReadOnlyList<FilePickerFileType>? fileTypeFilter,
            bool allowMultiple)
        {
            if (_storageProvider is null)
                throw new ArgumentNullException(nameof(_storageProvider));
            var result = await _storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
            {
                Title = title,
                FileTypeFilter = fileTypeFilter,
                AllowMultiple = allowMultiple
            });
            return result.Any() ? result.Select(file => file.Path.AbsolutePath).ToList() : default;
        }

        public async Task OpenFolderAsync(string folder)
        {
            Process.Start("Explorer.exe", $"\"{folder}\"");
            await Task.CompletedTask;
        }
    }
}