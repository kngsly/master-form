﻿using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace master_form_blazor_server.Data
{
    // The following code snippet is from: https://stackoverflow.com/a/22417240/17305173
    public class Storage 
    {
        public static string AppendSlugDirectory(string slug)
        {
            return $"Storage/Experiments/{slug}.json";
        }

        public static string AppendResponseDirectory(string slug)
        {
            return $"Storage/Responses/{slug}.json";
        }

        public static FileInfo GetUniquePath(string path)
        {
            string Directory = Path.GetDirectoryName(path);
            string FileName = Path.GetFileNameWithoutExtension(path);
            string FileExtension = Path.GetExtension(path);

            for (int i = 1; ; ++i)
            {
                if (!File.Exists(path))
                    return new FileInfo(path);

                path = Path.Combine(Directory, FileName + " " + i + FileExtension);
            }
        }

        /// <summary>
        /// Writes the given object instance to a Json file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// <para>Only Public properties and variables will be written to the file. These can be any type though, even other classes.</para>
        /// <para>If there are public properties/variables that you do not want written to the file, decorate them with the [JsonIgnore] attribute.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        public static void WriteToJsonFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
        {
            TextWriter writer = null;
            try
            {
                var contentsToWriteToFile = JsonConvert.SerializeObject(objectToWrite);
                writer = new StreamWriter(filePath, append);
                writer.Write(contentsToWriteToFile);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        /// <summary>
        /// Reads an object instance from an Json file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object to read from the file.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the Json file.</returns>
        public static T ReadFromJsonFile<T>(string filePath) where T : new()
        {
            TextReader reader = null;
            try
            {
                reader = new StreamReader(filePath);
                var fileContents = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(fileContents);
            }
            catch (Exception e)
            {
                if (reader != null)
                    reader.Close();

                return default(T); // Probably not found
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        public static void RemoveFile(string filePath)
        {
            if(File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}