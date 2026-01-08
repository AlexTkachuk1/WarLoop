using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using _Scripts.Models;
using UnityEngine;

namespace _Scripts.Save
{
    public static class SaveManager
    {
        private static readonly string SavePath = Application.persistentDataPath + "/gameSave.dat";

        public static void Save(GameData data)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(SavePath, FileMode.Create);

            try
            {
                formatter.Serialize(stream, data);
                Debug.Log("Game saved successfully.");
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to save game: " + e.Message);
            }
            finally
            {
                stream.Close();
            }
        }

        public static GameData Load()
        {
            if (!File.Exists(SavePath))
            {
                Debug.LogWarning("Save file not found. Creating new game data.");
                return new GameData();
            }

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(SavePath, FileMode.Open);

            GameData data = null;

            try
            {
                data = (GameData)formatter.Deserialize(stream);
                Debug.Log("Game loaded successfully.");
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to load game: " + e.Message);
            }
            finally
            {
                stream.Close();
            }

            return data;
        }

        public static void DeleteSave()
        {
            if (File.Exists(SavePath))
            {
                File.Delete(SavePath);
                Debug.Log("Save file deleted.");
            }
            else
            {
                Debug.LogWarning("No save file to delete.");
            }
        }
    }
}