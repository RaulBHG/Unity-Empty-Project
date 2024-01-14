using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using TFF.Salvatore.SaveLoadSystem.Core;
using UnityEngine;

namespace TFF.Salvatore.SaveLoadSystem.SaveLoadTypes
{
    public class FileSaveLoader : ISaveLoader
    {
        // Keys generated first time we create Aes
        private const string KEY = "fiezE8yOll7v9WAC1x4ajAhQP29J2eoxPg/lxD8SC1I=";
        private const string IV = "nkgaozvRYaWtxsjh80sYkA==";


        private bool encrypted = true;
        private Dictionary<string, List<DataChunk>> snapshotData;
        private Dictionary<string, List<DataChunk>> snapshotOutData;

        public FileSaveLoader(bool encrypted, ref Dictionary<string, List<DataChunk>> snapshotData, ref Dictionary<string, List<DataChunk>> snapshotOutData)
        {
            this.encrypted = encrypted;
            this.snapshotData = snapshotData;
            this.snapshotOutData = snapshotOutData;
        }

        /**
         * Slots:
         * 0 - Common
         * 1 - Game 1
         * 2 - Game 2
         * 3 - Game 3
         */
        // Used to save persistent game
        public bool OnSave(int slot = 0)
        {
            // On slot 0 We take the common data
            Dictionary<string, List<DataChunk>> dataToSave = slot != 0 ? snapshotData : snapshotOutData;

            string path = $"{Application.persistentDataPath}/slotGame{slot}.json";
            Debug.Log($"Selected slot: {slot}.");
            try
            {
                if (File.Exists(path)) // If exist we delete it
                {
                    Debug.Log("Data exist. Deleting old file and writing a new one.");
                    File.Delete(path);
                }
                else
                {
                    Debug.Log("Creating save file for the first time.");
                }

                using FileStream stream = File.Create(path);
                if (encrypted)
                {
                    WriteEncryptedData(dataToSave, stream);
                }
                else
                {
                    // Close sequency inmediately for not throwing exceptions
                    stream.Close();
                    File.WriteAllText(path, JsonConvert.SerializeObject(dataToSave)); // Convert on json and save
                }

                return true;
            }
            catch (Exception e)
            {
                Debug.Log($"Unable to save data due to: {e.Message} {e.StackTrace}");
                return false;
            }
        }

        // Used to load persistent game
        public Dictionary<string, List<DataChunk>> OnLoad(int slot = 0)
        {
            Dictionary<string, List<DataChunk>> dataLoaded;

            string path = $"{Application.persistentDataPath}/slotGame{slot}.json";
            Debug.Log($"Selected slot: {slot}.");
            if (!File.Exists(path)) // If exist we delete it
            {
                Debug.Log($"Cannot load file at {path}. File does not exits.");
                throw new FileNotFoundException($"{path} does not exist!");
            }

            try
            {
                if (encrypted)
                {
                    dataLoaded = ReadEncryptedData<Dictionary<string, List<DataChunk>>>(path);
                }
                else
                {
                    string json = File.ReadAllText(path);
                    dataLoaded = JsonConvert.DeserializeObject<Dictionary<string, List<DataChunk>>>(json);
                }

            }
            catch (Exception e)
            {
                Debug.Log($"Failed to load the data due to: {e.Message} {e.StackTrace}");
                throw e;
            }

            return dataLoaded;
        }


        /*******************/
        /* Private methods */
        /*******************/
        private void WriteEncryptedData<T>(T data, FileStream stream)
        {
            using Aes aesProvider = Aes.Create();
            // To use our KEY and IV
            aesProvider.Key = Convert.FromBase64String(KEY);
            aesProvider.IV = Convert.FromBase64String(IV);

            using ICryptoTransform cryptoTransform = aesProvider.CreateEncryptor();
            using CryptoStream cryptoStream = new CryptoStream(stream, cryptoTransform, CryptoStreamMode.Write); // With mode write we will write on stream using the cryptoTransform

            cryptoStream.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(data)));
        }

        private T ReadEncryptedData<T>(string path)
        {

            byte[] fileBytes = File.ReadAllBytes(path);
            using Aes aesProvider = Aes.Create();

            // To use our KEY and IV
            aesProvider.Key = Convert.FromBase64String(KEY);
            aesProvider.IV = Convert.FromBase64String(IV);

            using ICryptoTransform cryptoTransform = aesProvider.CreateDecryptor(aesProvider.Key, aesProvider.IV);
            using MemoryStream decryptionStream = new MemoryStream(fileBytes);
            using CryptoStream cryptoStream = new CryptoStream(decryptionStream, cryptoTransform, CryptoStreamMode.Read); // With mode read we will read on stream using the cryptoTransform
            using StreamReader reader = new StreamReader(cryptoStream);

            string result = reader.ReadToEnd();

            Debug.Log($"Decrypted result (if the following is not legible, probably there is a problem with de KEY and IV): {result}");
            return JsonConvert.DeserializeObject<T>(result);
        }
       
    }

}
