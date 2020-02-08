using System;
using System.Collections.Generic;
using System.IO;

namespace Jaxx.Net.Helpers.IO
{
    public static class FileOperations
    {
        public static void Copy(string sourceFileName, string destFileName, CopyOptions copyOptions)
        {
            if (copyOptions == null) throw new ArgumentNullException(nameof(copyOptions));

            switch (copyOptions.CopyStrategy)
            {
                default:
                case CopyStrategy.Stop:
                    File.Copy(sourceFileName, destFileName);
                    break;
                case CopyStrategy.Overwrite:
                    File.Copy(sourceFileName, destFileName, true);
                    break;
                case CopyStrategy.Rename:
                case CopyStrategy.RenameLatest:
                case CopyStrategy.RenameOld:
                    RenameOld(sourceFileName, destFileName, copyOptions);
                    break;
               
            }
        }

        private static void RenameOld (string sourceFileName, string destFileName, CopyOptions copyOptions)
        {
            if (File.Exists(destFileName))
            {
                bool isDestFileLatest = IsDestFileLatest(sourceFileName, destFileName, copyOptions.CompareDateOptions);
                if (isDestFileLatest)
                {
                    // Rename sourceFile
                   
                }
                else
                {
                    // Rename destFike
                }
            }
            else File.Copy(sourceFileName, destFileName);

        }

        /// <summary>
        /// Returns depending on CompareDateOption whether the target file is the newer one.
        /// </summary>
        /// <param name="sourceFileName"></param>
        /// <param name="destFileName"></param>
        /// <param name="compareDateOption"></param>
        /// <returns></returns>
        private static bool IsDestFileLatest(string sourceFileName, string destFileName, CompareDateOption compareDateOption)
        {
            var destFileInfo = new FileInfo(destFileName);
            var sourceFileInfo = new FileInfo(sourceFileName);

            DateTime sourceFileTime;
            DateTime destFileTime;

            switch (compareDateOption)
            {
                case CompareDateOption.CreationTime:
                    sourceFileTime = sourceFileInfo.CreationTimeUtc;
                    destFileTime = destFileInfo.CreationTimeUtc;
                    break;
                default:
                case CompareDateOption.LastWriteTime:
                    sourceFileTime = sourceFileInfo.LastWriteTimeUtc;
                    destFileTime = destFileInfo.LastWriteTimeUtc;
                    break;
            }


            var isDestFileLatest = destFileTime > sourceFileTime;
            return isDestFileLatest;
        }


        /// <summary>
        /// Methode zum Erstellen eindeutiger Dateinamen.
        /// Setzt am Endes des Dateinamen (ohne Extension) den nächstfreien Integerwert im Verzeichnis.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="filename">The filename.</param>
        /// <returns>a unique filename</returns>
        public static string GetCountedUpFilename(string filename)
        {
            if (!File.Exists(filename))
                return filename;
            else
            {
                int counter = 0;
                FileInfo fileInfo = new FileInfo(filename);
                while (File.Exists(filename))
                {
                    counter++;
                    filename = fileInfo.FullName.Replace(fileInfo.Extension, string.Empty, StringComparison.OrdinalIgnoreCase) + counter + fileInfo.Extension;
                }
                return filename;
            }
        }


        /// <summary>
        /// Methode zum Erstellen eindeutiger Dateinamen.
        /// Trennt den letzten Buchstaben der Extension ab und setzt den nächstfreien Integerwert im Verzeichnis.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="filename">The filename.</param>
        /// <returns>a unique filename</returns>
        public static string GetCountedUpExtension(string filename)
        {
            if (!File.Exists(filename))
                return filename;
            else
            {
                int counter = 0;
                FileInfo fileInfo = new FileInfo(filename);
                while (File.Exists(filename))
                {
                    counter++;
                    if (counter.ToString().Length > fileInfo.Extension.Length -1) throw new NotSupportedException("Extension length exceeded.");
                    filename = fileInfo.FullName.Replace(fileInfo.Extension, string.Empty, StringComparison.OrdinalIgnoreCase) + fileInfo.Extension.Substring(0, fileInfo.Extension.Length - counter.ToString().Length) + counter;
                }
                return filename;
            }
        }
        /// <summary>
        /// Obsolete
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        [System.Obsolete("Use GetCountedUpFilename or GetCountedUpExtension", true)]
        public static string RenameToUniqueExtension(string filename)
        {
            //obsolete
            return null;
        }

        /// <summary>
        /// Methode zum Erstellen eindeutiger Dateinamen.
        /// Hängt an eine Datei einen eindeutigen Datums- und Zeitstring an 
        /// </summary>
        /// <param name="Dateiname">Name der umzubenennen Datei</param>
        /// <returns>Gibt den neuen Dateinamen zurück</returns>
        public static string RenameToUniqueDateTime(string Dateiname)
        {
            string datumzeit = DateTime.Now.ToString("yyyyMMdd_HHmmssmss");
            string neuerDateiname = String.Format("{0}.{1}", Dateiname, datumzeit);
            File.Copy(Dateiname, neuerDateiname);
            File.Delete(Dateiname);
            return neuerDateiname;
        }

        /// <summary>
        /// Hängt einem Verzeichnispfad einen Unterordner mit dem aktuellen Datum
        /// und Zeit im Format JJMMTTHHMMSS an
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public static string GetUniqueDayDirectory(string Path)
        {
            string newPath = FileOperations.EnsureTrailingBackslash(Path);
            //FS#57 DayDirectory im Format JJMMTTHHMMSS
            newPath = newPath + DateTime.Now.ToString("yyMMddHHmmss");
            return FileOperations.EnsureTrailingBackslash(newPath);
        }

        /// <summary>
        /// Diese Methode gewährleistet, dass ein Pfad-String am Ende
        /// immer einen Backslash hat
        /// </summary>
        /// <param name="filePath">Der Pfadename mit oder ohne Backslash</param>
        /// <returns>Pfad mit abschliessendem Backslash</returns>
        public static string EnsureTrailingBackslash(string filePath)
        {
            // Zuerst prüfen, ob Pfad bereits einen Backslash enthält und diesen entfernen
            if (filePath.EndsWith("\\"))
            {
                filePath = filePath.Substring(0, filePath.Length - 1);
            }

            // Dem String einen Backslash hinzufügen
            filePath = filePath + "\\";
            return filePath;

        }

        /// <summary>
        /// Trennt einen Pfad an allen Backslash auf und liefert die 
        /// einzelnen Teile zurück
        /// </summary>
        /// <param name="FilePath">der aufzutrennende Pfad</param>
        /// <returns>Ein String Array mit den einzelnen Teilstrings</returns>
        public static string[] FilePathToArray(string FilePath)
        {
            char[] chSplit = { '\\' };
            string[] splittedFilePath = FilePath.Split(chSplit);
            return splittedFilePath;


        }

        /// <summary>
        /// Entfernt von einem Dateistring die Endung
        /// </summary>
        /// <param name="FullFileName"></param>
        /// <param name="Extension"></param>
        /// <returns></returns>
        public static string RemoveExtension(string FullFileName, string Extension)
        {
            string result;
            int fullFileNameLength = FullFileName.Length;
            int extensionLength = Extension.Length;
            int newFileNameLength = fullFileNameLength - extensionLength;
            result = FullFileName.Substring(0, newFileNameLength);
            return result;

        }

        /// <summary>
        /// Renames the file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="newName">The new name.</param>
        public static void RenameFile(string path, string newName)
        {
            var fileInfo = new FileInfo(path);
            File.Move(path, fileInfo.Directory + newName);
        }

        /// <summary>
        /// Diese Methode findet heraus ob es sich um einen Netzwerkshare oder um einen
        /// Laufwerkspfad handelt und schneidet diese Postionen ab um einen relativen Pfad
        /// zu erhalten
        /// Beginnt der ursprüngliche Name mit zwei Backslashes ist es wohl ein
        /// UNC Pfad und die Backslashes, also die ersten beiden Stellen,
        /// müssen abgetrennt werden, ansonsten ist es wohl ein Laufwerksbuchstabe
        /// (Bsp: C:\TEST) und die ersten drei Stellen müssen abgetrennt werden.
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public static string CutDriveOrUNCInformation(string Path)
        {
            // Beginnt der ursprüngliche Name mit zwei Backslashes ist es wohl ein
            // UNC Pfad und die Backslashes, also die ersten beiden Stellen,
            // müssen abgetrennt werden, ansonsten ist es wohl ein Laufwerksbuchstabe
            // (Bsp: C:\TEST) und die ersten drei Stellen müssen abgetrennt werden.
            if (Path.StartsWith(@"\\"))
            {
                //UNC Pfad
                return Path.Substring(2);

            }
            else
            {
                // Laufwerkspfad
                return Path.Substring(3);
            }
        }

        /// <summary>
        /// Diese Methode hängt dem übergebenen Dateinamen die übergebene Extension an.
        /// Ist zur Abtrennung von der ursprünglichen Extension ein Punkt gewünscht, muss dieser
        /// in der neuen Extension explizit angegeben werden.
        /// </summary>
        /// <param name="OriginalFileName"></param>
        /// <param name="ExtraExtension"></param>
        public static void AddExtraFileExtension(string OriginalFileName, string ExtraExtension)
        {
            string newFileName = String.Format("{0}{1}", OriginalFileName, ExtraExtension);
            File.Move(OriginalFileName, newFileName);
        }

        /// <summary>
        /// Dies Methode hängt den in der Liste übegebenen Dateinamen die übergebene Extension an.
        /// Ist zur Abtrennung von der ursprünglichen Extension ein Punkt gewünscht, muss dieser
        /// in der neuen Extension explizit angegeben werden.
        /// </summary>
        /// <param name="OrginalFileNameList"></param>
        /// <param name="ExtraExtension"></param>
        public static void AddExtraFileExtension(IEnumerable<string> OrginalFileNameList, string ExtraExtension)
        {
            foreach (string file in OrginalFileNameList)
            {
                AddExtraFileExtension(file, ExtraExtension);
            }
        }

        /// <summary>
        /// Compares the filesize of two given files.
        /// </summary>
        /// <param name="File1"></param>
        /// <param name="File2"></param>
        /// <returns>Returns true in case the filesize of both files match.
        /// Returns false in case the filesize of both files won't match.</returns>
        public static bool CompareFileSize(string File1, string File2)
        {
            try
            {
                System.IO.FileInfo infoFile1 = new System.IO.FileInfo(File1);
                System.IO.FileInfo infoFile2 = new System.IO.FileInfo(File2);
                return long.Equals(infoFile1.Length, infoFile2.Length);
            }
            catch
            {
                throw;
            }

        }
    }


}
