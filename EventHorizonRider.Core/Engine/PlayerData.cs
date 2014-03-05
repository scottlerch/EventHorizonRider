using Newtonsoft.Json;
using PCLStorage;
using System;
using System.Threading.Tasks;

namespace EventHorizonRider.Core.Engine
{
    internal class PlayerData : ComponentBase
    {
        public TimeSpan Highscore { get; set; }

        public async Task Save()
        {
            var rootFolder = FileSystem.Current.LocalStorage;
            var folder = await rootFolder.CreateFolderAsync("EventHorizon", CreationCollisionOption.OpenIfExists);
            var file = await folder.CreateFileAsync("player1.json", CreationCollisionOption.ReplaceExisting);
            await file.WriteAllTextAsync(JsonConvert.SerializeObject(this));
        }

        public async Task Load()
        {
            var rootFolder = FileSystem.Current.LocalStorage;
            var folder = await rootFolder.GetFolderAsync("EventHorizon");

            var fileExists = await folder.CheckExistsAsync("player1.json");

            if (fileExists == ExistenceCheckResult.FileExists)
            {
                var file = await folder.GetFileAsync("player1.json");
                var text = await file.ReadAllTextAsync();

                if (!string.IsNullOrEmpty(text))
                {
                    var data = JsonConvert.DeserializeObject<PlayerData>(text);

                    Highscore = data.Highscore;
                }
            }
        }

        internal async Task UpdateBestTime(TimeSpan timeSpan)
        {
            if (timeSpan > Highscore)
            {
                Highscore = timeSpan;
                await Save();
            }
        }
    }
}