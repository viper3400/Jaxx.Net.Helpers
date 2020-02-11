using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Jaxx.Net.Helpers.IO
{
    public class FileOperations
    {
        private ILogger logger;
        public FileOperations(ILogger logger)
        {
            this.logger = logger;
        }
        /// <summary>
        /// Copies a source file to a destination directory. It is being considered whether a file with the same name
        /// already exists in the target directory. You can use the CopyOptions parameter to control 
        /// how to deal with a file that already exists.
        /// </summary>
        /// <param name="sourceFileName"></param>
        /// <param name="destFileName"></param>
        /// <param name="copyOptions">Specifies CopyStrategy and CompareDateOption</param>
        public void Copy(string sourceFileName, string destFileName, CopyOptions copyOptions)
        {
            Contract.Assert(copyOptions != null);

            switch (copyOptions.CopyStrategy)
            {
                case CopyStrategy.Overwrite:
                    File.Copy(sourceFileName, destFileName, true);
                    break;
                case CopyStrategy.RenameNew:
                case CopyStrategy.RenameOld:
                    Rename(sourceFileName, destFileName, copyOptions);
                    break;
               
            }
        }
        
        private void Rename (string sourceFileName, string destFileName, CopyOptions copyOptions)
        {
            if (File.Exists(destFileName))
            {
                var sourceFileInfo = new FileInfo(sourceFileName);
                var destFileInfo = new FileInfo(destFileName);
                var comparer = new FileDateComparer(sourceFileInfo, destFileInfo, copyOptions.CompareDateOptions);

                var renamedFilename = GetCountedUpExtension(destFileName);


                switch (copyOptions.CopyStrategy)
                {
                    default:
                        throw new NotSupportedException();
                    case CopyStrategy.RenameNew:
                        RenameNew(sourceFileName, destFileName, comparer, renamedFilename);
                        break;
                    case CopyStrategy.RenameOld:
                        RenameOld(sourceFileName, destFileName, comparer, renamedFilename);
                        break;
                }

            }
            else File.Copy(sourceFileName, destFileName);
        } 

        private void RenameOld(string sourceFileName, string destFileName, FileDateComparer comparer, string renamedFilename)
        {
            if (comparer.OldFile.FullName == destFileName)
            {
                logger.LogDebug("RenameOld | Move {0} to {1}", destFileName, renamedFilename);
                File.Move(destFileName, renamedFilename);
                File.Copy(sourceFileName, destFileName);
            }
            else if (comparer.NewFile.FullName == destFileName)
            {
                File.Copy(sourceFileName, renamedFilename);
            }
        }

        private void RenameNew(string sourceFileName, string destFileName, FileDateComparer comparer, string renamedFilename)
        {
            if (comparer.NewFile.FullName == destFileName)
            {
                File.Move(destFileName, renamedFilename);
                File.Copy(sourceFileName, destFileName);
            }
            else if (comparer.OldFile.FullName == destFileName)
            {
                File.Copy(sourceFileName, renamedFilename);
            }
        }


        /// <summary>
        /// Methode zum Erstellen eindeutiger Dateinamen.
        /// Setzt am Endes des Dateinamen (ohne Extension) den nächstfreien Integerwert im Verzeichnis.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="filename">The filename.</param>
        /// <returns>a unique filename</returns>
        public string GetCountedUpFilename(string filename)
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
        public string GetCountedUpExtension(string filename)
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
        /// Methode zum Erstellen eindeutiger Dateinamen.
        /// Hängt an eine Datei einen eindeutigen Datums- und Zeitstring an 
        /// </summary>
        /// <param name="Dateiname">Name der umzubenennen Datei</param>
        /// <returns>Gibt den neuen Dateinamen zurück</returns>
        public string RenameToUniqueDateTime(string Dateiname)
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
        public string GetUniqueDayDirectory(string Path)
        {
            string newPath = EnsureTrailingBackslash(Path);
            //FS#57 DayDirectory im Format JJMMTTHHMMSS
            newPath = newPath + DateTime.Now.ToString("yyMMddHHmmss");
            return EnsureTrailingBackslash(newPath);
        }

        /// <summary>
        /// Diese Methode gewährleistet, dass ein Pfad-String am Ende
        /// immer einen Backslash hat
        /// </summary>
        /// <param name="filePath">Der Pfadename mit oder ohne Backslash</param>
        /// <returns>Pfad mit abschliessendem Backslash</returns>
        public string EnsureTrailingBackslash(string filePath)
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
        public string[] FilePathToArray(string FilePath)
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
        public string RemoveExtension(string FullFileName, string Extension)
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
        public void RenameFile(string path, string newName)
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
        public string CutDriveOrUNCInformation(string Path)
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
        public void AddExtraFileExtension(string OriginalFileName, string ExtraExtension)
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
        public void AddExtraFileExtension(IEnumerable<string> OrginalFileNameList, string ExtraExtension)
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
        public bool CompareFileSize(string File1, string File2)
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
