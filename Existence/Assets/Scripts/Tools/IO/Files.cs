using System.IO;
using System.Text;
using System.Linq;
using System.Collections;
using UnityEngine;
using System;

namespace Tools {

	namespace IO {

		/// <summary>
		/// Helper class to manage local files and directories
		/// </summary>
		public class Files {
            
			/// <summary>
			/// Checks if a directory at 'path' does not exist and creates one
			/// </summary>
			public static void CreateDirectoryIfNull(string path) {
				if (!Directory.Exists(path)) {
					//Debug.Log("Creating new directory at "+path);
					Directory.CreateDirectory(path);
				}
			}

			/// <summary>
			/// Checks if a file at 'path' does not exist and creates one with 'initialContents'
			/// </summary>
			public static void CreateFileIfNull(string path, string initialContents) {
				if (File.Exists(path)) return;
				using (FileStream fs = File.Create(path)) {
					//Debug.Log("Creating new file at "+path);
					byte[] info = new UTF8Encoding(true).GetBytes(initialContents);
					fs.Write(info, 0, info.Length);
				}
			}

			/// <summary>
			/// Checks if a file at 'path' exists and adds or replaces 'contents' based on 'extend'
			/// </summary>
			public static void WriteToFile(string path, string contents, bool extend) {
				if (InvalidFile(path)) return;
				using (StreamWriter sw = new StreamWriter(path, extend)) {
					//Debug.Log("Writing contents into file: "+contents);
					if (extend) {
						sw.WriteLine(contents);
					}
					else {
						sw.Write(contents);
					}
				}
			}

			/// <summary>
			/// Checks if a file at 'path' exists and writes 'bytes' to the file.
			/// Useful for videos or image files
			/// </summary>
			public static void WriteBytesToFile(string path, byte[] bytes) {
				if (InvalidFile(path)) return;
				File.WriteAllBytes(path, bytes);
			}

			/// <summary>
			/// Checks if a file at 'path' exists and returns a byte array
			/// </summary>
			public static byte[] GetBytesFromFile(string path) {
				if (InvalidFile(path)) return null;
				return File.ReadAllBytes(path);
			}
			
			/// <summary>
			/// Checks if a file at 'path' exists and returns the lines of the file as a string array
			/// </summary>
			public static string[] GetLines(string path) {
				if (InvalidFile(path)) return null;
				return File.ReadAllLines(path);
			}

			/// <summary>
			/// Returns an array of file names in directory
			/// </summary>
			public static string[] GetFiles(string directory) {
				if (!Directory.Exists(directory)) return null;
				return Directory.GetFiles(directory);
			}

			/// <summary>
			/// Returns an array of directory names in directory
			/// </summary>
			public static string[] GetDirectories(string directory) {
				if (!Directory.Exists(directory)) return null;
				return Directory.GetDirectories(directory);
			}

			/// <summary>
			/// Checks if a file at 'path' exists and returns the contents of the file as a single string
			/// </summary>
			public static string GetContents(string path) {
				if (InvalidFile(path)) return string.Empty;
				return File.ReadAllText(path);
			}

			/// <summary>
			///	Checks if a file at 'path' exists and deletes the file
			/// </summary>
			public static void DeleteFile(string path) {
				if (InvalidFile(path)) return;
				File.Delete(path);
			}

			public static void DeleteDirectory(string _path)
			{
				if (!Directory.Exists(_path)) return;
				Directory.Delete(_path, false);
			}

			/// <summary>
			///	Returns true if a file at 'path' exists
			/// </summary>
			public static bool FileExists(string path) {
				return File.Exists(path);
			}

			public static bool DirectoryExists(string path) {
				return Directory.Exists(path);
			}

			public static void MoveFile(string sourcePath, string destPath) {
				if (InvalidFile(sourcePath)) return;
				if (FileExists(destPath)) return;
				File.Move(sourcePath, destPath);
			}

			/// <summary>
			///	Returns true if a file at 'path' does not exist
			/// </summary>
			static bool InvalidFile(string path) {
				if (!File.Exists(path)){
					//Debug.Log("Path " +path+ " could not be found");
					return true;
				}
				return false;
			}
		}
	}
}