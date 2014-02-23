using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using Newtonsoft.Json;
using System;
using System.IO;

namespace EventHorizonRider.Core.Components
{
    internal class PlayerData
    {
        public TimeSpan Highscore { get; set; }

        public void Save()
        {
            var result = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);
            result.AsyncWaitHandle.WaitOne();
            var storageDevice = StorageDevice.EndShowSelector(result);

            result = storageDevice.BeginOpenContainer("EventHorizon", null, null);
            result.AsyncWaitHandle.WaitOne();

            using (var container = storageDevice.EndOpenContainer(result))
            using (var writer = new StreamWriter(container.CreateFile("user.dat")))
            {
                writer.WriteLine(JsonConvert.SerializeObject(this));
            }
        }

        public void Load()
        {
            var result = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);
            result.AsyncWaitHandle.WaitOne();
            var storageDevice = StorageDevice.EndShowSelector(result);
            result.AsyncWaitHandle.Dispose();

            result = storageDevice.BeginOpenContainer("EventHorizon", null, null);
            result.AsyncWaitHandle.WaitOne();

            using (var container = storageDevice.EndOpenContainer(result))
            {
                if (container.FileExists("user.dat"))
                {
                    // TODO: open file API not implemented yet in monogame
                }
            }

            result.AsyncWaitHandle.Dispose();
        }

        internal void Update(TimeSpan timeSpan)
        {
            if (timeSpan > this.Highscore)
            {
                this.Highscore = timeSpan;
                this.Save();
            }
        }
    }
}
