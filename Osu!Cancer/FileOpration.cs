using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Diagnostics;
using System.Windows.Media;

namespace Osu_Cancer
{
    /// <summary>
    /// Class for Fast and Convinient File Operation
    /// </summary>
    static class FileOperation
    {
        /// <summary>
        /// Get the text content from a file
        /// </summary>
        /// <param name="filePath">The destination of the file</param>
        /// <param name="type">The encoding type of the file</param>
        /// <returns></returns>
        public static string FileToString(string filePath, EncodingType type)
        {
            byte[] buffer = FileToByteArray(filePath);

            switch (type)
            {
                case EncodingType.UTF8:
                    return Encoding.Default.GetString(buffer);
                case EncodingType.Unicode:
                    return Encoding.Unicode.GetString(buffer);
                default:
                    break;
            }
            return Encoding.Default.GetString(buffer);
        }

        private static byte[] FileToByteArray(string filePath)
        {
            try
            {
                byte[] buffer = new byte[new FileInfo(filePath).Length];
                FileStream fs = new FileStream(filePath, FileMode.Open);
                fs.Read(buffer, 0, buffer.Length);
                fs.Close();
                return buffer;
            }
            catch (Exception e)
            {
                ExceptionHandle(e, "Fail to Gain Access, Pls Close program that use this resources!");
                return Encoding.Default.GetBytes("Fail to Gain Access, Pls Close program that use this resources!");
            }
        }
        public static void ExceptionHandle(Exception e, string quickMessage)
        {
            MessageBoxResult userReturn = MessageBox.Show(quickMessage+ "\r\n \r\nClick YES to Restart the game \r\nClick NO to ignore the exception\r\n \r\nDetail: " + e, "Problem Caused When Trying to Get information from Resources!", MessageBoxButton.YesNo, MessageBoxImage.Error);
            switch (userReturn)
            {
                case MessageBoxResult.None:
                case MessageBoxResult.No:
                    return;
                case MessageBoxResult.Yes:
                    Process.Start(AppDomain.CurrentDomain.BaseDirectory + "Osu!Restart.bat");
                    Environment.Exit(1);
                    return;
                default:
                    break;
            }
        }
        public static string TrimPath(string path)
        {
            return path.Substring(path.LastIndexOf('\\') + 1);
        }
        public static void ByteArraytoFile(string dir, byte[] data, int count)
        {
            FileStream fs = new FileStream(dir, FileMode.Create);
            fs.Write(data, 0, count);
            fs.Close();
        }

        public static string GetSettingValueFromFile(string filePath, string settingName)
        {
            string orginalFile = FileToString(filePath, EncodingType.UTF8);
            try
            {
                return orginalFile.Substring(orginalFile.IndexOf(settingName)).Split('\r')[0].Split(':')[1];
            }
            catch
            {
                return "Can Parsing Setting From The Resources!";
            }
        }

        public static void SetSettingValueToFile(string filePath, string settingName, string settingValue)
        {
            string orginalFile = FileToString(filePath, EncodingType.UTF8);
            string beginCut = orginalFile.Substring(0, orginalFile.IndexOf(settingName));
            string settingNameCut = settingName + ":";
            string strHeadless = orginalFile.Substring(orginalFile.IndexOf(settingName));
            string endCut = orginalFile.Substring((beginCut + strHeadless.Split('\r')[0]).Length);

            string rebuildString = beginCut + settingNameCut + settingValue + endCut;
            byte[] fileByte = Encoding.Default.GetBytes(rebuildString);
            ByteArraytoFile(filePath, fileByte, fileByte.Length);
        }
       
    }

    enum EncodingType
    {
        UTF8,
        Unicode,
    }

}
