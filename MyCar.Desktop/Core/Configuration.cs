using ModelsApi;
using MyCar.Desktop.Core.UI;
using MyCar.Desktop.ViewModels.Dialogs;
using System;
using System.Windows;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WK.Libraries.BetterFolderBrowserNS;
using System.Windows.Forms;

namespace MyCar.Desktop.Core
{
    public static class Configuration
    {
        public static UserApi CurrentUser { get; set; }

        public static Action CloseMainWindow { get; set; }

        static readonly string path = Environment.CurrentDirectory + "/config.json";

        public static Config GetConfiguration()
        {
            var config = new Config();
            if (!File.Exists(path))
                    SetConfiguration(config);
            try
            {
                var json = File.ReadAllText(path);
                config = (Config)JsonSerializer.Deserialize(json, typeof(Config))!;
                if (config.OrderColors == null || config.OrderColors.Count == 0)
                    config.OrderColors = new List<OrderColor>() {
                        new OrderColor{ Status = "Новый", Color = "#00FFFFFF" },
                        new OrderColor{ Status = "Завершен", Color = "#00FFFFFF" },
                        new OrderColor{ Status = "Отменен", Color = "#00FFFFFF" },
                        new OrderColor{ Status = "Ожидает оплаты", Color = "#00FFFFFF" },
                    };
                return config;
            }
            catch (Exception e)
            {
                UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = e.Message });
                return new Config();
            }

        }

        public static void SetConfiguration(Config config)
        {
            var json = JsonSerializer.Serialize(config, typeof(Config));
            File.WriteAllText(path, json);
        }

        public static void ExportConfig()
        {
            FolderBrowserDialog openFolderDialog = new FolderBrowserDialog();
            if (openFolderDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string selectedFolder = openFolderDialog.SelectedPath;
                    selectedFolder += "/config.json";
                    FileInfo config = new FileInfo(path);
                    config.CopyTo(selectedFolder, true);
                }
                catch (Exception e)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = e.Message });
                }
            }
        }
        public static void ImportConfig()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "json documents (.json)|*.json";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var info = new FileInfo(openFileDialog.FileName);
                    var json = File.ReadAllText(openFileDialog.FileName);
                    var config = (Config)JsonSerializer.Deserialize(json, typeof(Config))!;
                    info.CopyTo(path, true);
                }
                catch (Exception e)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = e.Message });
                }
            }
        }
    }
    public class Config
    {
        public List<ColorValue> ColorValues { get; set; } = new List<ColorValue>();

        public List<OrderColor> OrderColors { get; set; } = new List<OrderColor>()
        {
            new OrderColor{ Status = "Новый", Color = "#00FFFFFF" },
            new OrderColor{ Status = "Завершен", Color = "#00FFFFFF" },
            new OrderColor{ Status = "Отменен", Color = "#00FFFFFF" },
            new OrderColor{ Status = "Ожидает оплаты", Color = "#00FFFFFF" },
        };
    }
    public class ColorValue
    {
        public string Color { get; set; }
        public int DownValue { get; set; }
        public int UpValue { get; set; }
    }
    public class OrderColor
    {
       public string Color { get; set; }
       public string Status { get; set; }
    }

}
